using Aoite.Net.LoadBalancing.Configurations;
using Aoite.Net.LoadBalancing.Strategies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Aoite.Net.LoadBalancing.Elements
{
    /// <summary>
    /// 表示一个负载均衡的主机元素。
    /// </summary>
    public class LbHostElement : LbElementBase
    {
        const string TypeNameKey = "Strategy.Custom.TypeName";
        private LbHostConfiguration _configuration;
        /// <summary>
        /// 设置或获取一个值，该值指示当前主机是否允许远程管理。默认为 false。
        /// </summary>
        public bool AllowRemoteManage { get; set; }
        /// <summary>
        /// 设置或获取一个值，表示判定负载均衡节点的故障时间（毫秒）。默认为 5,000ms。
        /// </summary>
        public int Timeout { get; set; }
        /// <summary>
        /// 设置或获取一个值，表示尝试恢复故障节点的时间（毫秒）。默认为 60,000ms。
        /// </summary>
        public int RecoveryTimes { get; set; }
        /// <summary>
        /// 设置或获取负载均衡的节点策略。
        /// </summary>
        public LbStrategyBase Strategy { get; set; }

        /// <summary>
        /// 获取负载均衡的节点集。
        /// </summary>
        public LbNodeElement[] Nodes { get; private set; }

        /// <summary>
        /// 获取扩展的用户数据集合。
        /// </summary>
        public Dictionary<string, object> ExtendData
        {
            get { return this._configuration.ExtendData; }
        }

        private LbStrategyBase CreateStrategy(LbHostConfiguration configuration)
        {
            switch(configuration.Strategy)
            {
                case LbStrategy.IPQueue:
                    return new LbIPQueueStrategy(this);

                case LbStrategy.Queue:
                    return new LbQueueStrategy(this);

                case LbStrategy.Custom:
                    string typeName = null;
                    object o;
                    if(configuration.ExtendData.TryGetValue(TypeNameKey, out o))
                    {
                        if(o is string) typeName = (string)o;
                        else throw new ArgumentException("值必须是一个 String 类型。", TypeNameKey);
                    }
                    if(string.IsNullOrEmpty(typeName)) throw new ArgumentNullException(TypeNameKey);
                    Type type = Type.GetType(typeName, true, true);
                    return (LbStrategyBase)Activator.CreateInstance(type, this);
            }
            throw new NotSupportedException();
        }

        /// <summary>
        /// 提供负载均衡的配置，初始化一个 <see cref="Aoite.Net.LoadBalancing.Elements.LbHostElement"/> 类的新实例。
        /// </summary>
        /// <param name="configuration">负载均衡的配置。</param>
        public LbHostElement(LbHostConfiguration configuration)
        {
            if(configuration == null) throw new ArgumentNullException("configuration");
            if(configuration.Timeout < -1) throw new ArgumentOutOfRangeException("configuration.Timeout");
            if(configuration.RecoveryTimes < 10) throw new ArgumentOutOfRangeException("configuration.RecoveryTimes");

            this._configuration = configuration;
            this.EndPoint = configuration.ToIPEndPoint();

            this.AllowRemoteManage = configuration.AllowRemoteManage;
            if(configuration.Timeout != System.Threading.Timeout.Infinite) this.Timeout = configuration.Timeout * 1000;
            this.RecoveryTimes = configuration.RecoveryTimes * 1000;
            this.Nodes = new LbNodeElement[configuration.Nodes.Count];

            for(int i = 0; i < this.Nodes.Length; i++)
            {
                var node = configuration.Nodes[i];

                this.Nodes[i] = new LbNodeElement()
                {
                    IsEnabled = true,
                    EndPoint = node.ToIPEndPoint().ToLoopback(),
                    Weight = node.Weight < 1 ? 1 : node.Weight
                };
            }
            this.Strategy = this.CreateStrategy(configuration);
        }
    }
}
