using Aoite.Net.LoadBalancing.Elements;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Aoite.Net.LoadBalancing.Strategies
{
    /// <summary>
    /// 表示一个采用队列方式的节点策略。
    /// </summary>
    public class LbQueueStrategy : LbStrategyBase
    {
        /// <summary>
        /// 提供负载均衡的主机元素，初始化一个 <see cref="Aoite.Net.LoadBalancing.Strategies.LbQueueStrategy"/> 类的新实例。
        /// </summary>
        /// <param name="host">负载均衡的主机元素。</param>
        public LbQueueStrategy(LbHostElement host)
            : base(host)
        {
            this._nodes = new LinkedList<LbNodeElement>();
            var sort = from node in host.Nodes
                       orderby node.Weight descending
                       select node;
            foreach(var item in sort)
            {
                for(int i = 0; i < item.Weight; i++)
                {
                    this._nodes.AddLast(item);
                }
            }
            this._current = this._nodes.First;
        }

        private LinkedList<LbNodeElement> _nodes;
        private LinkedListNode<LbNodeElement> _current;

        /// <summary>
        /// 提供用户的客户端连接，获取负载均衡的节点元素。
        /// </summary>
        /// <param name="userClient">来自用户的客户端连接。</param>
        /// <returns>返回一个负载均衡的节点元素。</returns>
        protected override LbNodeElement OnGetNode(Socket userClient)
        {
            lock(this._nodes)
            {
                var current = this._current;
                for(int i = 0; i < this._nodes.Count; i++)
                {
                    var node = current.Value;
                    current = current.Next ?? this._nodes.First;
                    lock(node)
                    {
                        if(node.IsEnabled && !node.IsFailedNode)
                        {
                            this._current = current;
                            return node;
                        }
                    }
                }
            }
            return null;
        }
    }
}
