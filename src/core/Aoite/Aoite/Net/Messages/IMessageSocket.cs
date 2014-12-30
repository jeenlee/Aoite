using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Net
{
    internal interface IMessageSocket
    {
        /// <summary>
        /// 数据单次接收时发生。
        /// </summary>
        /// <param name="saea">当前事件参数关联的 <see cref="Aoite.Net.SaeaEventArgs"/>。</param>
        /// <param name="totalLength">消息的总长度。</param>
        /// <param name="receivedLength">已接收的总长度（此长度已计算 <paramref name="count"/> 参数的值）。</param>
        /// <param name="buffer">当前尚未接收的缓冲区。</param>
        /// <param name="offset">当前尚未接收的偏移量。</param>
        /// <param name="count">当前尚未接收的总长度。</param>
        void RaiseOnceReceive(SaeaEventArgs saea, int totalLength, int receivedLength, byte[] buffer, int offset, int count);
        /// <summary>
        /// 数据完整接收时发生。
        /// </summary>
        /// <param name="saea">当前事件参数关联的 <see cref="Aoite.Net.SaeaEventArgs"/>。</param>
        /// <param name="buffer">缓冲区。</param>
        void RaiseFullReceive(SaeaEventArgs saea, byte[] buffer);
    }
}
