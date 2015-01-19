using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Redis
{
    /// <summary>
    /// 表示 Redis 储存值的类型
    /// </summary>
    public enum RedisType
    {
        /// <summary>
        /// 表示无效或不存在的键类型。
        /// </summary>
        None,
        /// <summary>
        /// 表示字符串类型。
        /// </summary>
        String,
        /// <summary>
        /// 表示列表类型。
        /// </summary>
        List,
        /// <summary>
        /// 表示集合类型。
        /// </summary>
        Set,
        /// <summary>
        /// 表示有序集类型。
        /// </summary>
        ZSet,
        /// <summary>
        /// 表示哈希表类型。
        /// </summary>
        Hash,
    }
}
