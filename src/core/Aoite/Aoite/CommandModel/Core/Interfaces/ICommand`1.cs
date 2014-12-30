using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.CommandModel
{
    /// <summary>
    /// 表示一个包含返回值的命令模型。
    /// </summary>
    /// <typeparam name="TResultValue">返回值的数据类型。</typeparam>
    public interface ICommand<TResultValue> : ICommand
    {
        /// <summary>
        /// 获取或设置命令模型的执行后的返回值。
        /// </summary>
        TResultValue ResultValue { get; set; }
    }
}
