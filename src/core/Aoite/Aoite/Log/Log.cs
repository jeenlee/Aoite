using System.Collections.Generic;
using System.Text;

namespace System
{
    /// <summary>
    /// 日志。
    /// </summary>
    public static class Logger
    {
        /// <summary>
        /// 获取或设置日志源。
        /// </summary>
        public static string LogSource;
        /// <summary>
        /// 获取或设置一个值，指示是否开启线程缓冲模式。
        /// </summary>
        public static bool EnabledFlush;

        internal static void Write(LogType type, string message, params object[] args)
        {
            Log.Write(LogSource, type, message, args);
        }
        /// <summary>
        /// 写入调试日志。
        /// </summary>
        /// <param name="message">日志的信息。</param>
        /// <param name="args">包含零个或多个要格式化的对象的 <see cref="System.Object"/> 数组。</param>
        [System.Diagnostics.Conditional("DEBUG")]
        public static void Debug(string message, params object[] args)
        {
            Write(LogType.Debug, message, args);
        }

        /// <summary>
        /// 写入消息日志。
        /// </summary>
        /// <param name="message">日志的信息。</param>
        /// <param name="args">包含零个或多个要格式化的对象的 <see cref="System.Object"/> 数组。</param>
        public static void Info(string message, params object[] args)
        {
            Write(LogType.Info, message, args);
        }

        /// <summary>
        /// 写入警告日志。
        /// </summary>
        /// <param name="message">日志的信息。</param>
        /// <param name="args">包含零个或多个要格式化的对象的 <see cref="System.Object"/> 数组。</param>
        public static void Warn(string message, params object[] args)
        {
            Write(LogType.Warn, message, args);
        }

        /// <summary>
        /// 写入错误日志。
        /// </summary>
        /// <param name="exception">日志的异常信息。</param>
        public static void Error(Exception exception)
        {
            if(exception == null) return;
            Error(exception.ToString());
        }

        /// <summary>
        /// 写入错误日志。
        /// </summary>
        /// <param name="message">日志的信息。</param>
        /// <param name="args">包含零个或多个要格式化的对象的 <see cref="System.Object"/> 数组。</param>
        public static void Error(string message, params object[] args)
        {
            Write(LogType.Error, message, args);
        }

        /// <summary>
        /// 写入配置错误日志。
        /// </summary>
        /// <param name="message">日志的信息。</param>
        /// <param name="args">包含零个或多个要格式化的对象的 <see cref="System.Object"/> 数组。</param>
        public static void ConfigError(string message, params object[] args)
        {
            Write(LogType.ConfigError, message, args);
        }

        /// <summary>
        /// 写入致命错误日志。
        /// </summary>
        /// <param name="message">日志的信息。</param>
        /// <param name="args">包含零个或多个要格式化的对象的 <see cref="System.Object"/> 数组。</param>
        public static void FatalError(string message, params object[] args)
        {
            Write(LogType.FatalError, message, args);
        }
    }


    /// <summary>
    /// 日志提供程序的基类。
    /// </summary>
    public abstract class Log
    {
        /// <summary>
        /// 当日志需要访问唯一标识时发生。
        /// </summary>
        public static event Func<ILogEntry, string> IdentityAccessor;

        internal class EmptyLog : Log
        {
            protected override void Write(ILogEntry desc)
            {
                Console.WriteLine(desc.ToString());
            }
        }

        private readonly static EmptyLog Empty = new EmptyLog();
        private static Log _Instance = new Aoite.Log.IoTimerLog();
        /// <summary>
        /// 获取或设置默认的提供程序。该属性的设置必须在应用程序启动时。
        /// </summary>
        public static Log Instance
        {
            get
            {
                return _Instance;
            }
            set
            {
                if(value == null) value = Empty;
                _Instance = value;
            }
        }

        /// <summary>
        /// 将指定对象写入日志。
        /// </summary>
        /// <param name="desc">一个日志对象。</param>
        protected abstract void Write(ILogEntry desc);

        /// <summary>
        /// 日志写入前发生。
        /// </summary>
        public event EventHandler BeforeWrite;
        /// <summary>
        /// 日志写入前发生的方法。
        /// </summary>
        /// <param name="desc">一个日志对象。</param>
        protected virtual void OnBeforeWrite(ILogEntry desc)
        {
            var handler = this.BeforeWrite;
            if(handler != null) handler(desc, EventArgs.Empty);
        }

        internal static void Write(string source, LogType type, string message, params object[] args)
        {
            var desc = new LogEntry()
            {
                _Type = type,
                _Message = (args == null || args.Length == 0) ? message : string.Format(message, args),
            };
            desc.Source = source;
            if(IdentityAccessor != null) desc.Identity = IdentityAccessor(desc);
            if(Logger.EnabledFlush)
            {
                lock(local.Value)
                {
                    local.Value.Add(desc);
                }
            }
            else
            {
                var instance = Log.Instance;
                instance.OnBeforeWrite(desc);
                instance.Write(desc);
            }
        }
        private static Threading.ThreadLocal<List<LogEntry>> local = new Threading.ThreadLocal<List<LogEntry>>(() => new List<LogEntry>());
        /// <summary>
        /// 将日志写入文件。
        /// </summary>
        public static void Flush()
        {
            var instance = Log.Instance;
            LogEntry[] logs;
            lock(local.Value)
            {
                logs = local.Value.ToArray();
                local.Value.Clear();
            }
            foreach(var desc in logs)
            {

                instance.OnBeforeWrite(desc);
                instance.Write(desc);
            }
        }

        /// <summary>
        /// 写入自定义日志。
        /// </summary>
        /// <param name="desc">日志的信息。</param>
        public static void Custom(ILogEntry desc)
        {
            if(IdentityAccessor != null) desc.Identity = IdentityAccessor(desc);
            var instance = Log.Instance;
            instance.OnBeforeWrite(desc);
            instance.Write(desc);
        }
    }
}
