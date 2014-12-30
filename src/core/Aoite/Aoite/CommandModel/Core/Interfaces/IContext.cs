using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Aoite.CommandModel
{
    /// <summary>
    /// 定义一个执行命令模型的上下文。
    /// </summary>
    public interface IContext : IContainerProvider
    {
        /// <summary>
        /// 获取执行命令模型的用户。该属性可能返回 null 值。
        /// </summary>
        dynamic User { get; }
        /// <summary>
        /// 获取执行命令模型的其他参数，参数名称若为字符串则不区分大小写的序号字符串比较。
        /// </summary>
        HybridDictionary Data { get; }
        /// <summary>
        /// 获取正在执行的命令模型。
        /// </summary>
        ICommand Command { get; }
        /// <summary>
        /// 获取或设置键的值。
        /// </summary>
        /// <param name="key">键。</param>
        /// <returns>返回一个值。</returns>
        object this[object key] { get; set; }
    }
}
