using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.ServiceModel
{
    /// <summary>
    /// 提供客户端会话支持。
    /// </summary>
    public interface IClientSessionProvider
    {
        /// <summary>
        /// 获取或设置契约域的会话标识。
        /// </summary>
        string SessionId { get; set; }
    }
}
