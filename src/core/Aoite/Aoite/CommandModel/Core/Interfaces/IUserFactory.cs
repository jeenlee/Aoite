using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.CommandModel
{
    /// <summary>
    /// 定义一个执行命令模型的用户工厂。
    /// </summary>
    public interface IUserFactory
    {
        /// <summary>
        /// 获取执行命令模型的用户。
        /// </summary>
        /// <param name="container">服务容器。</param>
        dynamic GetUser(IIocContainer container);
    }
}
