using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.CommandModel
{
    /// <summary>
    /// 表示一个命令模型的事件。
    /// </summary>
    /// <typeparam name="TCommand">命令模型的数据类型。</typeparam>
    public class Event<TCommand> : IEvent<TCommand> where TCommand : ICommand
    {
        private List<CommandExecutingHandler<TCommand>> _ExecutingEvents = new List<CommandExecutingHandler<TCommand>>();
        /// <summary>
        /// 命令模型执行前发生的事件。事件在顺序广播时，遇到返回 false 则会中断广播。
        /// </summary>
        public event CommandExecutingHandler<TCommand> Executing
        {
            add { lock(_ExecutingEvents) _ExecutingEvents.Add(value); }
            remove { lock(_ExecutingEvents)_ExecutingEvents.Remove(value); }
        }
        /// <summary>
        /// 命令模型执行后发生的事件。
        /// </summary>
        public event CommandExecutedHandler<TCommand> Executed;
        /// <summary>
        /// 命令模型执行时抛出异常时发生的时间。
        /// <para>注：执行模型执行抛出异常后，仍然会触发方法 Executed 事件，随后再抛出异常。</para>
        /// </summary>
        public event CommandExecuteExceptionHandler<TCommand> Exception;

        /// <summary>
        /// 初始化 <see cref="Aoite.CommandModel.Event&lt;TCommand&gt;"/> 类的新实例。
        /// </summary>
        public Event() { }

        /// <summary>
        /// 命令模型执行前发生的方法。
        /// </summary>
        /// <param name="context">执行的上下文。</param>
        /// <param name="command">执行的命令模型。</param>
        public virtual bool RaiseExecuting(IContext context, TCommand command)
        {
            lock(_ExecutingEvents)
            {
                foreach(var item in this._ExecutingEvents)
                {
                    if(!item(context, command)) return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 命令模型执行后发生的方法。
        /// </summary>
        /// <param name="context">执行的上下文。</param>
        /// <param name="command">执行的命令模型。</param>
        public virtual void RaiseExecuted(IContext context, TCommand command)
        {
            var ev = this.Executed;
            if(ev != null) ev(context, command);
        }

        /// <summary>
        /// 命令模型执行时抛出异常时发生的方法。
        /// <para>注：执行模型执行抛出异常后，仍然会触发方法 RaiseExecuted，随后再抛出异常。</para>
        /// </summary>
        /// <param name="context">执行的上下文。</param>
        /// <param name="command">执行的命令模型。</param>
        /// <param name="exception">抛出的异常。</param>
        public virtual void RaiseException(IContext context, TCommand command, Exception exception)
        {
            var ev = this.Exception;
            if(ev != null) ev(context, command, exception);
        }
    }
}
