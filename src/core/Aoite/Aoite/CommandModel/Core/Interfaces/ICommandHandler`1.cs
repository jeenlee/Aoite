using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.CommandModel
{
    /// <summary>
    /// 定义一个命令的处理程序。
    /// </summary>
    /// <typeparam name="TCommand">命令模型的数据类型。</typeparam>
    public interface ICommandHandler<TCommand> where TCommand : ICommand
    {
        /// <summary>
        /// 命令模型执行前发生的方法。
        /// </summary>
        /// <param name="context">执行的上下文。</param>
        /// <param name="command">执行的命令模型。</param>
        /// <returns>返回一个值，指示是否继续执行命令。</returns>
        bool RaiseExecuting(IContext context, TCommand command);
        /// <summary>
        /// 命令模型执行后发生的方法。
        /// </summary>
        /// <param name="context">执行的上下文。</param>
        /// <param name="command">执行的命令模型。</param>
        void RaiseExecuted(IContext context, TCommand command);
        /// <summary>
        /// 命令模型执行时抛出异常时发生的方法。
        /// <para>注：执行模型执行抛出异常后，仍然会触发方法 RaiseExecuted，随后再抛出异常。</para>
        /// </summary>
        /// <param name="context">执行的上下文。</param>
        /// <param name="command">执行的命令模型。</param>
        /// <param name="exception">抛出的异常。</param>
        void RaiseException(IContext context, TCommand command, Exception exception);
    }
}
