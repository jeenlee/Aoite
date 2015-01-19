using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aoite.Redis;

namespace Aoite.Tests
{
    public class RedisHashTests : TestBase
    {
        [Fact()]
        public void TestHDel()
        {
            using(var mock = new MockConnector("localhost", 9999, ":2\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.Equal(2, redis.HDel("test", "test1", "test2"));
                Assert.Equal("*4\r\n$4\r\nHDEL\r\n$4\r\ntest\r\n$5\r\ntest1\r\n$5\r\ntest2\r\n", mock.GetMessage());
            }

            this.RealCall(redis =>
            {
                redis.HSet("test", "field1", "value1");
                redis.HSet("test", "field2", "value2");
                Assert.Equal(2, redis.HDel("test", "field1", "field2"));
            });
        }

        [Fact()]
        public void TestHExists()
        {
            using(var mock = new MockConnector("localhost", 9999, ":1\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.True(redis.HExists("test", "field"));
                Assert.Equal("*3\r\n$7\r\nHEXISTS\r\n$4\r\ntest\r\n$5\r\nfield\r\n", mock.GetMessage());
            }

            this.RealCall(redis =>
            {
                redis.HSet("test", "field", "value1");
                Assert.True(redis.HExists("test", "field"));
                Assert.False(redis.HExists("not_exists_key", "field"));
                Assert.False(redis.HExists("test", "not_exists_field"));
            });
        }

        [Fact()]
        public void TestHGet()
        {
            using(var mock = new MockConnector("localhost", 9999, "$5\r\nvalue\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.Equal("value", (string)redis.HGet("test", "field"));
                Assert.Equal("*3\r\n$4\r\nHGET\r\n$4\r\ntest\r\n$5\r\nfield\r\n", mock.GetMessage());
            }

            this.RealCall(redis =>
            {
                redis.HSet("test", "field", "value");
                Assert.Equal("value", (string)redis.HGet("test", "field"));
            });
        }

        [Fact()]
        public void TestHGetAll()
        {
            using(var mock = new MockConnector("localhost", 9999, "*2\r\n$6\r\nfield1\r\n$5\r\ntest1\r\n"))
            using(var redis = new RedisClient(mock))
            {
                var response = redis.HGetAll("test");
                Assert.Equal(1, response.Length);
                Assert.Equal("field1", response[0].Field);
                Assert.Equal("test1", (string)response[0].Value);
                Assert.Equal("*2\r\n$7\r\nHGETALL\r\n$4\r\ntest\r\n", mock.GetMessage());
            }

            this.RealCall(redis =>
            {
                redis.HSet("test", "field1", "value1");
                redis.HSet("test", "field2", "value2");

                var response = redis.HGetAll("test");
                Assert.Equal(2, response.Length);
                for(int i = 0; i < 2; i++)
                {
                    Assert.Equal("field" + (i + 1), response[i].Field);
                    Assert.Equal("value" + (i + 1), (string)response[i].Value);
                }
            });
        }

        [Fact()]
        public void TestHIncrBy()
        {
            using(var mock = new MockConnector("localhost", 9999, ":5\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.Equal(5, redis.HIncrBy("test", "field", 1));
                Assert.Equal("*4\r\n$7\r\nHINCRBY\r\n$4\r\ntest\r\n$5\r\nfield\r\n$1\r\n1\r\n", mock.GetMessage());
            }

            this.RealCall(redis =>
            {
                Assert.Equal(1, redis.HIncrBy("test", "field"));
                Assert.Equal(2, redis.HIncrBy("test", "field"));
                Assert.Equal(1, redis.HDecrBy("test", "field"));
                Assert.Equal(0, redis.HDecrBy("test", "field"));
            });
        }

        [Fact()]
        public void TestHIncrByFloat()
        {
            using(var mock = new MockConnector("localhost", 9999, "$4\r\n3.14\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.Equal(3.14, redis.HIncrByFloat("test", "field", 1.14));
                Assert.Equal("*4\r\n$12\r\nHINCRBYFLOAT\r\n$4\r\ntest\r\n$5\r\nfield\r\n$4\r\n1.14\r\n", mock.GetMessage());
            }

            this.RealCall(redis =>
            {
                Assert.Equal(.35, redis.HIncrByFloat("test", "field", .35));
                Assert.Equal(.7, redis.HIncrByFloat("test", "field", .35));
                Assert.Equal(.35, redis.HDecrByFloat("test", "field", .35));
                Assert.Equal(0, redis.HDecrByFloat("test", "field", .35));
            });
        }

        [Fact()]
        public void TestHKeys()
        {
            using(var mock = new MockConnector("localhost", 9999, "*2\r\n$5\r\ntest1\r\n$5\r\ntest2\r\n"))
            using(var redis = new RedisClient(mock))
            {
                var response = redis.HKeys("test");
                Assert.Equal(2, response.Length);
                Assert.Equal("test1", response[0]);
                Assert.Equal("test2", response[1]);
                Assert.Equal("*2\r\n$5\r\nHKEYS\r\n$4\r\ntest\r\n", mock.GetMessage());
            }

            this.RealCall(redis =>
            {
                redis.HSet("test", "field1", "value1");
                redis.HSet("test", "field2", "value2");

                var response = redis.HKeys("test");
                Assert.Equal(2, response.Length);
                Assert.Equal("field1", response[0]);
                Assert.Equal("field2", response[1]);
            });
        }

        [Fact()]
        public void TestHLen()
        {
            using(var mock = new MockConnector("localhost", 9999, ":5\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.Equal(5, redis.HLen("test"));
                Assert.Equal("*2\r\n$4\r\nHLEN\r\n$4\r\ntest\r\n", mock.GetMessage());
            }

            this.RealCall(redis =>
            {
                redis.HSet("test", "field1", "value1");
                redis.HSet("test", "field2", "value2");
                Assert.Equal(2, redis.HLen("test"));
            });
        }

        [Fact()]
        public void TestHMGet()
        {
            using(var mock = new MockConnector("localhost", 9999, "*2\r\n$5\r\ntest1\r\n$5\r\ntest2\r\n"))
            using(var redis = new RedisClient(mock))
            {
                var response = redis.HMGet("test", "field1", "field2");
                Assert.Equal(2, response.Length);
                Assert.Equal("test1", (string)response[0]);
                Assert.Equal("test2", (string)response[1]);
                Assert.Equal("*4\r\n$5\r\nHMGET\r\n$4\r\ntest\r\n$6\r\nfield1\r\n$6\r\nfield2\r\n", mock.GetMessage());
            }

            this.RealCall(redis =>
            {
                redis.HSet("test", "field1", "value1");
                redis.HSet("test", "field2", "value2");

                var response = redis.HMGet("test", "field1", "field2");
                Assert.Equal(2, response.Length);
                Assert.Equal("value1", (string)response[0]);
                Assert.Equal("value2", (string)response[1]);
            });
        }

        [Fact()]
        public void TestHMSet_RedisDictionary()
        {
            using(var mock = new MockConnector("localhost", 9999, "+OK\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.True(redis.HMSet("test", new RedisDictionary() { { "field1", "test1" } }));
                Assert.Equal("*4\r\n$5\r\nHMSET\r\n$4\r\ntest\r\n$6\r\nfield1\r\n$5\r\ntest1\r\n", mock.GetMessage());
            }

            this.RealCall(redis =>
            {
                Assert.True(redis.HMSet("test", new RedisDictionary() { { "field1", "value1" }, { "field2", "value2" } }));
                var response = redis.HMGet("test", "field1", "field2");
                Assert.Equal(2, response.Length);
                Assert.Equal("value1", (string)response[0]);
                Assert.Equal("value2", (string)response[1]);
            });
        }

        [Fact()]
        public void TestHMSet_Anonymous()
        {
            using(var mock = new MockConnector("localhost", 9999, "+OK\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.True(redis.HMSet("test", new { field1 = "test1" }));
                Assert.Equal("*4\r\n$5\r\nHMSET\r\n$4\r\ntest\r\n$6\r\nfield1\r\n$5\r\ntest1\r\n", mock.GetMessage());
            }

            this.RealCall(redis =>
            {
                Assert.True(redis.HMSet("test", new { field1 = "value1", field2 = "value2" }));
                var response = redis.HMGet("test", "field1", "field2");
                Assert.Equal(2, response.Length);
                Assert.Equal("value1", (string)response[0]);
                Assert.Equal("value2", (string)response[1]);
            });
        }

        [Fact()]
        public void TestHMSet_Dictionary()
        {
            using(var mock = new MockConnector("localhost", 9999, "+OK\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.True(redis.HMSet("test", new Dictionary<string, string> { { "field1", "test1" } }));
                Assert.Equal("*4\r\n$5\r\nHMSET\r\n$4\r\ntest\r\n$6\r\nfield1\r\n$5\r\ntest1\r\n", mock.GetMessage());
            }

            this.RealCall(redis =>
            {
                Assert.True(redis.HMSet("test", new Dictionary<string, string> { { "field1", "value1" }, { "field2", "value2" } }));
                var response = redis.HMGet("test", "field1", "field2");
                Assert.Equal(2, response.Length);
                Assert.Equal("value1", (string)response[0]);
                Assert.Equal("value2", (string)response[1]);
            });
        }

        [Fact()]
        public void TestHSet()
        {
            using(var mock = new MockConnector("localhost", 9999, ":1\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.True(redis.HSet("test", "field1", "test1"));
                Assert.Equal("*4\r\n$4\r\nHSET\r\n$4\r\ntest\r\n$6\r\nfield1\r\n$5\r\ntest1\r\n", mock.GetMessage());
            }

            this.RealCall(redis =>
            {
                Assert.True(redis.HSet("test", "field1", "value1"));

                var response = redis.HKeys("test");
                Assert.Equal(1, response.Length);
                Assert.Equal("field1", response[0]);
            });
        }

        [Fact()]
        public void TestHSetNX()
        {
            using(var mock = new MockConnector("localhost", 9999, ":1\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.True(redis.HSet("test", "field1", "test1", true));
                Assert.Equal("*4\r\n$6\r\nHSETNX\r\n$4\r\ntest\r\n$6\r\nfield1\r\n$5\r\ntest1\r\n", mock.GetMessage());
            }

            this.RealCall(redis =>
            {
                Assert.True(redis.HSet("test", "field1", "value1", true));
                Assert.False(redis.HSet("test", "field1", "value1", true));

                var response = redis.HKeys("test");
                Assert.Equal(1, response.Length);
                Assert.Equal("field1", response[0]);
            });
        }

        [Fact()]
        public void TestHVals()
        {
            using(var mock = new MockConnector("localhost", 9999, "*2\r\n$5\r\ntest1\r\n$5\r\ntest2\r\n"))
            using(var redis = new RedisClient(mock))
            {
                var response = redis.HVals("test");
                Assert.Equal(2, response.Length);
                Assert.Equal("test1", (string)response[0]);
                Assert.Equal("test2", (string)response[1]);
                Assert.Equal("*2\r\n$5\r\nHVALS\r\n$4\r\ntest\r\n", mock.GetMessage());
            }

            this.RealCall(redis =>
            {
                Assert.True(redis.HMSet("test", new { field1 = "value1", field2 = "value2" }));
                var response = redis.HVals("test");
                Assert.Equal(2, response.Length);
                Assert.Equal("value1", (string)response[0]);
                Assert.Equal("value2", (string)response[1]);
            });
        }

        [Fact()]
        public void TestHScan()
        {
            string reply = "*2\r\n$1\r\n0\r\n*4\r\n$6\r\nfield1\r\n$5\r\ntest1\r\n$6\r\nfield2\r\n$5\r\ntest2\r\n";
            using(var mock = new MockConnector("localhost", 9999, reply, reply, reply, reply))
            using(var redis = new RedisClient(mock))
            {
                var resp = (redis.HScan("test", 0) as RedisScan<RedisFieldItem>).GetEnumerator() as RedisScan<RedisFieldItem>.Enumerator;
                resp.MoveNext();
                Assert.Equal(0, resp.Cursor);
                Assert.Equal(1, resp.Items.Length);
                Assert.Equal("field1", resp.Current.Field);
                Assert.Equal("test1", (string)resp.Current.Value);
                resp.MoveNext();
                Assert.Equal(0, resp.Cursor);
                Assert.Equal(0, resp.Items.Length);
                Assert.Equal("field2", resp.Current.Field);
                Assert.Equal("test2", (string)resp.Current.Value);
                Assert.Equal("*3\r\n$5\r\nHSCAN\r\n$4\r\ntest\r\n$1\r\n0\r\n", mock.GetMessage());

                var response2 = redis.HScan("test", 0, pattern: "*").ToArray();
                Assert.Equal("*5\r\n$5\r\nHSCAN\r\n$4\r\ntest\r\n$1\r\n0\r\n$5\r\nMATCH\r\n$1\r\n*\r\n", mock.GetMessage());

                var response3 = redis.HScan("test", 0, count: 5).ToArray();
                Assert.Equal("*5\r\n$5\r\nHSCAN\r\n$4\r\ntest\r\n$1\r\n0\r\n$5\r\nCOUNT\r\n$1\r\n5\r\n", mock.GetMessage());

                var response4 = redis.HScan("test", 0, "*", 5).ToArray();
                Assert.Equal("*7\r\n$5\r\nHSCAN\r\n$4\r\ntest\r\n$1\r\n0\r\n$5\r\nMATCH\r\n$1\r\n*\r\n$5\r\nCOUNT\r\n$1\r\n5\r\n", mock.GetMessage());
            }


            this.RealCall(redis =>
            {
                Assert.True(redis.HMSet("test", new { field1 = "value1", field2 = "value2" }));
                var  response= redis.HScan("test").ToArray();
                Assert.Equal(2, response.Length);
                Assert.Equal("field1", response[0].Field);
                Assert.Equal("field2", response[1].Field);
                Assert.Equal("value1", (string)response[0].Value);
                Assert.Equal("value2", (string)response[1].Value);
            });
        }
    }
}
