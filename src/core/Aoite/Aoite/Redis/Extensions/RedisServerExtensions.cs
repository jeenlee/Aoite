using Aoite.Redis;
using Aoite.Redis.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// 表示 Redis 的 Server 扩展方法。
    /// </summary>
    public static class RedisServerExtensions
    {
        /// <summary>
        /// 清空整个 Redis 服务器的数据(删除所有数据库的所有键)。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        public static Result FlushAll(this IRedisClient client)
        {
            return client.Execute(new RedisStatus("FLUSHALL"));
        }
    }
}
