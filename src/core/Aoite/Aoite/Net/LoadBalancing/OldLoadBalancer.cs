using Aoite.Net.LoadBalancing.Configurations;
using Aoite.Net.LoadBalancing.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Aoite.Net.LoadBalancing
{
    /// <summary>
    /// 表示一个负载均衡的主机。
    /// </summary>
    internal sealed class OldLoadBalancer : CommunicationBase
    {
        private LbHostElement _host;
        private TcpListener _listener;
        private LbHostConfiguration _Configuration;
        /// <summary>
        /// 获取负载均衡的主机配置。
        /// </summary>
        public LbHostConfiguration Configuration { get { return this._Configuration; } }

        /// <summary>
        /// 提供负载均衡的配置路径，初始化一个 <see cref="Aoite.Net.LoadBalancing.OldLoadBalancer"/> 类的新实例。
        /// </summary>
        /// <param name="path">负载均衡的配置路径。</param>
        public OldLoadBalancer(string path) : this(JsonConf.LoadFromFile<LbHostConfiguration>(path)) { }

        /// <summary>
        /// 提供负载均衡的配置，初始化一个 <see cref="Aoite.Net.LoadBalancing.OldLoadBalancer"/> 类的新实例。
        /// </summary>
        /// <param name="configuration">负载均衡的配置。</param>
        public OldLoadBalancer(LbHostConfiguration configuration)
        {
            this._Configuration = configuration;
        }

        /// <summary>
        /// 打开通讯连接时发生。
        /// </summary>
        protected override void OnOpen()
        {
            this._host = new LbHostElement(this._Configuration);
            this._listener = new TcpListener(this._host.EndPoint);
            this._listener.Start();
            this._listener.BeginAcceptTcpClient(AcceptAsyncCallback, null);
        }

        /// <summary>
        /// 关闭通讯时发生。
        /// </summary>
        protected override void OnClose()
        {
            this._listener.Stop();
            this._listener = null;
            this._host.Strategy.Dispose();
            this._host = null;
        }

        private void AcceptAsyncCallback(IAsyncResult ar)
        {
            TcpClient userClient;
            try
            {
                userClient = this._listener.EndAcceptTcpClient(ar);
            }
            catch(Exception)
            {
                return;
            }

            this._listener.BeginAcceptTcpClient(AcceptAsyncCallback, null);

            TcpClient serverClient = this._host.Strategy.CreateServerClient(userClient);
            if(serverClient == null)
            {
                userClient.Close();
                return;
                //  throw new PlatformNotSupportedException("找不到活动的负载均衡节点，可能是所有的负载均衡节点均已禁用或故障。")
            }
            AsyncState sourceState = new AsyncState(userClient, serverClient);
            AsyncState targetState = new AsyncState(serverClient, userClient);

            Task.Factory.StartNew(AsyncProcessing, sourceState);
            Task.Factory.StartNew(AsyncProcessing, targetState);
        }

        private void AsyncProcessing(object obj)
        {
            var state = obj as AsyncState;
            try
            {
                state.FromStream.CopyTo(state.ToStream);
            }
            catch(Exception)
            {
                state.ToClient.Close();
            }
        }

    }
}
