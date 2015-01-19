using Aoite.Redis;
using Aoite.Redis.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{

    /// <summary>
    /// 表示 Redis 的 String 扩展方法。
    /// </summary>
    public static class RedisStringExtensions
    {
        /// <summary>
        /// 将 <paramref name="value"/> 追加到 <paramref name="key"/> 原来的值的末尾。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">键名。</param>
        /// <param name="value">键值。</param>
        /// <returns>返回追加 <paramref name="value"/> 之后，<paramref name="key"/> 中的总字节长度。</returns>
        public static long Append(this IRedisClient client, string key, BinaryValue value)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            if(value == null) throw new ArgumentNullException("value");

            return client.Execute(new RedisInteger("APPEND", key, value));
        }

        /// <summary>
        /// 将 <paramref name="key"/> 中储存的数字值递减指定的 <paramref name="decrement"/> 数值。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">键名。</param>
        /// <param name="decrement">递减量。</param>
        /// <returns>返回递减 <paramref name="decrement"/> 之后 <paramref name="key"/> 的值。</returns>
        public static long DecrBy(this IRedisClient client, string key, long decrement = 1)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            if(decrement != 1)
            {
                return client.Execute(new RedisInteger("DECRBY", key, decrement));
            }
            else
            {
                return client.Execute(new RedisInteger("DECR", key));
            }
        }

        /// <summary>
        /// 将 <paramref name="key"/> 中储存的数字值递减指定的 <paramref name="decrement"/> 浮点数数值。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">键名。</param>
        /// <param name="decrement">浮点数递减量。</param>
        /// <returns>返回递减浮点数 <paramref name="decrement"/> 之后 <paramref name="key"/> 的值。</returns>
        public static double DecrByFloat(this IRedisClient client, string key, double decrement = 1.0)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            return client.Execute(new RedisFloat("INCRBYFLOAT", key, -decrement));
        }

        /// <summary>
        /// 返回 <paramref name="key"/> 所关联的值。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">键名。</param>
        /// <returns>当 <paramref name="key"/> 不存在时，返回 null ，否则返回 <paramref name="key"/> 的值。</returns>
        public static BinaryValue Get(this IRedisClient client, string key)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            return client.Execute(new RedisValue("GET", key));
        }

        /// <summary>
        /// 返回 <paramref name="key"/> 中值的子值，值的截取范围由 <paramref name="start"/> 和 <paramref name="end"/> 两个偏移量决定(包括 <paramref name="start"/> 和 <paramref name="end"/> 在内)。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">键名。</param>
        /// <param name="start">开始索引（含）。负数偏移量表示从值最后开始计数，-1 表示最后一个，-2 表示倒数第二个，以此类推。</param>
        /// <param name="end">结束索引（含）。负数偏移量表示从值最后开始计数，-1 表示最后一个，-2 表示倒数第二个，以此类推。</param>
        /// <returns>当 <paramref name="key"/> 不存在时，返回 null ，否则返回 <paramref name="key"/> 的值。</returns>
        public static BinaryValue GetRange(this IRedisClient client, string key, long start, long end)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

            return client.Execute(new RedisValue("GETRANGE", key, start, end));
        }

        /// <summary>
        /// 将给定 <paramref name="key"/> 的值设为 <paramref name="value"/> ，并返回 key 的旧值。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">键名。</param>
        /// <param name="value">键值。</param>
        /// <returns>当 <paramref name="key"/> 不存在时，返回 null ，否则返回 <paramref name="key"/> 的旧值。</returns>
        public static BinaryValue GetSet(this IRedisClient client, string key, BinaryValue value)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            return client.Execute(new RedisValue("GETSET", key, value));
        }

        /// <summary>
        /// 将 <paramref name="key"/> 中储存的数字值递增指定的 <paramref name="increment"/> 数值。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">键名。</param>
        /// <param name="increment">递增量。</param>
        /// <returns>返回递增 <paramref name="increment"/> 之后 <paramref name="key"/> 的值。</returns>
        public static long IncrBy(this IRedisClient client, string key, long increment = 1)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            if(increment != 1)
            {
                return client.Execute(new RedisInteger("INCRBY", key, increment));
            }
            else
            {
                return client.Execute(new RedisInteger("INCR", key));
            }
        }

        /// <summary>
        /// 将 <paramref name="key"/> 中储存的数字值递增指定的 <paramref name="increment"/> 浮点数数值。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">键名。</param>
        /// <param name="increment">浮点数递增量。</param>
        /// <returns>返回递增浮点数 <paramref name="increment"/> 之后 <paramref name="key"/> 的值。</returns>
        public static double IncrByFloat(this IRedisClient client, string key, double increment = 1.0)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            return client.Execute(new RedisFloat("INCRBYFLOAT", key, increment));
        }

        /// <summary>
        /// 返回一个或多个键的值。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="keys">键的数组。</param>
        /// <returns>返回值的数组。如果给定的键里面，有某个键不存在，那么这个键对应的值为 null 值 。</returns>
        public static BinaryValue[] MGet(this IRedisClient client, params string[] keys)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(keys == null) throw new ArgumentNullException("keys");
            if(keys.Length == 0) return new BinaryValue[0];
            return client.Execute(RedisArray.Create(new RedisValue("MGET", keys)));
        }

        /// <summary>
        /// 同时设置一个或多个键值。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="keyValues">键值的字典。</param>
        /// <returns>返回一个结果。</returns>
        public static Result MSet(this IRedisClient client, RedisDictionary keyValues)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(keyValues == null) throw new ArgumentNullException("keyValues");
            if(keyValues.Count == 0) return Result.Successfully;

            var args = RedisArgs.Parse(keyValues).ToArray();
            return client.Execute(new RedisStatus("MSET", args));
        }

        /// <summary>
        /// 仅当所有给定键都不存在，同时设置一个或多个键值。即使只有一个给定键已存在，MSETNX 也会拒绝执行所有给定键的设置操作。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="keyValues">键值的字典。</param>
        /// <returns>当所有键都成功设置返回 true，否则返回 false。</returns>
        public static bool MSetNx(this IRedisClient client, RedisDictionary keyValues)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(keyValues == null) throw new ArgumentNullException("keyValues");
            if(keyValues.Count == 0) return false;

            var args = RedisArgs.Parse(keyValues).ToArray();
            return client.Execute(new RedisBoolean("MSETNX", args));
        }
        /// <summary>
        /// 设置键值。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">键名。</param>
        /// <param name="value">键值。</param>
        /// <returns>设置成功返回 true，否则返回 false。</returns>
        public static bool Set(this IRedisClient client, string key, BinaryValue value)
        {
            return Set(client, key, value, (long?)null);
        }

        /// <summary>
        /// 设置键值。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">键名。</param>
        /// <param name="value">键值。</param>
        /// <param name="expiration">生存时间。</param>
        /// <param name="timeUnit">定位生存时间的单位。</param>
        /// <param name="behavior">定位键的行为。</param>
        /// <returns>设置成功返回 true，否则返回 false。</returns>
        public static bool Set(this IRedisClient client, string key, BinaryValue value
            , TimeSpan? expiration = null
            , RedisExpireTimeUnit timeUnit = RedisExpireTimeUnit.EX
            , RedisKeyBehavior behavior = RedisKeyBehavior.None)
        {
            long? expiration_64 = null;
            if(expiration.HasValue) expiration_64 = expiration.Value.GetExpiration(timeUnit);

            return Set(client, key, value, expiration_64, timeUnit, behavior);
        }

        /// <summary>
        /// 设置键值。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">键名。</param>
        /// <param name="value">键值。</param>
        /// <param name="expiration">生存时间。</param>
        /// <param name="timeUnit">定位生存时间的单位。</param>
        /// <param name="behavior">定位键的行为。</param>
        /// <returns>设置成功返回 true，否则返回 false。</returns>
        public static bool Set(this IRedisClient client, string key, BinaryValue value
            , long? expiration = null
            , RedisExpireTimeUnit timeUnit = RedisExpireTimeUnit.EX
            , RedisKeyBehavior behavior = RedisKeyBehavior.None)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

            List<object> args = new List<object>(5);
            args.Add(key);
            args.Add(value);
            if(expiration.HasValue && expiration.Value > 0)
            {
                args.Add(timeUnit == RedisExpireTimeUnit.EX ? "EX" : "PX");
                args.Add(expiration.Value);
            }

            if(behavior != RedisKeyBehavior.None) args.Add(behavior == RedisKeyBehavior.NX ? "NX" : "XX");

            return client.Execute(new RedisStatus.MultiBulk("SET", args.ToArray()));
        }

        /// <summary>
        /// 用 <paramref name="value"/> 参数覆写(overwrite)给定 <paramref name="key"/> 所储存的值，从偏移量 <paramref name="offset"/> 开始。
        /// <para>不存在的 <paramref name="key"/> 当作空值处理。</para>
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">键名。</param>
        /// <param name="offset">偏移量。</param>
        /// <param name="value">覆盖的键值。</param>
        /// <returns>返回修改之后的字节总长度。</returns>
        public static long SetRange(this IRedisClient client, string key, long offset, BinaryValue value)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            if(offset < 0) throw new ArgumentOutOfRangeException("offset");
            return client.Execute(new RedisInteger("SETRANGE", key, offset, value));
        }

        /// <summary>
        /// 返回 <paramref name="key"/> 所储存的值的长度。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">键名。</param>
        /// <returns>返回值的字节总长度。</returns>
        public static long StrLen(this IRedisClient client, string key)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            return client.Execute(new RedisInteger("STRLEN", key));
        }

    }
}
