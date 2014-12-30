using System;
using System.Collections.Generic;
using System.Text;

namespace Aoite.Net
{
    /*
     * 为什么需要分包定长发送？因为数据实际在发包的过程中，可能产生【粘包】。
     * Full：接收指定长度的包，才触发事件。
     * Once：接收指定长度的包，每次接收都触发事件，直到长度满足。
     */
    /// <summary>
    /// 表示信息的模式。
    /// </summary>
    public enum MessageMode : byte
    {
        /// <summary>
        /// 完整模式。当消息接收到指定长度后，才会触发一次接收事件（接收指定长度的包，才触发事件）。
        /// </summary>
        Full = 1,
        /// <summary>
        /// 单次模式。每接收一次消息，便会触发一次接收事件（接收指定长度的包，每次接收都触发事件，直到长度满足）。
        /// </summary>
        Once = 2,
    }
}
