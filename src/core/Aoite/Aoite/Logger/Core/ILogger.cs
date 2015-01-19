using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Logger
{
    /// <summary>
    /// 定义一个日志管理器。
    /// </summary>
    [DefaultMapping(typeof(TextLogger))]
    public interface ILogger
    {
        /// <summary>
        /// 写入一个或多个日志项。
        /// </summary>
        /// <param name="items">日志项的数组。</param>
        void Write(params LogItem[] items);
    }
}
