using Aoite.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// 表示一个日志的公共管理器。
    /// </summary>
    public static class Log
    {
        static Log()
        {
            Logger = ObjectFactory.Global.GetService<ILogger>();
        }

        /// <summary>
        /// 获取或设置日志的管理器。
        /// </summary>
        public static ILogger Logger { get; set; }

        private static readonly System.Threading.ThreadLocal<LogContext> _threadLocalContent = new System.Threading.ThreadLocal<LogContext>();
        internal static void ResetContext()
        {
            _threadLocalContent.Value = null;
        }
        /// <summary>
        /// 获取一个值，指示当前上下文在线程中是否已创建。
        /// </summary>
        public static bool IsThreadContext { get { return _threadLocalContent.Value != null; } }
        /// <summary>
        /// 创建并返回一个 <see cref="System.LogContext"/>。返回当前线程上下文包含的 <see cref="System.LogContext"/> 或创建一个新的  <see cref="System.LogContext"/>。
        /// <para>当释放一个 <see cref="System.LogContext"/> 后，下一次调用获取将会重新创建上下文。</para>
        /// </summary>
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        public static LogContext Context
        {
            get
            {
                if(_threadLocalContent.Value == null) _threadLocalContent.Value = new LogContext(Logger);
                return _threadLocalContent.Value;
            }
        }

        internal static void Write(string type, string message, params object[] args)
        {
            var logger = Logger;
            if(logger == null) return;
            logger.Write(new LogItem() { Type = type, Time = DateTime.Now, Message = string.Format(message, args) });
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
    }

    /// <summary>
    /// 表示一个日志的上下文。直至上下文释放后才会批量写入到日志。
    /// <para>通过日志上下文，可以保证日志在时间上的连贯性。</para>
    /// </summary>
    public class LogContext : ObjectDisposableBase
    {
        private System.Collections.Concurrent.ConcurrentBag<LogItem> _Items = new Collections.Concurrent.ConcurrentBag<LogItem>();
        /// <summary>
        /// 获取当前上下文的所有日志项。
        /// </summary>
        public IEnumerable<LogItem> Items { get { return this._Items.ToArray(); } }

        private ILogger _logger;
        internal LogContext(ILogger logger)
        {
            this._logger = logger;
        }

        internal void Write(string type, string message, params object[] args)
        {
            this.ThrowWhenDisposed();

            this._Items.Add(new LogItem() { Type = type, Time = DateTime.Now, Message = string.Format(message, args) });
        }

        /// <summary>
        /// 写入消息日志。
        /// </summary>
        /// <param name="message">日志的信息。</param>
        /// <param name="args">包含零个或多个要格式化的对象的 <see cref="System.Object"/> 数组。</param>
        public void Info(string message, params object[] args)
        {
            Write(LogType.Info, message, args);
        }

        /// <summary>
        /// 写入警告日志。
        /// </summary>
        /// <param name="message">日志的信息。</param>
        /// <param name="args">包含零个或多个要格式化的对象的 <see cref="System.Object"/> 数组。</param>
        public void Warn(string message, params object[] args)
        {
            Write(LogType.Warn, message, args);
        }

        /// <summary>
        /// 写入错误日志。
        /// </summary>
        /// <param name="exception">日志的异常信息。</param>
        public void Error(Exception exception)
        {
            if(exception == null) return;
            Error(exception.ToString());
        }

        /// <summary>
        /// 写入错误日志。
        /// </summary>
        /// <param name="message">日志的信息。</param>
        /// <param name="args">包含零个或多个要格式化的对象的 <see cref="System.Object"/> 数组。</param>
        public void Error(string message, params object[] args)
        {
            Write(LogType.Error, message, args);
        }

        /// <summary>
        /// 执行与释放或重置托管资源相关的应用程序定义的任务。
        /// </summary>
        protected override void DisposeManaged()
        {
            if(this._logger != null) { this._logger.Write(this._Items.ToArray()); }
            this._logger = null;
            this._Items = null;
            Log.ResetContext();
            base.DisposeManaged();
        }
    }
}
