using Aoite.Net;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace Aoite.ServiceModel
{
    /// <summary>
    /// 表示一个契约客户端的生命周期的基类。
    /// </summary>
    public class ContractLifeCycle : MessageSocketClient, IContractLifeCycle
    {
        private IContractClient _Client;
        /// <summary>
        /// 设置或获取所属的契约客户端。
        /// </summary>
        public IContractClient Client { get { return this._Client; } set { this._Client = value; } }

        /// <summary>
        /// 提供契约的域，初始化一个 <see cref="Aoite.ServiceModel.ContractLifeCycle"/> 类的新实例。
        /// </summary>
        /// <param name="domain">契约的域。</param>
        public ContractLifeCycle(ContractDomain domain) : base(domain.ToIPEndPoint(), domain.MaxBufferSize) { }

        private volatile Result<ContractResponse> _lastMessage;
        private AutoResetEvent _autoResetEvent = new AutoResetEvent(false);
        private bool _waiting;
        /// <summary>
        /// 接收消息后发生的方法。
        /// </summary>
        /// <param name="mrea">当前事件参数关联的 <see cref="Aoite.Net.MessageReceiveEventArgs"/>。</param>
        protected override void OnMessageReceive(MessageReceiveEventArgs mrea)
        {
            base.OnMessageReceive(mrea);
            var value = this._Client.Domain.Serializer.ReadBytes<ContractResponse>(mrea.MessageBuffer);
            this._lastMessage = value;
            this._autoResetEvent.Set();
        }
        /// <summary>
        /// 关闭通讯连接时发生。
        /// </summary>
        protected override void OnClose()
        {
            base.OnClose();
            if(this._waiting)
            {
                this._lastMessage = new Result<ContractResponse>(new InvalidOperationException("与服务端断开连接。原因可能是：\r\n1、服务端返回值类型错误（如将数据库返回结果直接返回）。\r\n2、网络异常。"));
                this._autoResetEvent.Set();
            }
        }

        /// <summary>
        /// 获取指定请求的响应。
        /// </summary>
        /// <param name="request">契约的请求。</param>
        /// <returns>返回一个请求的响应。</returns>
        public ContractResponse GetResponse(ContractRequest request)
        {
            var w_bytes = this.Client.Domain.Serializer.WriteBytes<ContractRequest>(request).UnsafeValue;

            if(!this.SendAsync(w_bytes, 0, w_bytes.Length)) throw new NotSupportedException();

            try
            {
                this._waiting = true;
                if(!this._autoResetEvent.WaitOne(this._Client.Domain.ResponseTimeout)) throw new TimeoutException();

                var value = this._lastMessage.UnsafeValue;
                this._lastMessage = null;
                return value;
            }
            finally
            {
                this._waiting = false;
            }
        }
    }
}
