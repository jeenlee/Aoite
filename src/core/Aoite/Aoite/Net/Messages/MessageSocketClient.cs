using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Aoite.Net
{
    /// <summary>
    /// 表示一个基于消息异步套接字的客户端。
    /// </summary>
    public class MessageSocketClient : AsyncSocketClient, IMessageSocket
    {
        /// <summary>
        /// 初始化一个 <see cref="Aoite.Net.MessageSocketClient"/> 类的新实例。
        /// </summary>
        /// <param name="socketInfo">套接字的信息。</param>
        /// <param name="maxBufferSize">最大缓冲区大小。</param>
        public MessageSocketClient(SocketInfo socketInfo, int maxBufferSize = 2048)
            : base(socketInfo, maxBufferSize) { }

        /// <summary>
        /// 接收消息后发生。
        /// </summary>
        public event MessageReceiveEventHandler MessageReceive;
        /// <summary>
        /// 接收消息后发生的方法。
        /// </summary>
        /// <param name="mrea">当前事件参数关联的 <see cref="Aoite.Net.MessageReceiveEventArgs"/>。</param>
        protected virtual void OnMessageReceive(MessageReceiveEventArgs mrea)
        {
            var handler = this.MessageReceive;
            if(handler != null) handler(this, mrea);
        }

        /// <summary>
        /// 异步套接字接收数据后发生的方法。
        /// </summary>
        /// <param name="saea">当前事件参数关联的 <see cref="Aoite.Net.SaeaEventArgs"/>。</param>
        protected override void OnAsyncReceive(SaeaEventArgs saea)
        {
            base.OnAsyncReceive(saea);

            var msaea = saea as MessageReceiveEventArgs;
            if(!msaea.Translater.Process(saea)) this.ProcessShutdown(saea);
        }

        void IMessageSocket.RaiseOnceReceive(SaeaEventArgs saea, int totalLength, int receivedLength, byte[] buffer, int offset, int count)
        {

        }

        void IMessageSocket.RaiseFullReceive(SaeaEventArgs saea, byte[] buffer)
        {
            var mrea = saea as MessageReceiveEventArgs;
            mrea.MessageBuffer = buffer;
            this.OnMessageReceive(mrea);
        }

        /// <summary>
        /// 指定最大缓冲区大小，创建一个 <see cref="Aoite.Net.SaeaEventArgs"/> 对象。 
        /// </summary>
        /// <param name="maxBufferSize">最大缓冲区大小。</param>
        /// <returns>返回一个 <see cref="Aoite.Net.SaeaEventArgs"/> 对象。</returns>
        protected override SaeaEventArgs CreateReceiveSaea(int maxBufferSize)
        {
            MessageReceiveEventArgs receiveSaea = new MessageReceiveEventArgs(this);
            receiveSaea.SetBuffer(new byte[maxBufferSize], 0, maxBufferSize);
            return receiveSaea;
        }

        /// <summary>
        /// 将数据异步发送到连接的 <see cref="System.Net.Sockets.Socket"/> 对象。
        /// </summary>
        /// <param name="buffer">要用于异步套接字方法的数据缓冲区。</param>
        /// <param name="offset">数据缓冲区中操作开始位置处的偏移量，以字节为单位。</param>
        /// <param name="count">可在缓冲区中发送或接收的最大数据量（以字节为单位）。</param>
        /// <returns>返回一个值，指示异步发送的指令是否已完成。</returns>
        public override bool SendAsync(byte[] buffer, int offset, int count)
        {
            var newBuffer = buffer.WriteMessageLength(offset, count);
            return base.SendAsync(newBuffer, 0, newBuffer.Length);
        }

        /// <summary>
        /// 将文件集合或者内存中的数据缓冲区以异步方法发送给连接的 <see cref="System.Net.Sockets.Socket"/> 对象。
        /// </summary>
        /// <param name="packets">要发送的缓冲区数组的 <see cref="System.Net.Sockets.SendPacketsElement"/> 对象数组。</param>
        /// <returns>返回一个值，指示异步发送的指令是否已完成。</returns>
        public override bool SendPacketsAsync(params SendPacketsElement[] packets)
        {
            return base.SendPacketsAsync(packets.WriteMessageLength());
        }
    }
}
