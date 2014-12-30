using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.CommandModel
{
    /// <summary>
    /// 定义一个命令模型事件的仓库。
    /// </summary>
    public interface IEventStore : IContainerProvider
    {
        /// <summary>
        /// 注册一个事件到指定的命令模型类型。
        /// </summary>
        /// <param name="commandType">命令模型类型。</param>
        /// <param name="event">事件。</param>
        void Register(Type commandType, IEvent @event);
        /// <summary>
        /// 注销指定命令模型类型的一个事件。
        /// </summary>
        /// <param name="commandType">命令模型类型。</param>
        /// <param name="event">事件。</param>
        void Unregister(Type commandType, IEvent @event);
        /// <summary>
        /// 注销指定命令模型类型的所有事件。
        /// </summary>
        /// <param name="commandType">命令模型类型。</param>
        void UnregisterAll(Type commandType);
        /// <summary>
        /// 命令模型执行前发生的方法。
        /// </summary>
        /// <typeparam name="TCommand">命令模型的数据类型。</typeparam>
        /// <param name="context">执行的上下文。</param>
        /// <param name="command">执行的命令模型。</param>
        /// <returns>返回一个值，指示是否继续执行命令。</returns>
        bool RaiseExecuting<TCommand>(IContext context, TCommand command) where TCommand : ICommand;
        /// <summary>
        /// 命令模型执行后发生的方法。
        /// </summary>
        /// <typeparam name="TCommand">命令模型的数据类型。</typeparam>
        /// <param name="context">执行的上下文。</param>
        /// <param name="command">执行的命令模型。</param>
        void RaiseExecuted<TCommand>(IContext context, TCommand command) where TCommand : ICommand;
        /// <summary>
        /// 命令模型执行时抛出异常时发生的方法。
        /// <para>注：执行模型执行抛出异常后，仍然会触发方法 RaiseExecuted，随后再抛出异常。</para>
        /// </summary>
        /// <typeparam name="TCommand">命令模型的数据类型。</typeparam>
        /// <param name="context">执行的上下文。</param>
        /// <param name="command">执行的命令模型。</param>
        /// <param name="exception">抛出的异常。</param>
        void RaiseException<TCommand>(IContext context, TCommand command, Exception exception) where TCommand : ICommand;
    }
}
