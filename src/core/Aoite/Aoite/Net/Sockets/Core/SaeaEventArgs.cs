using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Aoite.Net
{
    /// <summary>
    /// 表示一个包含 <see cref="Aoite.Net.SaeaEventArgs"/> 的事件委托。
    /// </summary>
    /// <param name="sender">事件对象。</param>
    /// <param name="e">事件参数。</param>
    public delegate void SaeaEventHandler(object sender, SaeaEventArgs e);

    /// <summary>
    /// 表示一个扩展 <see cref="System.Net.Sockets.SocketAsyncEventArgs"/> 的异步套接字操作。
    /// </summary>
    public class SaeaEventArgs : SocketAsyncEventArgs
    {
        private Exception _Exception;
        /// <summary>
        /// 获取异步操作最后一个的异常。
        /// </summary>
        public Exception Exception
        {
            get
            {
                if(this._Exception == null && this.SocketError != SocketError.Success)
                {
                    this._Exception = new SocketException((int)this.SocketError);
                }
                return this._Exception;
            }
            internal set { this._Exception = value; }
        }

        private IAcceptedClient _Client;
        /// <summary>
        /// 获取异步操作关联的客户端。
        /// </summary>
        public IAcceptedClient Client { get { return this._Client; } internal set { this._Client = value; } }

        internal bool TryEndOperation()
        {
            if(this._Client != null)
            {
                this._Client.EndOperation();
                return true;
            }
            return false;
        }
        internal SaeaEventArgs() { }

        internal virtual void Release()
        {
            this._Exception = null;
            this._Client = null;
            this.SetBuffer(null, 0, 0);
            this.AcceptSocket = null;
            this.BufferList = null;
            this.RemoteEndPoint = null;
            this.SendPacketsElements = null;
            this.SendPacketsFlags = 0;
            this.SendPacketsSendSize = 0;
            this.SocketError = System.Net.Sockets.SocketError.Success;
            this.SocketFlags = System.Net.Sockets.SocketFlags.None;
            this.UserToken = null;
        }
    }
}
