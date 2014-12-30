using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Cache
{
    /// <summary>
    /// 定义一个缓存的提供程序。
    /// </summary>
    public interface ICacheProvider : ICacheGroup
    {
        /// <summary>
        /// 提供锁的功能。
        /// </summary>
        /// <param name="key">锁的键名。</param>
        /// <param name="timeout">锁的超时设定。</param>
        /// <returns>返回一个锁。</returns>
        IDisposable Lock(string key, TimeSpan? timeout = null);
        /// <summary>
        /// 清空所有全局锁。
        /// </summary>
        void ClearLockKeys();
        /// <summary>
        /// 获取指定会话标识的缓存分组。
        /// </summary>
        /// <param name="sessionId">会话标识。</param>
        /// <returns>返回一个缓存分组。</returns>
        ICacheGroup CreatetCacheGroup(string sessionId);
        /// <summary>
        /// 获取指定键的原子递增序列。
        /// </summary>
        /// <param name="key">序列的键。</param>
        /// <param name="increment">递增量。</param>
        /// <returns>返回递增的序列。</returns>
        long Increment(string key, long? increment = null);
        /// <summary>
        /// 重置原子递增。
        /// </summary>
        void ResetAllIncrement();
    }
}
