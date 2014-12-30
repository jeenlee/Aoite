using Aoite.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// 定义一个基础的服务。
    /// </summary>
    public interface IService : IDisposable { }
    /// <summary>
    /// 定义一个包含数据源查询与交互引擎的服务。
    /// </summary>
    public interface IDbEngineExtendService : IService
    {
        /// <summary>
        /// 设置或获取当前运行环境的数据库操作引擎集合管理器。
        /// </summary>
        DbEngineManager Manager { get; set; }
        /// <summary>
        /// 获取当前运行环境的数据库操作引擎的实例。
        /// </summary>
        DbEngine Engine { get; }

        /// <summary>
        /// 获取一个值，指示当前上下文在线程中是否已创建。
        /// </summary>
        bool IsThreadContext { get; }

        /// <summary>
        /// 获取当前运行环境的数据库操作引擎。
        /// </summary>
        DbEngine Readonly { get; }
    }

    /// <summary>
    /// 定义用户模型的扩展服务。
    /// </summary>
    public interface IIdentityExtendService : IService
    {
        /// <summary>
        /// 获取当前已授权的登录用户。
        /// </summary>
        dynamic User { get; }
    }
    /// <summary>
    /// 定义强类型的用户模型的扩展服务。
    /// </summary>
    /// <typeparam name="TUserModel">用户模型的数据类型。</typeparam>
    public interface IIdentityExtendService<TUserModel> : IIdentityExtendService
    {
        /// <summary>
        /// 获取当前已授权的登录用户。
        /// </summary>
        new TUserModel User { get; }
    }
}
