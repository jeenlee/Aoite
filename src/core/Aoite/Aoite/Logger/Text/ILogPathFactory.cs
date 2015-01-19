using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Logger
{
    /// <summary>
    /// 定义一个日志的路径生成工厂。
    /// </summary>
    [DefaultMapping(typeof(DayLogPathFactory))]
    public interface ILogPathFactory
    {
        /// <summary>
        /// 指定当前时间，判断路径是否已创建。
        /// </summary>
        /// <param name="now">当前时间。</param>
        /// <returns>如果路径已创建返回 true，否则返回 false。</returns>
        bool IsCreated(DateTime now);
        /// <summary>
        /// 创建指定时间、日志目录和日志后缀名，创建一个路径。
        /// </summary>
        /// <param name="now">当前时间。</param>
        /// <param name="logFolder">日志的目录。</param>
        /// <param name="logExtension">日志的后缀名。</param>
        /// <returns>返回一个日志的路径。</returns>
        string CreatePath(DateTime now, string logFolder, string logExtension);
    }
}
