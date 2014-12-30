using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.CommandModel
{

    /// <summary>
    /// 定义一个命令的处理程序。
    /// </summary>
    public interface IExecutorAttribute : ICommandHandler<ICommand> { }
}
