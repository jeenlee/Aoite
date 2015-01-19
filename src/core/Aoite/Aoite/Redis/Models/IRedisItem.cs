using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Redis
{
    interface IRedisItem
    {
        void Parse(RedisExecutor executor);
    }

}
