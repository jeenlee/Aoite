using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Logger
{
    /// <summary>
    /// 定义一个日志描述器。
    /// </summary>
    [DefaultMapping(typeof(DefaultLogDescriptor))]
    public interface ILogDescriptor
    {
        /// <summary>
        /// 描述指定日志管理器的日志项。
        /// </summary>
        /// <param name="logger">日志管理器。</param>
        /// <param name="item">日志项。</param>
        /// <returns>返回日志项的字符串形式。</returns>
        string Describe(ILogger logger, LogItem item);
    }

    /// <summary>
    /// 表示一个默认的日志描述器。
    /// </summary>
    public class DefaultLogDescriptor : ILogDescriptor
    {
        /// <summary>
        /// 初始化一个 <see cref="Aoite.Logger.DefaultLogDescriptor"/> 类的新实例。
        /// </summary>
        public DefaultLogDescriptor() { }

        /// <summary>
        /// 描述指定日志管理器的日志项。
        /// </summary>
        /// <param name="logger">日志管理器。</param>
        /// <param name="item">日志项。</param>
        /// <returns>返回日志项的字符串形式。</returns>
        public virtual string Describe(ILogger logger, LogItem item)
        {
            if(logger == null) throw new ArgumentNullException("logger");
            if(item == null) throw new ArgumentNullException("item");
            return item.ToString();
        }
    }
}
