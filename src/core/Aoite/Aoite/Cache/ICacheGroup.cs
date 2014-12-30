using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Cache
{
    /// <summary>
    /// 定义一个缓存的分组。
    /// </summary>
    public interface ICacheGroup : IDisposable
    {
        /// <summary>
        /// 获取或设置缓存数据。当设置的值为 null 时，则移除这个缓存。
        /// </summary>
        /// <param name="key">缓存标识。</param>
        /// <returns>如果当前会话存在缓存标识，则返回缓存值，否则返回 null 值。</returns>
        object this[string key] { get; set; }
        /// <summary>
        /// 延长键的过期间隔。
        /// </summary>
        /// <param name="key">要插入的缓存项的唯一标识符。</param>
        /// <param name="slidingExpiration">缓存项的弹性的过期间隔。</param>
        void Expire(string key, TimeSpan slidingExpiration);
        /// <summary>
        /// 使用键和值将某个缓存项插入缓存中，并指定基于间隔的过期详细信息。
        /// </summary>
        /// <param name="key">要插入的缓存项的唯一标识符。</param>
        /// <param name="value">该缓存项的数据。</param>
        /// <param name="slidingExpiration">缓存项的弹性的过期间隔。</param>
        void Set(string key, object value, TimeSpan slidingExpiration);
        /// <summary>
        /// 使用键和值将某个缓存项插入缓存中。
        /// </summary>
        /// <param name="key">要插入的缓存项的唯一标识符。</param>
        /// <param name="value">该缓存项的数据。</param>
        void Set(string key, object value);
        /// <summary>
        /// 从缓存中返回一个项。
        /// </summary>
        /// <param name="key">要获取的缓存项的唯一标识符。</param>
        /// <returns>如果该项存在，则为对 key 标识的缓存项的引用；否则为 null。</returns>
        object Get(string key);
        /// <summary>
        /// 从缓存中移除一个或多个缓存项。
        /// </summary>
        /// <param name="keys">要移除的缓存项的唯一标识符数组。</param>
        void Remove(params string[] keys);
        /// <summary>
        /// 清空关联的缓存。
        /// </summary>
        void Clear();
        /// <summary>
        /// 确定缓存中是否存在某个缓存项。
        /// </summary>
        /// <param name="key">要搜索的缓存项的唯一标识符。</param>
        /// <returns>如果缓存中包含其键与 key 匹配的缓存项，则为 true；否则为 false。</returns>
        bool Contains(string key);
    }

}
