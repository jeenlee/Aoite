using System.Collections.Generic;
using System.Text;

namespace System
{
    /// <summary>
    /// 基于定时器的日志提供程序。
    /// </summary>
    public abstract class TimerLog : Log
    {
        private readonly static System.Threading.ReaderWriterLock RWL = new System.Threading.ReaderWriterLock();
        private readonly List<ILogEntry> Logs = new List<ILogEntry>();
        private const int LockTimeout = 5 * 1000;

        /// <summary>
        /// 获取一个值，指示还有多少日志尚未输出。
        /// </summary>
        public int NonOutputCount
        {
            get
            {
                RWL.AcquireReaderLock(LockTimeout);
                try
                {
                    return Logs.Count;
                }
                finally
                {
                    RWL.ReleaseReaderLock();
                }
            }
        }

        /// <summary>
        /// 默认定时 1 秒，初始化一个 <see cref="System.TimerLog"/> 类的新实例。
        /// </summary>
        public TimerLog() : this(1000.0) { }

        /// <summary>
        /// 指定定时器的间隔时间，初始化一个 <see cref="System.TimerLog"/> 类的新实例。
        /// </summary>
        /// <param name="interval">事件之间经过的时间（以毫秒为单位）。</param>
        public TimerLog(double interval)
        {
            Ajob.Loop(OnTimer, interval / 1000);
        }

        private void OnTimer(IAsyncJob token)
        {
            try
            {
                RWL.AcquireWriterLock(LockTimeout);
                if(Logs.Count > 0)
                {
                    try
                    {
                        this.Write(Logs.ToArray());
                    }
                    catch(Exception ex)
                    {
                        var path = GA.IsWebRuntime ? GA.MapUrl("~/LogError.txt") : GA.FullPath("LogError.txt");
                        System.IO.File.AppendAllText(path, "{0} {1}".Fmt(DateTime.Now, ex.ToString()));
                    }
                    Logs.Clear();
                }
                RWL.ReleaseWriterLock();
            }
            catch(ApplicationException) { }
        }

        /// <summary>
        /// 将指定对象集合批量写入日志。
        /// </summary>
        /// <param name="descs">日志对象的集合。</param>
        protected abstract void Write(ILogEntry[] descs);

        /// <summary>
        /// 将指定对象写入日志。
        /// </summary>
        /// <param name="desc">一个日志对象。</param>
        protected override void Write(ILogEntry desc)
        {
            try
            {
                RWL.AcquireWriterLock(LockTimeout);
                Logs.Add(desc);
                RWL.ReleaseWriterLock();
            }
            catch(ApplicationException) { }
        }
    }

}
