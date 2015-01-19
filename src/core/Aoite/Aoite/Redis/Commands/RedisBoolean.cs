using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Redis.Commands
{
    class RedisBoolean : RedisCommand<Boolean>
    {
        public RedisBoolean(string command, params object[] args) : base(command, args) { }

        internal override bool Parse(RedisExecutor executor)
        {
            return executor.ReadInteger() == 1;
        }

        public class AllowError : RedisBoolean
        {
            public AllowError(string command, params object[] args) : base(command, args) { }

            internal override bool Parse(RedisExecutor executor)
            {
                var type = executor.ReadType(false);
                var str = executor.ReadLine();
                if(type == RedisReplyType.Error) return false;

                return Int64.Parse(str) == 1;
            }
        }
    }
}
