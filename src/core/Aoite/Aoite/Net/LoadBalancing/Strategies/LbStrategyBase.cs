using Aoite.Net.LoadBalancing.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Aoite.Net.LoadBalancing.Strategies
{
    /// <summary>
    /// 表示一个负载均衡的节点策略基类。
    /// </summary>
    public abstract class LbStrategyBase : IDisposable
    {
        private LbHostElement _Host;
        /// <summary>
        /// 获取负载均衡的主机元素。
        /// </summary>
        public LbHostElement Host { get { return this._Host; } }

        private bool _IsDisposed;
        /// <summary>
        /// 获取一个值，表示当前对象是否已释放资源。
        /// </summary>
        public bool IsDisposed { get { return this._IsDisposed; } }

        /// <summary>
        /// 提供负载均衡的主机元素，初始化一个 <see cref="Aoite.Net.LoadBalancing.Strategies.LbStrategyBase"/> 类的新实例。
        /// </summary>
        /// <param name="host">负载均衡的主机元素。</param>
        public LbStrategyBase(LbHostElement host)
        {
            if(host == null) throw new ArgumentNullException("host");
            this._Host = host;
        }

        /// <summary>
        /// 提供用户的客户端连接，获取负载均衡的节点元素。
        /// </summary>
        /// <param name="userClient">来自用户的客户端连接。</param>
        /// <returns>返回一个负载均衡的节点元素。</returns>
        protected abstract LbNodeElement OnGetNode(Socket userClient);

        /// <summary>
        /// 提供用户的客户端连接，创建一个服务端的客户端连接。
        /// </summary>
        /// <param name="userClient">来自用户的客户端连接。</param>
        /// <returns>一个服务端的客户端连接。</returns>
        internal virtual TcpClient CreateServerClient(TcpClient userClient)
        {
            LbNodeElement node = this.OnGetNode(userClient.Client);
            if(node == null) return null;
            TcpClient targetClient = new TcpClient();
            try
            {
                targetClient.Connect(node.EndPoint, this.Host.Timeout);
            }
            catch(Exception ex)
            {
                node.ToFailed(this.Host, ex);
                return this.CreateServerClient(userClient);
            }
            return targetClient;
        }

        /// <summary>
        /// 提供用户的客户端连接，获取负载均衡的节点元素。
        /// </summary>
        /// <param name="userClient">来自用户的客户端连接。</param>
        /// <returns>返回一个负载均衡的节点元素。</returns>
        public LbNodeElement GetNode(Socket userClient)
        {
            return this.OnGetNode(userClient);
        }

        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        /// <param name="disposing">为 true 则释放托管资源和非托管资源；为 false 则仅释放非托管资源。</param>
        protected virtual void Dispose(bool disposing)
        {
            if(this._IsDisposed) return;
            this._IsDisposed = true;
        }

        /// <summary>
        /// 析构函数。
        /// </summary>
        ~LbStrategyBase()
        {
            this.Dispose(false);
        }
    }
}
