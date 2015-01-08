using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
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
        /// <summary>
        /// 当要求抛出错误时的全局事件。
        /// </summary>
        public static event ExceptionEventHandler GlobalError;

        internal static void OnGlobalError(object sender, Exception exception)
        {
            System.Diagnostics.Trace.Assert(sender != null);
            System.Diagnostics.Trace.Assert(exception != null);
            System.Diagnostics.Trace.TraceError(exception.ToString());
            var handler = GlobalError;
            if(handler != null) handler(sender, new ExceptionEventArgs(exception));
        }

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

        [DllImport("rpcrt4.dll", SetLastError = true)]
        static extern int UuidCreateSequential(out Guid guid);

        /// <summary>
        /// 初始化一个有顺序规则 <see cref="System.Guid"/> 的新实例。
        /// </summary>
        /// <returns>返回一个 <see cref="System.Guid"/> 的新实例。</returns>
        public static Guid NewComb()
        {
            const int RPC_S_OK = 0;
            Guid guid;
            int result = UuidCreateSequential(out guid);
            if(result == RPC_S_OK)  return guid;
            else
                return Guid.NewGuid();
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

        private static bool _IsUnitTestRuntime;
        /// <summary>
        /// 获取或设置一个值，指示当前是否为单元测试的运行环境。
        /// </summary>
        public static bool IsUnitTestRuntime
        {
            get { return _IsUnitTestRuntime; }
            set { _IsUnitTestRuntime = value; }
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
            if(string.IsNullOrEmpty(path)) throw new ArgumentNullException("path");

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
            if(string.IsNullOrEmpty(contentPath)) throw new ArgumentNullException("contentPath");
            return VirtualPathUtility.Combine(HttpRuntime.AppDomainAppVirtualPath
                , VirtualPathUtility.ToAbsolute(contentPath, HttpRuntime.AppDomainAppVirtualPath));
        }

        /// <summary>
        /// 使用指定的对象数组和格式设置信息向 <see cref="System.Diagnostics.Trace.Listeners"/> 集合中的跟踪侦听器中写入错误消息。
        /// </summary>
        /// <param name="format">包含零个或多个格式项的格式字符串，这些项与 <paramref name="args"/> 数组中的对象相对应。</param>
        /// <param name="args">包含零个或多个要格式化的对象的 <see cref="object"/> 数组。</param>
        [Conditional("TRACE")]
        public static void TraceError(string format, params object[] args)
        {
            System.Diagnostics.Trace.TraceError(format, args);
        }

        /// <summary>
        /// 使用指定的对象数组和格式设置信息向 <see cref="System.Diagnostics.Trace.Listeners"/> 集合中的跟踪侦听器中写入警告信息。
        /// </summary>
        /// <param name="format">包含零个或多个格式项的格式字符串，这些项与 <paramref name="args"/> 数组中的对象相对应。</param>
        /// <param name="args">包含零个或多个要格式化的对象的 <see cref="object"/> 数组。</param>
        [Conditional("TRACE")]
        public static void TraceWarning(string format, params object[] args)
        {
            System.Diagnostics.Trace.TraceWarning(format, args);
        }

        /// <summary>
        /// 使用指定的对象数组和格式设置信息向 <see cref="System.Diagnostics.Trace.Listeners"/> 集合中的跟踪侦听器中写入信息性消息。
        /// </summary>
        /// <param name="format">包含零个或多个格式项的格式字符串，这些项与 <paramref name="args"/> 数组中的对象相对应。</param>
        /// <param name="args">包含零个或多个要格式化的对象的 <see cref="object"/> 数组。</param>
        [Conditional("TRACE")]
        public static void TraceInformation(string format, params object[] args)
        {
            System.Diagnostics.Trace.TraceInformation(format, args);
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

        //private static CompareResult Compare(string name, Type type, object t1, object t2)
        //{
        //    type = type.GetNullableType();
        //    switch(Type.GetTypeCode(type))
        //    {
        //        case TypeCode.Object:
        //            if(t1 == null || t2 == null) goto default;
        //            if(type.IsSubclassOf(Types.Exception) || type == Types.Exception)
        //            {
        //                t1 = t1.ToString();
        //                t2 = t2.ToString();
        //                goto default;
        //            }
        //            var mp = Aoite.Data.EntityMapper.Create(type);
        //            foreach(var p in mp.Properties)
        //            {
        //                try
        //                {
        //                    var v1 = p.GetValue(t1);
        //                    var v2 = p.GetValue(t2);
        //                    var r = Compare(p.Property.Name, p.Property.PropertyType, v1, v2);
        //                    if(r != null) return null;
        //                }
        //                catch(Exception)
        //                {
        //                    throw;
        //                }
        //            }
        //            break;
        //        default:
        //            if(!object.Equals(t1, t2))
        //            {
        //                return new CompareResult()
        //                {
        //                    Name = name,
        //                    Value1 = t1,
        //                    Value2 = t2
        //                };
        //            }
        //            break;
        //    }
        //    return null;
        //}

        ///// <summary>
        ///// 深度比较两个对象。
        ///// </summary>
        ///// <typeparam name="T">对象的数据类型。</typeparam>
        ///// <param name="t1">第一个对象的实例。</param>
        ///// <param name="t2">第二个对象的实例。</param>
        ///// <returns></returns>
        //public static CompareResult Compare<T>(this T t1, T t2) where T : class
        //{
        //    var type = typeof(T);
        //    return Compare(type.Name, type, t1, t2);
        //}

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
        /// 创建一个指定常用数据类型的随机值。
        /// </summary>
        /// <typeparam name="TValue">常用的数据类型。</typeparam>
        /// <returns>如果返回默认值，表示不支持此类型的随机生成。</returns>
        public static TValue CreateMockValue<TValue>()
        {
            var value = CreateMockValue(typeof(TValue));
            if(value == null) return default(TValue);
            return (TValue)value;
        }
        /// <summary>
        /// 创建一个指定常用数据类型的随机值。
        /// </summary>
        /// <param name="type">常用的数据类型。</param>
        /// <returns>如果返回 null 值，表示不支持此类型的随机生成。</returns>
        public static object CreateMockValue(Type type)
        {
            if(type == null) throw new ArgumentNullException("valueType");

            type = type.GetNullableType();

            if(type == Types.Guid) return Guid.NewGuid();
            var random = FastRandom.Instance;

            if(type.IsEnum)
            {
                var values = Enum.GetValues(type);
                return values.GetValue(random.Next() % values.Length);
            }
            if(type == Types.String) random.NextString(random.Next() % 30 + random.Next(3, 6));
            if(type == Types.Uri) return new Uri("www." + random.NextString(random.Next() % 10 + random.Next(3, 6)) + ".com");
            if(type == Types.TimeSpan) return TimeSpan.FromMinutes(random.NextDouble() % random.Next(124, 1025));
            if(type == Types.DateTime) return DateTime.Now.AddMinutes(random.NextDouble() % random.Next(124, 1025));
            if(type == Types.DateTimeOffset) return DateTimeOffset.Now.AddMinutes(random.NextDouble() % random.Next(124, 1025));
            if(type == Types.Boolean) return random.NextBool();

            var randomNumber = random.Next(1, 65535) * (random.NextDouble() + 2.0) + 65535;
            switch(Type.GetTypeCode(type))
            {
                case TypeCode.Decimal: return (Decimal)((decimal)randomNumber % Decimal.MaxValue);
                case TypeCode.Byte: return (Byte)(randomNumber % Byte.MaxValue);
                case TypeCode.Char: return (Char)(randomNumber % Char.MaxValue);
                case TypeCode.Double: return (Double)(randomNumber % Double.MaxValue);
                case TypeCode.Int16: return (Int16)(randomNumber % Int16.MaxValue);
                case TypeCode.Int32: return (Int32)(randomNumber % Int32.MaxValue);
                case TypeCode.Int64: return (Int64)(randomNumber % Int64.MaxValue);
                case TypeCode.SByte: return (SByte)(randomNumber % SByte.MaxValue);
                case TypeCode.Single: return (Single)(randomNumber % Single.MaxValue);
                case TypeCode.UInt16: return (UInt16)(randomNumber % UInt16.MaxValue);
                case TypeCode.UInt32: return (UInt32)(randomNumber % UInt32.MaxValue);
                case TypeCode.UInt64: return (UInt64)(randomNumber % UInt64.MaxValue);
            }
            return null;
        }

        /// <summary>
        /// 创建一个模拟对象。
        /// </summary>
        /// <typeparam name="TModel">对象的数据类型。</typeparam>
        /// <returns>返回要一个模拟的对象。</returns>
        public static TModel CreateMockModel<TModel>()
        {
            var mapper = TypeMapper.Instance<TModel>.Mapper;
            var m = Activator.CreateInstance<TModel>();
            foreach(var p in mapper.Properties)
            {
                var value = CreateMockValue(p.Property.PropertyType);
                if(value != null) p.SetValue(m, value);
            }
            return m;
        }
    }
}
