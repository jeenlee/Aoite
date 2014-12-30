using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.CommandModel
{
    /// <summary>
    /// 表示一个模拟的命令模型执行器工厂。
    /// </summary>
    public class MockExecutorFactory : CommandModelContainerProviderBase, IExecutorFactory
    {
        private Dictionary<Type, IExecutorMetadata> Executors = new Dictionary<Type, IExecutorMetadata>();

        /// <summary>
        /// 初始化一个 <see cref="Aoite.CommandModel.MockExecutorFactory"/> 类的新实例。
        /// </summary>
        /// <param name="container">服务容器。</param>
        public MockExecutorFactory(IIocContainer container) : base(container) { }

        /// <summary>
        /// 模拟指定命令模型的执行方法。
        /// </summary>
        /// <typeparam name="TCommand">命令模型的数据类型。</typeparam>
        /// <param name="mockHandler">模拟的执行器。</param>
        /// <returns>返回当前执行器工厂。</returns>
        public MockExecutorFactory Mock<TCommand>(CommandExecutedHandler<TCommand> mockHandler) where TCommand : ICommand
        {
            Executors[typeof(TCommand)] = new ExecutorMetadata<TCommand>(new MockExecutor<TCommand>(mockHandler));
            return this;
        }

        IExecutorMetadata<TCommand> IExecutorFactory.Create<TCommand>(TCommand command)
        {
            var executor = Executors.TryGetValue(typeof(TCommand)) as IExecutorMetadata<TCommand>;
            if(executor == null) throw new NotImplementedException("没有模拟实现命令 {0} 的执行器。".Fmt(typeof(TCommand).FullName));
            return executor;
        }
    }
}
