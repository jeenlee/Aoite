using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.CommandModel
{
    /// <summary>
    /// 表示一个命令模型事件的仓库。
    /// </summary>
    [SingletonMapping]
    public class EventStore : CommandModelContainerProviderBase, IEventStore
    {
        readonly System.Collections.Concurrent.ConcurrentDictionary<string, List<IEvent>>
            Events = new System.Collections.Concurrent.ConcurrentDictionary<string, List<IEvent>>();

        /// <summary>
        /// 初始化 <see cref="Aoite.CommandModel.EventStore"/> 类的新实例。
        /// </summary>
        /// <param name="container">服务容器。</param>
        public EventStore(IIocContainer container) : base(container) { }

        private string GetKey(Type commandType)
        {
            return commandType.FullName;
        }

        /// <summary>
        /// 注册一个事件到指定的命令模型类型。
        /// </summary>
        /// <param name="commandType">命令模型类型。</param>
        /// <param name="event">事件。</param>
        public virtual void Register(Type commandType, IEvent @event)
        {
            var key = this.GetKey(commandType);
            Events.AddOrUpdate(key, k => new List<IEvent>() { @event }, (k, l) =>
            {
                l.Add(@event);
                return l;
            });
        }

        /// <summary>
        /// 注销指定命令模型类型的一个事件。
        /// </summary>
        /// <param name="commandType">命令模型类型。</param>
        /// <param name="event">事件。</param>
        public virtual void Unregister(Type commandType, IEvent @event)
        {
            var key = this.GetKey(commandType);
            Events.AddOrUpdate(key, k => new List<IEvent>(0), (k, l) =>
            {
                l.Remove(@event);
                return l;
            });
        }

        /// <summary>
        /// 注销指定命令模型类型的所有事件。
        /// </summary>
        /// <param name="commandType">命令模型类型。</param>
        public virtual void UnregisterAll(Type commandType)
        {
            var key = this.GetKey(commandType);
            List<IEvent> list;
            Events.TryRemove(key, out list);
        }

        private IEvent[] GetEvents<TCommand>()
        {
            var key = this.GetKey(typeof(TCommand));
            var events = Events.TryGetValue(key);
            if(events == null) return new IEvent[0];
            return events.ToArray();
        }

        /// <summary>
        /// 命令模型执行前发生的方法。
        /// </summary>
        /// <typeparam name="TCommand">命令模型的数据类型。</typeparam>
        /// <param name="context">执行的上下文。</param>
        /// <param name="command">执行的命令模型。</param>
        /// <returns>返回一个值，指示是否继续执行命令。</returns>
        public virtual bool RaiseExecuting<TCommand>(IContext context, TCommand command) where TCommand : ICommand
        {
            foreach(IEvent<TCommand> item in this.GetEvents<TCommand>())
            {
                if(!item.RaiseExecuting(context, command)) return false;
            }
            return true;
        }

        /// <summary>
        /// 命令模型执行后发生的方法。
        /// </summary>
        /// <typeparam name="TCommand">命令模型的数据类型。</typeparam>
        /// <param name="context">执行的上下文。</param>
        /// <param name="command">执行的命令模型。</param>
        public virtual void RaiseExecuted<TCommand>(IContext context, TCommand command) where TCommand : ICommand
        {
            foreach(IEvent<TCommand> item in this.GetEvents<TCommand>())
            {
                item.RaiseExecuted(context, command);
            }
        }

        /// <summary>
        /// 命令模型执行时抛出异常时发生的方法。
        /// <para>注：执行模型执行抛出异常后，仍然会触发方法 RaiseExecuted，随后再抛出异常。</para>
        /// </summary>
        /// <typeparam name="TCommand">命令模型的数据类型。</typeparam>
        /// <param name="context">执行的上下文。</param>
        /// <param name="command">执行的命令模型。</param>
        /// <param name="exception">抛出的异常。</param>
        public virtual void RaiseException<TCommand>(IContext context, TCommand command, Exception exception) where TCommand : ICommand
        {
            foreach(IEvent<TCommand> item in this.GetEvents<TCommand>())
            {
                item.RaiseException(context, command, exception);
            }
        }
    }
}
