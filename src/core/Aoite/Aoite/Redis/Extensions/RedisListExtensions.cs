using Aoite.Redis;
using Aoite.Redis.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{

    /// <summary>
    /// 表示 Redis 的 List 扩展方法。
    /// </summary>
    public static class RedisListExtensions
    {
        /// <summary>
        /// 阻塞式(blocking)移除并返回列表键的头元素。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="timeout">接受一个以秒为单位的数字作为值。超时参数设为 0 表示阻塞时间可以无限期延长(block indefinitely) 。</param>
        /// <param name="keys">按参数键的先后顺序依次检查各个列表，弹出第一个非空列表的头元素。</param>
        /// <returns>在指定时间内，如果列表为空，返回一个 null。否则，返回一个含有键值的元素项。</returns>
        public static RedisKeyItem BLPop(this IRedisClient client, long timeout, params string[] keys)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(timeout < 0) throw new ArgumentOutOfRangeException("timeout");
            if(keys == null) throw new ArgumentNullException("keys");
            if(keys.Length == 0) return null;

            var args = RedisArgs.ConcatLast(keys, timeout).ToArray();
            return client.Execute(new RedisItem<RedisKeyItem>(true, "BLPOP", args));
        }

        /// <summary>
        /// 阻塞式(blocking)移除并返回列表键的头元素。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="timeout">接受一个以秒为单位的数字作为值。超时参数设为 0 表示阻塞时间可以无限期延长(block indefinitely) 。</param>
        /// <param name="keys">按参数键的先后顺序依次检查各个列表，弹出第一个非空列表的头元素。</param>
        /// <returns>在指定时间内，如果列表为空，返回一个 null。否则，返回一个含有键值的元素项。</returns>
        public static RedisKeyItem BLPop(this IRedisClient client, TimeSpan timeout, params string[] keys)
        {
            return BLPop(client, (long)timeout.TotalSeconds, keys);
        }

        /// <summary>
        /// 阻塞式(blocking)移除并返回列表键的尾元素。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="timeout">接受一个以秒为单位的数字作为值。超时参数设为 0 表示阻塞时间可以无限期延长(block indefinitely) 。</param>
        /// <param name="keys">按参数键的先后顺序依次检查各个列表，弹出第一个非空列表的尾元素。</param>
        /// <returns>在指定时间内，如果列表为空，返回一个 null。否则，返回一个含有键值的元素项。</returns>
        public static RedisKeyItem BRPop(this IRedisClient client, long timeout, params string[] keys)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(timeout < 0) throw new ArgumentOutOfRangeException("timeout");
            if(keys == null) throw new ArgumentNullException("keys");
            if(keys.Length == 0) return null;

            var args = RedisArgs.ConcatLast(keys, timeout).ToArray();
            return client.Execute(new RedisItem<RedisKeyItem>(true, "BRPOP", args));
        }

        /// <summary>
        /// 阻塞式(blocking)移除并返回列表键的尾元素。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="timeout">接受一个以秒为单位的数字作为值。超时参数设为 0 表示阻塞时间可以无限期延长(block indefinitely) 。</param>
        /// <param name="keys">按参数键的先后顺序依次检查各个列表，弹出第一个非空列表的尾元素。</param>
        /// <returns>在指定时间内，如果列表为空，返回一个 null。否则，返回一个含有键值的元素项。</returns>
        public static RedisKeyItem BRPop(this IRedisClient client, TimeSpan timeout, params string[] keys)
        {
            return BRPop(client, (long)timeout.TotalSeconds, keys);
        }

        /// <summary>
        /// 阻塞式(blocking)将列表 <paramref name="source"/> 中的尾元素弹出，并返回给客户端。并将 <paramref name="source"/> 弹出的元素插入到列表 <paramref name="destination"/> ，作为 <paramref name="destination"/> 列表的的头元素。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="source">源列表。</param>
        /// <param name="destination">目标列表。</param>
        /// <param name="timeout">接受一个以秒为单位的数字作为值。超时参数设为 0 表示阻塞时间可以无限期延长(block indefinitely) 。</param>
        /// <returns>在指定时间内，如果列表为空，返回一个 null。否则，返回元素项的值。</returns>
        public static BinaryValue BRPopLPush(this IRedisClient client, string source, string destination, long timeout)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(source)) throw new ArgumentNullException("source");
            if(string.IsNullOrEmpty(destination)) throw new ArgumentNullException("destination");
            if(timeout < 0) throw new ArgumentOutOfRangeException("timeout");

            return client.Execute(new RedisValue("BRPOPLPUSH", source, destination, timeout));
        }

        /// <summary>
        /// 阻塞式(blocking)将列表 <paramref name="source"/> 中的尾元素弹出，并返回给客户端。并将 <paramref name="source"/> 弹出的元素插入到列表 <paramref name="destination"/> ，作为 <paramref name="destination"/> 列表的的头元素。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="source">源列表。</param>
        /// <param name="destination">目标列表。</param>
        /// <param name="timeout">接受一个以秒为单位的数字作为值。超时参数设为 0 表示阻塞时间可以无限期延长(block indefinitely) 。</param>
        /// <returns>在指定时间内，如果列表为空，返回一个 null。否则，返回元素项的值。</returns>
        public static BinaryValue BRPopLPush(this IRedisClient client, string source, string destination, TimeSpan timeout)
        {
            return BRPopLPush(client, source, destination, (long)timeout.TotalSeconds);
        }

        /// <summary>
        /// 返回列表 <paramref name="key"/> 中，下标为 <paramref name="index"/> 的元素。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">列表的键名。</param>
        /// <param name="index">下标(index)参数 start 和 stop 都以 0 为底，也就是说，以 0 表示列表的第一个元素，以 1 表示列表的第二个元素，以此类推。
        /// <para>也可以使用负数下标，以 -1 表示列表的最后一个元素，-2 表示列表的倒数第二个元素，以此类推。</para>
        /// </param>
        /// <returns>返回列表中下标为 <paramref name="index"/> 的元素。如果 <paramref name="index"/> 参数的值不在列表的区间范围内(out of range)，返回 null。</returns>
        public static BinaryValue LIndex(this IRedisClient client, string key, long index)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

            return client.Execute(new RedisValue("LINDEX", key, index));
        }

        /// <summary>
        /// 将值 <paramref name="value"/> 插入到列表 <paramref name="key"/> 当中，位于值 <paramref name="pivot"/> 之前或之后。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">列表的键名。</param>
        /// <param name="position">插入的位置。</param>
        /// <param name="pivot">定位的支点键名。</param>
        /// <param name="value">插入的值。</param>
        /// <returns>返回插入的结果。</returns>
        public static long LInsert(this IRedisClient client, string key, RedisInsertPosition position, string pivot, BinaryValue value)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            if(string.IsNullOrEmpty(pivot)) throw new ArgumentNullException("pivot");
            return client.Execute(new RedisInteger("LINSERT", key
                , position == RedisInsertPosition.Before ? "BEFORE" : "AFTER"
                , pivot, value));
        }

        /// <summary>
        /// 返回列表 <paramref name="key"/> 的长度。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">列表的键名。</param>
        /// <returns>返回列表 <paramref name="key"/> 的长度。</returns>
        public static long LLen(this IRedisClient client, string key)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            return client.Execute(new RedisInteger("LLEN", key));
        }

        /// <summary>
        /// 移除并返回列表 <paramref name="key"/> 的头元素。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">列表的键名。</param>
        /// <returns>返回列表的头元素。当 <paramref name="key"/> 不存在时，返回 null。</returns>
        public static BinaryValue LPop(this IRedisClient client, string key)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

            return client.Execute(new RedisValue("LPOP", key));
        }

        /// <summary>
        /// 将一个或多个值按从左到右的顺序依次插入到列表 <paramref name="key"/> 的表头。
        /// <para>如果 <paramref name="key"/> 不存在，一个空列表会被创建并执行 LPUSH 操作。</para>
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">列表的键名。</param>
        /// <param name="values">值元素的数组。当数组长度为 0 时将会抛出异常。</param>
        /// <returns>返回执行命令列表的总长度。</returns>
        public static long LPush(this IRedisClient client, string key, params BinaryValue[] values)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            if(values == null || values.Length == 0) throw new ArgumentNullException("values");

            var args = RedisArgs.ConcatFirst(key, values).ToArray();
            return client.Execute(new RedisInteger("LPUSH", args));
        }

        /// <summary>
        /// 将一个或多个值按从左到右的顺序依次插入到列表 <paramref name="key"/> 的表头。
        /// <para>当 <paramref name="key"/> 不存在时，LPUSHX 命令什么也不做。</para>
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">列表的键名。</param>
        /// <param name="values">值元素的数组。当数组长度为 0 时将会抛出异常。</param>
        /// <returns>返回执行命令列表的总长度。</returns>
        public static long LPushX(this IRedisClient client, string key, params BinaryValue[] values)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            if(values == null || values.Length == 0) throw new ArgumentNullException("values");

            var args = RedisArgs.ConcatFirst(key, values).ToArray();
            return client.Execute(new RedisInteger("LPUSHX", args));
        }

        /// <summary>
        /// 返回列表 <paramref name="key"/> 中指定区间内的元素，区间以偏移量 <paramref name="start"/> 和 <paramref name="stop"/> 指定。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">列表的键名。</param>
        /// <param name="start">开始索引（含）。负数偏移量表示从值最后开始计数，-1 表示最后一个，-2 表示倒数第二个，以此类推。</param>
        /// <param name="stop">结束索引（含）。负数偏移量表示从值最后开始计数，-1 表示最后一个，-2 表示倒数第二个，以此类推。</param>
        /// <returns>返回包含指定区间内的元素。</returns>
        public static BinaryValue[] LRange(this IRedisClient client, string key, long start, long stop)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

            return client.Execute(RedisArray.Create(new RedisValue("LRANGE", key, start, stop)));
        }

        /// <summary>
        /// 根据参数 <paramref name="count"/> 的值，移除列表中与参数 <paramref name="value"/> 相等的元素。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">列表的键名。</param>
        /// <param name="count">移除的数量。
        /// <para>1、<paramref name="count"/> &gt; 0 : 从表头开始向表尾搜索，移除与 <paramref name="value"/> 相等的元素，数量为 <paramref name="count"/> 。</para>
        /// <para>2、<paramref name="count"/> &lt; 0 : 从表尾开始向表头搜索，移除与 <paramref name="value"/> 相等的元素，数量为 <paramref name="count"/> 的绝对值。</para>
        /// <para>3、<paramref name="count"/> = 0 : 移除表中所有与 <paramref name="value"/> 相等的值。</para>
        /// </param>
        /// <param name="value">匹配的元素值。</param>
        /// <returns>返回被移除元素的数量。</returns>
        public static long LRem(this IRedisClient client, string key, long count, BinaryValue value)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            return client.Execute(new RedisInteger("LREM", key, count, value));
        }

        /// <summary>
        /// 将列表 <paramref name="key"/> 下标为 <paramref name="index"/> 的元素的值设置为 <paramref name="value"/> 。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">列表的键名。</param>
        /// <param name="index">设置的索引。
        /// <para>当 <paramref name="index"/> 参数超出范围，或对一个空列表( <paramref name="key"/> 不存在)进行 LSET 时，返回一个错误</para>
        /// </param>
        /// <param name="value">设置的元素值。</param>
        /// <returns>返回一个结果。</returns>
        public static Result LSet(this IRedisClient client, string key, long index, BinaryValue value)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

            return client.Execute(new RedisStatus("LSET", key, index, value));
        }

        /// <summary>
        /// 对一个列表进行修剪(trim)，让列表只保留指定区间内的元素，不在指定区间之内的元素都将被删除。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">列表的键名。</param>
        /// <param name="start">开始索引（含）。负数偏移量表示从值最后开始计数，-1 表示最后一个，-2 表示倒数第二个，以此类推。</param>
        /// <param name="stop">结束索引（含）。负数偏移量表示从值最后开始计数，-1 表示最后一个，-2 表示倒数第二个，以此类推。</param>
        /// <returns>返回一个结果。</returns>
        public static Result LTrim(this IRedisClient client, string key, long start, long stop)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

            return client.Execute(new RedisStatus("LTRIM", key, start, stop));
        }

        /// <summary>
        /// 移除并返回列表 <paramref name="key"/> 的尾元素。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">列表的键名。</param>
        /// <returns>返回列表的尾元素。当 <paramref name="key"/> 不存在时，返回 null。</returns>
        public static BinaryValue RPop(this IRedisClient client, string key)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");

            return client.Execute(new RedisValue("RPOP", key));
        }

        /// <summary>
        /// 将列表 <paramref name="source"/> 中的尾元素弹出，并返回给客户端。并将 <paramref name="source"/> 弹出的元素插入到列表 <paramref name="destination"/> ，作为 <paramref name="destination"/> 列表的的头元素。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="source">源列表。</param>
        /// <param name="destination">目标列表。</param>
        /// <returns>如果列表为空，返回一个 null。否则，返回元素项的值。</returns>
        public static BinaryValue RPopLPush(this IRedisClient client, string source, string destination)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(source)) throw new ArgumentNullException("source");
            if(string.IsNullOrEmpty(destination)) throw new ArgumentNullException("destination");

            return client.Execute(new RedisValue("RPOPLPUSH", source, destination));
        }

        /// <summary>
        /// 将一个或多个值按从左到右的顺序依次插入到列表 <paramref name="key"/> 的表尾。
        /// <para>如果 <paramref name="key"/> 不存在，一个空列表会被创建并执行 RPush 操作。</para>
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">列表的键名。</param>
        /// <param name="values">值元素的数组。当数组长度为 0 时将会抛出异常。</param>
        /// <returns>返回执行命令列表的总长度。</returns>
        public static long RPush(this IRedisClient client, string key, params BinaryValue[] values)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            if(values == null || values.Length == 0) throw new ArgumentNullException("values");

            var args = RedisArgs.ConcatFirst(key, values).ToArray();
            return client.Execute(new RedisInteger("RPUSH", args));
        }

        /// <summary>
        /// 将一个或多个值按从左到右的顺序依次插入到列表 <paramref name="key"/> 的表尾。
        /// <para>当 <paramref name="key"/> 不存在时，RPUSHX 命令什么也不做。</para>
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">列表的键名。</param>
        /// <param name="values">值元素的数组。当数组长度为 0 时将会抛出异常。</param>
        /// <returns>返回执行命令列表的总长度。</returns>
        public static long RPushX(this IRedisClient client, string key, params BinaryValue[] values)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            if(values == null || values.Length == 0) throw new ArgumentNullException("values");

            var args = RedisArgs.ConcatFirst(key, values).ToArray();
            return client.Execute(new RedisInteger("RPUSHX", args));
        }
    }
}
