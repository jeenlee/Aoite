using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

namespace Aoite.Net
{
    /// <summary>
    /// 包含套接字的处理。
    /// </summary>
    internal class MessageTranslater
    {
        private IMessageSocket _messageSocket;
        /// <summary>
        /// 消息的缓冲区。
        /// </summary>
        internal byte[] _messageBuffer;
        /// <summary>
        /// 消息的总长度。
        /// </summary>
        internal int _messageTotalLength;
        /// <summary>
        /// 消息的已接收的长度。
        /// </summary>
        internal int _messageReceivedLength;
        /// <summary>
        /// 消息的模式。
        /// </summary>
        internal MessageMode _messageReceiveMode;

        public MessageTranslater(IMessageSocket messageSocket)
        {
            this._messageSocket = messageSocket;
        }

        public void Release()
        {
            this._messageBuffer = null;
            this._messageReceivedLength = 0;
            this._messageTotalLength = 0;
        }

        /// <summary>
        /// 开始处理异步接收操作。
        /// </summary>
        public bool Process(SaeaEventArgs saea)
        {
            return Process(saea, saea.Buffer, 0, saea.BytesTransferred);
        }

        private bool Process(SaeaEventArgs saea, byte[] buffer, int offset, int count)
        {
            //- 上节无消息，tcp/ip 首包
            if(this._messageTotalLength == 0)
            {
                if(count == 0) return true;
                //- 发包过程中需要指定 5 个字节的数据，分别是（Mode->1，Length->4）
                if(count < 6)
                {
                    //- step 001，保留不完全数据
                    this._messageBuffer = new byte[count];
                    Buffer.BlockCopy(buffer, offset, this._messageBuffer, 0, count);//- 长度不够，直接放入缓存
                    this._messageReceivedLength = count;
                    return true;
                }
                //使用情形：
                // tcp 发包
                //      包1......
                //      包2......
                // tcp 收包
                //      包1......包2.
                //      .....
                if(this._messageReceivedLength > 0)
                {
                    //- step 002，复制历史数据
                    //- 如果执行了 step 001，则需要执行此 step
                    var buffer2 = new byte[this._messageReceivedLength + count];
                    Buffer.BlockCopy(this._messageBuffer, 0, buffer2, 0, this._messageReceivedLength);
                    Buffer.BlockCopy(buffer, offset, buffer2, offset, count);
                    buffer = buffer2;
                    offset = 0;
                    count = buffer2.Length;
                    this._messageBuffer = null;
                    this._messageReceivedLength = 0;
                }

                var mode = buffer[offset++];//- 取出包的模式
                count -= 1;
                //- 首包包含垃圾包
                if(mode < 1 || mode > 2)
                {
                    this._messageBuffer = null;
                    this._messageReceivedLength = 0;
                    return false;
                }
                this._messageReceiveMode = (MessageMode)mode;
            }

            //try
            //{
            switch(this._messageReceiveMode)
            {
                case MessageMode.Once:
                    return this.OnOnceReceive(saea, buffer, offset, count);
                default:
                    return this.OnFullReceive(saea, buffer, offset, count);
            }
            //}
            //catch(Exception)
            //{
            //    //- 垃圾包
            //    return false;
            //}
        }

        private void FullReceiveCompleted(SaeaEventArgs saea)
        {
            this._messageSocket.RaiseFullReceive(saea, this._messageBuffer);
            this.Release();
        }
        private bool OnFullReceiveFirstTime(SaeaEventArgs saea, byte[] buffer, int offset, int count)
        {
            var messageLength = buffer.ReadMessageLength(ref offset, ref count);
            //if(Builder != null) Builder.AppendLine("!" + (messageLength + ":" + offset + ":" + count) + "!");

            this._messageBuffer = new byte[messageLength];
            //- 消息总长 是否小于或等于 缓冲区总长
            var messageLessEqualCount = messageLength <= count;

            if(messageLessEqualCount)
            {
                //- 缓冲区总长 大于或等于 消息总长，拷贝所需长度的 byte
                Buffer.BlockCopy(buffer, offset, this._messageBuffer, 0, messageLength);
                //- 消息总长 等于 缓冲区总长，【刚好】
                this.FullReceiveCompleted(saea);
                //- 消息总长 小于 缓冲区总长，【太多】
                if(messageLength < count) return Process(saea, buffer, offset + messageLength, count - messageLength);
            }
            else
            {
                //- 缓冲区总长 小于 消息总长，【太少】
                Buffer.BlockCopy(buffer, offset, this._messageBuffer, 0, count);
                this._messageTotalLength = messageLength;
                this._messageReceivedLength = count;
            }
            return true;
        }
        private bool OnFullReceive(SaeaEventArgs saea, byte[] buffer, int offset, int count)
        {
            if(this._messageReceivedLength == 0) return this.OnFullReceiveFirstTime(saea, buffer, offset, count);

            //- 计算还需要接收的消息长度
            var receivedLength = this._messageTotalLength - this._messageReceivedLength;

            //- 剩余总长 大于 缓冲区总长，【太少】
            if(receivedLength > count)
            {
                Buffer.BlockCopy(buffer, offset, this._messageBuffer, this._messageReceivedLength, count);
                this._messageReceivedLength += count;
            }
            else
            {
                //- 缓冲区总长 大于或等于 消息总长，拷贝所需长度的 byte
                Buffer.BlockCopy(buffer, offset, this._messageBuffer, this._messageReceivedLength, receivedLength);
                //- 消息总长 等于 缓冲区总长，【刚好】
                this.FullReceiveCompleted(saea);
                //- 剩余总长 小于 缓冲区总长，【太多】
                if(receivedLength != count) return this.Process(saea, buffer, offset + receivedLength, count - receivedLength);
            }
            return true;
        }

        private bool OnOnceReceiveFirstTime(SaeaEventArgs saea, byte[] buffer, int offset, int count)
        {
            int receivedLength;
            var messageLength = buffer.ReadMessageLength(ref offset, ref count);

            //- 消息总长 是否小于或等于 缓冲区总长
            var messageLessEqualCount = messageLength <= count;

            if(messageLessEqualCount)
            {
                receivedLength = messageLength;
            }
            else
            {
                this._messageTotalLength = messageLength;
                this._messageReceivedLength = receivedLength = count;
            }

            this._messageSocket.RaiseOnceReceive(saea, messageLength, receivedLength, buffer, offset, receivedLength);

            if(messageLessEqualCount)
            {
                //- 消息总长 等于 缓冲区总长，【刚好】
                this.Release();
                //- 消息总长 小于 缓冲区总长，【太多】
                if(messageLength < count) return this.Process(saea, buffer, offset + receivedLength, count - receivedLength);
            }
            //- else 消息总长 大于 缓冲区总长，【太少】
            return true;
        }

        private bool OnOnceReceive(SaeaEventArgs saea, byte[] buffer, int offset, int count)
        {
            //- 第一次解析包
            if(this._messageReceivedLength == 0) return this.OnOnceReceiveFirstTime(saea, buffer, offset, count);

            //- 计算还需要接收的消息长度
            var receivedLength = this._messageTotalLength - this._messageReceivedLength;

            //- 剩余总长 大于 缓冲区总长，【太少】
            if(receivedLength > count)
            {
                this._messageReceivedLength += count;
                this._messageSocket.RaiseOnceReceive(saea, this._messageTotalLength, this._messageReceivedLength, buffer, offset, count);
            }
            else
            {
                //- 消息总长 等于 缓冲区总长，【刚好】
                this._messageSocket.RaiseOnceReceive(saea, this._messageTotalLength, this._messageReceivedLength + receivedLength, buffer, offset, receivedLength);

                this.Release();

                //- 剩余总长 小于 缓冲区总长，【太多】
                if(receivedLength < count) return this.Process(saea, buffer, offset + receivedLength, count - receivedLength);
            }

            return true;
        }

    }

}
