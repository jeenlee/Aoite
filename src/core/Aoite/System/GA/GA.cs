using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;

namespace System
{
    /// <summary>
    /// 通用全局函数。
    /// </summary>
    public static partial class GA
    {
        /// <summary>
        /// 获取应用程序当前的操作系统主版本是否少于 6（XP/2003 含以下的操作系统）。
        /// </summary>
        public static readonly bool IsOldOS = Environment.OSVersion.Version.Major < 6;
        /// <summary>
        /// 获取包含该应用程序的目录的名称。该字符串结尾包含“\”。
        /// </summary>
        public static readonly string AppDirectory = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        /// <summary>
        /// 获取 Aoite 的临时目录。
        /// </summary>
        public static readonly string TempFolder = Path.Combine(Path.GetTempPath(), "Aoite - " + Aoite.AoiteInfo.AssemblyVersion);

        #region Comm

        static GA()
        {
            _IsUnitTestRuntime = (from ass in AppDomain.CurrentDomain.GetAssemblies()
                                  let fn = ass.FullName
                                  where fn.iStartsWith("Microsoft.VisualStudio.QualityTools.UnitTestFramework")
                                  || fn.iStartsWith("xunit")
                                  || fn.iStartsWith("nuint")
                                  select true).FirstOrDefault();
        }


        /// <summary>
        /// 将指定的命令行进行拆分。
        /// </summary>
        /// <param name="commandLine">命令行。</param>
        /// <returns>返回命令行。</returns>
        public static string[] ToCommandLines(string commandLine)
        {
            int numberOfArgs;
            IntPtr ptrToSplitArgs;
            string[] splitArgs;

            ptrToSplitArgs = CommandLineToArgvW(commandLine, out numberOfArgs);
            if(ptrToSplitArgs == IntPtr.Zero)
                throw new ArgumentException("Unable to split argument.",
                  new System.ComponentModel.Win32Exception());
            try
            {
                splitArgs = new string[numberOfArgs];
                for(int i = 0; i < numberOfArgs; i++)
                    splitArgs[i] = System.Runtime.InteropServices.Marshal.PtrToStringUni(
                        System.Runtime.InteropServices.Marshal.ReadIntPtr(ptrToSplitArgs, i * IntPtr.Size));
                return splitArgs;
            }
            finally
            {
                LocalFree(ptrToSplitArgs);
            }
        }

        [System.Runtime.InteropServices.DllImport("shell32.dll", SetLastError = true)]
        static extern IntPtr CommandLineToArgvW(
            [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)] string lpCmdLine,
            out int pNumArgs);

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        static extern IntPtr LocalFree(IntPtr hMem);

        private readonly static bool _IsUnitTestRuntime;
        /// <summary>
        /// 获取一个值，指示当前是否为单元测试的运行环境。
        /// </summary>
        public static bool IsUnitTestRuntime
        {
            get { return _IsUnitTestRuntime; }
        }

        /// <summary>
        /// 获取一个值，指示当前线程是否为 Web 线程。
        /// </summary>
        public static bool IsWebRuntime { get { return System.Web.HttpContext.Current != null; } }
        /// <summary>
        /// 获取一个值，该值指示当前应用程序是否以管理员权限的运行。
        /// </summary>
        public static bool IsAdministrator
        {
            get
            {
                if(GA.IsOldOS) return true;

                WindowsIdentity identity = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        /// <summary>
        /// 以管理员权限重新运行当前应用程序。
        /// </summary>
        public static void RunAsAdministrator()
        {
            if(GA.IsAdministrator) return;
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.UseShellExecute = true;
            startInfo.WorkingDirectory = Environment.CurrentDirectory;
            startInfo.FileName = Assembly.GetEntryAssembly().Location;
            startInfo.Verb = "runas";
            Process.Start(startInfo);
        }

        /// <summary>
        /// 获取指定路径的完整路径。
        /// </summary>
        /// <param name="path">绝对路径或相对路径。</param>
        /// <returns>若 <paramref name="path"/> 是绝对路径，则返回本身，否则返回基于当前应用程序目录的绝对路径。</returns>
        public static string FullPath(string path)
        {
            if(Path.IsPathRooted(path)) return path;
            return Path.Combine(AppDirectory, path);
        }

        /// <summary>
        /// 返回一个包含内容 URL 的字符串。
        /// </summary>
        /// <param name="contentPath">内容路径。</param>
        /// <returns>一个包含内容 URL 的字符串。</returns>
        public static string MapUrl(string contentPath)
        {
            return VirtualPathUtility.Combine(HttpRuntime.AppDomainAppVirtualPath
                , VirtualPathUtility.ToAbsolute(contentPath, HttpRuntime.AppDomainAppVirtualPath));
        }

        #endregion

        //- 还未融入工厂模式
        #region NewId

        /// <summary>
        /// 生成全局唯一标识（存在重复的可能性，但极低）。
        /// </summary>
        /// <returns>返回一个 <see cref="System.Int64"/> 的实例。</returns>
        [Obsolete("请调用 System.GA.NewId(string = null)")]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static long CreateUniqueIdentifier()
        {
            return BitConverter.ToInt64(Guid.NewGuid().ToByteArray(), 0);
        }

        /// <summary>
        /// 提供键的关联类型，生成一个唯一的 64 位键值。
        /// </summary>
        /// <typeparam name="T">键的关联类型。</typeparam>
        /// <returns>返回一个 64 位值。</returns>
        public static long NewId<T>()
            where T : class
        {
            return NewId(typeof(T).Name);
        }

        /// <summary>
        /// 提供一个键名，生成一个唯一的 64 位键值。
        /// </summary>
        /// <param name="key">键名。</param>
        /// <returns>返回一个 64 位值。</returns>
        public static long NewId(string key = null)
        {
            return BitConverter.ToInt64(Guid.NewGuid().ToByteArray(), 0);
        }

        #endregion

        #region Lock

        /// <summary>
        /// 原子操作形式判断 <paramref name="localtion"/> 是否与 <paramref name="value"/> 匹配。
        /// </summary>
        /// <param name="localtion">要加载的 64 位值。</param>
        /// <param name="value">要判断的 64 位值。</param>
        /// <returns>如果匹配则返回 true，否则返回 false。</returns>
        public static bool LockEquals(ref long localtion, long value)
        {
            return Interlocked.Read(ref localtion) == value;
        }

        /// <summary>
        /// 返回一个以原子操作形式加载的 64 位值。
        /// </summary>
        /// <param name="localtion">要加载的 64 位值。</param>
        /// <returns>加载的值。</returns>
        public static long LockRead(ref long localtion)
        {
            return Interlocked.Read(ref localtion);
        }

        /// <summary>
        /// 以原子操作的形式，将 64 位值设置为指定的值并返回原始 64 位值。
        /// </summary>
        /// <param name="localtion">要设置为指定值的变量。</param>
        /// <param name="value">参数被设置为的值。</param>
        /// <returns>原始值。</returns>
        public static long LockWrite(ref long localtion, long value)
        {
            return Interlocked.Exchange(ref localtion, value);
        }

        #endregion

        #region Compare

        private static CompareResult Compare(string name, Type type, object t1, object t2)
        {
            type = type.GetNullableType();
            switch(Type.GetTypeCode(type))
            {
                case TypeCode.Object:
                    if(t1 == null || t2 == null) goto default;
                    if(type.IsSubclassOf(Types.Exception) || type == Types.Exception)
                    {
                        t1 = t1.ToString();
                        t2 = t2.ToString();
                        goto default;
                    }
                    var mp = Aoite.Data.EntityMapper.Create(type);
                    foreach(var p in mp.Properties)
                    {
                        try
                        {
                            var v1 = p.GetValue(t1);
                            var v2 = p.GetValue(t2);
                            var r = Compare(p.Property.Name, p.Property.PropertyType, v1, v2);
                            if(r != null) return null;
                        }
                        catch(Exception)
                        {
                            throw;
                        }
                    }
                    break;
                default:
                    if(!object.Equals(t1, t2))
                    {
                        return new CompareResult()
                        {
                            Name = name,
                            Value1 = t1,
                            Value2 = t2
                        };
                    }
                    break;
            }
            return null;
        }
        /// <summary>
        /// 深度比较两个对象。
        /// </summary>
        /// <typeparam name="T">对象的数据类型。</typeparam>
        /// <param name="t1">第一个对象的实例。</param>
        /// <param name="t2">第二个对象的实例。</param>
        /// <returns></returns>
        public static CompareResult Compare<T>(this T t1, T t2) where T : class
        {
            var type = typeof(T);
            return Compare(type.Name, type, t1, t2);
        }

        #endregion


        /// <summary>
        /// 加载指定程序集列表的程序集（避免程序集的延迟加载）。
        /// </summary>
        /// <param name="assemblies">程序集列表。</param>
        /// <returns>返回已加载的程序集列表。</returns>
        public static Assembly[] LoadAssemblies(string assemblies)
        {
            List<Assembly> AssemblyList = new List<Assembly>();
            if(string.IsNullOrEmpty(assemblies))
            {
                AssemblyList.Add(Assembly.GetEntryAssembly());
            }
            else
            {
                foreach(var assemblyName in assemblies.Split(';'))
                {
                    Assembly assembly;
                    try
                    {
                        assembly = Assembly.Load(assemblyName);
                    }
                    catch(Exception)
                    {
                        if(!File.Exists(assemblyName)) throw new FileNotFoundException("程序集文件不存在。", assemblyName);
                        assembly = Assembly.Load(AssemblyName.GetAssemblyName(assemblyName));
                    }
                    AssemblyList.Add(assembly);
                }
            }
            return AssemblyList.ToArray();
        }

        /// <summary>
        /// 创建一个模拟对象。
        /// </summary>
        /// <typeparam name="TModel">对象的数据类型。</typeparam>
        /// <returns>返回要一个模拟的对象。</returns>
        public static TModel CreateMockModel<TModel>()
        {
            var mapper = Aoite.Data.EntityMapper.Instance<TModel>.Mapper;
            var m = Activator.CreateInstance<TModel>();
            foreach(var p in mapper.Properties)
            {
                var pType = p.Property.PropertyType.GetNullableType();
                if(pType == Types.Guid)
                {
                    p.SetValue(m, Guid.NewGuid());
                    continue;
                }
                var randomNumber = RandomString.Instance.GenerateNumber();
                if(pType.IsEnum)
                {
                    var values = Enum.GetValues(pType);
                    p.SetValue(m, values.GetValue(randomNumber % values.Length));
                    continue;
                }
                switch(Type.GetTypeCode(pType))
                {
                    case TypeCode.Boolean:
                        p.SetValue(m, randomNumber % 2 == 0);
                        break;
                    case TypeCode.Byte:
                        p.SetValue(m, (Byte)((randomNumber * 1.5 + 65535) % Byte.MaxValue));
                        break;
                    case TypeCode.Char:
                        p.SetValue(m, (Char)((randomNumber * 1.5 + 65535) % Char.MaxValue));
                        break;
                    case TypeCode.DBNull:
                        p.SetValue(m, DBNull.Value);
                        break;
                    case TypeCode.DateTime:
                        p.SetValue(m, DateTime.Now.AddMinutes(randomNumber % 1024));
                        break;
                    case TypeCode.Decimal:
                        p.SetValue(m, (Decimal)((randomNumber * 1.5M + 65535) % Decimal.MaxValue));
                        break;
                    case TypeCode.Double:
                        p.SetValue(m, (Double)((randomNumber * 1.5 + 65535) % Double.MaxValue));
                        break;
                    case TypeCode.Empty:
                        p.SetValue(m, null);
                        break;
                    case TypeCode.Int16:
                        p.SetValue(m, (Int16)((randomNumber * 1.5 + 65535) % Int16.MaxValue));
                        break;
                    case TypeCode.Int32:
                        p.SetValue(m, (Int32)((randomNumber * 1.5 + 65535) % Int32.MaxValue));
                        break;
                    case TypeCode.Int64:
                        p.SetValue(m, (Int64)((randomNumber * 1.5 + 65535) % Int64.MaxValue));
                        break;
                    case TypeCode.SByte:
                        p.SetValue(m, (SByte)((randomNumber * 1.5 + 65535) % SByte.MaxValue));
                        break;
                    case TypeCode.Single:
                        p.SetValue(m, (Single)((randomNumber * 1.5 + 65535) % Single.MaxValue));
                        break;
                    case TypeCode.String:
                        p.SetValue(m, RandomString.Instance.Generate(randomNumber % 30 + 3));
                        break;
                    case TypeCode.UInt16:
                        p.SetValue(m, (UInt16)((randomNumber * 1.5 + 65535) % UInt16.MaxValue));
                        break;
                    case TypeCode.UInt32:
                        p.SetValue(m, (UInt32)((randomNumber * 1.5 + 65535) % UInt32.MaxValue));
                        break;
                    case TypeCode.UInt64:
                        p.SetValue(m, (UInt64)((randomNumber * 1.5 + 65535) % UInt64.MaxValue));
                        break;
                    default:
                        throw new NotSupportedException(pType.FullName);
                }
            }
            return m;
        }

    }
}
