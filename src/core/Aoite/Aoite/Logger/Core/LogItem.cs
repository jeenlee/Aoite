using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Logger
{
    /// <summary>
    /// 表示一个日志的信息。
    /// </summary>
    public class LogItem
    {
        /// <summary>
        /// 获取或设置一个值，表示日志的类型。
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 获取一个值，表示日志的日期时间。
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// 获取或设置一个值，表示日志的消息。
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 初始化一个 <see cref="Aoite.Logger.LogItem"/> 类的新实例。
        /// </summary>
        public LogItem()
        {
            this.Time = DateTime.Now;
        }

        /// <summary>
        /// 返回描述当前日志信息的短字符串。
        /// </summary>
        public override string ToString()
        {
            return this.Time.ToString("HH:mm:ss.ffff") + " [" + this.Type + "] " + this.Message;
        }
    }
}
