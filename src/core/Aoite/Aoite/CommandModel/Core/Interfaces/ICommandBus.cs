using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.CommandModel
{
    /// <summary>
    /// 定义一个命令总线。
    /// </summary>
    public interface ICommandBus : IContainerProvider
    {
        /// <summary>
        /// 执行一个命令模型。
        /// </summary>
        /// <typeparam name="TCommand">命令模型的数据类型。</typeparam>
        /// <param name="command">命令模型。</param>
        /// <param name="success">执行完成后发生的回调方法。</param>
        /// <param name="error">执行抛出异常后发生的回调方法。</param>
        /// <returns>返回命令模型。</returns>
        TCommand Execute<TCommand>(TCommand command
            , CommandExecutingHandler<TCommand> success = null
            , CommandExecuteExceptionHandler<TCommand> error = null) where TCommand : ICommand;
        /// <summary>
        /// 以异步的方式执行一个命令模型。
        /// </summary>
        /// <typeparam name="TCommand">命令模型的数据类型。</typeparam>
        /// <param name="command">命令模型。</param>
        /// <param name="success">执行完成后发生的回调方法。</param>
        /// <param name="error">执行抛出异常后发生的回调方法。</param>
        void ExecuteAsync<TCommand>(TCommand command
            , CommandExecutingHandler<TCommand> success = null
            , CommandExecuteExceptionHandler<TCommand> error = null) where TCommand : ICommand;
    }
}
