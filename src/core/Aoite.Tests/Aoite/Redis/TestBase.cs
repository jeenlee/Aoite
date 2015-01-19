using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Redis
{
    public abstract class TestBase
    {
        public IRedisClient CreateClient()
        {
            return new RedisClient(6379);
        }
        private readonly static System.Threading.CountdownEvent cde = new System.Threading.CountdownEvent(1);
        public void RealCall(Action<IRedisClient> callback)
        {
            //- 打开测试的 redis-server，请注意，每次调用都会清空数据。
#if REAL_REDIS
            cde.AddCount();
            using(var client = this.CreateClient())
            {
                client.FlushAll();
                callback(client);
            }
            cde.Signal();
#endif
        }
    }
}
