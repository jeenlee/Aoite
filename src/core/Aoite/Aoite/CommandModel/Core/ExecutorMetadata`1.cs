using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.CommandModel
{
    /// <summary>
    /// 表示一个命令模型执行器的元数据。
    /// </summary>
    /// <typeparam name="TCommand">命令模型的数据类型。</typeparam>
    public sealed class ExecutorMetadata<TCommand> : IExecutorMetadata<TCommand> where TCommand : ICommand
    {
        private IExecutor<TCommand> _Executor;
        IExecutor<TCommand> IExecutorMetadata<TCommand>.Executor { get { return _Executor; } }
        IExecutor IExecutorMetadata.Executor { get { return _Executor; } }

        private IExecutorAttribute[] _executorAttributes;
        /// <summary>
        /// 初始化一个 <see cref="Aoite.CommandModel.ExecutorMetadata&lt;TCommand&gt;"/> 类的新实例。
        /// </summary>
        /// <param name="executor">命令模型的执行器。</param>
        public ExecutorMetadata(IExecutor<TCommand> executor)
        {
            if(executor == null) throw new ArgumentNullException("executor");

            var executorType = executor.GetType();
            var attrs = new List<IExecutorAttribute>(executorType.GetAttributes<IExecutorAttribute>());
            attrs.AddRange(typeof(TCommand).GetAttributes<IExecutorAttribute>());

            this._executorAttributes = attrs.ToArray();
            this._Executor = executor;
        }

        bool ICommandHandler<ICommand>.RaiseExecuting(IContext context, ICommand command)
        {
            foreach(var item in this._executorAttributes)
            {
                if(!item.RaiseExecuting(context, command)) return false;
            }
            return true;
        }

        void ICommandHandler<ICommand>.RaiseExecuted(IContext context, ICommand command)
        {
            foreach(var item in this._executorAttributes)
            {
                item.RaiseExecuted(context, command);
            }
        }

        void ICommandHandler<ICommand>.RaiseException(IContext context, ICommand command, Exception exception)
        {
            foreach(var item in this._executorAttributes)
            {
                item.RaiseException(context, command, exception);
            }
        }
    }
}
