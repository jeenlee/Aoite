using Aoite.Net.LoadBalancing.Elements;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Caching;
using System.Text;

namespace Aoite.Net.LoadBalancing.Strategies
{
    /// <summary>
    /// 表示一个采用用户 IP 队列方式的节点策略。
    /// </summary>
    public class LbIPQueueStrategy : LbQueueStrategy
    {
        const string ExpirationKey = "Strategy.IPQueue.Expiration";
        private MemoryCache _cache = new MemoryCache(Guid.NewGuid().ToString());

        private TimeSpan _Expiration;
        /// <summary>
        /// 提供负载均衡的主机元素，初始化一个 <see cref="Aoite.Net.LoadBalancing.Strategies.LbIPQueueStrategy"/> 类的新实例。
        /// </summary>
        /// <param name="host">负载均衡的主机元素。</param>
        public LbIPQueueStrategy(LbHostElement host)
            : base(host)
        {
            int expiration = 60;
            object o;
            if(host.ExtendData.TryGetValue(ExpirationKey, out o))
            {
                if(o is int) expiration = (int)o;
                else throw new ArgumentException("值必须是一个 int 类型。", ExpirationKey);
            }
            if(expiration < 10) expiration = 10;
            this._Expiration = TimeSpan.FromSeconds(expiration);
        }

        private ConcurrentDictionary<long, LbNodeElement> DictCache = new ConcurrentDictionary<long, LbNodeElement>();

        /// <summary>
        /// 提供用户的客户端连接，获取负载均衡的节点元素。
        /// </summary>
        /// <param name="userClient">来自用户的客户端连接。</param>
        /// <returns>返回一个负载均衡的节点元素。</returns>
        protected override LbNodeElement OnGetNode(Socket userClient)
        {
            var endPoint = (userClient.RemoteEndPoint as IPEndPoint);
            var key = BitConverter.ToInt32(endPoint.Address.GetAddressBytes(), 0);
            LbNodeElement node = DictCache.GetOrAdd(key, k => base.OnGetNode(userClient));
            if(node == null) DictCache.TryRemove(key, out node);
            return node;
        }

        /// <summary>
        /// 提供用户的客户端连接 IP，从缓存中获取负载均衡的节点元素。
        /// </summary>
        /// <param name="ip">用户的客户端连接 IP。</param>
        /// <param name="node">缓存中的负载均衡节点元素，如果获取失败将会是一个 null 值。</param>
        /// <returns>返回一个值，表示是否获取成功。</returns>
        protected virtual bool GetNodeFromCache(string ip, out LbNodeElement node)
        {
            node = this._cache.Get(ip) as LbNodeElement;
            if(node != null)
            {
                lock(string.Intern(ip))
                {
                    lock(node)
                    {
                        if(node.IsEnabled && !node.IsFailedNode)
                        {
                            return true;
                        }
                    }
                    this._cache.Remove(ip);
                }
            }
            return false;
        }

        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        /// <param name="disposing">为 true 则释放托管资源和非托管资源；为 false 则仅释放非托管资源。</param>
        protected override void Dispose(bool disposing)
        {
            if(this.IsDisposed) return;
            if(disposing)
            {
                this._cache.Dispose();
                this._cache = null;
            }
            base.Dispose(disposing);
        }
    }
}
