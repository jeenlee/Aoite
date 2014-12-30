using Aoite.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.ServiceModel
{
    /// <summary>
    /// 定义一个契约的服务器。
    /// </summary>
    public interface IContractServer
    {
        /// <summary>
        /// 获取服务器的配置信息。
        /// </summary>
        ServerConfigurationBase Configuration { get; }
        /// <summary>
        /// 获取缓存的提供程序。
        /// </summary>
        ICacheProvider CacheProvider { get; }
        /// <summary>
        /// 获取服务容器。
        /// </summary>
        IIocContainer Container { get; }
        /// <summary>
        /// 服务器抛出已捕获的错误时发生。
        /// </summary>
        event ExceptionEventHandler Error;
        /// <summary>
        /// 契约方法调用前发生。
        /// </summary>
        event EventHandler Calling;
        /// <summary>
        /// 契约方法调用后发生。
        /// </summary>
        event EventHandler Called;

        /// <summary>
        /// 契约实例创建后发生。
        /// </summary>
        event EventHandler InstanceCreated;
    }
}
