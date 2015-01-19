using Aoite.Redis;
using Aoite.Redis.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// 表示 Redis 的 HyperLogLog 扩展方法。
    /// </summary>
    public static class RedisHyperLogLogExtensions
    {
        /// <summary>
        /// 将任意数量的元素添加到指定的 HyperLogLog 里面。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="key">HyperLogLog 的键名。</param>
        /// <param name="elements">元素的数组。</param>
        /// <returns>返回一个值，如果 HyperLogLog 的内部储存被修改了，那么返回 true， 否则返回 false。</returns>
        public static bool PFAdd(this IRedisClient client, string key, params BinaryValue[] elements)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            if(elements == null) throw new ArgumentNullException("elements");
            if(elements.Length == 0) return false;

            var args = RedisArgs.ConcatFirst(key, elements).ToArray();
            return client.Execute(new RedisBoolean("PFADD", args));
        }
        /// <summary>
        /// 返回储存在给定键（或给定键数组的并集）的 HyperLogLog 的近似基数。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="keys">HyperLogLog 的键名数组。</param>
        /// <returns>给定单个键时，返回储存在给定键的 HyperLogLog 的近似基数。给定多个键时，返回所有给定 HyperLogLog 的并集的近似基数，这个近似基数是通过将所有给定 HyperLogLog 合并至一个临时 HyperLogLog 来计算得出的。</returns>
        public static long PFCount(this IRedisClient client, params string[] keys)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(keys == null) throw new ArgumentNullException("keys");
            if(keys.Length == 0) return 0;

            return client.Execute(new RedisInteger("PFCOUNT", keys));
        }
        /// <summary>
        /// 将多个 HyperLogLog 合并（merge）为一个 HyperLogLog， 合并后的 HyperLogLog 的基数接近于所有输入 HyperLogLog 的可见集合（observed set）的并集。
        /// </summary>
        /// <param name="client">Redis 客户端。</param>
        /// <param name="destKey">目标键名。合并得出的 HyperLogLog 会被储存在 <paramref name="destKey"/> 键里面，如果该键并不存在，那么命令在执行之前，会先为该键创建一个空的 HyperLogLog。</param>
        /// <param name="sourceKeys">源键名列表。</param>
        /// <returns>返回一个结果。</returns>
        public static Result PFMerge(this IRedisClient client, string destKey, params string[] sourceKeys)
        {
            if(client == null) throw new ArgumentNullException("client");
            if(string.IsNullOrEmpty(destKey)) throw new ArgumentNullException("destKey");
            if(sourceKeys == null) throw new ArgumentNullException("elements");
            if(sourceKeys.Length == 0) return Result.SuccessedString;

            var args = RedisArgs.ConcatFirst(destKey, sourceKeys).ToArray();
            return client.Execute(new RedisStatus("PFMERGE", args));
        }
    }
}
