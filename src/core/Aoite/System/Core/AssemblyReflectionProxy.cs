using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;

namespace System
{
    /// <summary>
    /// 程序集反射管理器。
    /// </summary>
    public class AssemblyReflectionManager : IDisposable
    {
        Dictionary<string, AppDomain> _mapDomains = new Dictionary<string, AppDomain>();
        Dictionary<string, AppDomain> _loadedAssemblies = new Dictionary<string, AppDomain>();
        Dictionary<string, AssemblyReflectionProxy> _proxies = new Dictionary<string, AssemblyReflectionProxy>();

        /// <summary>
        /// 初始化一个 <see cref="System.AssemblyReflectionManager"/> 类的新实例。
        /// </summary>
        public AssemblyReflectionManager() { }

        /// <summary>
        /// 指定域的友好名称和程序集路径加载程序集。
        /// </summary>
        /// <param name="assemblyPath">程序集路径。</param>
        /// <param name="domainName">域的友好名称。</param>
        /// <returns>加载成功返回 true，否则返回 false。</returns>
        public bool LoadAssembly(string assemblyPath, string domainName)
        {
            // if the assembly file does not exist then fail
            if(!File.Exists(assemblyPath))
                return false;

            // if the assembly was already loaded then fail
            if(_loadedAssemblies.ContainsKey(assemblyPath))
            {
                return false;
            }

            // check if the appdomain exists, and if not create a new one
            AppDomain appDomain = null;
            if(_mapDomains.ContainsKey(domainName))
            {
                appDomain = _mapDomains[domainName];
            }
            else
            {
                appDomain = CreateChildDomain(AppDomain.CurrentDomain, domainName);
                _mapDomains[domainName] = appDomain;
            }

            // load the assembly in the specified app domain
            try
            {
                Type proxyType = typeof(AssemblyReflectionProxy);
                if(proxyType.Assembly != null)
                {
                    var proxy =
                        (AssemblyReflectionProxy)appDomain.
                            CreateInstanceFrom(
                            proxyType.Assembly.Location,
                            proxyType.FullName).Unwrap();

                    proxy.LoadAssembly(assemblyPath);

                    _loadedAssemblies[assemblyPath] = appDomain;
                    _proxies[assemblyPath] = proxy;

                    return true;
                }
            }
            catch
            { }

            return false;
        }

        /// <summary>
        /// 卸载指定程序集路径的程序集。
        /// </summary>
        /// <param name="assemblyPath">程序集路径。</param>
        /// <returns>卸载成功返回 true，否则返回 false。</returns>
        public bool UnloadAssembly(string assemblyPath)
        {
            if(!File.Exists(assemblyPath))
                return false;

            // check if the assembly is found in the internal dictionaries
            if(_loadedAssemblies.ContainsKey(assemblyPath) &&

               _proxies.ContainsKey(assemblyPath))
            {
                // check if there are more assemblies loaded in the same app domain; in this case fail
                AppDomain appDomain = _loadedAssemblies[assemblyPath];
                int count = _loadedAssemblies.Values.Count(a => a == appDomain);
                if(count != 1)
                    return false;

                try
                {
                    // remove the appdomain from the dictionary and unload it from the process
                    _mapDomains.Remove(appDomain.FriendlyName);
                    AppDomain.Unload(appDomain);

                    // remove the assembly from the dictionaries
                    _loadedAssemblies.Remove(assemblyPath);
                    _proxies.Remove(assemblyPath);

                    return true;
                }
                catch
                {
                }
            }

            return false;
        }

        /// <summary>
        /// 卸载指定域的友好名称的程序集。
        /// </summary>
        /// <param name="domainName">域的友好名称。</param>
        /// <returns>卸载成功返回 true，否则返回 false。</returns>
        public bool UnloadDomain(string domainName)
        {
            // check the appdomain name is valid
            if(string.IsNullOrEmpty(domainName))
                return false;

            // check we have an instance of the domain
            if(_mapDomains.ContainsKey(domainName))
            {
                try
                {
                    var appDomain = _mapDomains[domainName];

                    // check the assemblies that are loaded in this app domain
                    var assemblies = new List<string>();
                    foreach(var kvp in _loadedAssemblies)
                    {
                        if(kvp.Value == appDomain)
                            assemblies.Add(kvp.Key);
                    }

                    // remove these assemblies from the internal dictionaries
                    foreach(var assemblyName in assemblies)
                    {
                        _loadedAssemblies.Remove(assemblyName);
                        _proxies.Remove(assemblyName);
                    }

                    // remove the appdomain from the dictionary
                    _mapDomains.Remove(domainName);

                    // unload the appdomain
                    AppDomain.Unload(appDomain);

                    return true;
                }
                catch
                {
                }
            }

            return false;
        }

        /// <summary>
        /// 反射调用指定程序集路径的程序集。
        /// </summary>
        /// <typeparam name="TResult">反射调用返回的结果类型。</typeparam>
        /// <param name="assemblyPath">程序集路径。</param>
        /// <param name="func">反射的方法委托。</param>
        /// <returns>反射调用返回的结果。</returns>
        public TResult Reflect<TResult>(string assemblyPath, Func<Assembly, TResult> func)
        {
            // check if the assembly is found in the internal dictionaries
            if(_loadedAssemblies.ContainsKey(assemblyPath) &&
               _proxies.ContainsKey(assemblyPath))
            {
                return _proxies[assemblyPath].Reflect(func);
            }

            return default(TResult);
        }

        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 析构函数。
        /// </summary>
        ~AssemblyReflectionManager()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        /// <param name="disposing">为 true 则释放托管资源和非托管资源；为 false 则仅释放非托管资源。</param>
        protected void Dispose(bool disposing)
        {
            if(disposing)
            {
                foreach(var appDomain in _mapDomains.Values)
                    AppDomain.Unload(appDomain);

                _loadedAssemblies.Clear();
                _proxies.Clear();
                _mapDomains.Clear();
            }
        }

        private AppDomain CreateChildDomain(AppDomain parentDomain, string domainName)
        {
            Evidence evidence = new Evidence(parentDomain.Evidence);
            AppDomainSetup setup = parentDomain.SetupInformation;
            return AppDomain.CreateDomain(domainName, evidence, setup);
        }
    }

    class AssemblyReflectionProxy : MarshalByRefObject
    {
        private string _assemblyPath;

        public void LoadAssembly(String assemblyPath)
        {
            try
            {
                _assemblyPath = assemblyPath;
                Assembly.ReflectionOnlyLoadFrom(assemblyPath);
            }
            catch(FileNotFoundException)
            {
                // Continue loading assemblies even if an assembly can not be loaded in the new AppDomain.
            }
        }

        public TResult Reflect<TResult>(Func<Assembly, TResult> func)
        {
            DirectoryInfo directory = new FileInfo(_assemblyPath).Directory;
            ResolveEventHandler resolveEventHandler =
                (s, e) =>
                {
                    return OnReflectionOnlyResolve(
                        e, directory);
                };

            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += resolveEventHandler;

            var assembly = AppDomain.CurrentDomain.ReflectionOnlyGetAssemblies().FirstOrDefault(a => a.Location.CompareTo(_assemblyPath) == 0);

            var result = func(assembly);

            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve -= resolveEventHandler;

            return result;
        }

        private Assembly OnReflectionOnlyResolve(ResolveEventArgs args, DirectoryInfo directory)
        {
            Assembly loadedAssembly =
                AppDomain.CurrentDomain.ReflectionOnlyGetAssemblies()
                    .FirstOrDefault(
                      asm => string.Equals(asm.FullName, args.Name,
                          StringComparison.OrdinalIgnoreCase));

            if(loadedAssembly != null)
            {
                return loadedAssembly;
            }

            AssemblyName assemblyName =
                new AssemblyName(args.Name);
            string dependentAssemblyFilename =
                Path.Combine(directory.FullName,
                assemblyName.Name + ".dll");

            if(File.Exists(dependentAssemblyFilename))
            {
                return Assembly.ReflectionOnlyLoadFrom(
                    dependentAssemblyFilename);
            }
            return Assembly.ReflectionOnlyLoad(args.Name);
        }
    }
}
