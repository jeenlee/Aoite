using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoite.Redis
{
    public class RedisHyperLogLogTests : TestBase
    {
        [Fact()]
        public void PfAddTest()
        {
            using(var mock = new MockConnector("localhost", 9999, ":1\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.True(redis.PFAdd("test", "test1", "test2"));
                Assert.Equal("*4\r\n$5\r\nPFADD\r\n$4\r\ntest\r\n$5\r\ntest1\r\n$5\r\ntest2\r\n", mock.GetMessage());
            }
            this.RealCall(redis =>
            {
                Assert.True(redis.PFAdd("key1", "value1", "value2"));
            });
        }

        [Fact()]
        public void PfCountTest()
        {
            using(var mock = new MockConnector("localhost", 9999, ":2\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.Equal(2, redis.PFCount("test1", "test2"));
                Assert.Equal("*3\r\n$7\r\nPFCOUNT\r\n$5\r\ntest1\r\n$5\r\ntest2\r\n", mock.GetMessage());
            }
            this.RealCall(redis =>
            {
                Assert.True(redis.PFAdd("key1", "value1", "value2"));
                Assert.True(redis.PFAdd("key2", "value1", "value3"));
                Assert.Equal(3, redis.PFCount("key1", "key2"));
            });
        }

        [Fact()]
        public void PfMergeTest()
        {
            using(var mock = new MockConnector("localhost", 9999, "+OK\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.True(redis.PFMerge("destination", "source1", "source2"));
                Assert.Equal("*4\r\n$7\r\nPFMERGE\r\n$11\r\ndestination\r\n$7\r\nsource1\r\n$7\r\nsource2\r\n", mock.GetMessage());
            }
            this.RealCall(redis =>
            {
                Assert.True(redis.PFAdd("key1", "value1", "value2"));
                Assert.True(redis.PFAdd("key2", "value1", "value3"));
                Assert.True(redis.PFMerge("key3", "key1", "key2"));
                Assert.Equal(3, redis.PFCount("key3"));
            });
        }
    }
}
