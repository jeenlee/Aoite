using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Aoite.Log
{
    /// <summary>
    /// 基于文件系统的定时器的日志提供程序。
    /// </summary>
    public class IoTimerLog : TimerLog
    {
        private readonly object _webSyncObject = new object();

        private string _logFolder;
        private string _logExtension;
        private Encoding _encoding;
        private LogFileGroup _mode;
        /// <summary>
        /// 获取或设置一个值，指示是否为单行模式。
        /// </summary>
        public bool SingleLine { get; set; }
        /// <summary>
        /// 初始化一个默认的 <see cref="Aoite.Log.IoTimerLog"/> 类的新实例。
        /// </summary>
        public IoTimerLog() : this(GA.FullPath("Logs")) { }
        /// <summary>
        /// 初始化一个默认的 <see cref="Aoite.Log.IoTimerLog"/> 类的新实例。
        /// </summary>
        /// <param name="mode">日志的文件模式。</param>
        public IoTimerLog(LogFileGroup mode) : this(mode, GA.FullPath("Logs")) { }
        /// <summary>
        /// 初始化一个 <see cref="Aoite.Log.IoTimerLog"/> 类的新实例。
        /// </summary>
        /// <param name="logFolder">日志所在的目录。</param>
        public IoTimerLog(string logFolder) : this(LogFileGroup.Month, logFolder) { }
        /// <summary>
        /// 初始化一个默认的 <see cref="Aoite.Log.IoTimerLog"/> 类的新实例。
        /// </summary>
        /// <param name="mode">日志的文件模式。</param>
        /// <param name="logFolder">日志所在的目录。</param>
        public IoTimerLog(LogFileGroup mode, string logFolder) : this(mode, logFolder, ".log", Encoding.UTF8, 1000) { }

        /// <summary>
        /// 初始化一个 <see cref="Aoite.Log.IoTimerLog"/> 类的新实例。
        /// </summary>
        /// <param name="mode">日志的文件模式。</param>
        /// <param name="logFolder">日志所在的目录。</param>
        /// <param name="logExtension">日志文件的后缀名。</param>
        /// <param name="encoding">日志文件默认的编码格式。</param>
        /// <param name="interval">事件之间经过的时间（以毫秒为单位）。</param>
        public IoTimerLog(LogFileGroup mode, string logFolder, string logExtension, Encoding encoding, double interval)
            : base(interval)
        {
            this._mode = mode;
            this._logFolder = logFolder;
            this._logExtension = logExtension;
            this._encoding = encoding;
        }

        private string CreateLogPath()
        {
            var now = DateTime.Now;
            string folder;
            string fileName;
            switch(_mode)
            {
                case LogFileGroup.Day:
                    var s_now = now.ToString("yyyy年MM月dd日");
                    folder = Path.Combine(this._logFolder, s_now);
                    fileName = s_now + this._logExtension;
                    break;

                case LogFileGroup.Year:
                    folder = Path.Combine(this._logFolder, now.ToString("yyyy年"));
                    fileName = now.ToString("MM月dd日") + this._logExtension;
                    break;
                case LogFileGroup.Hour:
                    folder = Path.Combine(this._logFolder, now.ToString("yyyy年MM月"), now.ToString("dd"));
                    fileName = now.ToString("HH") + this._logExtension;
                    break;
                default:
                case LogFileGroup.Month:
                    folder = Path.Combine(this._logFolder, now.ToString("yyyy年MM月"));
                    fileName = now.ToString("dd") + this._logExtension;
                    break;
            }

            GA.IO.CreateDirectory(folder);
            return Path.Combine(folder, fileName);

        }

        private StreamWriter CreateWriter()
        {
            return new StreamWriter(new FileStream(this.CreateLogPath(), FileMode.Append, FileAccess.Write, FileShare.ReadWrite), this._encoding);
        }

        readonly static string breakLine1 = new string('=', 60);
        readonly static string breakLine2 = new string('*', 60);

        /// <summary>
        /// 将指定对象集合批量写入日志。
        /// </summary>
        /// <param name="descs">日志对象的集合。</param>
        protected override void Write(ILogEntry[] descs)
        {
            if(SingleLine)
            {
                using(var writer = CreateWriter())
                {
                    foreach(var item in descs)
                    {
                        writer.WriteLine(item.ToSingle());
                    }
                }
            }
            else
            {
                using(var writer = CreateWriter())
                {
                    foreach(var item in descs)
                    {
                        writer.WriteLine(breakLine1);
                        writer.WriteLine(Convert.ToString(item));
                        writer.WriteLine(breakLine1);
                        writer.WriteLine(breakLine2);
                    }
                }
            }
        }
    }
}
