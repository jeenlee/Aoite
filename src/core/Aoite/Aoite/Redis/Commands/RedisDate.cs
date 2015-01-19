using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Redis.Commands
{
    class RedisDate : RedisCommand<DateTime>
    {
        static readonly DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public RedisDate(string command, params object[] args) : base(command, args) { }

        internal override DateTime Parse(RedisExecutor executor)
        {
            return FromTimestamp(executor.ReadInteger());
        }

        public class Micro : RedisCommand<DateTime>
        {
            public Micro(string command, params object[] args) : base(command, args) { }

            internal override DateTime Parse(RedisExecutor executor)
            {
                executor.AssertType(RedisReplyType.MultiBulk);
                executor.AssertSize(2);

                int timestamp = Int32.Parse(executor.ReadBulkString());
                int microseconds = Int32.Parse(executor.ReadBulkString());

                return FromTimestamp(timestamp, microseconds);
            }

            public static DateTime FromTimestamp(long timestamp, long microseconds)
            {
                return RedisDate.FromTimestamp(timestamp) + FromMicroseconds(microseconds);
            }

            public static TimeSpan FromMicroseconds(long microseconds)
            {
                return TimeSpan.FromTicks(microseconds * (TimeSpan.TicksPerMillisecond / 1000));
            }

            public static long ToMicroseconds(TimeSpan span)
            {
                return span.Ticks / (TimeSpan.TicksPerMillisecond / 1000);
            }
        }

        public static DateTime FromTimestamp(long seconds)
        {
            return _epoch.AddSeconds(seconds).ToLocalTime();
        }

        public static TimeSpan ToTimestamp(DateTime date)
        {
            return date.ToUniversalTime() - _epoch;
        }
    }
}
