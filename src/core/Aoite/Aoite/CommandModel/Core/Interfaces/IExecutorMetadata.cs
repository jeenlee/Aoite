using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.CommandModel
{
    /// <summary>
    /// 定义一个命令模型执行器的元数据。
    /// </summary>
    public interface IExecutorMetadata : IExecutorAttribute
    {
        /// <summary>
        /// 获取命令模型的执行器。
        /// </summary>
        IExecutor Executor { get; }
    }
}
