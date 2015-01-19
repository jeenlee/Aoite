using Aoite.Redis;
using Aoite.Redis.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// 表示 Redis 的 Set 扩展方法。
    /// </summary>
    public static class RedisSetExtensions
    {
        /// <summary>
        /// 将一个或多个成员加入到集合 <paramref name="key"/> 当中，已经存在于集合的成员将被忽略。
        /// <para>假如 <paramref name="key"/> 不存在，则创建一个只包含成员作成员的集合。</para>
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">集合的键名。</param>
        /// <param name="members">成员的数组。</param>
        /// <returns>返回被添加到集合中的新成员的数量，不包括被忽略的成员。</returns>
        public static long SAdd(this IRedisClient client, string key, params BinaryValue[] members)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            if(members == null) throw new ArgumentNullException("members");
            if(members.Length == 0) return 0;

            var args = RedisArgs.ConcatFirst(key, members).ToArray();
            return client.Execute(new RedisInteger("SADD", args));
        }

        /// <summary>
        /// 返回集合 <paramref name="key"/> 的基数(集合中成员的数量)。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">集合的键名。</param>
        /// <returns>返回集合 <paramref name="key"/> 的基数。</returns>
        public static long SCard(this IRedisClient client, string key)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            return client.Execute(new RedisInteger("SCARD", key));
        }

        /// <summary>
        /// 返回一个集合的全部成员，该集合是所有给定集合之间的差集。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="keys">集合的键名数组。</param>
        /// <returns>返回一个包含差集成员列表。</returns>
        public static BinaryValue[] SDiff(this IRedisClient client, params string[] keys)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(keys == null) throw new ArgumentNullException("keys");
            if(keys.Length == 0) return new BinaryValue[0];

            return client.Execute(RedisArray.Create(new RedisValue("SDIFF", keys)));
        }

        /// <summary>
        /// 将所有给定集合之间的差集保存到 <paramref name="destination"/> 集合。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="destination">目标集合。</param>
        /// <param name="keys">集合的键名数组。</param>
        /// <returns>返回结果集中的成员数量。</returns>
        public static long SDiffStore(this IRedisClient client, string destination, params string[] keys)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(destination)) throw new ArgumentNullException("destination");
            if(keys == null) throw new ArgumentNullException("keys");
            if(keys.Length == 0) return 0;

            var args = RedisArgs.ConcatFirst(destination, keys).ToArray();
            return client.Execute(new RedisInteger("SDIFFSTORE", args));
        }

        /// <summary>
        /// 返回一个集合的全部成员，该集合是所有给定集合之间的交集。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="keys">集合的键名数组。</param>
        /// <returns>返回一个包含交集成员列表。</returns>
        public static BinaryValue[] SInter(this IRedisClient client, params string[] keys)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(keys == null) throw new ArgumentNullException("keys");
            if(keys.Length == 0) return new BinaryValue[0];

            return client.Execute(RedisArray.Create(new RedisValue("SINTER", keys)));
        }

        /// <summary>
        /// 将所有给定集合之间的交集保存到 <paramref name="destination"/> 集合。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="destination">目标集合。</param>
        /// <param name="keys">集合的键名数组。</param>
        /// <returns>返回结果集中的成员数量。</returns>
        public static long SInterStore(this IRedisClient client, string destination, params string[] keys)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(destination)) throw new ArgumentNullException("destination");
            if(keys == null) throw new ArgumentNullException("keys");
            if(keys.Length == 0) return 0;

            var args = RedisArgs.ConcatFirst(destination, keys).ToArray();
            return client.Execute(new RedisInteger("SINTERSTORE", args));
        }

        /// <summary>
        /// 判断 <paramref name="member"/> 是否集合 <paramref name="key"/> 的成员。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">集合的键名。</param>
        /// <param name="member">集合的成员。</param>
        /// <returns>存在返回 true，否则返回 false。</returns>
        public static bool SIsMember(this IRedisClient client, string key, BinaryValue member)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

            return client.Execute(new RedisBoolean("SISMEMBER", key, member));
        }

        /// <summary>
        /// 返回一个集合的全部成员。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">集合的键名。</param>
        /// <returns>返回一个集合的所有成员。</returns>
        public static BinaryValue[] SMembers(this IRedisClient client, string key)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

            return client.Execute(RedisArray.Create(new RedisValue("SMEMBERS", key)));
        }

        /// <summary>
        /// 将 <paramref name="member"/> 从 <paramref name="source"/> 集合移动到 <paramref name="destination"/> 集合。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="source">源集合。</param>
        /// <param name="destination">目标集合。</param>
        /// <param name="member">移动的成员。</param>
        /// <returns>返回一个值，表示移动是否成功。</returns>
        public static bool SMove(this IRedisClient client, string source, string destination, BinaryValue member)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(source)) throw new ArgumentNullException("source");
            if(string.IsNullOrEmpty(destination)) throw new ArgumentNullException("destination");

            return client.Execute(new RedisBoolean("SMOVE", source, destination, member));
        }

        /// <summary>
        /// 移除并返回集合中的一个随机成员。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">集合的键名。</param>
        /// <returns>返回被移除的随机成员。当 <paramref name="key"/> 不存在或 <paramref name="key"/> 是空集时，返回 null。</returns>
        public static BinaryValue SPop(this IRedisClient client, string key)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

            return client.Execute(new RedisValue("SPOP", key));
        }

        /// <summary>
        /// 返回集合中的一个随机成员。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">集合的键名。</param>
        /// <returns>返回一个的随机成员。当 <paramref name="key"/> 不存在或 <paramref name="key"/> 是空集时，返回 null。</returns>
        public static BinaryValue SRandMember(this IRedisClient client, string key)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

            return client.Execute(new RedisValue("SRANDMEMBER", key));
        }

        /// <summary>
        /// 返回集合中的一组随机成员。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">集合的键名。</param>
        /// <param name="count">数组的长度。
        /// <para>1、如果 <paramref name="count"/> 为正数，且小于集合基数，那么命令返回一个包含 <paramref name="count"/> 个元素的数组，数组中的元素各不相同。如果 <paramref name="count"/> 大于等于集合基数，那么返回整个集合。</para>
        /// <para>2、如果 <paramref name="count"/> 为负数，那么命令返回一个数组，数组中的元素可能会重复出现多次，而数组的长度为 <paramref name="count"/> 的绝对值。</para>
        /// </param>
        /// <returns>返回一组的随机成员。当 <paramref name="key"/> 不存在或 <paramref name="key"/> 是空集时，返回空数组。</returns>
        public static BinaryValue[] SRandMember(this IRedisClient client, string key, long count)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

            return client.Execute(RedisArray.Create(new RedisValue("SRANDMEMBER", key, count)));
        }

        /// <summary>
        /// 移除集合 <paramref name="key"/> 中的一个或多成员，不存在的成员会被忽略。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">集合的键名。</param>
        /// <param name="members">成员的数组。</param>
        /// <returns>返回被成功移除的成员的数量，不包括被忽略的成员。</returns>
        public static long SRem(this IRedisClient client, string key, params BinaryValue[] members)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            if(members == null) throw new ArgumentNullException("members");
            if(members.Length == 0) return 0L;

            var args = RedisArgs.ConcatFirst(key, members).ToArray();
            return client.Execute(new RedisInteger("SREM", args));
        }

        /// <summary>
        /// 返回一个集合的全部成员，该集合是所有给定集合之间的并集。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="keys">集合的键名数组。</param>
        /// <returns>返回一个包含并集成员列表。</returns>
        public static BinaryValue[] SUnion(this IRedisClient client, params string[] keys)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(keys == null) throw new ArgumentNullException("keys");
            if(keys.Length == 0) return new BinaryValue[0];

            return client.Execute(RedisArray.Create(new RedisValue("SUNION", keys)));
        }

        /// <summary>
        /// 将所有给定集合之间的并集保存到 <paramref name="destination"/> 集合。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="destination">目标集合。</param>
        /// <param name="keys">集合的键名数组。</param>
        /// <returns>返回结果集中的成员数量。</returns>
        public static long SUnionStore(this IRedisClient client, string destination, params string[] keys)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(destination)) throw new ArgumentNullException("destination");
            if(keys == null) throw new ArgumentNullException("keys");
            if(keys.Length == 0) return 0;

            var args = RedisArgs.ConcatFirst(destination, keys).ToArray();
            return client.Execute(new RedisInteger("SUNIONSTORE", args));
        }

        /// <summary>
        /// 集合 <paramref name="key"/> 增量地迭代（incrementally iterate）一集元素（a collection of elements）。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">键名</param>
        /// <param name="cursor">起始游标，0 表示开始一次新的迭代。</param>
        /// <param name="pattern">给定模式相匹配的元素。匹配语法可以参考 <seealso cref="System.RedisKeyExtensions.Keys(IRedisClient, String)"/> 方法。</param>
        /// <param name="count">每次迭代所返回的元素数量。</param>
        /// <returns>返回一个支持迭代的枚举。</returns>
        public static IEnumerable<BinaryValue> SScan(this IRedisClient client, string key, long cursor = 0, string pattern = null, long count = 10)
        {
            return new RedisScan<BinaryValue>(client, "SSCAN", key, cursor, pattern, count
                , (command, args) => new RedisValue(command, args));
        }

    }
}
