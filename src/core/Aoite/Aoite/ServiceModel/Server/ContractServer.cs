using Aoite.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Aoite.ServiceModel
{
    /// <summary>
    /// 表示一个契约服务器。
    /// </summary>
    public class ContractServer : ContractServerBase, ICommunication
    {
        private ServerConfiguration _Configuration;
        /// <summary>
        /// 获取服务器的配置信息。
        /// </summary>
        public ServerConfiguration Configuration { get { return this._Configuration; } }

        /// <summary>
        /// 获取服务端的终结点端口号。
        /// </summary>
        public int Port { get { return this._server.Port; } }

        MessageSocketServer _server;
  
        /// <summary>
        /// 提供配置信息，初始化一个 <see cref="Aoite.ServiceModel.ContractServer"/> 类的新实例。
        /// </summary>
        /// <param name="configuration">服务器的配置信息。</param>
        public ContractServer(ServerConfiguration configuration)
        {
            if(configuration == null) throw new ArgumentNullException("configuration");

            this._server = new MessageSocketServer(configuration.ToIPEndPoint(), configuration.MaxConnectionCount, configuration.MaxBufferSize, configuration.ListenBacklog);
            this._server.MessageReceive += _server_MessageReceive;

            this._Configuration = configuration;
            this.Initiailze();
        }

        /// <summary>
        /// 获取服务器的配置信息。
        /// </summary>
        /// <returns>返回服务器的配置信息。</returns>
        protected override ServerConfigurationBase GetConfiguration()
        {
            return this._Configuration;
        }

        private void _server_MessageReceive(object sender, MessageReceiveEventArgs e)
        {
            var requestResult = this._Configuration.Serializer.ReadBytes<ContractRequest>(e.MessageBuffer);
            if(requestResult.IsFailed)
            {
                this.OnError(requestResult.Exception);
                e.Client.Shutdown();
                return;
            }
            var request = requestResult.Value;
            var context = this.ProcessRequest(request, e.AcceptSocket.RemoteEndPoint.ToString());
            var writeResult = this._Configuration.Serializer.WriteBytes(context.Source.Response);
            if(writeResult.IsFailed)
            {
                this.OnError(writeResult.Exception);
                e.Client.Shutdown();
                return;
            }
            e.Client.SendAsync(writeResult.Value, 0, writeResult.Value.Length);
        }


        #region ICommunication

        /// <summary>
        /// 获取或设置与此客户端关联的用户或应用程序对象。
        /// </summary>
        public object Tag { get { return this._server.Tag; } set { this._server.Tag = value; } }

        /// <summary>
        /// 获取客户端自定义数据的集合。
        /// </summary>
        public AsyncDataCollection Data { get { return this._server.Data; } }

        /// <summary>
        /// 获取一个值，该值指示通讯是否正在运行中。
        /// </summary>
        public virtual bool IsRunning { get { return this._server.IsRunning; } }

        /// <summary>
        /// 获取一个值，表示通讯的状态。
        /// </summary>
        public CommunicationState State { get { return this._server.State; } }

        /// <summary>
        /// 通讯状态发生更改后发生。
        /// </summary>
        public event CommunicationStateEventHandler StateChanged
        {
            add { this._server.StateChanged += value; }
            remove { this._server.StateChanged -= value; }
        }

        /// <summary>
        /// 使用 <paramref name="key"/> 获取或设置自定义数据的集合的值。
        /// </summary>
        /// <param name="key">自定义数据的键。</param>
        /// <returns>获取一个已存在的自定义数据的值，或一个 null 值。</returns>
        public object this[string key] { get { return this._server.Data.TryGetValue(key); } set { this._server.Data[key] = value; } }

        /// <summary>
        /// 打开通讯连接。
        /// </summary>
        /// <returns>返回打开通讯的结果。</returns>
        public Result Open()
        {
            return this._server.Open();
        }

        /// <summary>
        /// 关闭通讯连接时发生。
        /// </summary>
        public void Close()
        {
            this._server.Close();
        }

        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        public void Dispose()
        {
            this._server.Dispose();
        }

        #endregion
    }
}
