using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Aoite.Redis
{
    public class RedisTranTests : TestBase
    {
        [Fact()]
        public void CommitTests()
        {
            this.RealCall(redis =>
            {
                int x = 0;
                using(var tran = redis.BeginTransaction())
                {
                    tran.On(tran.Set("key1", "value1"), r =>
                    {
                        Assert.True(r);
                        x += 1;
                    });
                    tran.On(tran.Set("key2", "value2"), r =>
                    {
                        Assert.True(r);
                        Assert.Equal(1, x);
                        x += 2;
                    });
                    tran.Commit().ThrowIfFailded();
                }
                Assert.Equal(3, x);

                Assert.Equal("value1", (string)redis.Get("key1"));
                Assert.Equal("value2", (string)redis.Get("key2"));
            });
        }


        [Fact()]
        public void RollbackTests()
        {
            this.RealCall(redis =>
            {
                int x = 0;
                using(var tran = redis.BeginTransaction())
                {
                    tran.On(tran.Set("key1", "value1"), r =>
                    {
                        Assert.True(r);
                        x += 1;
                    });
                    tran.On(tran.Set("key2", "value2"), r =>
                    {
                        Assert.True(r);
                        Assert.Equal(1, x);
                        x += 2;
                    });
                }
                Assert.Equal(0, x);

                Assert.Null(redis.Get("key1"));
                Assert.Null(redis.Get("key2"));
            });
        }

        [Fact()]
        public void TranOutErrorTests()
        {
            this.RealCall(redis =>
            {
                using(var tran = redis.BeginTransaction())
                {
                    tran.On(tran.Set("key1", "value1"), r =>
                    {
                        Assert.True(r);
                    });
                    tran.IncrBy("key1");
                    Assert.Throws<RedisReplyException>(() => tran.Commit());
                }
            });
        }
    }
}
