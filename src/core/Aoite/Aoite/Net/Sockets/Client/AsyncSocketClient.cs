using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Aoite.Net
{
    /// <summary>
    /// 表示一个可靠的异步套接字的客户端。
    /// </summary>
    public class AsyncSocketClient : AsyncSocketBase, IAcceptedClient
    {
        private long _activeOperationCount;
        private Socket _serverSocket;
        /// <summary>
        /// 可复用的 <see cref="Aoite.Net.SaeaEventArgs"/> 的管理池。
        /// </summary>
        private SaeaPool _sendSaeaPool;
        private SaeaEventArgs _receiveSaea;
        /// <summary>
        /// 获取本地终结点。
        /// </summary>
        public System.Net.EndPoint LocalEndPoint { get { return this._serverSocket == null ? null : this._serverSocket.LocalEndPoint; } }

        private int _ConnectTimeout =
#if DEBUG
 System.Threading.Timeout.Infinite;
#else
 5000;
#endif
        /// <summary>
        /// 设置或获取一个值，表示连接到服务端的超时时间。默认为 5000 毫秒。
        /// </summary>
        public int ConnectTimeout { get { return this._ConnectTimeout; } set { this._ConnectTimeout = value; } }

        private int _MaxBufferSize;
        /// <summary>
        /// 获取最大缓冲区大小。默认为 2048 字节。
        /// </summary>
        public int MaxBufferSize { get { return this._MaxBufferSize; } }

        /// <summary>
        /// 获取一个值，该值指示客户端是否处于忙碌状态。
        /// </summary>
        public override bool IsBusy { get { return Interlocked.Read(ref this._activeOperationCount) > 0L && base.IsBusy; } }

        private DateTime _AcceptTime;
        /// <summary>
        /// 获取客户端的接入时间。
        /// </summary>
        public DateTime AcceptTime { get { return this._AcceptTime; } }

        private string _EndPoint;
        /// <summary>
        /// 获取客户端的终结点地址。
        /// </summary>
        public string EndPoint { get { return this._EndPoint; } }

        /// <summary>
        /// 获取一个值，该值指示客户端是否已成功连接。
        /// </summary>
        public bool Connected { get { return this._serverSocket != null && this._serverSocket.Connected; } }

        /// <summary>
        /// 初始化一个 <see cref="Aoite.Net.AsyncSocketClient"/> 类的新实例。
        /// </summary>
        /// <param name="socketInfo">套接字的信息。</param>
        /// <param name="maxBufferSize">最大缓冲区大小。</param>
        public AsyncSocketClient(SocketInfo socketInfo, int maxBufferSize = 2048)
        {
            if(socketInfo == null) throw new ArgumentNullException("socketInfo");
            this._MaxBufferSize = maxBufferSize;
            this._EndPoint = socketInfo.EndPoint.ToString();
            this.SocketInfo = socketInfo;
            this._sendSaeaPool = new SaeaPool(this.SocketAsyncCompleted);
        }

        /// <summary>
        /// 指定最大缓冲区大小，创建一个 <see cref="Aoite.Net.SaeaEventArgs"/> 对象。 
        /// </summary>
        /// <param name="maxBufferSize">最大缓冲区大小。</param>
        /// <returns>返回一个 <see cref="Aoite.Net.SaeaEventArgs"/> 对象。</returns>
        protected virtual SaeaEventArgs CreateReceiveSaea(int maxBufferSize)
        {
            SaeaEventArgs receiveSaea = new SaeaEventArgs();
            receiveSaea.SetBuffer(new byte[maxBufferSize], 0, maxBufferSize);
            return receiveSaea;
        }

    
        /// <summary>
        /// 处理一个完成异步数据发送请求的 <see cref="Aoite.Net.SaeaEventArgs"/> 对象。
        /// </summary>
        /// <param name="saea">当前方法关联的 <see cref="Aoite.Net.SaeaEventArgs"/>。</param>
        protected override void ProcessSend(SaeaEventArgs saea)
        {
            base.ProcessSend(saea);
            this._sendSaeaPool.Release(saea);
        }

        /// <summary>
        /// 创建一个发送数据的 <see cref="Aoite.Net.SaeaEventArgs"/> 对象。
        /// </summary>
        /// <returns>返回一个 <see cref="Aoite.Net.SaeaEventArgs"/> 对象。</returns>
        protected virtual SaeaEventArgs CreateSendSaea()
        {
            var saea = this._sendSaeaPool.Acquire();
            saea.AcceptSocket = this._serverSocket;
            saea.Client = this;
            return saea;
        }

        /// <summary>
        /// 将文件集合或者内存中的数据缓冲区以异步方法发送给连接的 <see cref="System.Net.Sockets.Socket"/> 对象。
        /// </summary>
        /// <param name="packets">要发送的缓冲区数组的 <see cref="System.Net.Sockets.SendPacketsElement"/> 对象数组。</param>
        /// <returns>返回一个值，指示异步发送的指令是否已完成。</returns>
        public virtual bool SendPacketsAsync(params SendPacketsElement[] packets)
        {
            var saea = this.CreateSendSaea();
            saea.SendPacketsElements = packets;
            base.SendPacketsAsync(saea);
            return true;
        }

        /// <summary>
        /// 将数据异步发送到连接的 <see cref="System.Net.Sockets.Socket"/> 对象。
        /// </summary>
        /// <param name="buffer">要用于异步套接字方法的数据缓冲区。</param>
        /// <param name="offset">数据缓冲区中操作开始位置处的偏移量，以字节为单位。</param>
        /// <param name="count">可在缓冲区中发送或接收的最大数据量（以字节为单位）。</param>
        /// <returns>返回一个值，指示异步发送的指令是否已完成。</returns>
        public virtual bool SendAsync(byte[] buffer, int offset, int count)
        {
            var saea = this.CreateSendSaea();
            saea.SetBuffer(buffer, offset, count);
            base.SendAsync(saea);
            return true;
        }

        void IAcceptedClient.Shutdown()
        {
            this.Close();
        }

        /// <summary>
        /// 打开通讯连接时发生。
        /// </summary>
        protected override void OnOpen()
        {
            var socketInfo = this.SocketInfo;

            this._serverSocket = socketInfo.CreateSocket();
            this._serverSocket.SendBufferSize = this._MaxBufferSize;
            this._serverSocket.ReceiveBufferSize = this._MaxBufferSize;

            this._serverSocket.Connect(socketInfo.EndPoint, this._ConnectTimeout);
            this._receiveSaea = this.CreateReceiveSaea(this._MaxBufferSize);
            this._receiveSaea.Completed += this.SocketAsyncCompleted;
            this._receiveSaea.Client = this;
            this._receiveSaea.AcceptSocket = this._serverSocket;
            this._AcceptTime = DateTime.Now;

            this.BeginOperation();
            if(!this._serverSocket.ReceiveAsync(this._receiveSaea)) this.ProcessReceive(this._receiveSaea);
        }


        /// <summary>
        /// 关闭通讯连接时发生。
        /// </summary>
        protected override void OnClose()
        {
            while(this.IsBusy) Thread.Sleep(333);

            if(this._serverSocket != null) this._serverSocket.Shutdown(true);
            this._serverSocket = null;

            if(this._receiveSaea != null)
            {
                this._receiveSaea.Completed -= this.SocketAsyncCompleted;
                this._receiveSaea.Dispose();
            }
            this._receiveSaea = null;
        }

        /// <summary>
        /// 开始一个操作。
        /// </summary>
        public void BeginOperation()
        {
            Interlocked.Increment(ref this._activeOperationCount);
        }

        /// <summary>
        /// 结束一个操作。
        /// </summary>
        public void EndOperation()
        {
            Interlocked.Decrement(ref this._activeOperationCount);
        }

        /// <summary>
        /// 处理一个已经断开连接的 <see cref="Aoite.Net.SaeaEventArgs"/> 对象。
        /// </summary>
        /// <param name="saea">当前事件参数关联的 <see cref="Aoite.Net.SaeaEventArgs"/>。</param>
        protected override void ProcessShutdown(SaeaEventArgs saea)
        {
            base.ProcessShutdown(saea);
            this.Close();
        }

        /// <summary>
        /// 执行与释放或重置托管资源相关的应用程序定义的任务。
        /// </summary>
        protected override void DisposeManaged()
        {
            if(this._sendSaeaPool != null) this._sendSaeaPool.Dispose();
            this._sendSaeaPool = null;
            base.DisposeManaged();
        }
    }
}
