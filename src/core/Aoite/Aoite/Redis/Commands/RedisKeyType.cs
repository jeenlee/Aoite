using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Redis.Commands
{
    class RedisKeyType : RedisCommand<RedisType>
    {
        private readonly static Type REDIS_TYPE = typeof(RedisType);
        public RedisKeyType(string command, params object[] args) : base(command, args) { }

        internal override RedisType Parse(RedisExecutor executor)
        {
            executor.AssertType(RedisReplyType.Status);
            var type_s = executor.ReadLine();
            return (RedisType)Enum.Parse(REDIS_TYPE, type_s, true);
        }
    }
}
