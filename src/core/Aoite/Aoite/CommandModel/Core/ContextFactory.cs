using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.CommandModel
{
    /// <summary>
    /// 表示一个命令模型上下文的工厂。
    /// </summary>
    [SingletonMapping]
    public class ContextFactory : CommandModelContainerProviderBase, IContextFactory
    {
        /// <summary>
        /// 初始化 <see cref="Aoite.CommandModel.ContextFactory"/> 类的新实例。
        /// </summary>
        /// <param name="container">服务容器。</param>
        public ContextFactory(IIocContainer container)  : base(container) { }

        /// <summary>
        /// 创建一个命令模型的上下文。
        /// </summary>
        /// <typeparam name="TCommand">命令模型的数据类型。</typeparam>
        /// <param name="command">命令模型。</param>
        /// <returns>返回命令模型的上下文。</returns>
        public IContext Create<TCommand>(TCommand command) where TCommand : ICommand
        {
            return new Context(this.Container, command);
        }
    }
}
