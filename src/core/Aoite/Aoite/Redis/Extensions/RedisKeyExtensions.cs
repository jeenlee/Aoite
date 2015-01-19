using Aoite.Redis;
using Aoite.Redis.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// 表示 Redis 的 Key 扩展方法。
    /// </summary>
    public static class RedisKeyExtensions
    {
        /// <summary>
        /// 删除给定的一个或多个键。不存在的键会被忽略。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="keys">键的数组。</param>
        /// <returns>返回被删除键的数量。</returns>
        public static long Del(this IRedisClient client, params string[] keys)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(keys == null) throw new ArgumentNullException("keys");
            if(keys.Length == 0) return 0L;

            return client.Execute(new RedisInteger("DEL", keys));
        }

        /// <summary>
        /// 检查给定 <paramref name="key"/> 是否存在。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">键名。</param>
        /// <returns>如果 <paramref name="key"/> 存在，那么返回 true。否则返回 false。</returns>
        public static bool Exists(this IRedisClient client, string key)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            return client.Execute(new RedisBoolean("EXISTS", key));
        }

        /// <summary>
        /// 为给定 <paramref name="key"/> 设置生存时间，当 <paramref name="key"/> 过期时(生存时间为 0 )，它会被自动删除。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">键名。</param>
        /// <param name="expiration">生存时间。</param>
        /// <param name="timeUnit">定位生存时间的单位。</param>
        /// <returns>设置成功返回 true，否则返回 false。</returns>
        public static bool Expire(this IRedisClient client, string key, TimeSpan expiration,
            RedisExpireTimeUnit timeUnit = RedisExpireTimeUnit.EX)
        {
            return Expire(client, key, expiration.GetExpiration(timeUnit), timeUnit);
        }

        /// <summary>
        /// 为给定 <paramref name="key"/> 设置生存时间，当 <paramref name="key"/> 过期时(生存时间为 0 )，它会被自动删除。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">键名。</param>
        /// <param name="expiration">生存时间。</param>
        /// <param name="timeUnit">定位生存时间的单位。</param>
        /// <returns>如果设置生存时间成功返回 true，否则返回 false。</returns>
        public static bool Expire(this IRedisClient client, string key, long expiration
            , RedisExpireTimeUnit timeUnit = RedisExpireTimeUnit.EX)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            if(expiration < 0) throw new ArgumentOutOfRangeException("expiration");
            if(timeUnit == RedisExpireTimeUnit.EX)
                return client.Execute(new RedisBoolean("EXPIRE", key, expiration));
            else
                return client.Execute(new RedisBoolean("PEXPIRE", key, expiration));
        }

        /// <summary>
        /// 为给定 <paramref name="key"/> 设置生存时间，当 <paramref name="key"/> 过期时(生存时间为 0 )，它会被自动删除。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">键名。</param>
        /// <param name="expirationDate">到期时间。</param>
        /// <param name="timeUnit">定位生存时间的单位。</param>
        /// <returns>如果设置生存时间成功返回 true，否则返回 false。</returns>
        public static bool ExpireAt(this IRedisClient client, string key, DateTime expirationDate
            , RedisExpireTimeUnit timeUnit = RedisExpireTimeUnit.EX)
        {
            return ExpireAt(client, key, RedisDate.ToTimestamp(expirationDate).GetExpiration(timeUnit), timeUnit);
        }
        /// <summary>
        /// 为给定 <paramref name="key"/> 设置生存时间，当 <paramref name="key"/> 过期时(生存时间为 0 )，它会被自动删除。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">键名。</param>
        /// <param name="timestamp">UNIX 时间戳(unix timestamp)。</param>
        /// <param name="timeUnit">定位生存时间的单位。</param>
        /// <returns>如果设置生存时间成功返回 true，否则返回 false。</returns>
        public static bool ExpireAt(this IRedisClient client, string key, long timestamp
            , RedisExpireTimeUnit timeUnit = RedisExpireTimeUnit.EX)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            if(timestamp < 0) throw new ArgumentOutOfRangeException("timestamp");
            if(timeUnit == RedisExpireTimeUnit.EX)
                return client.Execute(new RedisBoolean("EXPIREAT", key, timestamp));
            else
                return client.Execute(new RedisBoolean("PEXPIREAT", key, timestamp));
        }

        /// <summary>
        /// 返回给定 <paramref name="key"/> 的剩余生存时间。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">键名。</param>
        /// <param name="timeUnit">定位生存时间的单位。</param>
        /// <returns>当 key 不存在时，返回 -2 。当 <paramref name="key"/> 存在但没有设置剩余生存时间时，返回 -1 。否则，以秒为单位，返回 <paramref name="key"/> 的剩余生存时间。</returns>
        public static long Ttl(this IRedisClient client, string key
            , RedisExpireTimeUnit timeUnit = RedisExpireTimeUnit.EX)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

            if(timeUnit == RedisExpireTimeUnit.EX)
                return client.Execute(new RedisInteger("TTL", key));
            else
                return client.Execute(new RedisInteger("PTTL", key));
        }

        /// <summary>
        /// 查找所有符合给定模式 <paramref name="pattern"/> 的键 。
        /// <para>KEYS 的速度非常快，但在一个大的数据库中使用它仍然可能造成性能问题，如果你需要从一个数据集中查找特定的键，你最好还是用 Redis 的集合结构(set)来代替。</para>
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="pattern">模式表达式。
        /// <para>1、KEYS * 匹配数据库中所有 key 。</para>
        /// <para>2、KEYS h?llo 匹配 hello ，hallo 和 hxllo 等。</para>
        /// <para>3、KEYS h*llo 匹配 hllo 和 heeeeello 等。</para>
        /// <para>4、KEYS h[ae]llo 匹配 hello 和 hallo ，但不匹配 hillo 。</para>
        /// <para>5、特殊符号用 \ 隔开。</para>
        /// </param>
        /// <returns>返回符合给定模式的键列表。</returns>
        public static string[] Keys(this IRedisClient client, string pattern)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(pattern)) throw new ArgumentNullException("pattern");
            return client.Execute(RedisArray.Create(new RedisString("KEYS", pattern)));
        }

        /// <summary>
        /// 将当前数据库的 <paramref name="key"/> 移动到给定的数据库 <paramref name="database"/> 当中。
        /// <para>1、若源数据库不存在 <paramref name="key"/>，移动将会失败。</para>
        /// <para>2、若目标数据库已存在 <paramref name="key"/>，移动将会失败。</para>
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">键名。</param>
        /// <param name="database">目标数据库索引。</param>
        /// <returns>如果移动成功返回 true，否则返回 false。</returns>
        public static bool Move(this IRedisClient client, string key, int database)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            if(database < 0) throw new ArgumentOutOfRangeException("database");

            return client.Execute(new RedisBoolean("MOVE", key, database));
        }

        /// <summary>
        /// 移除给定 <paramref name="key"/> 的生存时间，将这个 <paramref name="key"/> 从『易失的』(带生存时间 <paramref name="key"/> )转换成『持久的』(一个不带生存时间、永不过期的 <paramref name="key"/> )。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">键名。</param>
        /// <returns>如果生存时间移除成功，那么返回 true。否则返回 false。</returns>
        public static bool Persist(this IRedisClient client, string key)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            return client.Execute(new RedisBoolean("PERSIST", key));
        }

        /// <summary>
        /// 从当前数据库中随机返回一个键 。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <returns>当数据库不为空时，返回一个键。当数据库为空时，返回 null。</returns>
        public static string RandomKey(this IRedisClient client)
        {
            if(client == null) throw new ArgumentNullException("client");
            return client.Execute(new RedisString("RANDOMKEY"));
        }

        /// <summary>
        /// 将 <paramref name="key"/> 改名为 <paramref name="newKey"/>。
        /// <para>1、当 <paramref name="key"/> 和 <paramref name="newKey"/> 相同，或者 <paramref name="key"/> 不存在时，返回一个错误。</para>
        /// <para>2、当 <paramref name="newKey"/> 已经存在时，RENAME 命令将覆盖旧值。</para>
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">键名。</param>
        /// <param name="newKey">新的键名。</param>
        /// <returns>返回一个状态的结果。</returns>
        public static Result Rename(this IRedisClient client, string key, string newKey)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            if(string.IsNullOrEmpty(newKey)) throw new ArgumentNullException("newKey");
            if(key == newKey) return "新的键名不能和旧的相同";
            return client.Execute(new RedisStatus("RENAME", key, newKey));
        }

        /// <summary>
        /// 当 <paramref name="newKey"/> 不存在时，将 <paramref name="key"/> 改名为 <paramref name="newKey"/>。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">键名。</param>
        /// <param name="newKey">新的键名。</param>
        /// <returns>返回一个状态的结果。</returns>
        public static bool RenameNx(this IRedisClient client, string key, string newKey)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            if(string.IsNullOrEmpty(newKey)) throw new ArgumentNullException("newKey");
            if(key == newKey) return false;
            return client.Execute(new RedisBoolean.AllowError("RENAMENX", key, newKey));
        }

        /// <summary>
        /// 返回 <paramref name="key"/> 所储存的值的类型。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">键名。</param>
        /// <returns>返回储存的值的类型。</returns>
        public static RedisType Type(this IRedisClient client, string key)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            return client.Execute(new RedisKeyType("TYPE", key));
        }

        /// <summary>
        /// 增量地迭代（incrementally iterate）一集元素（a collection of elements）。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="cursor">起始游标，0 表示开始一次新的迭代。</param>
        /// <param name="pattern">给定模式相匹配的元素。匹配语法可以参考 <seealso cref="Keys(IRedisClient, String)"/> 方法。</param>
        /// <param name="count">每次迭代所返回的元素数量。</param>
        /// <returns>返回一个支持迭代的枚举。</returns>
        public static IEnumerable<string> Scan(this IRedisClient client, long cursor = 0, string pattern = null, long count = 10)
        {
            return new RedisScan<string>(client, "SCAN", null, cursor, pattern, count
                , (command, args) => new RedisString(command, args));
        }
    }
}
