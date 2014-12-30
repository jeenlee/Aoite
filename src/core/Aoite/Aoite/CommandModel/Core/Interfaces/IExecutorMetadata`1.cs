using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.CommandModel
{
    /// <summary>
    /// 定义一个命令模型执行器的元数据。
    /// </summary>
    /// <typeparam name="TCommand">命令模型的数据类型。</typeparam>
    public interface IExecutorMetadata<TCommand> : IExecutorMetadata where TCommand : ICommand
    {
        /// <summary>
        /// 获取命令模型的执行器。
        /// </summary>
        new IExecutor<TCommand> Executor { get; }
    }
}
