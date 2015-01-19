using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Redis.Commands
{
    class RedisValue : RedisCommand<BinaryValue>
    {
        public RedisValue(string command, params object[] args) : base(command, args) { }

        internal override BinaryValue Parse(RedisExecutor executor)
        {
            var type = executor.ReadType();
            if(type == RedisReplyType.Bulk)
            {
                var bytes = executor.ReadBulk(false);
                if(bytes == null || bytes.Length == 0) return null;
                return new BinaryValue(bytes);
            }

            executor.AssertType(RedisReplyType.MultiBulk, type);
            executor.AssertSize(-1);
            return null;

        }
    }
}
