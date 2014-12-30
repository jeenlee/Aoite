using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.CommandModel
{
    /// <summary>
    /// 定义一个命令模型上下文的工厂。
    /// </summary>
    public interface IContextFactory : IContainerProvider
    {
        /// <summary>
        /// 创建一个命令模型的上下文。
        /// </summary>
        /// <typeparam name="TCommand">命令模型的数据类型。</typeparam>
        /// <param name="command">命令模型。</param>
        /// <returns>返回命令模型的上下文。</returns>
        IContext Create<TCommand>(TCommand command) where TCommand : ICommand;
    }
}
