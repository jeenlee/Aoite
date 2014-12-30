using System.Collections.Generic;
using System.Text;

namespace System
{
    /// <summary>
    /// 定义一个日志。
    /// </summary>
    public interface ILogEntry
    {
        /// <summary>
        /// 获取日志时间。
        /// </summary>
        DateTime Time { get; }
        /// <summary>
        /// 获取标志性字符串。通常指的是用户名或用户编号。
        /// </summary>
        string Identity { get; set; }

        /// <summary>
        /// 获取单行的日志信息。
        /// </summary>
        string ToSingle();
    }
}
