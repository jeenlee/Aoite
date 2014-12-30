using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.CommandModel
{
    /// <summary>
    /// 定义一个命令模型的执行器。
    /// </summary>
    /// <typeparam name="TCommand">命令模型的数据类型。</typeparam>
    public interface IExecutor<TCommand> : IExecutor where TCommand : ICommand
    {
        /// <summary>
        /// 执行一个命令模型。
        /// </summary>
        /// <param name="context">命令模型的上下文。</param>
        /// <param name="command">命令模型。</param>
        void Execute(IContext context, TCommand command);
    }
}
