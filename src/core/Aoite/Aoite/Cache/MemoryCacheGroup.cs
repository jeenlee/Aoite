using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;

namespace Aoite.Cache
{
    using MC = System.Runtime.Caching.MemoryCache;
    /// <summary>
    /// 表示一个实现内存中的缓存分组。
    /// </summary>
    public class MemoryCacheGroup : ObjectDisposableBase, ICacheGroup
    {
        static MC NewMC() { return new MC(Guid.NewGuid().ToString()); }

        /// <summary>
        /// 内存缓存。
        /// </summary>
        protected MC _innerCache = NewMC();

        internal readonly TimeSpan _defaultSlidingExpiration;

        internal MemoryCacheGroup(TimeSpan defaultSlidingExpiration)
        {
            if(defaultSlidingExpiration.TotalSeconds < 1) throw new ArgumentOutOfRangeException("defaultSlidingExpiration");
            this._defaultSlidingExpiration = defaultSlidingExpiration;
        }

        internal MemoryCacheGroup(Dictionary<string, object> options)
        {
            if(options == null) throw new ArgumentNullException("options");
            this._defaultSlidingExpiration = TimeSpan.FromSeconds(options.TryGetValue(CacheOptions.SessionExpiration).CastTo<int?>() ?? 1200);
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
            this.ThrowWhenDisposed();

            this._innerCache.Set(new CacheItem(key, value)
                , new CacheItemPolicy() { SlidingExpiration = slidingExpiration });
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
        /// 延长键的过期间隔。
        /// </summary>
        /// <param name="key">要插入的缓存项的唯一标识符。</param>
        /// <param name="slidingExpiration">缓存项的弹性的过期间隔。</param>
        public void Expire(string key, TimeSpan slidingExpiration)
        {
            var obj = this._innerCache.Get(key);
            this.Set(key, obj, slidingExpiration);
        }
        /// <summary>
        /// 从缓存中返回一个项。
        /// </summary>
        /// <param name="key">要获取的缓存项的唯一标识符。</param>
        /// <returns>如果该项存在，则为对 key 标识的缓存项的引用；否则为 null。</returns>
        public object Get(string key)
        {
            this.ThrowWhenDisposed();

            return this._innerCache.Get(key);
        }

        /// <summary>
        /// 从缓存中移除一个或多个缓存项。
        /// </summary>
        /// <param name="keys">要移除的缓存项的唯一标识符数组。</param>
        public void Remove(params string[] keys)
        {
            this.ThrowWhenDisposed();

            foreach(var key in keys)
            {
                this._innerCache.Remove(key);
            }
        }

        /// <summary>
        /// 确定缓存中是否存在某个缓存项。
        /// </summary>
        /// <param name="key">要搜索的缓存项的唯一标识符。</param>
        /// <returns>如果缓存中包含其键与 key 匹配的缓存项，则为 true；否则为 false。</returns>
        public bool Contains(string key)
        {
            this.ThrowWhenDisposed();

            return this._innerCache.Contains(key);
        }

        /// <summary>
        /// 清空关联的缓存。
        /// </summary>
        public void Clear()
        {
            this.ClearCache();
            this._innerCache = NewMC();
        }

        /// <summary>
        /// 执行与释放或重置托管资源相关的应用程序定义的任务。
        /// </summary>
        protected override void DisposeManaged()
        {
            this.ClearCache();
        }

        private void ClearCache()
        {
            this._innerCache.Dispose();
            this._innerCache = null;
        }
    }
}
