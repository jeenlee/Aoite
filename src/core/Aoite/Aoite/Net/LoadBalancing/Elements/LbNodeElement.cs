using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aoite.Net.LoadBalancing.Elements
{
    /// <summary>
    /// 表示一个负载均衡的节点元素。
    /// </summary>
    public class LbNodeElement : LbElementBase
    {
        /// <summary>
        /// 初始化一个 <see cref="Aoite.Net.LoadBalancing.Elements.LbNodeElement"/> 类的新实例。
        /// </summary>
        public LbNodeElement() { }
        /// <summary>
        /// 设置或获取负载均衡元素的权重。
        /// </summary>
        public int Weight { get; set; }

        /// <summary>
        /// 设置或获取一个值，该值指示当前负载均衡阶段是否启用。
        /// </summary>
        public bool IsEnabled { get; set; }
        /// <summary>
        /// 获取一个值，该值指示当前负载均衡阶段是否发生故障。
        /// </summary>
        public bool IsFailedNode { get; private set; }
        /// <summary>
        /// 获取负载均衡最后故障时间。
        /// </summary>
        public DateTime LastFailedTime { get; private set; }
        /// <summary>
        /// 获取负载均衡最后故障次数。
        /// </summary>
        public int LastFailedCount { get; set; }
        /// <summary>
        /// 获取负载均衡最后故障消息。
        /// </summary>
        public string LastFailedMessage { get; private set; }

        private void TryRecovery(object state)
        {
            var host = state as LbHostElement;
            TcpClient targetClient = new TcpClient();
            while(true)
            {
                Thread.Sleep(host.RecoveryTimes);
                try
                {
                    targetClient.Connect(this.EndPoint, host.Timeout);
                    this.IsFailedNode = false;
                    break;
                }
                catch(Exception ex)
                {
                    this.LastFailedTime = DateTime.Now;
                    this.LastFailedMessage = ex.Message;
                    this.LastFailedCount++;
                }
            }
        }

        /// <summary>
        /// 将当前负载均衡节点转为故障节点。
        /// </summary>
        /// <param name="host">负载均衡的主机元素。</param>
        /// <param name="exception">发生故障的原因。</param>
        public void ToFailed(LbHostElement host, Exception exception)
        {
            lock(this)
            {
                this.IsFailedNode = true;
                this.LastFailedTime = DateTime.Now;
                this.LastFailedCount = 1;
                this.LastFailedMessage = exception.Message;
            }
            Task.Factory.StartNew(this.TryRecovery, host);
        }
    }
}
