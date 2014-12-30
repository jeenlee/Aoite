using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoite.CommandModel
{
    /// <summary>
    /// 表示一个命令模型的总线。
    /// </summary>
    [SingletonMapping]
    public class CommandBus : CommandModelContainerProviderBase, ICommandBus
    {
        /// <summary>
        /// 初始化 <see cref="Aoite.CommandModel.CommandBus"/> 类的新实例。
        /// </summary>
        /// <param name="container">服务容器。</param>
        public CommandBus(IIocContainer container) : base(container) { }

        /// <summary>
        /// 执行一个命令模型。
        /// </summary>
        /// <typeparam name="TCommand">命令模型的数据类型。</typeparam>
        /// <param name="command">命令模型。</param>
        /// <param name="success">执行完成后发生的回调方法。</param>
        /// <param name="error">执行抛出异常后发生的回调方法。</param>
        /// <returns>返回命令模型。</returns>
        public virtual TCommand Execute<TCommand>(TCommand command
            , CommandExecutingHandler<TCommand> success = null
            , CommandExecuteExceptionHandler<TCommand> error = null) where TCommand : ICommand
        {
            if(command == null) throw new ArgumentNullException("command");

            var eventStore = this.Container.GetService<IEventStore>();
            var contextFactory = this.Container.GetService<IContextFactory>();
            var executorFactory = this.Container.GetService<IExecutorFactory>();

            var context = contextFactory.Create<TCommand>(command);
            var executorMetadata = executorFactory.Create<TCommand>(command);
            var executor = executorMetadata.Executor;

            // 执行器元数据 && 事务仓储
            if(executorMetadata.RaiseExecuting(context, command)
                && eventStore.RaiseExecuting<TCommand>(context, command))
            {
                try
                {
                    executor.Execute(context, command);
                    if(success != null) success(context, command);
                }
                catch(Exception ex)
                {
                    if(error != null) error(context, command, ex);
                    eventStore.RaiseException(context, command, ex);
                    executorMetadata.RaiseException(context, command, ex);

                    throw;
                }
                finally
                {
                    eventStore.RaiseExecuted<TCommand>(context, command);
                    executorMetadata.RaiseExecuted(context, command);
                }
            }

            return command;
        }

        /// <summary>
        /// 以异步的方式执行一个命令模型。
        /// </summary>
        /// <typeparam name="TCommand">命令模型的数据类型。</typeparam>
        /// <param name="command">命令模型。</param>
        /// <param name="success">执行完成后发生的回调方法。</param>
        /// <param name="error">执行抛出异常后发生的回调方法。</param>
        public virtual void ExecuteAsync<TCommand>(TCommand command
            , CommandExecutingHandler<TCommand> success = null
            , CommandExecuteExceptionHandler<TCommand> error = null) where TCommand : ICommand
        {

            //- https://github.com/StephenCleary/AspNetBackgroundTasks/blob/master/src/AspNetBackgroundTasks/Internal/RegisteredTasks.cs
            //- 如何解决升级维护时，异步任务丢失？
            if(command == null) throw new ArgumentNullException("command");
            System.Threading.ThreadPool.QueueUserWorkItem(o => this.Execute(command, success, error));
        }
    }
}
