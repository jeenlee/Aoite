using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// 定义一个包含服务容器的提供程序。
    /// </summary>
    public interface IContainerProvider
    {
        /// <summary>
        /// 获取服务容器。
        /// </summary>
        IIocContainer Container { get; }
    }
}
