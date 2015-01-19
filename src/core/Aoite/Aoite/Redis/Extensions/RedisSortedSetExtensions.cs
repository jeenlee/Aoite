using Aoite.Redis;
using Aoite.Redis.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// 表示 Redis 的 SortedSet 扩展方法。
    /// </summary>
    public static class RedisSortedSetExtensions
    {
        /// <summary>
        /// 将一个或多个成员及其权重值加入到有序集 <paramref name="key"/> 当中。
        /// <para>如果某个成员已经是有序集的成员，那么更新这个成员的权重值，并通过重新插入这个成员，来保证该成员在正确的位置上。</para>
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">有序集的键名。</param>
        /// <param name="items">成员权重项的数组。</param>
        /// <returns>返回被成功添加的新成员的数量，不包括那些被更新的、已经存在的成员。</returns>
        public static long ZAdd(this IRedisClient client, string key, params RedisScoreItem[] items)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            if(items == null) throw new ArgumentNullException("items");
            if(items.Length == 0) return 0;

            var args = RedisArgs.Parse(key, items).ToArray();
            return client.Execute(new RedisInteger("ZADD", args));
        }

        /// <summary>
        /// 返回有序集 <paramref name="key"/> 的基数(集合中成员的数量)。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">有序集的键名。</param>
        /// <returns>返回集合 <paramref name="key"/> 的基数。</returns>
        public static long ZCard(this IRedisClient client, string key)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            return client.Execute(new RedisInteger("ZCARD", key));
        }

        /// <summary>
        /// 返回有序集 <paramref name="key"/> 中，权重值在 <paramref name="min"/> 和 <paramref name="max"/> 之间的成员的数量。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">有序集的键名。</param>
        /// <param name="min">权重最小值。<paramref name="min"/> 可以是 <see cref="System.Double.MinValue"/> -或- <see cref="System.Double.NegativeInfinity"/>，表示有序集的最小值。</param>
        /// <param name="max">权重最大值。<paramref name="max"/> 可以是 <see cref="System.Double.MaxValue"/> -或- <see cref="System.Double.PositiveInfinity"/>，表示有序集的最高值。</param>
        /// <param name="exclusiveMin">指示最小值是否为开区间（true 时表示不含最小值）。</param>
        /// <param name="exclusiveMax">指示最大值是否为开区间（true 时表示不含最大值）。</param>
        /// <returns>返回权重值包含指定区间的成员数量。</returns>
        public static long ZCount(this IRedisClient client, string key, double min, double max, bool exclusiveMin = false, bool exclusiveMax = false)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

            return client.Execute(new RedisInteger("ZCOUNT", key
                , RedisArgs.GetScore(min, exclusiveMin)
                , RedisArgs.GetScore(max, exclusiveMax)));
        }

        /// <summary>
        /// 为有序集 <paramref name="key"/> 的成员 <paramref name="member"/> 的权重值加上增量 <paramref name="increment"/>。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">有序集的键名。</param>
        /// <param name="increment">递增量。可以通过传递一个负数值 <paramref name="increment"/> ，让权重减去相应的值。</param>
        /// <param name="member">有序集的成员。</param>
        /// <returns>返回递增 <paramref name="increment"/> 之后 <paramref name="key"/> 的 <paramref name="member"/> 的权重值。</returns>
        public static double ZIncrBy(this IRedisClient client, string key, double increment, BinaryValue member)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            if(member == null) throw new ArgumentNullException("member");

            return client.Execute(new RedisFloat("ZINCRBY", key, increment, member));
        }

        /// <summary>
        /// 返回有序集 <paramref name="key"/> 中，指定区间内的成员。
        /// <para>1、其中成员的位置按权重值递减(从小到大)来排列。</para>
        /// <para>2、具有相同权重值的成员按字典序(lexicographical order )来排列。</para>
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">有序集的键名。</param>
        /// <param name="start">开始索引（含）。负数偏移量表示从值最后开始计数，-1 表示最后一个，-2 表示倒数第二个，以此类推。</param>
        /// <param name="stop">结束索引（含）。负数偏移量表示从值最后开始计数，-1 表示最后一个，-2 表示倒数第二个，以此类推。</param>
        /// <returns>返回指定区间内的有序集成员的列表。</returns>
        public static BinaryValue[] ZRange(this IRedisClient client, string key, long start, long stop)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

            return client.Execute(RedisArray.Create(new RedisValue("ZRANGE", key, start, stop)));
        }

        /// <summary>
        /// 返回有序集 <paramref name="key"/> 中，指定区间内的成员（含成员的权重值）。
        /// <para>1、其中成员的位置按权重值递减(从小到大)来排列。</para>
        /// <para>2、具有相同权重值的成员按字典序(lexicographical order )来排列。</para>
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">有序集的键名。</param>
        /// <param name="start">开始索引（含）。负数偏移量表示从值最后开始计数，-1 表示最后一个，-2 表示倒数第二个，以此类推。</param>
        /// <param name="stop">结束索引（含）。负数偏移量表示从值最后开始计数，-1 表示最后一个，-2 表示倒数第二个，以此类推。</param>
        /// <returns>返回指定区间内的有序集成员（含成员的权重值）的列表。</returns>
        public static RedisScoreItem[] ZRangeWithScores(this IRedisClient client, string key, long start, long stop)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

            return client.Execute(RedisArray.Create(new RedisItem<RedisScoreItem>(false, "ZRANGE", key, start, stop, "WITHSCORES"), 2));
        }

        /// <summary>
        /// 返回有序集 <paramref name="key"/> 中，权重值在 <paramref name="min"/> 和 <paramref name="max"/> 之间的成员。
        /// <para>1、其中成员的位置按权重值递减(从小到大)来排列。</para>
        /// <para>2、具有相同权重值的成员按字典序(lexicographical order )来排列。</para>
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">有序集的键名。</param>
        /// <param name="min">权重最小值。<paramref name="min"/> 可以是 <see cref="System.Double.MinValue"/> -或- <see cref="System.Double.NegativeInfinity"/>，表示有序集的最小值。</param>
        /// <param name="max">权重最大值。<paramref name="max"/> 可以是 <see cref="System.Double.MaxValue"/> -或- <see cref="System.Double.PositiveInfinity"/>，表示有序集的最高值。</param>
        /// <param name="exclusiveMin">指示最小值是否为开区间（true 时表示不含最小值）。</param>
        /// <param name="exclusiveMax">指示最大值是否为开区间（true 时表示不含最大值）。</param>
        /// <param name="offset">返回结果的偏移量。</param>
        /// <param name="count">返回结果的数量。</param>
        /// <returns>返回权重值包含指定区间的成员。</returns>
        public static BinaryValue[] ZRangeByScore(this IRedisClient client, string key, double min, double max
            , bool exclusiveMin = false, bool exclusiveMax = false
            , long? offset = null, long? count = null)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            IEnumerable<object> args = new object[] { key, RedisArgs.GetScore(min, exclusiveMin), RedisArgs.GetScore(max, exclusiveMax) };
            if(offset.HasValue && count.HasValue)
            {
                args = RedisArgs.ConcatAll(args, new[] { "LIMIT", offset.Value.ToString(), count.Value.ToString() });
            }
            return client.Execute(RedisArray.Create(new RedisValue("ZRANGEBYSCORE", args.ToArray())));
        }

        /// <summary>
        /// 返回有序集 <paramref name="key"/> 中，权重值在 <paramref name="min"/> 和 <paramref name="max"/> 之间的成员（含成员的权重值）。
        /// <para>1、其中成员的位置按权重值递减(从小到大)来排列。</para>
        /// <para>2、具有相同权重值的成员按字典序(lexicographical order )来排列。</para>
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">有序集的键名。</param>
        /// <param name="min">权重最小值。<paramref name="min"/> 可以是 <see cref="System.Double.MinValue"/> -或- <see cref="System.Double.NegativeInfinity"/>，表示有序集的最小值。</param>
        /// <param name="max">权重最大值。<paramref name="max"/> 可以是 <see cref="System.Double.MaxValue"/> -或- <see cref="System.Double.PositiveInfinity"/>，表示有序集的最高值。</param>
        /// <param name="exclusiveMin">指示最小值是否为开区间（true 时表示不含最小值）。</param>
        /// <param name="exclusiveMax">指示最大值是否为开区间（true 时表示不含最大值）。</param>
        /// <param name="offset">返回结果的偏移量。</param>
        /// <param name="count">返回结果的数量。</param>
        /// <returns>返回权重值包含指定区间的成员（含成员的权重值）。</returns>
        public static RedisScoreItem[] ZRangeByScoreWithScores(this IRedisClient client, string key, double min, double max
            , bool exclusiveMin = false, bool exclusiveMax = false
            , long? offset = null, long? count = null)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

            IEnumerable<object> args = new object[] { key, RedisArgs.GetScore(min, exclusiveMin), RedisArgs.GetScore(max, exclusiveMax), "WITHSCORES" };
            if(offset.HasValue && count.HasValue)
            {
                args = RedisArgs.ConcatAll(args, new[] { "LIMIT", offset.Value.ToString(), count.Value.ToString() });
            }
            return client.Execute(RedisArray.Create(new RedisItem<RedisScoreItem>(false, "ZRANGEBYSCORE", args.ToArray()), 2));
        }

        /// <summary>
        /// 返回有序集 <paramref name="key"/> 中成员 <paramref name="member"/> 的排名。其中有序集成员按权重值递增(从小到大)顺序排列。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">有序集的键名。</param>
        /// <param name="member">有序集的成员。</param>
        /// <returns>如果 <paramref name="member"/> 是有序集 <paramref name="key"/> 的成员，返回 <paramref name="member"/> 的排名。否则返回 null 值。</returns>
        public static long? ZRank(this IRedisClient client, string key, BinaryValue member)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            if(member == null) throw new ArgumentNullException("member");

            return client.Execute(new RedisInteger.Nullable("ZRANK", key, member));
        }

        /// <summary>
        /// 移除有序集 <paramref name="key"/> 中的一个或多成员，不存在的成员会被忽略。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">集合的键名。</param>
        /// <param name="members">成员的数组。</param>
        /// <returns>返回被成功移除的成员的数量，不包括被忽略的成员。</returns>
        public static long ZRem(this IRedisClient client, string key, params BinaryValue[] members)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            if(members == null) throw new ArgumentNullException("members");
            if(members.Length == 0) return 0L;

            var args = RedisArgs.ConcatFirst(key, members).ToArray();
            return client.Execute(new RedisInteger("ZREM", args));
        }

        /// <summary>
        /// 移除有序集 <paramref name="key"/> 中，指定排名区间内的所有成员。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">有序集的键名。</param>
        /// <param name="start">开始索引（含）。负数偏移量表示从值最后开始计数，-1 表示最后一个，-2 表示倒数第二个，以此类推。</param>
        /// <param name="stop">结束索引（含）。负数偏移量表示从值最后开始计数，-1 表示最后一个，-2 表示倒数第二个，以此类推。</param>
        /// <returns>被移除成员的数量。</returns>
        public static long ZRemRangeByRank(this IRedisClient client, string key, long start, long stop)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

            return client.Execute(new RedisInteger("ZREMRANGEBYRANK", key, start, stop));
        }

        /// <summary>
        /// 移除有序集 <paramref name="key"/> 中，权重值在 <paramref name="min"/> 和 <paramref name="max"/> 之间的成员。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">有序集的键名。</param>
        /// <param name="min">权重最小值。<paramref name="min"/> 可以是 <see cref="System.Double.MinValue"/> -或- <see cref="System.Double.NegativeInfinity"/>，表示有序集的最小值。</param>
        /// <param name="max">权重最大值。<paramref name="max"/> 可以是 <see cref="System.Double.MaxValue"/> -或- <see cref="System.Double.PositiveInfinity"/>，表示有序集的最高值。</param>
        /// <param name="exclusiveMin">指示最小值是否为开区间（true 时表示不含最小值）。</param>
        /// <param name="exclusiveMax">指示最大值是否为开区间（true 时表示不含最大值）。</param>
        /// <returns>被移除成员的数量。</returns>
        public static long ZRemRangeByScore(this IRedisClient client, string key, double min, double max, bool exclusiveMin = false, bool exclusiveMax = false)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

            return client.Execute(new RedisInteger("ZREMRANGEBYSCORE", key
                , RedisArgs.GetScore(min, exclusiveMin)
                , RedisArgs.GetScore(max, exclusiveMax)));
        }

        /// <summary>
        /// 返回有序集 <paramref name="key"/> 中，指定区间内的成员。
        /// <para>1、其中成员的位置按权重值递减(从大到小)来排列。</para>
        /// <para>2、具有相同权重值的成员按字典序的逆序(reverse lexicographical order)排列。</para>
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">有序集的键名。</param>
        /// <param name="start">开始索引（含）。负数偏移量表示从值最后开始计数，-1 表示最后一个，-2 表示倒数第二个，以此类推。</param>
        /// <param name="stop">结束索引（含）。负数偏移量表示从值最后开始计数，-1 表示最后一个，-2 表示倒数第二个，以此类推。</param>
        /// <returns>返回指定区间内的有序集成员的列表。</returns>
        public static BinaryValue[] ZRevRange(this IRedisClient client, string key, long start, long stop)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

            return client.Execute(RedisArray.Create(new RedisValue("ZREVRANGE", key, start, stop)));
        }

        /// <summary>
        /// 返回有序集 <paramref name="key"/> 中，指定区间内的成员（含成员的权重值）。
        /// <para>1、其中成员的位置按权重值递减(从大到小)来排列。</para>
        /// <para>2、具有相同权重值的成员按字典序的逆序(reverse lexicographical order)排列。</para>
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">有序集的键名。</param>
        /// <param name="start">开始索引（含）。负数偏移量表示从值最后开始计数，-1 表示最后一个，-2 表示倒数第二个，以此类推。</param>
        /// <param name="stop">结束索引（含）。负数偏移量表示从值最后开始计数，-1 表示最后一个，-2 表示倒数第二个，以此类推。</param>
        /// <returns>返回指定区间内的有序集成员（含成员的权重值）的列表。</returns>
        public static RedisScoreItem[] ZRevRangeWithScores(this IRedisClient client, string key, long start, long stop)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

            return client.Execute(RedisArray.Create(new RedisItem<RedisScoreItem>(false, "ZREVRANGE", key, start, stop, "WITHSCORES"), 2));
        }

        /// <summary>
        /// 返回有序集 <paramref name="key"/> 中，权重值在 <paramref name="min"/> 和 <paramref name="max"/> 之间的成员。
        /// <para>1、其中成员的位置按权重值递减(从大到小)来排列。</para>
        /// <para>2、具有相同权重值的成员按字典序的逆序(reverse lexicographical order)排列。</para>
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">有序集的键名。</param>
        /// <param name="min">权重最小值。<paramref name="min"/> 可以是 <see cref="System.Double.MinValue"/> -或- <see cref="System.Double.NegativeInfinity"/>，表示有序集的最小值。</param>
        /// <param name="max">权重最大值。<paramref name="max"/> 可以是 <see cref="System.Double.MaxValue"/> -或- <see cref="System.Double.PositiveInfinity"/>，表示有序集的最高值。</param>
        /// <param name="exclusiveMin">指示最小值是否为开区间（true 时表示不含最小值）。</param>
        /// <param name="exclusiveMax">指示最大值是否为开区间（true 时表示不含最大值）。</param>
        /// <param name="offset">返回结果的偏移量。</param>
        /// <param name="count">返回结果的数量。</param>
        /// <returns>返回权重值包含指定区间的成员。</returns>
        public static BinaryValue[] ZRevRangeByScore(this IRedisClient client, string key, double min, double max
            , bool exclusiveMin = false, bool exclusiveMax = false
            , long? offset = null, long? count = null)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            IEnumerable<object> args = new object[] { key, RedisArgs.GetScore(min, exclusiveMin), RedisArgs.GetScore(max, exclusiveMax) };
            if(offset.HasValue && count.HasValue)
            {
                args = RedisArgs.ConcatAll(args, new[] { "LIMIT", offset.Value.ToString(), count.Value.ToString() });
            }
            return client.Execute(RedisArray.Create(new RedisValue("ZREVRANGEBYSCORE", args.ToArray())));
        }

        /// <summary>
        /// 返回有序集 <paramref name="key"/> 中，权重值在 <paramref name="min"/> 和 <paramref name="max"/> 之间的成员（含成员的权重值）。
        /// <para>1、其中成员的位置按权重值递减(从大到小)来排列。</para>
        /// <para>2、具有相同权重值的成员按字典序的逆序(reverse lexicographical order)排列。</para>
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">有序集的键名。</param>
        /// <param name="min">权重最小值。<paramref name="min"/> 可以是 <see cref="System.Double.MinValue"/> -或- <see cref="System.Double.NegativeInfinity"/>，表示有序集的最小值。</param>
        /// <param name="max">权重最大值。<paramref name="max"/> 可以是 <see cref="System.Double.MaxValue"/> -或- <see cref="System.Double.PositiveInfinity"/>，表示有序集的最高值。</param>
        /// <param name="exclusiveMin">指示最小值是否为开区间（true 时表示不含最小值）。</param>
        /// <param name="exclusiveMax">指示最大值是否为开区间（true 时表示不含最大值）。</param>
        /// <param name="offset">返回结果的偏移量。</param>
        /// <param name="count">返回结果的数量。</param>
        /// <returns>返回权重值包含指定区间的成员（含成员的权重值）。</returns>
        public static RedisScoreItem[] ZRevRangeByScoreWithScores(this IRedisClient client, string key, double min, double max
            , bool exclusiveMin = false, bool exclusiveMax = false
            , long? offset = null, long? count = null)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

            IEnumerable<object> args = new object[] { key, RedisArgs.GetScore(min, exclusiveMin), RedisArgs.GetScore(max, exclusiveMax), "WITHSCORES" };
            if(offset.HasValue && count.HasValue)
            {
                args = RedisArgs.ConcatAll(args, new[] { "LIMIT", offset.Value.ToString(), count.Value.ToString() });
            }
            return client.Execute(RedisArray.Create(new RedisItem<RedisScoreItem>(false, "ZREVRANGEBYSCORE", args.ToArray()), 2));
        }

        /// <summary>
        /// 返回有序集 <paramref name="key"/> 中成员 <paramref name="member"/> 的排名。其中有序集成员按权重值递减(从大到小)顺序排列。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">有序集的键名。</param>
        /// <param name="member">有序集的成员。</param>
        /// <returns>如果 <paramref name="member"/> 是有序集 <paramref name="key"/> 的成员，返回 <paramref name="member"/> 的排名。否则返回 null 值。</returns>
        public static long? ZRevRank(this IRedisClient client, string key, BinaryValue member)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            if(member == null) throw new ArgumentNullException("member");

            return client.Execute(new RedisInteger.Nullable("ZREVRANK", key, member));
        }

        /// <summary>
        /// 返回有序集 <paramref name="key"/> 中，成员 <paramref name="member"/> 的权重值。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">有序集的键名。</param>
        /// <param name="member">有序集的成员。</param>
        /// <returns>如果 <paramref name="member"/> 是有序集 <paramref name="key"/> 的成员，返回 <paramref name="member"/> 的权重值。否则返回 null 值。</returns>
        public static double? ZScore(this IRedisClient client, string key, BinaryValue member)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            if(member == null) throw new ArgumentNullException("member");

            return client.Execute(new RedisFloat.Nullable("ZSCORE", key, member));
        }

        private static long ZStore(string command, IRedisClient client, string destination, RedisWeightDictionary keyWeights, RedisAggregate? aggregate)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(destination)) throw new ArgumentNullException("destination");
            if(keyWeights == null) throw new ArgumentNullException("keyWeights");
            if(keyWeights.Count == 0) return 0L;

            IEnumerable<object> args = new object[] { destination, keyWeights.Count };
            args = RedisArgs.ConcatAll(args, keyWeights.Keys);
            if(keyWeights.Values.Where(weight => weight != 1.0).Count() > 0)
            {
                args = RedisArgs.ConcatAll(RedisArgs.ConcatLast(args, "WEIGHTS"), keyWeights.Values.Cast<object>());
            }
            if(aggregate.HasValue)
            {
                args = RedisArgs.ConcatLasts(args, "AGGREGATE", aggregate.Value.ToString().ToUpperInvariant());
            }
            return client.Execute(new RedisInteger(command, args.ToArray()));
        }

        /// <summary>
        /// 计算给定的一个或多个有序集的权限值交集，并将该并集(结果集)储存到 <paramref name="destination"/> 。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="destination">目标有序集的键名。如果有序集已存在，则会覆盖。</param>
        /// <param name="keyWeights">有序集键名和乘法因子的字典。</param>
        /// <param name="aggregate">聚合的方式。</param>
        /// <returns>保存到 <paramref name="destination"/> 的结果集的基数。</returns>
        public static long ZInterStore(this IRedisClient client, string destination, RedisWeightDictionary keyWeights, RedisAggregate? aggregate = null)
        {
            return ZStore("ZINTERSTORE", client, destination, keyWeights, aggregate);
        }

        /// <summary>
        /// 计算给定的一个或多个有序集的权限值并集，并将该并集(结果集)储存到 <paramref name="destination"/> 。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="destination">目标有序集的键名。如果有序集已存在，则会覆盖。</param>
        /// <param name="keyWeights">有序集键名和乘法因子的字典。</param>
        /// <param name="aggregate">聚合的方式。</param>
        /// <returns>保存到 <paramref name="destination"/> 的结果集的基数。</returns>
        public static long ZUnionStore(this IRedisClient client, string destination, RedisWeightDictionary keyWeights, RedisAggregate? aggregate = null)
        {
            return ZStore("ZUNIONSTORE", client, destination, keyWeights, aggregate);
        }

        /// <summary>
        /// 有序集 <paramref name="key"/> 增量地迭代（incrementally iterate）一集元素（a collection of elements）。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">有序集的键名</param>
        /// <param name="cursor">起始游标，0 表示开始一次新的迭代。</param>
        /// <param name="pattern">给定模式相匹配的元素。匹配语法可以参考 <seealso cref="System.RedisKeyExtensions.Keys(IRedisClient, String)"/> 方法。</param>
        /// <param name="count">每次迭代所返回的元素数量。</param>
        /// <returns>返回一个支持迭代的枚举。</returns>
        public static IEnumerable<RedisScoreItem> ZScan(this IRedisClient client, string key, long cursor = 0, string pattern = null, long count = 10)
        {
            return new RedisScan<RedisScoreItem>(client, "ZSCAN", key, cursor, pattern, count
                , (command, args) => new RedisItem<RedisScoreItem>(false, command, args),2);
        }

        /// <summary>
        /// 返回给定的有序集合键 <paramref name="key"/> 中，值介于 <paramref name="min"/> 和 <paramref name="max"/> 之间从低到高的顺序的成员。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">有序集的键名</param>
        /// <param name="min">最小成员值。可以为 null 值，表示负无限。</param>
        /// <param name="max">最大成员值。可以为 null 值，表示正无限。</param>
        /// <param name="exclusiveMin">指示最小是否为开区间（true 时表示不含最小值）。</param>
        /// <param name="exclusiveMax">指示最大值是否为开区间（true 时表示不含最大值）。</param>
        /// <param name="offset">返回结果的偏移量。</param>
        /// <param name="count">返回结果的数量。</param>
        /// <returns>返回一个从低到高的顺序列表，列表里面包含了有序集合在指定范围内的成员。</returns>
        public static BinaryValue[] ZRangeByLex(this IRedisClient client, string key
            , BinaryValue min, BinaryValue max
            , bool exclusiveMin = false, bool exclusiveMax = false
            , long? offset = null, long? count = null)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

            IEnumerable<object> args = new object[] { key
                , RedisArgs.GetBinaryValue(min, exclusiveMin, "-")
                , RedisArgs.GetBinaryValue(max, exclusiveMax, "+") };
            if(offset.HasValue && count.HasValue)
            {
                args = RedisArgs.ConcatAll(args, new[] { "LIMIT", offset.Value.ToString(), count.Value.ToString() });
            }

            return client.Execute(RedisArray.Create(new RedisValue("ZRANGEBYLEX", args.ToArray())));
        }

        /// <summary>
        /// 返回给定的有序集合键 <paramref name="key"/> 中，值介于 <paramref name="min"/> 和 <paramref name="max"/> 之间的成员数量。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">有序集的键名</param>
        /// <param name="min">最小成员值。可以为 null 值，表示负无限。</param>
        /// <param name="max">最大成员值。可以为 null 值，表示正无限。</param>
        /// <param name="exclusiveMin">指示最小是否为开区间（true 时表示不含最小值）。</param>
        /// <param name="exclusiveMax">指示最大值是否为开区间（true 时表示不含最大值）。</param>
        /// <returns>返回一个包含了有序集合在指定范围内的成员数量。</returns>
        public static long ZLexCount(this IRedisClient client, string key
            , BinaryValue min, BinaryValue max
            , bool exclusiveMin = false, bool exclusiveMax = false)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

            return client.Execute(new RedisInteger("ZLEXCOUNT", key
                , RedisArgs.GetBinaryValue(min, exclusiveMin, "-")
                , RedisArgs.GetBinaryValue(max, exclusiveMax, "+")));
        }

        /// <summary>
        /// 移除给定的有序集合键 <paramref name="key"/> 中，值介于 <paramref name="min"/> 和 <paramref name="max"/> 之间的成员。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">有序集的键名</param>
        /// <param name="min">最小成员值。可以为 null 值，表示负无限。</param>
        /// <param name="max">最大成员值。可以为 null 值，表示正无限。</param>
        /// <param name="exclusiveMin">指示最小是否为开区间（true 时表示不含最小值）。</param>
        /// <param name="exclusiveMax">指示最大值是否为开区间（true 时表示不含最大值）。</param>
        /// <returns>返回被移除的成员数量。</returns>
        public static long ZRemRangeByLex(this IRedisClient client, string key
            , BinaryValue min, BinaryValue max
            , bool exclusiveMin = false, bool exclusiveMax = false)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

            return client.Execute(new RedisInteger("ZREMRANGEBYLEX", key
                , RedisArgs.GetBinaryValue(min, exclusiveMin, "-")
                , RedisArgs.GetBinaryValue(max, exclusiveMax, "+")));
        }
    }
}
