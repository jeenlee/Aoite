using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Aoite.Net
{
    /// <summary>
    /// 表示一个可靠的异步套接字的基类。
    /// </summary>
    public abstract class AsyncSocketBase : CommunicationBase
    {
        #region Properties & Fields & Constructors

        private long _SendQueueCount = 0;
        private SocketInfo _SocketInfo;
        /// <summary>
        /// 获取异步套接字的基础信息。
        /// </summary>
        public virtual SocketInfo SocketInfo
        {
            get { return this._SocketInfo; }
            protected set
            {
                value.EndPoint.ToLoopback();
                this._SocketInfo = value;
            }
        }
        /// <summary>
        /// 获取一个值，该值指示当前异步套接字是否处于忙碌状态。
        /// </summary>
        public virtual bool IsBusy { get { return Interlocked.Read(ref this._SendQueueCount) != 0L; } }
        /// <summary>
        /// 获取一个值，该值指示当前异步套接字正在发送数据的数量。
        /// </summary>
        public virtual long SendQueueCount { get { return Interlocked.Read(ref this._SendQueueCount); } }

        /// <summary>
        /// 初始化一个 <see cref="Aoite.Net.AsyncSocketBase"/> 类的新实例。
        /// </summary>
        protected AsyncSocketBase() { }

        /// <summary>
        /// 提供套接字的信息，初始化一个 <see cref="Aoite.Net.AsyncSocketBase"/> 类的新实例。
        /// </summary>
        /// <param name="socketInfo">套接字的信息。</param>
        public AsyncSocketBase(SocketInfo socketInfo)
        {
            if(socketInfo == null) throw new ArgumentNullException("socketInfo");
            this._SocketInfo = socketInfo;
        }

        #endregion

        #region Events

        /// <summary>
        /// 异步套接字发送数据后发生。
        /// </summary>
        public event SaeaEventHandler AsyncSend;
        /// <summary>
        /// 异步套接字发送数据后发生的方法。
        /// </summary>
        /// <param name="saea">当前事件参数关联的 <see cref="Aoite.Net.SaeaEventArgs"/>。</param>
        protected virtual void OnAsyncSend(SaeaEventArgs saea)
        {
            var handler = this.AsyncSend;
            if(handler != null) handler(this, saea);
        }

        /// <summary>
        /// 异步套接字接收数据后发生。
        /// </summary>
        public event SaeaEventHandler AsyncReceive;
        /// <summary>
        /// 异步套接字接收数据后发生的方法。
        /// </summary>
        /// <param name="saea">当前事件参数关联的 <see cref="Aoite.Net.SaeaEventArgs"/>。</param>
        protected virtual void OnAsyncReceive(SaeaEventArgs saea)
        {
            var handler = this.AsyncReceive;
            if(handler != null) handler(this, saea);
        }


        #endregion

        #region Async

        /// <summary>
        /// 检测指定 <see cref="Aoite.Net.SaeaEventArgs"/> 对象是否在线。
        /// </summary>
        /// <param name="saea">当前方法关联的 <see cref="Aoite.Net.SaeaEventArgs"/>。</param>
        /// <returns>如果 <see cref="Aoite.Net.SaeaEventArgs"/> 在线返回 true，否则返回 false。</returns>
        protected virtual bool IsOnline(SaeaEventArgs saea)
        {
            if(!this.IsRunning)
            {
                saea.TryEndOperation();
                return false;
            }

            if(saea.SocketError != SocketError.Success || !saea.AcceptSocket.Connected)
            {
                saea.TryEndOperation();
                this.ProcessShutdown(saea);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 开始一个异步请求。
        /// </summary>
        /// <param name="saea">当前方法关联的 <see cref="Aoite.Net.SaeaEventArgs"/>。</param>
        /// <param name="asyncMethod">异步请求的方法。</param>
        /// <param name="processMethod">异步处理的方法。</param>
        protected virtual void Async(SaeaEventArgs saea
            , Func<SaeaEventArgs, bool> asyncMethod
            , Action<SaeaEventArgs> processMethod)
        {
            if(!this.IsOnline(saea)) return;

            bool isAsyncProcess;
            try
            {
                isAsyncProcess = asyncMethod(saea);
            }
            catch(Exception ex)
            {
                saea.Exception = ex;
                this.ProcessShutdown(saea);
                return;
            }

            if(!isAsyncProcess) processMethod(saea);
        }

        /// <summary>
        /// 开始一个异步请求以便从连接的 <see cref="Aoite.Net.SaeaEventArgs"/> 对象中接收数据。
        /// </summary>
        /// <param name="saea">当前方法关联的 <see cref="Aoite.Net.SaeaEventArgs"/>。</param>
        protected virtual void ReceiveAsync(SaeaEventArgs saea)
        {
            saea.Client.BeginOperation();
            this.Async(saea, saea.AcceptSocket.ReceiveAsync, this.ProcessReceive);
        }

        /// <summary>
        /// 将数据异步发送到连接的 <see cref="Aoite.Net.SaeaEventArgs"/> 对象。
        /// </summary>
        /// <param name="saea">当前方法关联的 <see cref="Aoite.Net.SaeaEventArgs"/>。</param>
        protected virtual void SendAsync(SaeaEventArgs saea)
        {
            saea.Client.BeginOperation();
            Interlocked.Increment(ref this._SendQueueCount);
            this.Async(saea, saea.AcceptSocket.SendAsync, this.ProcessSend);
        }

        /// <summary>
        /// 将文件集合或者内存中的数据缓冲区以异步方法发送给连接的 <see cref="Aoite.Net.SaeaEventArgs"/> 对象。
        /// </summary>
        /// <param name="saea">当前方法关联的 <see cref="Aoite.Net.SaeaEventArgs"/>。</param>
        protected virtual void SendPacketsAsync(SaeaEventArgs saea)
        {
            saea.Client.BeginOperation();
            Interlocked.Increment(ref this._SendQueueCount);
            this.Async(saea, saea.AcceptSocket.SendPacketsAsync, this.ProcessSend);
        }

        #endregion

        #region Process

        /// <summary>
        /// 套接字异步操作完成的方法。
        /// </summary>
        /// <param name="sender">事件对象。</param>
        /// <param name="e">当前事件参数关联的 <see cref="System.Net.Sockets.SocketAsyncEventArgs"/>。</param>
        protected virtual void SocketAsyncCompleted(object sender, SocketAsyncEventArgs e)
        {
            if(e.AcceptSocket == null) return;
            var saea = (SaeaEventArgs)e;

            switch(e.LastOperation)
            {
                case SocketAsyncOperation.Accept:
                    this.ProcessAccept(saea);
                    return;
                case SocketAsyncOperation.Receive:
                    this.ProcessReceive(saea);
                    return;
                case SocketAsyncOperation.Send:
                case SocketAsyncOperation.SendPackets:
                    this.ProcessSend(saea);
                    return;
            }
            throw new NotSupportedException();
        }

        /// <summary>
        /// 处理一个完成异步接受连接请求的 <see cref="Aoite.Net.SaeaEventArgs"/> 对象。
        /// <para><see cref="Aoite.Net.AsyncSocketBase"/> 并没有实现此方法，重载时请勿向上调用。</para>
        /// </summary>
        /// <param name="saea">当前方法关联的 <see cref="Aoite.Net.SaeaEventArgs"/>。</param>
        protected virtual void ProcessAccept(SaeaEventArgs saea)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 处理一个已经断开连接的 <see cref="Aoite.Net.SaeaEventArgs"/> 对象。
        /// </summary>
        /// <param name="saea">当前事件参数关联的 <see cref="Aoite.Net.SaeaEventArgs"/>。</param>
        protected virtual void ProcessShutdown(SaeaEventArgs saea)
        {
            var client = saea.Client;

            while(client.IsBusy)
            {
                Thread.Sleep(333);
            }

            switch(saea.LastOperation)
            {
                case SocketAsyncOperation.Send:
                case SocketAsyncOperation.SendPackets:
                    Interlocked.Decrement(ref this._SendQueueCount);
                    break;
            }
        }

        /// <summary>
        /// 处理一个完成异步数据接收请求的 <see cref="Aoite.Net.SaeaEventArgs"/> 对象。
        /// </summary>
        /// <param name="saea">当前方法关联的 <see cref="Aoite.Net.SaeaEventArgs"/>。</param>
        protected virtual void ProcessReceive(SaeaEventArgs saea)
        {
            if(!this.IsOnline(saea)) return;
            if(saea.BytesTransferred == 0)
            {
                saea.TryEndOperation();
                this.ProcessShutdown(saea);
                return;
            }
            //- 内部可能已经强制终止了这次请求
            this.OnAsyncReceive(saea);

            if(saea.TryEndOperation())
            {
                this.ReceiveAsync(saea);
            }
        }

        /// <summary>
        /// 处理一个完成异步数据发送请求的 <see cref="Aoite.Net.SaeaEventArgs"/> 对象。
        /// </summary>
        /// <param name="saea">当前方法关联的 <see cref="Aoite.Net.SaeaEventArgs"/>。</param>
        protected virtual void ProcessSend(SaeaEventArgs saea)
        {
            if(this.IsRunning)
            {
                this.OnAsyncSend(saea);
                saea.Client.EndOperation();
            }
            
            switch(saea.LastOperation)
            {
                case SocketAsyncOperation.Send:
                    saea.SetBuffer(null, 0, 0);
                    break;
                case SocketAsyncOperation.SendPackets:
                    saea.SendPacketsElements = null;
                    break;
            }

            Interlocked.Decrement(ref this._SendQueueCount);
        }

        #endregion
    }
}
