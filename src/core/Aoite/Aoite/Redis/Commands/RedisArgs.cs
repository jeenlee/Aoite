using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Redis.Commands
{
    static class RedisArgs
    {
        public static BinaryValue GetBinaryValue(BinaryValue value, bool isExclusive, string nullValue)
        {
            if(!value.HasValue()) return nullValue;
            if(isExclusive) return "(".ToBytes().Concat(value.ByteArray).ToArray();
            return "[".ToBytes().Concat(value.ByteArray).ToArray();
        }
        public static IEnumerable<object> ConcatAll(params IEnumerable<object>[] enumerables)
        {
            foreach(var enumerable in enumerables)
            {
                foreach(var item in enumerable)
                {
                    yield return item;
                }
            }
        }
        public static IEnumerable<object> ConcatLast(IEnumerable<object> args, object last)
        {
            foreach(var arg in args)
            {
                yield return arg;
            }
            yield return last;
        }
        public static IEnumerable<object> ConcatFirst(object first, IEnumerable<object> args)
        {
            yield return first;
            foreach(var item in args)
            {
                yield return item;
            }
        }
        public static IEnumerable<object> ConcatLasts(IEnumerable<object> args, params object[] lasts)
        {
            foreach(var item in args)
            {
                yield return item;
            }
            foreach(var item in lasts)
            {
                yield return item;
            }
        }
        public static long GetExpiration(this TimeSpan expiration, RedisExpireTimeUnit timeUnit)
        {
            if(timeUnit == RedisExpireTimeUnit.EX) return (long)expiration.TotalSeconds;
            return (long)expiration.TotalMilliseconds;
        }

        public static IEnumerable<object> Parse(object first, IEnumerable<RedisScoreItem> items)
        {
            yield return first;
            foreach(var item in items)
            {
                yield return item.Score;
                yield return item.Member;
            }
        }
        public static IEnumerable<object> Parse<Key, Value>(Dictionary<Key, Value> keyValues, object firstArg = null)
        {
            if(firstArg != null) yield return firstArg;

            foreach(var item in keyValues)
            {
                yield return item.Key;
                yield return item.Value;
            }
        }

        public static string GetScore(double score, bool isExclusive)
        {
            if(Double.IsNegativeInfinity(score) || score == Double.MinValue)
                return "-inf";
            else if(Double.IsPositiveInfinity(score) || score == Double.MaxValue)
                return "+inf";
            else if(isExclusive)
                return '(' + score.ToString();
            else
                return score.ToString();
        }
    }
}
