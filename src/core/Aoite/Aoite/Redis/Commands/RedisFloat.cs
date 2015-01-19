using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Aoite.Redis.Commands
{
    class RedisFloat : RedisCommand<Double>
    {
        public RedisFloat(string command, params object[] args) : base(command, args) { }

        internal static Double FromString(string input)
        {
            return Double.Parse(input, NumberStyles.Float, CultureInfo.InvariantCulture);
        }

        internal override Double Parse(RedisExecutor executor)
        {
            return FromString(executor.ReadBulkString());
        }

        public class Nullable : RedisCommand<Double?>
        {
            public Nullable(string command, params object[] args) : base(command, args) { }

            internal override Double? Parse(RedisExecutor executor)
            {
                var result = executor.ReadBulkString();
                if(result == null)
                    return null;
                return RedisFloat.FromString(result);
            }
        }
    }
}
