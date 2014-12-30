using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.CommandModel
{
    /// <summary>
    /// 表示事件仓库的扩展方法。
    /// </summary>
    public static class EventStoreExtensions
    {
        /// <summary>
        /// 注册一个事件到指定的命令模型类型。
        /// </summary>
        /// <typeparam name="TCommand">命令模型的数据类型。</typeparam>
        /// <param name="eventStore">事件的仓库。</param>
        /// <param name="event">事件。</param>
        public static void Register<TCommand>(this IEventStore eventStore, IEvent<TCommand> @event) where TCommand : ICommand
        {
            eventStore.Register(typeof(TCommand), @event);
        }

        /// <summary>
        /// 注销指定命令模型类型的一个事件。
        /// </summary>
        /// <typeparam name="TCommand">命令模型的数据类型。</typeparam>
        /// <param name="eventStore">事件的仓库。</param>
        /// <param name="event">事件。</param>
        public static void Unregister<TCommand>(this IEventStore eventStore, IEvent<TCommand> @event) where TCommand : ICommand
        {
            eventStore.Unregister(typeof(TCommand), @event);
        }

        /// <summary>
        /// 注销指定命令模型类型的所有事件。
        /// </summary>
        /// <typeparam name="TCommand">命令模型的数据类型。</typeparam>
        /// <param name="eventStore">事件的仓库。</param>
        public static void UnregisterAll<TCommand>(this IEventStore eventStore) where TCommand : ICommand
        {
            eventStore.UnregisterAll(typeof(TCommand));
        }
    }
}
