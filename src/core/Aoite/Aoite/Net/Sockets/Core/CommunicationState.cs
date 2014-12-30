using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Net
{
    /// <summary>
    /// 表示通讯的状态。
    /// </summary>
    public enum CommunicationState : long
    {
        /// <summary>
        /// 表示通讯已关闭。
        /// </summary>
        Closed = 0,
        /// <summary>
        /// 表示通讯正在启动。
        /// </summary>
        Opening = 1,
        /// <summary>
        /// 表示通讯已启动。
        /// </summary>
        Opened = 2,
        /// <summary>
        /// 表示通讯正在关闭。
        /// </summary>
        Closing = 3,
        /// <summary>
        /// 表示通讯抛出异常。按照约定，一旦通讯状体为此类型，则表示当前通讯已被强制停止。 
        /// </summary>
        Failed = 4,
    }
}
