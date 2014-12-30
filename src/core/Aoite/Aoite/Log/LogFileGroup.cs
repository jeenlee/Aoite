using System;
using System.Collections.Generic;
using System.Text;

namespace Aoite.Log
{
    /// <summary>
    /// 定义文件日志的分组模式。
    /// </summary>
    public enum LogFileGroup
    {
        /// <summary>
        /// 表示按月进行日志分组。
        /// </summary>
        Month = 0,
        /// <summary>
        /// 表示按天进行日志分组。
        /// </summary>
        Day = 1,
        /// <summary>
        /// 表示按年进行日志分组。
        /// </summary>
        Year = 2,
        /// <summary>
        /// 表示按小时进行日志分组。
        /// </summary>
        Hour=3,
    }
}
