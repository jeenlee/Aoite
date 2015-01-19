using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Redis
{
    /// <summary>
    /// 表示 Redis 的回复类型。
    /// </summary>
    public enum RedisReplyType
    {
        /// <summary>
        /// '+' 状态回复。
        /// </summary>
        Status = '+',
        /// <summary>
        /// '-' 错误回复。
        /// </summary>
        Error = '-',
        /// <summary>
        /// ':' 整数回复。
        /// </summary>
        Integer = ':',
        /// <summary>
        /// '$' 批量回复。
        /// </summary>
        Bulk = '$',
        /// <summary>
        /// '*'多条批量回复。
        /// </summary>
        MultiBulk = '*',
    }
}
