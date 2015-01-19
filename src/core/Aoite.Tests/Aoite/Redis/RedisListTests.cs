using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoite.Redis
{
    public class RedisListTests
    {
        [Fact()]
        public void TestBLPopWithKey()
        {
            string reply = "*2\r\n$4\r\ntest\r\n$5\r\ntest1\r\n";
            using (var mock = new MockConnector("localhost", 9999, reply, reply))
            using (var redis = new RedisClient(mock))
            {
                var response1 = redis.BLPop(60, "test");
                Assert.Equal("test", response1.Key);
                Assert.Equal("test1", (string)response1.Value);
                Assert.Equal("*3\r\n$5\r\nBLPOP\r\n$4\r\ntest\r\n$2\r\n60\r\n", mock.GetMessage());

                var response2 = redis.BLPop(TimeSpan.FromMinutes(1), "test");
                Assert.Equal("*3\r\n$5\r\nBLPOP\r\n$4\r\ntest\r\n$2\r\n60\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void TestBLPop()
        {
            string reply = "*2\r\n$4\r\ntest\r\n$5\r\ntest1\r\n";
            using (var mock = new MockConnector("localhost", 9999, reply, reply))
            using (var redis = new RedisClient(mock))
            {
                Assert.Equal("test1", (string)redis.BLPop(60, "test").Value);
                Assert.Equal("*3\r\n$5\r\nBLPOP\r\n$4\r\ntest\r\n$2\r\n60\r\n", mock.GetMessage());

                Assert.Equal("test1", (string)redis.BLPop(TimeSpan.FromMinutes(1), "test").Value);
                Assert.Equal("*3\r\n$5\r\nBLPOP\r\n$4\r\ntest\r\n$2\r\n60\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void TestBRPopWithKey()
        {
            string reply = "*2\r\n$4\r\ntest\r\n$5\r\ntest1\r\n";
            using (var mock = new MockConnector("localhost", 9999, reply, reply))
            using (var redis = new RedisClient(mock))
            {
                var response1 = redis.BRPop(60, "test");
                Assert.Equal("test", response1.Key);
                Assert.Equal("test1", (string)response1.Value);
                Assert.Equal("*3\r\n$5\r\nBRPOP\r\n$4\r\ntest\r\n$2\r\n60\r\n", mock.GetMessage());

                var response2 = redis.BRPop(TimeSpan.FromMinutes(1), "test");
                Assert.Equal("*3\r\n$5\r\nBRPOP\r\n$4\r\ntest\r\n$2\r\n60\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void TestBRPop()
        {
            string reply = "*2\r\n$4\r\ntest\r\n$5\r\ntest1\r\n";
            using (var mock = new MockConnector("localhost", 9999, reply, reply))
            using (var redis = new RedisClient(mock))
            {
                Assert.Equal("test1", (string)redis.BRPop(60, "test").Value);
                Assert.Equal("*3\r\n$5\r\nBRPOP\r\n$4\r\ntest\r\n$2\r\n60\r\n", mock.GetMessage());

                Assert.Equal("test1", (string)redis.BRPop(TimeSpan.FromMinutes(1), "test").Value);
                Assert.Equal("*3\r\n$5\r\nBRPOP\r\n$4\r\ntest\r\n$2\r\n60\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void TestBRPopLPush()
        {
            string reply = "$5\r\ntest1\r\n";
            using (var mock = new MockConnector("localhost", 9999, reply, reply))
            using (var redis = new RedisClient(mock))
            {
                Assert.Equal("test1", (string)redis.BRPopLPush("test", "new", 60));
                Assert.Equal("*4\r\n$10\r\nBRPOPLPUSH\r\n$4\r\ntest\r\n$3\r\nnew\r\n$2\r\n60\r\n", mock.GetMessage());

                Assert.Equal("test1", (string)redis.BRPopLPush("test", "new", TimeSpan.FromMinutes(1)));
                Assert.Equal("*4\r\n$10\r\nBRPOPLPUSH\r\n$4\r\ntest\r\n$3\r\nnew\r\n$2\r\n60\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void TestLIndex()
        {
            using (var mock = new MockConnector("localhost", 9999, "$5\r\ntest1\r\n"))
            using (var redis = new RedisClient(mock))
            {
                Assert.Equal("test1", (string)redis.LIndex("test", 0));
                Assert.Equal("*3\r\n$6\r\nLINDEX\r\n$4\r\ntest\r\n$1\r\n0\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void TestLInsert()
        {
            string reply = ":2\r\n";
            using (var mock = new MockConnector("localhost", 9999, reply, reply))
            using (var redis = new RedisClient(mock))
            {
                Assert.Equal(2, redis.LInsert("test", RedisInsertPosition.Before, "field1", "test1"));
                Assert.Equal("*5\r\n$7\r\nLINSERT\r\n$4\r\ntest\r\n$6\r\nBEFORE\r\n$6\r\nfield1\r\n$5\r\ntest1\r\n", mock.GetMessage());

                Assert.Equal(2, redis.LInsert("test", RedisInsertPosition.After, "field1", "test1"));
                Assert.Equal("*5\r\n$7\r\nLINSERT\r\n$4\r\ntest\r\n$5\r\nAFTER\r\n$6\r\nfield1\r\n$5\r\ntest1\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void TestLLen()
        {
            using (var mock = new MockConnector("localhost", 9999, ":3\r\n"))
            using (var redis = new RedisClient(mock))
            {
                Assert.Equal(3, redis.LLen("test"));
                Assert.Equal("*2\r\n$4\r\nLLEN\r\n$4\r\ntest\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void TestLPop()
        {
            using (var mock = new MockConnector("localhost", 9999, "$5\r\ntest1\r\n"))
            using (var redis = new RedisClient(mock))
            {
                Assert.Equal("test1", (string)redis.LPop("test"));
                Assert.Equal("*2\r\n$4\r\nLPOP\r\n$4\r\ntest\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void TestLPush()
        {
            using (var mock = new MockConnector("localhost", 9999, ":2\r\n"))
            using (var redis = new RedisClient(mock))
            {
                Assert.Equal(2, redis.LPush("test", "test1", "test2"));
                Assert.Equal("*4\r\n$5\r\nLPUSH\r\n$4\r\ntest\r\n$5\r\ntest1\r\n$5\r\ntest2\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void TestLPushX()
        {
            using (var mock = new MockConnector("localhost", 9999, ":2\r\n"))
            using (var redis = new RedisClient(mock))
            {
                Assert.Equal(2, redis.LPushX("test", "test1"));
                Assert.Equal("*3\r\n$6\r\nLPUSHX\r\n$4\r\ntest\r\n$5\r\ntest1\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void TestLRange()
        {
            using (var mock = new MockConnector("localhost", 9999, "*2\r\n$5\r\ntest1\r\n$5\r\ntest2\r\n"))
            using (var redis = new RedisClient(mock))
            {
                var response = redis.LRange("test", -10, 10);
                Assert.Equal(2, response.Length);
                Assert.Equal("test1", (string)response[0]);
                Assert.Equal("test2", (string)response[1]);
                Assert.Equal("*4\r\n$6\r\nLRANGE\r\n$4\r\ntest\r\n$3\r\n-10\r\n$2\r\n10\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void TestLRem()
        {
            using (var mock = new MockConnector("localhost", 9999, ":2\r\n"))
            using (var redis = new RedisClient(mock))
            {
                Assert.Equal(2, redis.LRem("test", -2, "test1"));
                Assert.Equal("*4\r\n$4\r\nLREM\r\n$4\r\ntest\r\n$2\r\n-2\r\n$5\r\ntest1\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void TestLSet()
        {
            using (var mock = new MockConnector("localhost", 9999, "+OK\r\n"))
            using (var redis = new RedisClient(mock))
            {
                Assert.True(redis.LSet("test", 0, "test1"));
                Assert.Equal("*4\r\n$4\r\nLSET\r\n$4\r\ntest\r\n$1\r\n0\r\n$5\r\ntest1\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void TestLTrim()
        {
            using (var mock = new MockConnector("localhost", 9999, "+OK\r\n"))
            using (var redis = new RedisClient(mock))
            {
                Assert.True(redis.LTrim("test", 0, 3));
                Assert.Equal("*4\r\n$5\r\nLTRIM\r\n$4\r\ntest\r\n$1\r\n0\r\n$1\r\n3\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void TestRPop()
        {
            using (var mock = new MockConnector("localhost", 9999, "$5\r\ntest1\r\n"))
            using (var redis = new RedisClient(mock))
            {
                Assert.Equal("test1", (string)redis.RPop("test"));
                Assert.Equal("*2\r\n$4\r\nRPOP\r\n$4\r\ntest\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void TestRPopLPush()
        {
            using (var mock = new MockConnector("localhost", 9999, "$5\r\ntest1\r\n"))
            using (var redis = new RedisClient(mock))
            {
                Assert.Equal("test1", (string)redis.RPopLPush("test", "new"));
                Assert.Equal("*3\r\n$9\r\nRPOPLPUSH\r\n$4\r\ntest\r\n$3\r\nnew\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void TestRPush()
        {
            using (var mock = new MockConnector("localhost", 9999, ":3\r\n"))
            using (var redis = new RedisClient(mock))
            {
                Assert.Equal(3, redis.RPush("test", "test1"));
                Assert.Equal("*3\r\n$5\r\nRPUSH\r\n$4\r\ntest\r\n$5\r\ntest1\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void TestRPushX()
        {
            using (var mock = new MockConnector("localhost", 9999, ":3\r\n"))
            using (var redis = new RedisClient(mock))
            {
                Assert.Equal(3, redis.RPushX("test", "test1"));
                Assert.Equal("*3\r\n$6\r\nRPUSHX\r\n$4\r\ntest\r\n$5\r\ntest1\r\n", mock.GetMessage());
            }
        }
    }
}
