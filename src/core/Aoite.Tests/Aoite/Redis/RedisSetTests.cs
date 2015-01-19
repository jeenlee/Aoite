using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoite.Redis
{
    public class RedisSetTests
    {
        [Fact()]
        public void TestSAdd()
        {
            using(var mock = new MockConnector("localhost", 9999, ":3\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.Equal(3, redis.SAdd("test", "test1"));
                Assert.Equal("*3\r\n$4\r\nSADD\r\n$4\r\ntest\r\n$5\r\ntest1\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void TestSCard()
        {
            using(var mock = new MockConnector("localhost", 9999, ":3\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.Equal(3, redis.SCard("test"));
                Assert.Equal("*2\r\n$5\r\nSCARD\r\n$4\r\ntest\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void TestSDiff()
        {
            using(var mock = new MockConnector("localhost", 9999, "*2\r\n$5\r\ntest1\r\n$5\r\ntest2\r\n"))
            using(var redis = new RedisClient(mock))
            {
                var response = redis.SDiff("test", "another");
                Assert.Equal(2, response.Length);
                Assert.Equal("test1", (string)response[0]);
                Assert.Equal("test2", (string)response[1]);
                Assert.Equal("*3\r\n$5\r\nSDIFF\r\n$4\r\ntest\r\n$7\r\nanother\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void TestSDiffStore()
        {
            using(var mock = new MockConnector("localhost", 9999, ":3\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.Equal(3, redis.SDiffStore("destination", "key1", "key2"));
                Assert.Equal("*4\r\n$10\r\nSDIFFSTORE\r\n$11\r\ndestination\r\n$4\r\nkey1\r\n$4\r\nkey2\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void TestInter()
        {
            using(var mock = new MockConnector("localhost", 9999, "*2\r\n$5\r\ntest1\r\n$5\r\ntest2\r\n"))
            using(var redis = new RedisClient(mock))
            {
                var response = redis.SInter("test", "another");
                Assert.Equal(2, response.Length);
                Assert.Equal("test1", (string)response[0]);
                Assert.Equal("test2", (string)response[1]);
                Assert.Equal("*3\r\n$6\r\nSINTER\r\n$4\r\ntest\r\n$7\r\nanother\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void TestSInterStore()
        {
            using(var mock = new MockConnector("localhost", 9999, ":3\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.Equal(3, redis.SInterStore("destination", "key1", "key2"));
                Assert.Equal("*4\r\n$11\r\nSINTERSTORE\r\n$11\r\ndestination\r\n$4\r\nkey1\r\n$4\r\nkey2\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void TestSIsMember()
        {
            using(var mock = new MockConnector("localhost", 9999, ":1\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.True(redis.SIsMember("test", "test1"));
                Assert.Equal("*3\r\n$9\r\nSISMEMBER\r\n$4\r\ntest\r\n$5\r\ntest1\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void TestSMembers()
        {
            using(var mock = new MockConnector("localhost", 9999, "*2\r\n$5\r\ntest1\r\n$5\r\ntest2\r\n"))
            using(var redis = new RedisClient(mock))
            {
                var response = redis.SMembers("test");
                Assert.Equal(2, response.Length);
                Assert.Equal("test1", (string)response[0]);
                Assert.Equal("test2", (string)response[1]);
                Assert.Equal("*2\r\n$8\r\nSMEMBERS\r\n$4\r\ntest\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void TestSMove()
        {
            using(var mock = new MockConnector("localhost", 9999, ":1\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.True(redis.SMove("test", "destination", "test1"));
                Assert.Equal("*4\r\n$5\r\nSMOVE\r\n$4\r\ntest\r\n$11\r\ndestination\r\n$5\r\ntest1\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void TestSPop()
        {
            using(var mock = new MockConnector("localhost", 9999, "$5\r\ntest1\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.Equal("test1", (string)redis.SPop("test"));
                Assert.Equal("*2\r\n$4\r\nSPOP\r\n$4\r\ntest\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void TestSRandMember()
        {
            using(var mock = new MockConnector("localhost", 9999, "$5\r\ntest1\r\n", "*2\r\n$5\r\ntest1\r\n$5\r\ntest2\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.Equal("test1", (string)redis.SRandMember("test"));
                Assert.Equal("*2\r\n$11\r\nSRANDMEMBER\r\n$4\r\ntest\r\n", mock.GetMessage());

                var response = redis.SRandMember("test", 2);
                Assert.Equal(2, response.Length);
                Assert.Equal("test1", (string)response[0]);
                Assert.Equal("test2", (string)response[1]);
                Assert.Equal("*3\r\n$11\r\nSRANDMEMBER\r\n$4\r\ntest\r\n$1\r\n2\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void TestSRem()
        {
            using(var mock = new MockConnector("localhost", 9999, ":2\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.Equal(2, redis.SRem("test", "test1", "test2"));
                Assert.Equal("*4\r\n$4\r\nSREM\r\n$4\r\ntest\r\n$5\r\ntest1\r\n$5\r\ntest2\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void TestSScan()
        {
            string reply = "*2\r\n$1\r\n0\r\n*2\r\n$5\r\ntest1\r\n$5\r\ntest2\r\n";
            using(var mock = new MockConnector("localhost", 9999, reply, reply, reply, reply))
            using(var redis = new RedisClient(mock))
            {
                var resp = (redis.SScan("test", 0) as RedisScan<BinaryValue>).GetEnumerator() as RedisScan<BinaryValue>.Enumerator;
                resp.MoveNext();
                Assert.Equal(0, resp.Cursor);
                Assert.Equal(1, resp.Items.Length);
                Assert.Equal("test1", (string)resp.Current);
                resp.MoveNext();
                Assert.Equal(0, resp.Cursor);
                Assert.Equal(0, resp.Items.Length);
                Assert.Equal("test2", (string)resp.Current);
                Assert.Equal("*3\r\n$5\r\nSSCAN\r\n$4\r\ntest\r\n$1\r\n0\r\n", mock.GetMessage());

                var response2 = redis.SScan("test", 0, pattern: "*").ToArray();
                Assert.Equal("*5\r\n$5\r\nSSCAN\r\n$4\r\ntest\r\n$1\r\n0\r\n$5\r\nMATCH\r\n$1\r\n*\r\n", mock.GetMessage());

                var response3 = redis.SScan("test", 0, count: 5).ToArray();
                Assert.Equal("*5\r\n$5\r\nSSCAN\r\n$4\r\ntest\r\n$1\r\n0\r\n$5\r\nCOUNT\r\n$1\r\n5\r\n", mock.GetMessage());

                var response4 = redis.SScan("test", 0, "*", 5).ToArray();
                Assert.Equal("*7\r\n$5\r\nSSCAN\r\n$4\r\ntest\r\n$1\r\n0\r\n$5\r\nMATCH\r\n$1\r\n*\r\n$5\r\nCOUNT\r\n$1\r\n5\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void TestSUnion()
        {
            using(var mock = new MockConnector("localhost", 9999, "*2\r\n$5\r\ntest1\r\n$5\r\ntest2\r\n"))
            using(var redis = new RedisClient(mock))
            {
                var response = redis.SUnion("test", "another");
                Assert.Equal(2, response.Length);
                Assert.Equal("test1", (string)response[0]);
                Assert.Equal("test2", (string)response[1]);
                Assert.Equal("*3\r\n$6\r\nSUNION\r\n$4\r\ntest\r\n$7\r\nanother\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void TestSUnionStore()
        {
            using(var mock = new MockConnector("localhost", 9999, ":3\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.Equal(3, redis.SUnionStore("destination", "key1", "key2"));
                Assert.Equal("*4\r\n$11\r\nSUNIONSTORE\r\n$11\r\ndestination\r\n$4\r\nkey1\r\n$4\r\nkey2\r\n", mock.GetMessage());
            }
        }
    }
}
