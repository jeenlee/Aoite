using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Net.LoadBalancing
{
    /// <summary>
    /// 定义负载均衡的节点策略。
    /// </summary>
    public enum LbStrategy
    {
        /// <summary>
        /// 表示 IP 队列模式，根据用户的客户端连接 IP，在相同的弹性时间（扩展的用户数据集合中包含键名“Strategy.IPQueue.Expiration”）内，访问的都是相同的负载均衡节点。
        /// </summary>
        IPQueue,
        /// <summary>
        /// 表示队列模式，每一次连接都访问不同的负载均衡节点。
        /// </summary>
        Queue,
        /// <summary>
        /// 表示自定义模式，需要扩展的用户数据集合中包含键名为 "Strategy.Custom.TypeName" 的数据。
        /// </summary>
        Custom,
    }
}
