using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading;

namespace Aoite.Cache
{
    /// <summary>
    /// 表示一个基于内存的缓存提供程序，这并不建议发布在生产环境。
    /// </summary>
    public class MemoryCacheProvider : MemoryCacheGroup, ICacheProvider
    {
        private int _minTimeout;
        /// <summary>
        /// 初始化一个 <see cref="Aoite.Cache.MemoryCacheProvider"/> 类的新实例。
        /// </summary>
        /// <param name="options">缓存配置。</param>
        public MemoryCacheProvider(Dictionary<string, object> options)
            : base(options)
        {
            this._minTimeout = options.TryGetValue(CacheOptions.MinTimeout).CastTo<int?>() ?? 5000;
        }

        /// <summary>
        /// 初始化一个 <see cref="Aoite.Cache.MemoryCacheProvider"/> 类的新实例。
        /// </summary>
        public MemoryCacheProvider()
            : this(new Dictionary<string, object>()) { }

        /// <summary>
        /// 获取指定会话标识的缓存分组。
        /// </summary>
        /// <param name="sessionId">会话标识。</param>
        /// <returns>返回一个缓存分组。</returns>
        public virtual ICacheGroup CreatetCacheGroup(string sessionId)
        {
            this.ThrowWhenDisposed();

            if(string.IsNullOrEmpty(sessionId)) throw new ArgumentNullException("sessionId");

            ICacheGroup session = this._innerCache.Get(sessionId) as ICacheGroup;
            if(session == null)
            {
                lock(string.Intern(sessionId))
                {
                    session = this._innerCache.Get(sessionId) as ICacheGroup;
                    if(session == null)
                    {
                        session = new MemoryCacheGroup(this._defaultSlidingExpiration);
                        this._innerCache.Set(new CacheItem(sessionId, session),
                            new CacheItemPolicy() { SlidingExpiration = this._defaultSlidingExpiration, RemovedCallback = SessionExpirationCallBack });
                    }
                }
            }
            return session;
        }

        void SessionExpirationCallBack(CacheEntryRemovedArguments e)
        {
            var session = (e.CacheItem.Value as ICacheGroup);
            if(session != null) session.Dispose();
        }

        /// <summary>
        /// 提供锁的功能。
        /// </summary>
        /// <param name="key">锁的键名。</param>
        /// <param name="timeout">锁的超时设定。</param>
        /// <returns>返回一个锁。</returns>
        public IDisposable Lock(string key, TimeSpan? timeout = null)
        {
            this.ThrowWhenDisposed();

            int milliseconds = CacheOptions.GetLockTimeout(timeout, this._minTimeout);

            if(!Monitor.TryEnter(string.Intern(key), milliseconds))
                LockItem.TimeoutError(key);

            return new LockItem(() => Monitor.Exit(string.Intern(key)));
        }

        /// <summary>
        /// 清空所有全局锁。
        /// </summary>
        public void ClearLockKeys()
        {
            this.ThrowWhenDisposed();
        }

        /// <summary>
        /// 重置原子递增。
        /// </summary>
        public void ResetAllIncrement()
        {
            this.ThrowWhenDisposed();
        }

        /// <summary>
        /// 获取指定键的原子递增序列。
        /// </summary>
        /// <param name="key">序列的键。</param>
        /// <param name="increment">递增量。</param>
        /// <returns>返回递增的序列。</returns>
        public long Increment(string key, long? increment = null)
        {
            this.ThrowWhenDisposed();
            return GA.NewId();
        }

    }
}
