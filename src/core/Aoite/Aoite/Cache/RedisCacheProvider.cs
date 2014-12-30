using Aoite.Cache;
using Aoite.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Aoite.Cache
{
    /// <summary>
    /// 表示一个基于 Redis 的缓存提供程序。
    /// </summary>
    public class RedisCacheProvider : RedisCacheGroup, ICacheProvider
    {
        const string LockKey = "$Contract.LockKey$";
        const string AIKey = "$Contract.AIKey$";
        private int _minTimeout;

        /// <summary>
        /// 提供契约服务器，初始化一个 <see cref="Aoite.Cache.RedisCacheProvider"/> 类的新实例。
        /// </summary>
        /// <param name="options">缓存配置。</param>
        public RedisCacheProvider(Dictionary<string, object> options)
        {
            if(options == null) throw new ArgumentNullException("options");

            var defaultSlidingExpiration = TimeSpan.FromSeconds(options.TryGetValue(CacheOptions.SessionExpiration).CastTo<int?>() ?? 1200);
            this._minTimeout = options.TryGetValue(CacheOptions.MinTimeout).CastTo<int?>() ?? 5000;
            var redisHost = Convert.ToString(options.TryGetValue("RedisHost"));
            var redisPort = options.TryGetValue("RedisPort").CastTo<int>();
            var maxRedisClientCount = options.TryGetValue("MaxRedisClientCount").CastTo<int?>()
                ?? 1111;
            if(string.IsNullOrEmpty(redisHost)) throw new ArgumentNullException("redisHost");
            if(redisPort < 1 || redisPort > 65535) throw new ArgumentOutOfRangeException("redisPort");

            this.Initialize(new RedisPool(redisHost, redisPort, Convert.ToString(options.TryGetValue("RedisPassword"))) { MaxObjectCount = maxRedisClientCount }
                , defaultSlidingExpiration, "G:");

            //this._clients.AcquireRelease(client => client.Ping()).ThrowIfFailded();
        }

        /// <summary>
        /// 获取指定会话标识的缓存。
        /// </summary>
        /// <param name="sessionId">会话标识。</param>
        /// <returns>返回一个缓存。</returns>
        public ICacheGroup CreatetCacheGroup(string sessionId)
        {
            this.ThrowWhenDisposed();

            return new RedisCacheGroup(this._clients, sessionId, this._defaultSlidingExpiration);
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
            var lockKey = this.CreateKey(LockKey);
            if(!this._clients.AcquireRelease(client =>
                SpinWait.SpinUntil(() =>
                    client.HSetNx(lockKey, key, 1), milliseconds)))
                LockItem.TimeoutError(key);

            return new LockItem(() => this._clients.AcquireRelease(client => client.HDel(lockKey, key)));
        }

        /// <summary>
        /// 清空所有全局锁。
        /// </summary>
        public void ClearLockKeys()
        {
            this.ThrowWhenDisposed();
            this._clients.AcquireRelease(client => client.Del(this.CreateKey(LockKey)));
        }

        /// <summary>
        /// 重置原子递增。
        /// </summary>
        public void ResetAllIncrement()
        {
            this.ThrowWhenDisposed();
            var aiKey = this.CreateKey(AIKey);
            this._clients.AcquireRelease(client => client.Del(aiKey));
        }

        /// <summary>
        /// 调用 Redis 客户端。
        /// </summary>
        /// <param name="callback">回调函数。</param>
        public void Call(Action<RedisClient> callback)
        {
            this.ThrowWhenDisposed();
            this._clients.AcquireRelease(callback);
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
            var aiKey = this.CreateKey(AIKey);
            if(increment.HasValue) return this._clients.AcquireRelease(client => client.HIncrBy(aiKey, key, increment.Value));
            return this._clients.AcquireRelease(client => client.HIncr(aiKey, key));
        }
    }
}
