using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoite.Redis
{
    public class RedisStringTests
    {
        [Fact()]
        public void AppendTest()
        {
            using(var mock = new MockConnector("localhost", 9999, ":10\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.Equal(10, redis.Append("key", "x"));
                Assert.Equal("*3\r\n$6\r\nAPPEND\r\n$3\r\nkey\r\n$1\r\nx\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void DecrTest()
        {
            using(var mock = new MockConnector("localhost", 9999, ":10\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.Equal(10, redis.DecrBy("key"));
                Assert.Equal("*2\r\n$4\r\nDECR\r\n$3\r\nkey\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void DecrByTest()
        {
            using(var mock = new MockConnector("localhost", 9999, ":10\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.Equal(10, redis.DecrBy("key", 5));
                Assert.Equal("*3\r\n$6\r\nDECRBY\r\n$3\r\nkey\r\n$1\r\n5\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void GetTest()
        {
            using(var mock = new MockConnector("localhost", 9999, "$5\r\nhello\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.Equal("hello", (string)redis.Get("key"));
                Assert.Equal("*2\r\n$3\r\nGET\r\n$3\r\nkey\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void GetRangeTest()
        {
            using(var mock = new MockConnector("localhost", 9999, "$5\r\nhello\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.Equal("hello", (string)redis.GetRange("key", 0, 10));
                Assert.Equal("*4\r\n$8\r\nGETRANGE\r\n$3\r\nkey\r\n$1\r\n0\r\n$2\r\n10\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void GetSetTest()
        {
            using(var mock = new MockConnector("localhost", 9999, "$5\r\nhello\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.Equal("hello", (string)redis.GetSet("key", "new"));
                Assert.Equal("*3\r\n$6\r\nGETSET\r\n$3\r\nkey\r\n$3\r\nnew\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void IncrTest()
        {
            using(var mock = new MockConnector("localhost", 9999, ":5\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.Equal(5, redis.IncrBy("key"));
                Assert.Equal("*2\r\n$4\r\nINCR\r\n$3\r\nkey\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void IncrByTest()
        {
            using(var mock = new MockConnector("localhost", 9999, ":5\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.Equal(5, redis.IncrBy("key", 2));
                Assert.Equal("*3\r\n$6\r\nINCRBY\r\n$3\r\nkey\r\n$1\r\n2\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void IncrByFloatTest()
        {
            using(var mock = new MockConnector("localhost", 9999, "$4\r\n4.14\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.Equal(4.14, redis.IncrByFloat("key", 3.14));
                Assert.Equal("*3\r\n$11\r\nINCRBYFLOAT\r\n$3\r\nkey\r\n$4\r\n3.14\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void MGetTest()
        {
            using(var mock = new MockConnector("localhost", 9999, "*2\r\n$4\r\nval1\r\n$4\r\nval2\r\n"))
            using(var redis = new RedisClient(mock))
            {
                var response = redis.MGet("key1", "key2");
                Assert.Equal(2, response.Length);
                Assert.Equal("val1", (string)response[0]);
                Assert.Equal("val2", (string)response[1]);
                Assert.Equal("*3\r\n$4\r\nMGET\r\n$4\r\nkey1\r\n$4\r\nkey2\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void MSetTest()
        {
            using(var mock = new MockConnector("localhost", 9999, "+OK\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.True(redis.MSet(new RedisDictionary() { { "key1", "val1" }, { "key2", "val2" } }));
                Assert.Equal("*5\r\n$4\r\nMSET\r\n$4\r\nkey1\r\n$4\r\nval1\r\n$4\r\nkey2\r\n$4\r\nval2\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void MSetNxTest()
        {
            using(var mock = new MockConnector("localhost", 9999, ":1\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.True(redis.MSetNx(new RedisDictionary() { { "key1", "val1" }, { "key2", "val2" } }));
                Assert.Equal("*5\r\n$6\r\nMSETNX\r\n$4\r\nkey1\r\n$4\r\nval1\r\n$4\r\nkey2\r\n$4\r\nval2\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void SetTest()
        {
            using(var mock = new MockConnector("localhost", 9999, "+OK\r\n", "+OK\r\n", "+OK\r\n", "+OK\r\n", "$-1\r\n", "$-1\r\n", "$-1\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.True(redis.Set("key", "value"));
                Assert.Equal("*3\r\n$3\r\nSET\r\n$3\r\nkey\r\n$5\r\nvalue\r\n", mock.GetMessage());

                Assert.True(redis.Set("key", "value", 1));
                Assert.Equal("*5\r\n$3\r\nSET\r\n$3\r\nkey\r\n$5\r\nvalue\r\n$2\r\nEX\r\n$1\r\n1\r\n", mock.GetMessage());

                Assert.True(redis.Set("key", "value", 1, RedisExpireTimeUnit.EX, behavior: RedisKeyBehavior.NX));
                Assert.Equal("*6\r\n$3\r\nSET\r\n$3\r\nkey\r\n$5\r\nvalue\r\n$2\r\nEX\r\n$1\r\n1\r\n$2\r\nNX\r\n", mock.GetMessage());

                Assert.True(redis.Set("key", "value", 1, RedisExpireTimeUnit.EX, behavior: RedisKeyBehavior.XX));
                Assert.Equal("*6\r\n$3\r\nSET\r\n$3\r\nkey\r\n$5\r\nvalue\r\n$2\r\nEX\r\n$1\r\n1\r\n$2\r\nXX\r\n", mock.GetMessage());

                Assert.False(redis.Set("key", "value", 1, RedisExpireTimeUnit.PX));
                Assert.Equal("*5\r\n$3\r\nSET\r\n$3\r\nkey\r\n$5\r\nvalue\r\n$2\r\nPX\r\n$1\r\n1\r\n", mock.GetMessage());

                Assert.False(redis.Set("key", "value", 1, RedisExpireTimeUnit.PX, behavior: RedisKeyBehavior.NX));
                Assert.Equal("*6\r\n$3\r\nSET\r\n$3\r\nkey\r\n$5\r\nvalue\r\n$2\r\nPX\r\n$1\r\n1\r\n$2\r\nNX\r\n", mock.GetMessage());

                Assert.False(redis.Set("key", "value", 1, RedisExpireTimeUnit.PX, behavior: RedisKeyBehavior.XX));
                Assert.Equal("*6\r\n$3\r\nSET\r\n$3\r\nkey\r\n$5\r\nvalue\r\n$2\r\nPX\r\n$1\r\n1\r\n$2\r\nXX\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void SetRangeTest()
        {
            using(var mock = new MockConnector("localhost", 9999, ":10\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.Equal(10, redis.SetRange("key", 4, "value"));
                Assert.Equal("*4\r\n$8\r\nSETRANGE\r\n$3\r\nkey\r\n$1\r\n4\r\n$5\r\nvalue\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void StrLenTest()
        {
            using(var mock = new MockConnector("localhost", 9999, ":10\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.Equal(10, redis.StrLen("key"));
                Assert.Equal("*2\r\n$6\r\nSTRLEN\r\n$3\r\nkey\r\n", mock.GetMessage());
            }
        }
    }
}
