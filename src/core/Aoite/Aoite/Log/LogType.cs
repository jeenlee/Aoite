using System.Collections.Generic;
using System.Text;

namespace System
{

    /// <summary>
    /// 定义日志类型。
    /// </summary>
    public enum LogType
    {
        /// <summary>
        /// 表示调试的日志。
        /// </summary>
        Debug = -1,
        /// <summary>
        /// 表示消息的日志。
        /// </summary>
        Info = 0,
        /// <summary>
        /// 表示警告的日志。
        /// </summary>
        Warn = 1,
        /// <summary>
        /// 表示普通错误的日志。
        /// </summary>
        Error = 2,
        /// <summary>
        /// 表示配置错误的日志。
        /// </summary>
        ConfigError = 3,
        /// <summary>
        /// 表示致命错误的日志。
        /// </summary>
        FatalError = 4
    }

}
