using Aoite.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Cache
{
    internal class RedisCacheItem
    {
        public object Value;
        public int? Expiration;
    }
    /// <summary>
    /// 表示一个实现 Redis 的缓存。
    /// </summary>
    public class RedisCacheGroup : ObjectDisposableBase, ICacheGroup
    {
        private string _keyPrefix;
        /// <summary>
        /// 默认的滑动超时时间。
        /// </summary>
        protected TimeSpan _defaultSlidingExpiration;
        /// <summary>
        /// Redis 客户端的对象池。
        /// </summary>
        protected ObjectPool<RedisClient> _clients;

        /// <summary>
        /// 提供相关参数，初始化一个 <see cref="Aoite.Cache.RedisCacheGroup"/> 类的新实例。
        /// </summary>
        /// <param name="clients">Redis 客户端的对象池。</param>
        /// <param name="sessionId">用户的会话标识。</param>
        /// <param name="defaultSlidingExpiration">默认的滑动超时时间。</param>
        internal RedisCacheGroup(ObjectPool<RedisClient> clients, string sessionId, TimeSpan defaultSlidingExpiration)
        {
            if(string.IsNullOrEmpty(sessionId)) throw new ArgumentNullException("sessionId");
            this.Initialize(clients, defaultSlidingExpiration, BitConverter.ToInt64(Guid.Parse(sessionId).ToByteArray(), 0) + ":");
        }

        /// <summary>
        /// 初始化一个空的 <see cref="Aoite.Cache.RedisCacheGroup"/> 类的新实例。
        /// </summary>
        internal RedisCacheGroup() { }

        /// <summary>
        /// 初始化  Redis 的缓存。
        /// </summary>
        /// <param name="clients">Redis 客户端的对象池。</param>
        /// <param name="defaultSlidingExpiration">默认的滑动超时时间。</param>
        /// <param name="keyPrefix">键的前缀。</param>
        protected void Initialize(ObjectPool<RedisClient> clients, TimeSpan defaultSlidingExpiration, string keyPrefix)
        {
            if(clients == null) throw new ArgumentNullException("clients");
            if(defaultSlidingExpiration.TotalSeconds < 1.0) throw new ArgumentOutOfRangeException("defaultSlidingExpiration");

            this._clients = clients;
            this._defaultSlidingExpiration = defaultSlidingExpiration;
            this._keyPrefix = keyPrefix;
        }

        /// <summary>
        /// 创建指定键的完整标识。
        /// </summary>
        /// <param name="key">缓存标识。</param>
        /// <returns>返回缓存的标识。</returns>
        protected string CreateKey(string key)
        {
            return this._keyPrefix + key;
        }

        /// <summary>
        /// 获取或设置会话缓存数据。当设置的值为 null 时，则移除这个缓存。
        /// </summary>
        /// <param name="key">缓存标识。</param>
        /// <returns>如果当前会话存在缓存标识，则返回缓存值，否则返回 null 值。</returns>
        public object this[string key]
        {
            get
            {
                return this.Get(key);
            }
            set
            {
                if(value == null) this.Remove(key);
                else this.Set(key, value);
            }
        }

        /// <summary>
        /// 使用键和值将某个缓存项插入缓存中，并指定基于间隔的过期详细信息。
        /// </summary>
        /// <param name="key">要插入的缓存项的唯一标识符。</param>
        /// <param name="value">该缓存项的数据。</param>
        /// <param name="slidingExpiration">缓存项的弹性的过期间隔。</param>
        public void Set(string key, object value, TimeSpan slidingExpiration)
        {
            this.Set(key, value, (int)slidingExpiration.TotalSeconds);
        }

        private void Set(string key, object value, double expiration)
        {
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            if(value == null) return;
            this.ThrowWhenDisposed();
            key = this.CreateKey(key);
            var item = new RedisCacheItem { Value = value, Expiration = expiration < 0 ? null : (int?)expiration };
            this._clients.AcquireRelease(client =>
                client.Set(key, Serializer.Quickly.FastWriteBytes(item)
                , item.Expiration));
        }

        /// <summary>
        /// 使用键和值将某个缓存项插入缓存中。
        /// </summary>
        /// <param name="key">要插入的缓存项的唯一标识符。</param>
        /// <param name="value">该缓存项的数据。</param>
        public void Set(string key, object value)
        {
            this.Set(key, value, this._defaultSlidingExpiration);
        }

        /// <summary>
        /// 从缓存中返回一个项。
        /// </summary>
        /// <param name="key">要获取的缓存项的唯一标识符。</param>
        /// <returns>如果该项存在，则为对 key 标识的缓存项的引用；否则为 null。</returns>
        public object Get(string key)
        {
            this.ThrowWhenDisposed();

            key = this.CreateKey(key);
            return this._clients.AcquireRelease(client =>
            {
                var valueBytes = client.GetBytes(key);
                if(valueBytes == null) return null;
                var item = Serializer.Quickly.FastReadBytes(valueBytes) as RedisCacheItem;
                if(item.Expiration.HasValue)
                {
                    client.ExpireAsync(key, item.Expiration.Value);
                }
                return item.Value;
            });
        }

        /// <summary>
        /// 延长键的过期间隔。
        /// </summary>
        /// <param name="key">要插入的缓存项的唯一标识符。</param>
        /// <param name="slidingExpiration">缓存项的弹性的过期间隔。</param>
        public void Expire(string key, TimeSpan slidingExpiration)
        {
            this._clients.AcquireRelease(client => client.ExpireAsync(this.CreateKey(key)
                , (int)slidingExpiration.TotalSeconds));
        }
        /// <summary>
        /// 从缓存中移除一个或多个缓存项。
        /// </summary>
        /// <param name="keys">要移除的缓存项的唯一标识符数组。</param>
        public void Remove(params string[] keys)
        {
            this.ThrowWhenDisposed();
            keys = keys.Each(key => this.CreateKey(key));
            this._clients.AcquireRelease(client => client.Del(keys));
        }

        /// <summary>
        /// 确定缓存中是否存在某个缓存项。
        /// </summary>
        /// <param name="key">要搜索的缓存项的唯一标识符。</param>
        /// <returns>如果缓存中包含其键与 key 匹配的缓存项，则为 true；否则为 false。</returns>
        public bool Contains(string key)
        {
            this.ThrowWhenDisposed();

            key = this.CreateKey(key);
            return this._clients.AcquireRelease(client => client.Exists(key));
        }

        /// <summary>
        /// 清空关联的缓存。
        /// </summary>
        public void Clear()
        {
            this._clients.AcquireRelease(client =>
            {
                client.Del(client.Keys(this._keyPrefix));
            });
        }

        /// <summary>
        /// 执行与释放或重置托管资源相关的应用程序定义的任务。
        /// </summary>
        protected override void DisposeManaged()
        {
            this._clients = null;
        }
    }

}
