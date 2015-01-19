using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Redis.Commands
{
    class RedisInteger : RedisCommand<Int64>
    {
        public RedisInteger(string command, params object[] args) : base(command, args) { }

        internal override Int64 Parse(RedisExecutor executor)
        {
            return executor.ReadInteger();
        }
        public class Nullable : RedisCommand<Int64?>
        {
            public Nullable(string command, params object[] args) : base(command, args) { }

            internal override Int64? Parse(RedisExecutor executor)
            {
                var type = executor.ReadType();
                if(type == RedisReplyType.Integer)
                    return executor.ReadInteger(false);
                executor.ReadBulkString(false);
                return null;
            }
        }
    }
}
