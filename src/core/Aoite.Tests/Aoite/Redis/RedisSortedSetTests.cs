using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoite.Redis
{
    public class RedisSortedSetTests
    {
        [Fact()]
        public void TestZAdd_Array()
        {
            using(var mock = new MockConnector("localhost", 9999, ":2\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.Equal(2, redis.ZAdd("test", new RedisScoreItem(1.1, "test1"), new RedisScoreItem(2.2, "test2")));
                Assert.Equal("*6\r\n$4\r\nZADD\r\n$4\r\ntest\r\n$3\r\n1.1\r\n$5\r\ntest1\r\n$3\r\n2.2\r\n$5\r\ntest2\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void TestZCard()
        {
            using(var mock = new MockConnector("localhost", 9999, ":2\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.Equal(2, redis.ZCard("test"));
                Assert.Equal("*2\r\n$5\r\nZCARD\r\n$4\r\ntest\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void TestZCount()
        {
            string reply = ":2\r\n";
            using(var mock = new MockConnector("localhost", 9999, reply, reply, reply, reply, reply))
            using(var redis = new RedisClient(mock))
            {
                Assert.Equal(2, redis.ZCount("test", 1, 3));
                Assert.Equal("*4\r\n$6\r\nZCOUNT\r\n$4\r\ntest\r\n$1\r\n1\r\n$1\r\n3\r\n", mock.GetMessage());

                Assert.Equal(2, redis.ZCount("test", Double.NegativeInfinity, Double.PositiveInfinity));
                Assert.Equal("*4\r\n$6\r\nZCOUNT\r\n$4\r\ntest\r\n$4\r\n-inf\r\n$4\r\n+inf\r\n", mock.GetMessage());

                Assert.Equal(2, redis.ZCount("test", 1, 3, exclusiveMin: true, exclusiveMax: true));
                Assert.Equal("*4\r\n$6\r\nZCOUNT\r\n$4\r\ntest\r\n$2\r\n(1\r\n$2\r\n(3\r\n", mock.GetMessage());

                Assert.Equal(2, redis.ZCount("test", double.MinValue, double.MaxValue));
                Assert.Equal("*4\r\n$6\r\nZCOUNT\r\n$4\r\ntest\r\n$4\r\n-inf\r\n$4\r\n+inf\r\n", mock.GetMessage());

                Assert.Equal(2, redis.ZCount("test", double.NegativeInfinity, double.PositiveInfinity));
                Assert.Equal("*4\r\n$6\r\nZCOUNT\r\n$4\r\ntest\r\n$4\r\n-inf\r\n$4\r\n+inf\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void TestZIncrby()
        {
            using(var mock = new MockConnector("localhost", 9999, "$4\r\n3.14\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.Equal(3.14, redis.ZIncrBy("test", 1.5, "test1"));
                Assert.Equal("*4\r\n$7\r\nZINCRBY\r\n$4\r\ntest\r\n$3\r\n1.5\r\n$5\r\ntest1\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void TestZInterStore()
        {
            string reply = ":2\r\n";
            using(var mock = new MockConnector("localhost", 9999, reply, reply, reply, reply, reply))
            using(var redis = new RedisClient(mock))
            {
                Assert.Equal(2, redis.ZInterStore("destination", new RedisWeightDictionary("key1", "key2")));
                Assert.Equal("*5\r\n$11\r\nZINTERSTORE\r\n$11\r\ndestination\r\n$1\r\n2\r\n$4\r\nkey1\r\n$4\r\nkey2\r\n", mock.GetMessage());

                var dict = new RedisWeightDictionary(new[] { "key1", "key2" }, new[] { 1D, 2D });
                Assert.Equal(2, redis.ZInterStore("destination", dict));
                Assert.Equal("*8\r\n$11\r\nZINTERSTORE\r\n$11\r\ndestination\r\n$1\r\n2\r\n$4\r\nkey1\r\n$4\r\nkey2\r\n$7\r\nWEIGHTS\r\n$1\r\n1\r\n$1\r\n2\r\n", mock.GetMessage());

                Assert.Equal(2, redis.ZInterStore("destination", dict, RedisAggregate.Max));
                Assert.Equal("*10\r\n$11\r\nZINTERSTORE\r\n$11\r\ndestination\r\n$1\r\n2\r\n$4\r\nkey1\r\n$4\r\nkey2\r\n$7\r\nWEIGHTS\r\n$1\r\n1\r\n$1\r\n2\r\n$9\r\nAGGREGATE\r\n$3\r\nMAX\r\n", mock.GetMessage());

                Assert.Equal(2, redis.ZInterStore("destination", dict, RedisAggregate.Min));
                Assert.Equal("*10\r\n$11\r\nZINTERSTORE\r\n$11\r\ndestination\r\n$1\r\n2\r\n$4\r\nkey1\r\n$4\r\nkey2\r\n$7\r\nWEIGHTS\r\n$1\r\n1\r\n$1\r\n2\r\n$9\r\nAGGREGATE\r\n$3\r\nMIN\r\n", mock.GetMessage());

                Assert.Equal(2, redis.ZInterStore("destination", dict, RedisAggregate.Sum));
                Assert.Equal("*10\r\n$11\r\nZINTERSTORE\r\n$11\r\ndestination\r\n$1\r\n2\r\n$4\r\nkey1\r\n$4\r\nkey2\r\n$7\r\nWEIGHTS\r\n$1\r\n1\r\n$1\r\n2\r\n$9\r\nAGGREGATE\r\n$3\r\nSUM\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void TestZLexCount()
        {
            using(var mock = new MockConnector("localhost", 9999, ":3\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.Equal(3, redis.ZLexCount("test", null, null));
                Assert.Equal("*4\r\n$9\r\nZLEXCOUNT\r\n$4\r\ntest\r\n$1\r\n-\r\n$1\r\n+\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void TestZRange()
        {
            string reply1 = "*2\r\n$5\r\ntest1\r\n$5\r\ntest2\r\n";
            string reply2 = "*4\r\n$5\r\ntest1\r\n$3\r\n1.1\r\n$5\r\ntest2\r\n$3\r\n2.2\r\n";
            using(var mock = new MockConnector("localhost", 9999, reply1, reply2))
            using(var redis = new RedisClient(mock))
            {
                var response1 = redis.ZRange("test", 0, -1);
                Assert.Equal(2, response1.Length);
                Assert.Equal("test1", (string)response1[0]);
                Assert.Equal("test2", (string)response1[1]);
                Assert.Equal("*4\r\n$6\r\nZRANGE\r\n$4\r\ntest\r\n$1\r\n0\r\n$2\r\n-1\r\n", mock.GetMessage());

                var response2 = redis.ZRangeWithScores("test", 0, -1);
                Assert.Equal(2, response2.Length);
                Assert.Equal(1.1, response2[0].Score);
                Assert.Equal("test1", (string)response2[0].Member);
                Assert.Equal(2.2, response2[1].Score);
                Assert.Equal("test2", (string)response2[1].Member);
                Assert.Equal("*5\r\n$6\r\nZRANGE\r\n$4\r\ntest\r\n$1\r\n0\r\n$2\r\n-1\r\n$10\r\nWITHSCORES\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void TestZRangeByLex()
        {
            string reply = "*2\r\n$5\r\ntest1\r\n$5\r\ntest2\r\n";
            using(var mock = new MockConnector("localhost", 9999, reply, reply))
            using(var redis = new RedisClient(mock))
            {
                var response1 = redis.ZRangeByLex("test", null, "c");
                Assert.Equal(2, response1.Length);
                Assert.Equal("test1", (string)response1[0]);
                Assert.Equal("test2", (string)response1[1]);
                Assert.Equal("*4\r\n$11\r\nZRANGEBYLEX\r\n$4\r\ntest\r\n$1\r\n-\r\n$2\r\n[c\r\n", mock.GetMessage());

                var response2 = redis.ZRangeByLex("test", null, "c", offset: 10, count: 5);
                Assert.Equal("*7\r\n$11\r\nZRANGEBYLEX\r\n$4\r\ntest\r\n$1\r\n-\r\n$2\r\n[c\r\n$5\r\nLIMIT\r\n$2\r\n10\r\n$1\r\n5\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void TestZRangeByScore()
        {
            string reply1 = "*2\r\n$5\r\ntest1\r\n$5\r\ntest2\r\n";
            string reply2 = "*4\r\n$5\r\ntest1\r\n$3\r\n1.1\r\n$5\r\ntest2\r\n$3\r\n2.2\r\n";
            using(var mock = new MockConnector("localhost", 9999, reply1, reply1, reply2, reply2, reply2))
            using(var redis = new RedisClient(mock))
            {
                var response1 = redis.ZRangeByScore("test", 0, 10);
                Assert.Equal(2, response1.Length);
                Assert.Equal("test1", (string)response1[0]);
                Assert.Equal("test2", (string)response1[1]);
                Assert.Equal("*4\r\n$13\r\nZRANGEBYSCORE\r\n$4\r\ntest\r\n$1\r\n0\r\n$2\r\n10\r\n", mock.GetMessage());

                var response2 = redis.ZRangeByScore("test", 0, 10, true, true);
                Assert.Equal("*4\r\n$13\r\nZRANGEBYSCORE\r\n$4\r\ntest\r\n$2\r\n(0\r\n$3\r\n(10\r\n", mock.GetMessage());

                var response3 = redis.ZRangeByScoreWithScores("test", 0, 10);
                Assert.Equal(2, response3.Length);
                Assert.Equal(1.1, response3[0].Score);
                Assert.Equal("test1", (string)response3[0].Member);
                Assert.Equal(2.2, response3[1].Score);
                Assert.Equal("test2", (string)response3[1].Member);
                Assert.Equal("*5\r\n$13\r\nZRANGEBYSCORE\r\n$4\r\ntest\r\n$1\r\n0\r\n$2\r\n10\r\n$10\r\nWITHSCORES\r\n", mock.GetMessage());

                var response4 = redis.ZRangeByScoreWithScores("test", 0, 10, true, true);
                Assert.Equal("*5\r\n$13\r\nZRANGEBYSCORE\r\n$4\r\ntest\r\n$2\r\n(0\r\n$3\r\n(10\r\n$10\r\nWITHSCORES\r\n", mock.GetMessage());

                var response5 = redis.ZRangeByScoreWithScores("test", 0, 10, true, true, 1, 5);
                Assert.Equal("*8\r\n$13\r\nZRANGEBYSCORE\r\n$4\r\ntest\r\n$2\r\n(0\r\n$3\r\n(10\r\n$10\r\nWITHSCORES\r\n$5\r\nLIMIT\r\n$1\r\n1\r\n$1\r\n5\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void TestZRangeByScoreWithScores()
        {
            string reply = "*4\r\n$5\r\ntest1\r\n$3\r\n1.1\r\n$5\r\ntest2\r\n$3\r\n2.2\r\n";
            using(var mock = new MockConnector("localhost", 9999, reply, reply, reply, reply))
            using(var redis = new RedisClient(mock))
            {
                var response1 = redis.ZRangeByScoreWithScores("test", 0, 10);
                Assert.Equal(2, response1.Length);
                Assert.Equal(1.1, response1[0].Score);
                Assert.Equal("test1", (string)response1[0].Member);
                Assert.Equal(2.2, response1[1].Score);
                Assert.Equal("test2", (string)response1[1].Member);
                Assert.Equal("*5\r\n$13\r\nZRANGEBYSCORE\r\n$4\r\ntest\r\n$1\r\n0\r\n$2\r\n10\r\n$10\r\nWITHSCORES\r\n", mock.GetMessage());

                var response3 = redis.ZRangeByScoreWithScores("test", 0, 10, true, true);
                Assert.Equal("*5\r\n$13\r\nZRANGEBYSCORE\r\n$4\r\ntest\r\n$2\r\n(0\r\n$3\r\n(10\r\n$10\r\nWITHSCORES\r\n", mock.GetMessage());

                var response4 = redis.ZRangeByScoreWithScores("test", 0, 10, true, true, 1, 5);
                Assert.Equal("*8\r\n$13\r\nZRANGEBYSCORE\r\n$4\r\ntest\r\n$2\r\n(0\r\n$3\r\n(10\r\n$10\r\nWITHSCORES\r\n$5\r\nLIMIT\r\n$1\r\n1\r\n$1\r\n5\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void TestZRank()
        {
            string reply1 = ":3\r\n";
            string reply2 = "$-1\r\n";
            using(var mock = new MockConnector("localhost", 9999, reply1, reply2))
            using(var redis = new RedisClient(mock))
            {
                Assert.Equal(3, redis.ZRank("test", "member"));
                Assert.Equal("*3\r\n$5\r\nZRANK\r\n$4\r\ntest\r\n$6\r\nmember\r\n", mock.GetMessage());

                Assert.Null(redis.ZRank("test", "member"));
            }
        }

        [Fact()]
        public void TestZRem()
        {
            using(var mock = new MockConnector("localhost", 9999, ":2\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.Equal(2, redis.ZRem("test", "m1", "m2"));
                Assert.Equal("*4\r\n$4\r\nZREM\r\n$4\r\ntest\r\n$2\r\nm1\r\n$2\r\nm2\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void TestZRemRangeByLex()
        {
            using(var mock = new MockConnector("localhost", 9999, ":2\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.Equal(2, redis.ZRemRangeByLex("test", "a", "z"));
                Assert.Equal("*4\r\n$14\r\nZREMRANGEBYLEX\r\n$4\r\ntest\r\n$2\r\n[a\r\n$2\r\n[z\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void TestZRemRangeByRank()
        {
            using(var mock = new MockConnector("localhost", 9999, ":2\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.Equal(2, redis.ZRemRangeByRank("test", 0, 10));
                Assert.Equal("*4\r\n$15\r\nZREMRANGEBYRANK\r\n$4\r\ntest\r\n$1\r\n0\r\n$2\r\n10\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void TestZRemRangeByScore()
        {
            using(var mock = new MockConnector("localhost", 9999, ":2\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.Equal(2, redis.ZRemRangeByScore("test", 0, 10));
                Assert.Equal("*4\r\n$16\r\nZREMRANGEBYSCORE\r\n$4\r\ntest\r\n$1\r\n0\r\n$2\r\n10\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void TestZRevRange()
        {
            string reply1 = "*2\r\n$5\r\ntest1\r\n$5\r\ntest2\r\n";
            string reply2 = "*4\r\n$5\r\ntest1\r\n$3\r\n1.1\r\n$5\r\ntest2\r\n$3\r\n2.2\r\n";
            using(var mock = new MockConnector("localhost", 9999, reply1, reply2, reply2))
            using(var redis = new RedisClient(mock))
            {
                var response1 = redis.ZRevRange("test", 0, -1);
                Assert.Equal(2, response1.Length);
                Assert.Equal("test1", (string)response1[0]);
                Assert.Equal("test2", (string)response1[1]);
                Assert.Equal("*4\r\n$9\r\nZREVRANGE\r\n$4\r\ntest\r\n$1\r\n0\r\n$2\r\n-1\r\n", mock.GetMessage());

                var response2 = redis.ZRevRangeWithScores("test", 0, -1);
                Assert.Equal(2, response2.Length);
                Assert.Equal(1.1, response2[0].Score);
                Assert.Equal("test1", (string)response2[0].Member);
                Assert.Equal(2.2, response2[1].Score);
                Assert.Equal("test2", (string)response2[1].Member);
                Assert.Equal("*5\r\n$9\r\nZREVRANGE\r\n$4\r\ntest\r\n$1\r\n0\r\n$2\r\n-1\r\n$10\r\nWITHSCORES\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void TestZRevRangeByScore()
        {
            string reply1 = "*2\r\n$5\r\ntest1\r\n$5\r\ntest2\r\n";
            string reply2 = "*4\r\n$5\r\ntest1\r\n$3\r\n1.1\r\n$5\r\ntest2\r\n$3\r\n2.2\r\n";
            using(var mock = new MockConnector("localhost", 9999, reply1, reply1, reply2, reply2, reply2))
            using(var redis = new RedisClient(mock))
            {
                var response1 = redis.ZRevRangeByScore("test", 0, 10);
                Assert.Equal(2, response1.Length);
                Assert.Equal("test1", (string)response1[0]);
                Assert.Equal("test2", (string)response1[1]);
                Assert.Equal("*4\r\n$16\r\nZREVRANGEBYSCORE\r\n$4\r\ntest\r\n$1\r\n0\r\n$2\r\n10\r\n", mock.GetMessage());

                var response2 = redis.ZRevRangeByScore("test", 0, 10, true, true);
                Assert.Equal("*4\r\n$16\r\nZREVRANGEBYSCORE\r\n$4\r\ntest\r\n$2\r\n(0\r\n$3\r\n(10\r\n", mock.GetMessage());

                var response3 = redis.ZRevRangeByScoreWithScores("test", 0, 10);
                Assert.Equal(2, response3.Length);
                Assert.Equal(1.1, response3[0].Score);
                Assert.Equal("test1", (string)response3[0].Member);
                Assert.Equal(2.2, response3[1].Score);
                Assert.Equal("test2", (string)response3[1].Member);
                Assert.Equal("*5\r\n$16\r\nZREVRANGEBYSCORE\r\n$4\r\ntest\r\n$1\r\n0\r\n$2\r\n10\r\n$10\r\nWITHSCORES\r\n", mock.GetMessage());

                var response4 = redis.ZRevRangeByScoreWithScores("test", 0, 10, exclusiveMin: true, exclusiveMax: true);
                Assert.Equal("*5\r\n$16\r\nZREVRANGEBYSCORE\r\n$4\r\ntest\r\n$2\r\n(0\r\n$3\r\n(10\r\n$10\r\nWITHSCORES\r\n", mock.GetMessage());

                var response5 = redis.ZRevRangeByScoreWithScores("test", 0, 10, exclusiveMin: true, exclusiveMax: true, offset: 1, count: 5);
                Assert.Equal("*8\r\n$16\r\nZREVRANGEBYSCORE\r\n$4\r\ntest\r\n$2\r\n(0\r\n$3\r\n(10\r\n$10\r\nWITHSCORES\r\n$5\r\nLIMIT\r\n$1\r\n1\r\n$1\r\n5\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void TestZRevRangeByScoreWithScores()
        {
            string reply = "*4\r\n$5\r\ntest1\r\n$3\r\n1.1\r\n$5\r\ntest2\r\n$3\r\n2.2\r\n";
            using(var mock = new MockConnector("localhost", 9999, reply, reply, reply, reply))
            using(var redis = new RedisClient(mock))
            {
                var response1 = redis.ZRevRangeByScoreWithScores("test", 0, 10);
                Assert.Equal(2, response1.Length);
                Assert.Equal(1.1, response1[0].Score);
                Assert.Equal("test1", (string)response1[0].Member);
                Assert.Equal(2.2, response1[1].Score);
                Assert.Equal("test2", (string)response1[1].Member);
                Assert.Equal("*5\r\n$16\r\nZREVRANGEBYSCORE\r\n$4\r\ntest\r\n$1\r\n0\r\n$2\r\n10\r\n$10\r\nWITHSCORES\r\n", mock.GetMessage());

                var response3 = redis.ZRevRangeByScoreWithScores("test", 0, 10, exclusiveMin: true, exclusiveMax: true);
                Assert.Equal("*5\r\n$16\r\nZREVRANGEBYSCORE\r\n$4\r\ntest\r\n$2\r\n(0\r\n$3\r\n(10\r\n$10\r\nWITHSCORES\r\n", mock.GetMessage());

                var response4 = redis.ZRevRangeByScoreWithScores("test", 0, 10, exclusiveMin: true, exclusiveMax: true, offset: 1, count: 5);
                Assert.Equal("*8\r\n$16\r\nZREVRANGEBYSCORE\r\n$4\r\ntest\r\n$2\r\n(0\r\n$3\r\n(10\r\n$10\r\nWITHSCORES\r\n$5\r\nLIMIT\r\n$1\r\n1\r\n$1\r\n5\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void TestZRevRank()
        {
            using(var mock = new MockConnector("localhost", 9999, ":2\r\n", "$-1\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.Equal(2, redis.ZRevRank("test", "test1"));
                Assert.Equal("*3\r\n$8\r\nZREVRANK\r\n$4\r\ntest\r\n$5\r\ntest1\r\n", mock.GetMessage());

                Assert.Null(redis.ZRevRank("test", "test1"));
            }
        }

        [Fact()]
        public void TestZScan()
        {
            string reply = "*2\r\n$1\r\n0\r\n*4\r\n$7\r\nmember1\r\n$3\r\n1.1\r\n$7\r\nmember2\r\n$3\r\n2.2\r\n";
            using(var mock = new MockConnector("localhost", 9999, reply, reply, reply, reply))
            using(var redis = new RedisClient(mock))
            {
                var resp = (redis.ZScan("test", 0) as RedisScan<RedisScoreItem>).GetEnumerator() as RedisScan<RedisScoreItem>.Enumerator;
                resp.MoveNext();
                Assert.Equal(0, resp.Cursor);
                Assert.Equal(1, resp.Items.Length);
                Assert.Equal("member1", (string)resp.Current.Member);
                Assert.Equal(1.1, resp.Current.Score);
                resp.MoveNext();
                Assert.Equal(0, resp.Cursor);
                Assert.Equal(0, resp.Items.Length);
                Assert.Equal("member2", (string)resp.Current.Member);
                Assert.Equal(2.2, resp.Current.Score);
                Assert.Equal("*3\r\n$5\r\nZSCAN\r\n$4\r\ntest\r\n$1\r\n0\r\n", mock.GetMessage());

                var response2 = redis.ZScan("test", 0, pattern: "*").ToArray();
                Assert.Equal("*5\r\n$5\r\nZSCAN\r\n$4\r\ntest\r\n$1\r\n0\r\n$5\r\nMATCH\r\n$1\r\n*\r\n", mock.GetMessage());

                var response3 = redis.ZScan("test", 0, count: 5).ToArray();
                Assert.Equal("*5\r\n$5\r\nZSCAN\r\n$4\r\ntest\r\n$1\r\n0\r\n$5\r\nCOUNT\r\n$1\r\n5\r\n", mock.GetMessage());

                var response4 = redis.ZScan("test", 0, "*", 5).ToArray();
                Assert.Equal("*7\r\n$5\r\nZSCAN\r\n$4\r\ntest\r\n$1\r\n0\r\n$5\r\nMATCH\r\n$1\r\n*\r\n$5\r\nCOUNT\r\n$1\r\n5\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void TestZScore()
        {
            using(var mock = new MockConnector("localhost", 9999, "$3\r\n1.1\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.Equal(1.1, redis.ZScore("test", "member1"));
                Assert.Equal("*3\r\n$6\r\nZSCORE\r\n$4\r\ntest\r\n$7\r\nmember1\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void TestZUnionStore()
        {
            string reply = ":2\r\n";
            using(var mock = new MockConnector("localhost", 9999, reply, reply, reply, reply, reply))
            using(var redis = new RedisClient(mock))
            {
                Assert.Equal(2, redis.ZUnionStore("destination", new RedisWeightDictionary("key1", "key2")));
                Assert.Equal("*5\r\n$11\r\nZUNIONSTORE\r\n$11\r\ndestination\r\n$1\r\n2\r\n$4\r\nkey1\r\n$4\r\nkey2\r\n", mock.GetMessage());

                var dict = new RedisWeightDictionary(new[] { "key1", "key2" }, new[] { 1D, 2D });
                Assert.Equal(2, redis.ZUnionStore("destination", dict));
                Assert.Equal("*8\r\n$11\r\nZUNIONSTORE\r\n$11\r\ndestination\r\n$1\r\n2\r\n$4\r\nkey1\r\n$4\r\nkey2\r\n$7\r\nWEIGHTS\r\n$1\r\n1\r\n$1\r\n2\r\n", mock.GetMessage());

                Assert.Equal(2, redis.ZUnionStore("destination", dict, RedisAggregate.Max));
                Assert.Equal("*10\r\n$11\r\nZUNIONSTORE\r\n$11\r\ndestination\r\n$1\r\n2\r\n$4\r\nkey1\r\n$4\r\nkey2\r\n$7\r\nWEIGHTS\r\n$1\r\n1\r\n$1\r\n2\r\n$9\r\nAGGREGATE\r\n$3\r\nMAX\r\n", mock.GetMessage());

                Assert.Equal(2, redis.ZUnionStore("destination", dict, RedisAggregate.Min));
                Assert.Equal("*10\r\n$11\r\nZUNIONSTORE\r\n$11\r\ndestination\r\n$1\r\n2\r\n$4\r\nkey1\r\n$4\r\nkey2\r\n$7\r\nWEIGHTS\r\n$1\r\n1\r\n$1\r\n2\r\n$9\r\nAGGREGATE\r\n$3\r\nMIN\r\n", mock.GetMessage());

                Assert.Equal(2, redis.ZUnionStore("destination", dict, RedisAggregate.Sum));
                Assert.Equal("*10\r\n$11\r\nZUNIONSTORE\r\n$11\r\ndestination\r\n$1\r\n2\r\n$4\r\nkey1\r\n$4\r\nkey2\r\n$7\r\nWEIGHTS\r\n$1\r\n1\r\n$1\r\n2\r\n$9\r\nAGGREGATE\r\n$3\r\nSUM\r\n", mock.GetMessage());
            }
        }
    }
}
