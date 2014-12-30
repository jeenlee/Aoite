using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Aoite.Net
{
    /// <summary>
    /// 表示一个可靠的异步套接字的服务端。
    /// </summary>
    public class AsyncSocketServer : AsyncSocketBase
    {
        #region Fields

        /// <summary>
        /// 最大连接数的信号量。
        /// </summary>
        protected Semaphore _connectionSemaphore;
        /// <summary>
        /// 一个防止内存碎片的全局缓冲区管理。
        /// </summary>
        internal BufferManager _bufferManager;
        /// <summary>
        /// 可复用的 <see cref="Aoite.Net.SaeaEventArgs"/> 的管理池。
        /// </summary>
        internal SaeaPool _saeaPool;
        /// <summary>
        /// 可复用的 <see cref="Aoite.Net.IAcceptedClient"/> 的管理池。
        /// </summary>
        internal AcceptedClientPool _clientPool;
        /// <summary>
        /// 服务端套接字。
        /// </summary>
        protected Socket _serverSocket;
        /// <summary>
        /// 已接入的客户端列表。
        /// </summary>
        protected ConcurrentDictionary<Socket, IAcceptedClient> _acceptClients;

        #endregion

        #region Properties

        /// <summary>
        /// 获取服务端的终结点端口号。
        /// </summary>
        public int Port
        {
            get { return this._serverSocket == null ? 0 : (this._serverSocket.LocalEndPoint as System.Net.IPEndPoint).Port; }
        }

        /// <summary>
        /// 获取已接入的客户端列表。
        /// </summary>
        public IAcceptedClient[] Clients { get { return this._acceptClients.Values.ToArray(); } }

        /// <summary>
        /// 获取已接入的客户端数。
        /// </summary>
        public int ClientCount { get { return this._acceptClients.Count; } }

        private int _MaxBufferSize;
        /// <summary>
        /// 获取最大缓冲区大小。默认为 2048 字节。
        /// </summary>
        public int MaxBufferSize { get { return this._MaxBufferSize; } }

        private int _MaxConnectionCount;
        /// <summary>
        /// 获取允许最大的连接数。默认为 10240 个连接数。
        /// </summary>
        public int MaxConnectionCount { get { return this._MaxConnectionCount; } }

        private int _ListenBacklog;
        /// <summary>
        /// 获取一个值，表示挂起连接队列的最大长度。默认为 1024 并发数。
        /// </summary>
        public int ListenBacklog { get { return this._ListenBacklog; } }

        #endregion

        #region Constructors

        /// <summary>
        /// 使用默认最大连接数和接收的最大缓冲区长度，初始化一个 <see cref="Aoite.Net.AsyncSocketServer"/> 类的新实例。
        /// </summary>
        /// <param name="socketInfo">套接字的信息。</param>
        public AsyncSocketServer(SocketInfo socketInfo)
            : this(socketInfo, 10240, 2048, 1024) { }

        /// <summary>
        /// 指定最大连接数和接收的最大缓冲区长度，初始化一个 <see cref="Aoite.Net.AsyncSocketServer"/> 类的新实例。
        /// </summary>
        /// <param name="socketInfo">套接字的信息。</param>
        /// <param name="maxConnectionCount">最大连接数。</param>
        /// <param name="maxBufferSize">最大缓冲区大小。</param>
        public AsyncSocketServer(SocketInfo socketInfo, int maxConnectionCount, int maxBufferSize)
            : this(socketInfo, maxConnectionCount, maxBufferSize, maxConnectionCount / 10) { }

        /// <summary>
        /// 指定最大连接数、接收的最大缓冲区长度和最大的侦听并发数，初始化一个 <see cref="Aoite.Net.AsyncSocketServer"/> 类的新实例。
        /// </summary>
        /// <param name="socketInfo">套接字的信息。</param>
        /// <param name="maxConnectionCount">最大连接数。</param>
        /// <param name="maxBufferSize">最大缓冲区大小。</param>
        /// <param name="listenBacklog">最大的侦听并发数。</param>
        public AsyncSocketServer(SocketInfo socketInfo, int maxConnectionCount, int maxBufferSize, int listenBacklog)
            : base(socketInfo)
        {
            if(maxConnectionCount < 1024) maxConnectionCount = 1024;
            if(maxBufferSize < 128) maxBufferSize = 128;
            if(listenBacklog < 1) listenBacklog = maxConnectionCount / 10;

            this._MaxBufferSize = maxBufferSize;
            this._MaxConnectionCount = maxConnectionCount;
            this._ListenBacklog = listenBacklog;

            this._connectionSemaphore = new Semaphore(listenBacklog + 1, maxConnectionCount
                , socketInfo.EndPoint.ToString() + ".ConnectionSemaphore");

            this._bufferManager = new BufferManager(listenBacklog, this._MaxBufferSize);
            this._saeaPool = this.CreateSaeaPool();
            this._clientPool = new AcceptedClientPool(this);
        }

        internal virtual SaeaPool CreateSaeaPool()
        {
            return new SaeaPool(this.SocketAsyncCompleted);
        }

        #endregion

        #region Events

        /// <summary>
        /// 打开通讯连接时发生。
        /// </summary>
        protected override void OnOpen()
        {
            if(this._serverSocket == null)
            {
                this._serverSocket = this.SocketInfo.CreateSocket();
                this._serverSocket.SendBufferSize = this._MaxBufferSize;
                this._serverSocket.ReceiveBufferSize = this._MaxBufferSize;
                this._serverSocket.Bind(this.SocketInfo.EndPoint);
                this._serverSocket.Listen(this._ListenBacklog);
            }
            this._acceptClients = new ConcurrentDictionary<Socket, IAcceptedClient>();
            this.AcceptAsync();
        }

        /// <summary>
        /// 关闭通讯连接时发生。
        /// </summary>
        protected override void OnClose()
        {
            if(this._acceptClients != null)
            {
                while(this.IsBusy || this._acceptClients.Count > 0)
                {
                    Thread.Sleep(333);
                }
            }

            this._serverSocket.Shutdown(false);
            this._acceptClients = null;
        }

        /// <summary>
        /// 异步套接字连接后发生。
        /// </summary>
        public event SaeaEventHandler AsyncConnected;
        /// <summary>
        /// 异步套接字连接成功后发生的方法。
        /// </summary>
        /// <param name="saea">当前事件参数关联的 <see cref="Aoite.Net.SaeaEventArgs"/>。</param>
        protected virtual void OnAsyncConnected(SaeaEventArgs saea)
        {
            var handler = this.AsyncConnected;
            if(handler != null) handler(this, saea);
        }

        /// <summary>
        /// 异步套接字断开后发生。
        /// </summary>
        public event SaeaEventHandler AsyncDisconnected;
        /// <summary>
        /// 异步套接字断开连接后发生的方法。
        /// </summary>
        /// <param name="saea">当前事件参数关联的 <see cref="Aoite.Net.SaeaEventArgs"/>。</param>
        protected virtual void OnAsyncDisconnected(SaeaEventArgs saea)
        {
            var handler = this.AsyncDisconnected;
            if(handler != null) handler(this, saea);
        }

        #endregion

        #region Async

        /// <summary>
        /// 开始一个异步操作来接受一个传入的连接尝试。
        /// </summary>
        protected virtual void AcceptAsync()
        {
            if(!this.IsRunning) return;
            //- 分配 Saea 资源
            var saea = this._saeaPool.Acquire();
            if(!this._serverSocket.AcceptAsync(saea)) this.ProcessAccept(saea);
        }

        /// <summary>
        /// 将数据异步发送到连接的 <see cref="System.Net.Sockets.Socket"/> 对象。
        /// </summary>
        /// <param name="acceptSocket">连接的 <see cref="System.Net.Sockets.Socket"/> 对象。</param>
        /// <param name="setBuffer">设置缓冲区的委托。</param>
        /// <param name="sendAsync">发送数据的委托。</param>
        /// <returns>返回一个值，指示异步发送的指令是否已完成。</returns>
        protected virtual bool Send(Socket acceptSocket
            , Action<SaeaEventArgs> setBuffer
            , Action<SaeaEventArgs> sendAsync)
        {
            IAcceptedClient client;
            if(!this._acceptClients.TryGetValue(acceptSocket, out client)) return false;
            if(!acceptSocket.Connected) return false;

            //- 设置缓冲区发送大小
            acceptSocket.SendBufferSize = this._MaxBufferSize;

            //- 分配 Saea 资源
            var sendSaea = this._saeaPool.Acquire();
            sendSaea.Client = client;
            sendSaea.AcceptSocket = acceptSocket;
            setBuffer(sendSaea);
            sendAsync(sendSaea);
            return true;
        }

        /// <summary>
        /// 将文件集合或者内存中的数据缓冲区以异步方法发送给连接的 <see cref="System.Net.Sockets.Socket"/> 对象。
        /// </summary>
        /// <param name="acceptSocket">连接的 <see cref="System.Net.Sockets.Socket"/> 对象。</param>
        /// <param name="packets">要发送的缓冲区数组的 <see cref="System.Net.Sockets.SendPacketsElement"/> 对象数组。</param>
        /// <returns>返回一个值，指示异步发送的指令是否已完成。</returns>
        public virtual bool SendPacketsAsync(Socket acceptSocket, params SendPacketsElement[] packets)
        {
            return this.Send(acceptSocket
                , sendSaea => sendSaea.SendPacketsElements = packets
                , base.SendPacketsAsync);
        }

        /// <summary>
        /// 将数据异步发送到连接的 <see cref="System.Net.Sockets.Socket"/> 对象。
        /// </summary>
        /// <param name="acceptSocket">连接的 <see cref="System.Net.Sockets.Socket"/> 对象。</param>
        /// <param name="buffer">要用于异步套接字方法的数据缓冲区。</param>
        /// <param name="offset">数据缓冲区中操作开始位置处的偏移量，以字节为单位。</param>
        /// <param name="count">可在缓冲区中发送或接收的最大数据量（以字节为单位）。</param>
        /// <returns>返回一个值，指示异步发送的指令是否已完成。</returns>
        public virtual bool SendAsync(Socket acceptSocket, byte[] buffer, int offset, int count)
        {
            return this.Send(acceptSocket
                , sendSaea => sendSaea.SetBuffer(buffer, offset, count)
                , base.SendAsync);
        }

        #endregion

        #region Process

        /// <summary>
        /// 处理一个异步操作来接受一个传入的连接尝试。
        /// </summary>
        /// <param name="saea">当前方法关联的 <see cref="Aoite.Net.SaeaEventArgs"/>。</param>
        protected override void ProcessAccept(SaeaEventArgs saea)
        {
            if(!this.IsRunning) return;
            var acceptSocket = saea.AcceptSocket;
            if(acceptSocket == null || acceptSocket.RemoteEndPoint == null || !acceptSocket.Connected) return;

            //- 分配 Client 资源，并附加当前 Saea
            var client = this._clientPool.Acquire();
            client.Attach(saea, acceptSocket.RemoteEndPoint.ToString());

            //- 设置缓冲区接收大小
            acceptSocket.ReceiveBufferSize = this._MaxBufferSize;
            //- 分配缓冲区
            this._bufferManager.AssignBuffer(saea);
            //- 增加连接数资源
            this._connectionSemaphore.WaitOne();
            //- 加入客户端列表
            this._acceptClients.TryAdd(acceptSocket, saea.Client);

            this.OnAsyncConnected(saea);

            this.ReceiveAsync(saea);
            this.AcceptAsync();
        }

        /// <summary>
        /// 处理一个已经断开连接的 <see cref="Aoite.Net.SaeaEventArgs"/> 对象。
        /// </summary>
        /// <param name="saea">当前事件参数关联的 <see cref="Aoite.Net.SaeaEventArgs"/>。</param>
        protected override void ProcessShutdown(SaeaEventArgs saea)
        {
            base.ProcessShutdown(saea);

            //- 释放缓冲区
            this._bufferManager.FreeBuffer(saea);
            //- 释放连接数资源
            this._connectionSemaphore.Release();
            //- 移出客户端列表
            IAcceptedClient s2;
            this._acceptClients.TryRemove(saea.AcceptSocket, out s2);

            this.OnAsyncDisconnected(saea);

            //- 释放 Client 资源
            this._clientPool.Release((DefaultAcceptedClient)saea.Client);
            //- 释放 Saea 资源
            this._saeaPool.Release(saea);
        }

        /// <summary>
        /// 处理一个完成异步数据发送请求的 <see cref="Aoite.Net.SaeaEventArgs"/> 对象。
        /// </summary>
        /// <param name="saea">当前方法关联的 <see cref="Aoite.Net.SaeaEventArgs"/>。</param>
        protected override void ProcessSend(SaeaEventArgs saea)
        {
            base.ProcessSend(saea);
            this._saeaPool.Release(saea);
        }

        #endregion

        /// <summary>
        /// 执行与释放或重置托管资源相关的应用程序定义的任务。
        /// </summary>
        protected override void DisposeManaged()
        {
            if(this._saeaPool != null) this._saeaPool.Dispose();
            if(this._bufferManager != null) this._bufferManager.Dispose();
            if(this._connectionSemaphore != null) this._connectionSemaphore.Dispose();
            if(this._clientPool != null) this._clientPool.Dispose();
            if(this._serverSocket != null) this._serverSocket.Shutdown(true);

            this._saeaPool = null;
            this._bufferManager = null;
            this._connectionSemaphore = null;
            this._clientPool = null;
            this._serverSocket = null;

            base.DisposeManaged();
        }

    }
}
