using Aoite.Net.LoadBalancing.Configurations;
using Aoite.Net.LoadBalancing.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Net.LoadBalancing
{
    /// <summary>
    /// 表示一个负载均衡的主机。
    /// </summary>
    public sealed class LoadBalancer : AsyncSocketServer
    {
        private LbHostElement _host;
        private LbHostConfiguration _Configuration;
        /// <summary>
        /// 获取负载均衡的主机配置。
        /// </summary>
        public LbHostConfiguration Configuration { get { return this._Configuration; } }

        /// <summary>
        /// 提供负载均衡的配置路径，初始化一个 <see cref="Aoite.Net.LoadBalancing.LoadBalancer"/> 类的新实例。
        /// </summary>
        /// <param name="path">负载均衡的配置路径。</param>
        public LoadBalancer(string path) : this(JsonConf.LoadFromFile<LbHostConfiguration>(path)) { }

        /// <summary>
        /// 提供负载均衡的配置，初始化一个 <see cref="Aoite.Net.LoadBalancing.LoadBalancer"/> 类的新实例。
        /// </summary>
        /// <param name="configuration">负载均衡的配置。</param>
        public LoadBalancer(LbHostConfiguration configuration)
            : base(configuration.ToIPEndPoint(), configuration.MaxConnectionCount, configuration.MaxBufferSize, configuration.ListenBacklog)
        {
            this._Configuration = configuration;
        }

        /// <summary>
        /// 打开通讯连接时发生。
        /// </summary>
        protected override void OnOpen()
        {
            base.OnOpen();
            this._host = new LbHostElement(this._Configuration);
        }
        /// <summary>
        /// 关闭通讯连接时发生。
        /// </summary>
        protected override void OnClose()
        {
            base.OnClose();
            this._host.Strategy.Dispose();
            this._host = null;
        }

        /// <summary>
        /// 异步套接字连接成功后发生的方法。
        /// </summary>
        /// <param name="saea">当前事件参数关联的 <see cref="Aoite.Net.SaeaEventArgs"/>。</param>
        protected override void OnAsyncConnected(SaeaEventArgs saea)
        {
            base.OnAsyncConnected(saea);
            while(true)
            {
                var node = this._host.Strategy.GetNode(saea.AcceptSocket);
                if(node == null)
                {
                    this.ProcessShutdown(saea);
                    return;
                }
                AsyncSocketClient client = new AsyncSocketClient(node.EndPoint);
                var r = client.Open();
                if(r.IsFailed)
                {
                    node.ToFailed(this._host, r.Exception);
                    continue;
                }
                client.AsyncReceive += (ss, ee) =>
                {
                    saea.Client.SendAsync(ee.Buffer, ee.Offset, ee.BytesTransferred);
                };
                client.StateChanged += (ss, ee) =>
                {
                    if(ee.State == CommunicationState.Closed)
                    {
                        if(saea.Client != null && saea.AcceptSocket.Connected && saea.Client.Connected) this.ProcessShutdown(saea);
                    }
                };
                saea.UserToken = client;
                break;
            }
        }

        /// <summary>
        /// 异步套接字断开连接后发生的方法。
        /// </summary>
        /// <param name="saea">当前事件参数关联的 <see cref="Aoite.Net.SaeaEventArgs"/>。</param>
        protected override void OnAsyncDisconnected(SaeaEventArgs saea)
        {
            base.OnAsyncDisconnected(saea);
            var client = saea.UserToken as AsyncSocketClient;
            if(client != null && client.IsRunning) using(client) client.Close();
        }

        /// <summary>
        /// 异步套接字接收数据后发生的方法。
        /// </summary>
        /// <param name="saea">当前事件参数关联的 <see cref="Aoite.Net.SaeaEventArgs"/>。</param>
        protected override void OnAsyncReceive(SaeaEventArgs saea)
        {
            base.OnAsyncReceive(saea);
            var client = saea.UserToken as AsyncSocketClient;
            if(client != null && client.IsRunning) client.SendAsync(saea.Buffer, saea.Offset, saea.BytesTransferred);
        }

    }
}
