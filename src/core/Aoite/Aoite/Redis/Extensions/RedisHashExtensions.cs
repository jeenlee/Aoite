using Aoite.Redis;
using Aoite.Redis.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// 表示 Redis 的 Hash 扩展方法。
    /// </summary>
    public static class RedisHashExtensions
    {
        /// <summary>
        /// 删除哈希表 <paramref name="key"/> 中的一个或多个指定域，不存在的域将被忽略。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">哈希表的键名。</param>
        /// <param name="fields">哈希表域的数组。</param>
        /// <returns>返回键成功被删除域的数量。</returns>
        public static long HDel(this IRedisClient client, string key, params string[] fields)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            if(fields == null) throw new ArgumentNullException("fields");
            if(fields.Length == 0) return 0L;

            var args = RedisArgs.ConcatFirst(key, fields).ToArray();
            return client.Execute(new RedisInteger("HDEL", args));
        }

        /// <summary>
        /// 检查给定哈希表 <paramref name="key"/> 的域是否存在。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">哈希表的键名。</param>
        /// <param name="field">键的域。</param>
        /// <returns>如果 <paramref name="key"/> 的域存在，那么返回 true。否则返回 false。</returns>
        public static bool HExists(this IRedisClient client, string key, string field)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            if(string.IsNullOrEmpty(field)) throw new ArgumentNullException("field");

            return client.Execute(new RedisBoolean("HEXISTS", key, field));
        }

        /// <summary>
        /// 返回哈希表 <paramref name="key"/> 所关联域的值。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">哈希表的键名。</param>
        /// <param name="field">键的域。</param>
        /// <returns>当 <paramref name="key"/> 的域不存在时，返回 null ，否则返回 <paramref name="key"/> 的域值。</returns>
        public static BinaryValue HGet(this IRedisClient client, string key, string field)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            if(string.IsNullOrEmpty(field)) throw new ArgumentNullException("field");

            return client.Execute(new RedisValue("HGET", key, field));
        }

        /// <summary>
        /// 返回哈希表 <paramref name="key"/> 中所有的域和值。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">哈希表的键名。</param>
        /// <returns>返回 <paramref name="key"/> 所有的域和值。</returns>
        public static RedisFieldItem[] HGetAll(this IRedisClient client, string key)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

            return client.Execute(RedisArray.Create(new RedisItem<RedisFieldItem>(false, "HGETALL", key), 2));
        }

        /// <summary>
        /// 将哈希表 <paramref name="key"/> 中储存给定的域 <paramref name="field"/> 的数字值递增指定的 <paramref name="increment"/> 数值。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">哈希表的键名。</param>
        /// <param name="field">键的域。</param>
        /// <param name="increment">递增量。</param>
        /// <returns>返回递增 <paramref name="increment"/> 之后 <paramref name="key"/> 的域 <paramref name="field"/> 的值。</returns>
        public static long HIncrBy(this IRedisClient client, string key, string field, long increment = 1)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            if(string.IsNullOrEmpty(field)) throw new ArgumentNullException("field");

            return client.Execute(new RedisInteger("HINCRBY", key, field, increment));
        }

        /// <summary>
        /// 将 <paramref name="key"/> 中储存给定的域 <paramref name="field"/> 的数字值递增指定的 <paramref name="increment"/> 浮点数数值。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">哈希表的键名。</param>
        /// <param name="field">键的域。</param>
        /// <param name="increment">浮点数递增量。</param>
        /// <returns>返回递增浮点数 <paramref name="increment"/> 之后 <paramref name="key"/> 的域 <paramref name="field"/> 的值。</returns>
        public static double HIncrByFloat(this IRedisClient client, string key, string field, double increment = 1.0)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            return client.Execute(new RedisFloat("HINCRBYFLOAT", key, field, increment));
        }

        /// <summary>
        /// 将哈希表 <paramref name="key"/> 中储存给定的域 <paramref name="field"/> 的数字值递减指定的 <paramref name="decrement"/> 数值。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">哈希表的键名。</param>
        /// <param name="field">键的域。</param>
        /// <param name="decrement">递减量。</param>
        /// <returns>返回递减 <paramref name="decrement"/> 之后 <paramref name="key"/> 的域 <paramref name="field"/> 的值。</returns>
        public static long HDecrBy(this IRedisClient client, string key, string field, long decrement = 1)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            if(string.IsNullOrEmpty(field)) throw new ArgumentNullException("field");

            return client.Execute(new RedisInteger("HINCRBY", key, field, -decrement));
        }

        /// <summary>
        /// 将 <paramref name="key"/> 中储存给定的域 <paramref name="field"/> 的数字值递减指定的 <paramref name="decrement"/> 浮点数数值。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">哈希表的键名。</param>
        /// <param name="field">键的域。</param>
        /// <param name="decrement">浮点数递减量。</param>
        /// <returns>返回递减浮点数 <paramref name="decrement"/> 之后 <paramref name="key"/> 的域 <paramref name="field"/> 的值。</returns>
        public static double HDecrByFloat(this IRedisClient client, string key, string field, double decrement = 1.0)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            return client.Execute(new RedisFloat("HINCRBYFLOAT", key, field, -decrement));
        }

        /// <summary>
        /// 查找哈希表 <paramref name="key"/> 所有的域名。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">键名</param>
        /// <returns>返回 <paramref name="key"/> 所有的域名。当 <paramref name="key"/> 不存在时，返回一个空数组。</returns>
        public static string[] HKeys(this IRedisClient client, string key)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            return client.Execute(RedisArray.Create(new RedisString("HKEYS", key)));
        }

        /// <summary>
        /// 返回哈希表 <paramref name="key"/> 中域的数量。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">键名</param>
        /// <returns>返回哈希表中域的数量。当 <paramref name="key"/> 不存在时返回 0。</returns>
        public static long HLen(this IRedisClient client, string key)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            return client.Execute(new RedisInteger("HLEN", key));
        }

        /// <summary>
        /// 返回哈希表 <paramref name="key"/> 中一个或多个域的值。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">键名</param>
        /// <param name="fields">哈希表域的数组。</param>
        /// <returns>返回值的数组。如果给定的域里面，有某个域不存在，那么这个域对应的值为 null 值。</returns>
        public static BinaryValue[] HMGet(this IRedisClient client, string key, params string[] fields)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            if(fields == null) throw new ArgumentNullException("fields");
            if(fields.Length == 0) return new BinaryValue[0];
            var args = RedisArgs.ConcatFirst(key, fields).ToArray();
            return client.Execute(RedisArray.Create(new RedisValue("HMGET", args)));
        }

        /// <summary>
        /// 同时设置哈希表 <paramref name="key"/> 中一个或多个域值。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">键名</param>
        /// <param name="fieldValues">域值的字典。</param>
        /// <returns>返回一个结果。</returns>
        public static Result HMSet(this IRedisClient client, string key, RedisDictionary fieldValues)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(fieldValues == null) throw new ArgumentNullException("fieldValues");
            if(fieldValues.Count == 0) return Result.Successfully;

            var args = RedisArgs.Parse(fieldValues, key).ToArray();
            return client.Execute(new RedisStatus("HMSET", args));
        }

        /// <summary>
        /// 同时设置哈希表 <paramref name="key"/> 中一个或多个域值。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">键名</param>
        /// <param name="fieldValues">域值的匿名对象。</param>
        /// <returns>返回一个结果。</returns>
        public static Result HMSet(this IRedisClient client, string key, params RedisFieldItem[] fieldValues)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(fieldValues == null) throw new ArgumentNullException("fieldValues");
            return HMSet(client, key, new RedisDictionary(fieldValues));
        }

        /// <summary>
        /// 同时设置哈希表 <paramref name="key"/> 中一个或多个域值。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">键名</param>
        /// <param name="fieldValues">域值的匿名对象。</param>
        /// <returns>返回一个结果。</returns>
        public static Result HMSet(this IRedisClient client, string key, object fieldValues)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(fieldValues == null) throw new ArgumentNullException("fieldValues");
            RedisDictionary redisDict;
            if(fieldValues is System.Collections.IDictionary)
            {
                redisDict = new RedisDictionary(fieldValues as System.Collections.IDictionary);
            }
            else
            {
                var typeMapper = TypeMapper.Create(fieldValues.GetType());
                redisDict = new RedisDictionary(typeMapper.Count);
                foreach(var propertyMapper in typeMapper.Properties)
                {
                    redisDict.Add(propertyMapper.Name, BinaryValue.Create(propertyMapper.GetValue(fieldValues)));
                }
            }
            return HMSet(client, key, redisDict);
        }

        /// <summary>
        /// 设置设置哈希表 <paramref name="key"/> 中一个域值。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">哈希表的键名。</param>
        /// <param name="field">键的域。</param>
        /// <param name="value">域的值。</param>
        /// <param name="nx">为 true 表示仅当域 <paramref name="field"/> 不存在时设置。</param>
        /// <returns>设置成功返回 true，否则返回 false。</returns>
        public static bool HSet(this IRedisClient client, string key, string field, BinaryValue value, bool nx = false)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            if(nx)
            {
                return client.Execute(new RedisBoolean("HSETNX", key, field, value));
            }
            else
            {
                return client.Execute(new RedisBoolean("HSET", key, field, value));
            }
        }

        /// <summary>
        /// 查找哈希表 <paramref name="key"/> 所有的域值。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">键名</param>
        /// <returns>返回 <paramref name="key"/> 所有的域值。当 <paramref name="key"/> 不存在时，返回一个空数组。</returns>
        public static BinaryValue[] HVals(this IRedisClient client, string key)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            return client.Execute(RedisArray.Create(new RedisValue("HVALS", key)));
        }

        /// <summary>
        /// 哈希表 <paramref name="key"/> 增量地迭代（incrementally iterate）一集元素（a collection of elements）。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">键名</param>
        /// <param name="cursor">起始游标，0 表示开始一次新的迭代。</param>
        /// <param name="pattern">给定模式相匹配的元素。匹配语法可以参考 <seealso cref="System.RedisKeyExtensions.Keys(IRedisClient, String)"/> 方法。</param>
        /// <param name="count">每次迭代所返回的元素数量。</param>
        /// <returns>返回一个支持迭代的枚举。</returns>
        public static IEnumerable<RedisFieldItem> HScan(this IRedisClient client, string key, long cursor = 0, string pattern = null, long count = 10)
        {
            return new RedisScan<RedisFieldItem>(client, "HSCAN", key, cursor, pattern, count
                , (command, args) => new RedisItem<RedisFieldItem>(false, command, args), 2);
        }
    }
}
