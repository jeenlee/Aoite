using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace System
{
    /// <summary>
    /// 表示日志的描述。
    /// </summary>
    public class LogEntry : ILogEntry
    {
        /// <summary>
        /// 初始化一个 <see cref="System.LogEntry"/> 类的新实例。
        /// </summary>
        public LogEntry() { }

        private DateTime _Time = DateTime.Now;
        /// <summary>
        /// 获取一个值，表示日志的日期时间。
        /// </summary>
        public DateTime Time
        {
            get
            {
                return this._Time;
            }
        }

        /// <summary>
        /// 获取标志性字符串。通常指的是用户名或用户编号。
        /// </summary>
        public string Identity { get; set; }

        /// <summary>
        /// 获取或设置日志的来源。
        /// </summary>
        public string Source { get; set; }

        internal LogType _Type;
        /// <summary>
        /// 获取或设置一个值，表示日志的类型。
        /// </summary>
        public LogType Type
        {
            get
            {
                return this._Type;
            }
            set
            {
                this._Type = value;
            }
        }

        internal string _Message;
        /// <summary>
        /// 获取或设置一个值，表示日志的消息。
        /// </summary>
        public string Message
        {
            get
            {
                return this._Message;
            }
            set
            {
                this._Message = value;
            }
        }

        /// <summary>
        /// 返回描述当前日志信息的字符串。
        /// </summary>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            if(!string.IsNullOrEmpty(this.Source)) builder.Append("[日志来源]：").AppendLine(this.Source);
            builder.Append("[日志时间]：").Append(this.Time).AppendLine();
            builder.Append("[日志类型]：").Append(this.Type).AppendLine();
            if(!string.IsNullOrEmpty(this.Identity)) builder.Append("[关键标识]：").AppendLine(this.Identity);
            builder.Append("[日志消息]：").AppendLine(this.Message);
            return builder.ToString();
        }
        string ILogEntry.ToSingle()
        {
            return this.Time.ToString("HH:mm:ss.ffff") + " " + this.Type + "：" + this.Message;
        }

    }

}
