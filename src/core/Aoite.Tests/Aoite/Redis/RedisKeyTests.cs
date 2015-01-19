using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoite.Redis
{
    public class RedisKeyTests : TestBase
    {
        [Fact()]
        public void TestDel()
        {
            using(var mock = new MockConnector("localhost", 9999, ":3\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.Equal(3, redis.Del("test"));
                Assert.Equal("*2\r\n$3\r\nDEL\r\n$4\r\ntest\r\n", mock.GetMessage());
            }

            this.RealCall(redis =>
            {
                redis.MSet(new RedisDictionary { { "key1", "value1" }, { "key2", "value2" } });
                Assert.Equal(2, redis.Del("key1", "key2"));
            });
        }

        [Fact()]
        public void TestExists()
        {
            using(var mock = new MockConnector("localhost", 9999, ":1\r\n", ":0\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.True(redis.Exists("test1"));
                Assert.Equal("*2\r\n$6\r\nEXISTS\r\n$5\r\ntest1\r\n", mock.GetMessage());
                Assert.False(redis.Exists("test2"));
                Assert.Equal("*2\r\n$6\r\nEXISTS\r\n$5\r\ntest2\r\n", mock.GetMessage());
            }
            this.RealCall(redis =>
            {
                redis.MSet(new RedisDictionary { { "key1", "value1" }, { "key2", "value2" } });

                Assert.True(redis.Exists("key1"));
                Assert.True(redis.Exists("key2"));
                Assert.False(redis.Exists("test3"));
            });
        }

        [Fact()]
        public void TestExpire()
        {
            using(var mock = new MockConnector("localhost", 9999, ":1\r\n", ":0\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.True(redis.Expire("test1", TimeSpan.FromSeconds(10)));
                Assert.Equal("*3\r\n$6\r\nEXPIRE\r\n$5\r\ntest1\r\n$2\r\n10\r\n", mock.GetMessage());
                Assert.False(redis.Expire("test2", 20));
                Assert.Equal("*3\r\n$6\r\nEXPIRE\r\n$5\r\ntest2\r\n$2\r\n20\r\n", mock.GetMessage());
            }

            this.RealCall(redis =>
            {
                redis.MSet(new RedisDictionary { { "key1", "value1" }, { "key2", "value2" } });
                Assert.True(redis.Expire("key1", TimeSpan.FromSeconds(10)));
                Assert.True(redis.Expire("key2", 20));
                Assert.True(redis.Ttl("key1") <= 10);
                Assert.True(redis.Ttl("key2") <= 20);
            });
        }

        [Fact()]
        public void TestExpireAt()
        {
            using(var mock = new MockConnector("localhost", 9999, ":1\r\n", ":0\r\n"))
            using(var redis = new RedisClient(mock))
            {
                // 1402643208 = ISO 8601:2014-06-13T07:06:48Z
                Assert.True(redis.ExpireAt("test1", new DateTime(2014, 6, 13, 7, 6, 48, DateTimeKind.Utc)));
                Assert.Equal("*3\r\n$8\r\nEXPIREAT\r\n$5\r\ntest1\r\n$10\r\n1402643208\r\n", mock.GetMessage());
                Assert.False(redis.ExpireAt("test2", 1402643208));
                Assert.Equal("*3\r\n$8\r\nEXPIREAT\r\n$5\r\ntest2\r\n$10\r\n1402643208\r\n", mock.GetMessage());
            }

            this.RealCall(redis =>
            {
                redis.MSet(new RedisDictionary { { "key1", "value1" } });
                var now = DateTime.Now;
                Assert.True(redis.ExpireAt("key1", now.AddSeconds(10)));
                var ttl = redis.Ttl("key1");
                Assert.True(ttl <= 10);
            });
        }


        [Fact()]
        public void TestKeys()
        {
            using(var mock = new MockConnector("localhost", 9999, "*3\r\n$5\r\ntest1\r\n$5\r\ntest2\r\n$5\r\ntest3\r\n"))
            using(var redis = new RedisClient(mock))
            {
                var response = redis.Keys("test*");
                Assert.Equal(3, response.Length);
                for(int i = 0; i < response.Length; i++)
                    Assert.Equal("test" + (i + 1), response[i]);
            }

            this.RealCall(redis =>
            {
                redis.MSet(new RedisDictionary { { "key1", "value1" }, { "key2", "value2" }, { "test1", "value2" } });
                var response = redis.Keys("key*");
                Assert.Equal(2, response.Length);
            });
        }


        [Fact()]
        public void TestMove()
        {
            using(var mock = new MockConnector("localhost", 9999, ":1\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.True(redis.Move("test", 5));
                Assert.Equal("*3\r\n$4\r\nMOVE\r\n$4\r\ntest\r\n$1\r\n5\r\n", mock.GetMessage());
            }

            this.RealCall(redis =>
            {
                redis.MSet(new RedisDictionary { { "key1", "value1" } });
                Assert.True(redis.Move("key1", 5));
            });
        }

        [Fact()]
        public void Persist()
        {
            using(var mock = new MockConnector("localhost", 9999, ":1\r\n", ":0\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.True(redis.Persist("test1"));
                Assert.Equal("*2\r\n$7\r\nPERSIST\r\n$5\r\ntest1\r\n", mock.GetMessage());
                Assert.False(redis.Persist("test2"));
                Assert.Equal("*2\r\n$7\r\nPERSIST\r\n$5\r\ntest2\r\n", mock.GetMessage());
            }

            this.RealCall(redis =>
            {
                redis.MSet(new RedisDictionary { { "key1", "value1" }, { "key2", "value2" } });
                Assert.True(redis.Expire("key1", TimeSpan.FromSeconds(10)));
                Assert.True(redis.Expire("key2", 20));
                Assert.True(redis.Ttl("key1") <= 10);
                Assert.True(redis.Ttl("key2") <= 20);

                Assert.True(redis.Persist("key1"));
                Assert.True(redis.Persist("key2"));
                Assert.Equal(-1, redis.Ttl("key1"));
                Assert.Equal(-1, redis.Ttl("key2"));
            });
        }

        [Fact()]
        public void PExpire()
        {
            using(var mock = new MockConnector("localhost", 9999, ":1\r\n", ":0\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.True(redis.Expire("test1", TimeSpan.FromMilliseconds(5000), RedisExpireTimeUnit.PX));
                Assert.Equal("*3\r\n$7\r\nPEXPIRE\r\n$5\r\ntest1\r\n$4\r\n5000\r\n", mock.GetMessage());
                Assert.False(redis.Expire("test2", 6000, RedisExpireTimeUnit.PX));
                Assert.Equal("*3\r\n$7\r\nPEXPIRE\r\n$5\r\ntest2\r\n$4\r\n6000\r\n", mock.GetMessage());
            }

            this.RealCall(redis =>
            {
                redis.MSet(new RedisDictionary { { "key1", "value1" }, { "key2", "value2" } });
                Assert.True(redis.Expire("key1", TimeSpan.FromSeconds(10), RedisExpireTimeUnit.PX));
                Assert.True(redis.Expire("key2", 20 * 1000, RedisExpireTimeUnit.PX));
                Assert.True(redis.Ttl("key1") <= 10 * 1000);
                Assert.True(redis.Ttl("key2") <= 20 * 1000);
            });
        }

        [Fact()]
        public void PExpireAt()
        {
            using(var mock = new MockConnector("localhost", 9999, ":1\r\n", ":0\r\n"))
            using(var redis = new RedisClient(mock))
            {
                // 1402643208123 = ISO 8601:2014-06-13T07:06:48Z +123ms

                Assert.True(redis.ExpireAt("test1", new DateTime(2014, 6, 13, 7, 6, 48, 123, DateTimeKind.Utc), RedisExpireTimeUnit.PX));
                Assert.Equal("*3\r\n$9\r\nPEXPIREAT\r\n$5\r\ntest1\r\n$13\r\n1402643208123\r\n", mock.GetMessage());
                Assert.False(redis.ExpireAt("test2", 1402643208123, RedisExpireTimeUnit.PX));
                Assert.Equal("*3\r\n$9\r\nPEXPIREAT\r\n$5\r\ntest2\r\n$13\r\n1402643208123\r\n", mock.GetMessage());
            }

            this.RealCall(redis =>
            {
                redis.MSet(new RedisDictionary { { "key1", "value1" } });
                Assert.True(redis.ExpireAt("key1", DateTime.Now.AddSeconds(10), RedisExpireTimeUnit.PX));
                Assert.True(redis.Ttl("key1") <= 10 * 1000);
            });
        }

        [Fact()]
        public void TestPttl()
        {
            using(var mock = new MockConnector("localhost", 9999, ":123\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.Equal(123, redis.Ttl("test", RedisExpireTimeUnit.PX));
                Assert.Equal("*2\r\n$4\r\nPTTL\r\n$4\r\ntest\r\n", mock.GetMessage());
            }

            this.RealCall(redis =>
            {
                redis.MSet(new RedisDictionary { { "key1", "value1" }, { "key2", "value2" } });
                Assert.True(redis.Expire("key1", TimeSpan.FromSeconds(10)));
                Assert.True(redis.Expire("key2", 20));
                Assert.True(redis.Ttl("key1", RedisExpireTimeUnit.PX) <= 10 * 1000);
                Assert.True(redis.Ttl("key2", RedisExpireTimeUnit.PX) <= 20 * 1000);
            });
        }

        [Fact()]
        public void TestRandomKey()
        {
            using(var mock = new MockConnector("localhost", 9999, "$7\r\nsomekey\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.Equal("somekey", redis.RandomKey());
                Assert.Equal("*1\r\n$9\r\nRANDOMKEY\r\n", mock.GetMessage());
            }

            this.RealCall(redis =>
            {
                redis.MSet(new RedisDictionary { { "key1", "value1" }, { "key2", "value2" } });
                var key = redis.RandomKey();
                Assert.True(key == "key1" || key == "key2");
            });
        }

        [Fact()]
        public void TestRename()
        {
            using(var mock = new MockConnector("localhost", 9999, "+OK\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.True(redis.Rename("test1", "test2"));
                Assert.Equal("*3\r\n$6\r\nRENAME\r\n$5\r\ntest1\r\n$5\r\ntest2\r\n", mock.GetMessage());
            }
            this.RealCall(redis =>
            {
                redis.MSet(new RedisDictionary { { "key1", "value1" }, { "key2", "value2" } });
                redis.Rename("key1", "key2").ThrowIfFailded();
                Assert.Equal("value1", (string)redis.Get("key2"));
                Assert.True(redis.Rename("key3", "key4").IsFailed);
            });
        }

        [Fact()]
        public void TestRenameNx()
        {
            using(var mock = new MockConnector("localhost", 9999, ":1\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.True(redis.RenameNx("test1", "test2"));
                Assert.Equal("*3\r\n$8\r\nRENAMENX\r\n$5\r\ntest1\r\n$5\r\ntest2\r\n", mock.GetMessage());
            }

            this.RealCall(redis =>
            {
                redis.MSet(new RedisDictionary { { "key1", "value1" }, { "key2", "value2" } });
                Assert.False(redis.RenameNx("key1", "key2"));
                Assert.False(redis.RenameNx("key3", "key4"));
                Assert.True(redis.RenameNx("key1", "key3"));
                Assert.Equal("value1", (string)redis.Get("key3"));
            });
        }
        /*
        [Fact()]
        public void TestSort()
        {
            using (var mock = new MockConnector("localhost", 9999, "*2\r\n$2\r\nab\r\n$2\r\ncd\r\n", "*0\r\n", "*0\r\n", "*0\r\n", "*0\r\n", "*0\r\n", "*0\r\n", "*0\r\n"))
            using (var redis = new RedisClient(mock))
            {
                var resp1 = redis.Sort("test1");
                Assert.Equal(2, resp1.Length);
                Assert.Equal("ab", resp1[0]);
                Assert.Equal("cd", resp1[1]);
                Assert.Equal("*2\r\n$4\r\nSORT\r\n$5\r\ntest1\r\n", mock.GetMessage());

                var resp2 = redis.Sort("test2", offset: 0, count: 2);
                Assert.Equal("*5\r\n$4\r\nSORT\r\n$5\r\ntest2\r\n$5\r\nLIMIT\r\n$1\r\n0\r\n$1\r\n2\r\n", mock.GetMessage());

                var resp3 = redis.Sort("test3", by: "xyz");
                Assert.Equal("*4\r\n$4\r\nSORT\r\n$5\r\ntest3\r\n$2\r\nBY\r\n$3\r\nxyz\r\n", mock.GetMessage());

                var resp4 = redis.Sort("test4", dir: RedisSortDir.Asc);
                Assert.Equal("*3\r\n$4\r\nSORT\r\n$5\r\ntest4\r\n$3\r\nASC\r\n", mock.GetMessage());

                var resp5 = redis.Sort("test5", dir: RedisSortDir.Desc);
                Assert.Equal("*3\r\n$4\r\nSORT\r\n$5\r\ntest5\r\n$4\r\nDESC\r\n", mock.GetMessage());

                var resp6 = redis.Sort("test6", isAlpha: true);
                Assert.Equal("*3\r\n$4\r\nSORT\r\n$5\r\ntest6\r\n$5\r\nALPHA\r\n", mock.GetMessage());

                var resp7 = redis.Sort("test7", get: new[] { "get1", "get2" });
                Assert.Equal("*6\r\n$4\r\nSORT\r\n$5\r\ntest7\r\n$3\r\nGET\r\n$4\r\nget1\r\n$3\r\nGET\r\n$4\r\nget2\r\n", mock.GetMessage());

                var resp8 = redis.Sort("test8", offset: 0, count: 2, by: "xyz", dir: RedisSortDir.Asc, isAlpha: true, get: new[] { "a", "b" });
                Assert.Equal("*13\r\n$4\r\nSORT\r\n$5\r\ntest8\r\n$2\r\nBY\r\n$3\r\nxyz\r\n$5\r\nLIMIT\r\n$1\r\n0\r\n$1\r\n2\r\n$3\r\nGET\r\n$1\r\na\r\n$3\r\nGET\r\n$1\r\nb\r\n$3\r\nASC\r\n$5\r\nALPHA\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void TestSortAndStore()
        {
            using (var mock = new MockConnector("localhost", 9999, ":1\r\n", ":1\r\n", ":1\r\n", ":1\r\n", ":1\r\n", ":1\r\n", ":1\r\n", ":1\r\n"))
            using (var redis = new RedisClient(mock))
            {
                Assert.Equal(1, redis.SortAndStore("test1", "test2"));
                Assert.Equal("*4\r\n$4\r\nSORT\r\n$5\r\ntest1\r\n$5\r\nSTORE\r\n$5\r\ntest2\r\n", mock.GetMessage());

                Assert.Equal(1, redis.SortAndStore("test2", "test3", offset:0, count:2));
                Assert.Equal("*7\r\n$4\r\nSORT\r\n$5\r\ntest2\r\n$5\r\nLIMIT\r\n$1\r\n0\r\n$1\r\n2\r\n$5\r\nSTORE\r\n$5\r\ntest3\r\n", mock.GetMessage());

                Assert.Equal(1, redis.SortAndStore("test3", "test4", by: "xyz"));
                Assert.Equal("*6\r\n$4\r\nSORT\r\n$5\r\ntest3\r\n$2\r\nBY\r\n$3\r\nxyz\r\n$5\r\nSTORE\r\n$5\r\ntest4\r\n", mock.GetMessage());

                Assert.Equal(1, redis.SortAndStore("test5", "test6", dir: RedisSortDir.Asc));
                Assert.Equal("*5\r\n$4\r\nSORT\r\n$5\r\ntest5\r\n$3\r\nASC\r\n$5\r\nSTORE\r\n$5\r\ntest6\r\n", mock.GetMessage());

                Assert.Equal(1, redis.SortAndStore("test7", "test8", dir: RedisSortDir.Desc));
                Assert.Equal("*5\r\n$4\r\nSORT\r\n$5\r\ntest7\r\n$4\r\nDESC\r\n$5\r\nSTORE\r\n$5\r\ntest8\r\n", mock.GetMessage());

                Assert.Equal(1, redis.SortAndStore("test9", "test10", isAlpha: true));
                Assert.Equal("*5\r\n$4\r\nSORT\r\n$5\r\ntest9\r\n$5\r\nALPHA\r\n$5\r\nSTORE\r\n$6\r\ntest10\r\n", mock.GetMessage());

                Assert.Equal(1, redis.SortAndStore("test11", "test12", get: new[] { "get1", "get2" }));
                Assert.Equal("*8\r\n$4\r\nSORT\r\n$6\r\ntest11\r\n$3\r\nGET\r\n$4\r\nget1\r\n$3\r\nGET\r\n$4\r\nget2\r\n$5\r\nSTORE\r\n$6\r\ntest12\r\n", mock.GetMessage());

                Assert.Equal(1, redis.SortAndStore("test13", "test14", offset: 0, count: 2, by: "xyz", dir: RedisSortDir.Asc, isAlpha: true, get: new[] { "a", "b" }));
                Assert.Equal("*15\r\n$4\r\nSORT\r\n$6\r\ntest13\r\n$2\r\nBY\r\n$3\r\nxyz\r\n$5\r\nLIMIT\r\n$1\r\n0\r\n$1\r\n2\r\n$3\r\nGET\r\n$1\r\na\r\n$3\r\nGET\r\n$1\r\nb\r\n$3\r\nASC\r\n$5\r\nALPHA\r\n$5\r\nSTORE\r\n$6\r\ntest14\r\n", mock.GetMessage());
            }
        }
        */
        [Fact()]
        public void TestTtl()
        {
            using(var mock = new MockConnector("localhost", 9999, ":123\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.Equal(123, redis.Ttl("test"));
                Assert.Equal("*2\r\n$3\r\nTTL\r\n$4\r\ntest\r\n", mock.GetMessage());
            }

            this.RealCall(redis =>
            {
                redis.MSet(new RedisDictionary { { "key1", "value1" }, { "key2", "value2" } });
                Assert.True(redis.Expire("key1", TimeSpan.FromSeconds(10)));
                Assert.True(redis.Expire("key2", 20));
                Assert.True(redis.Ttl("key1", RedisExpireTimeUnit.PX) <= 10 * 1000);
                Assert.True(redis.Ttl("key2", RedisExpireTimeUnit.PX) <= 20 * 1000);
            });
        }

        [Fact()]
        public void TestType()
        {
            using(var mock = new MockConnector("localhost", 9999, "+string\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.Equal(RedisType.String, redis.Type("test"));
                Assert.Equal("*2\r\n$4\r\nTYPE\r\n$4\r\ntest\r\n", mock.GetMessage());
            }

            this.RealCall(redis =>
            {
                redis.MSet(new RedisDictionary { { "key1", "value1" }, { "key2", "value2" } });

                Assert.Equal(RedisType.String, redis.Type("key1"));
                Assert.Equal(RedisType.String, redis.Type("key2"));
            });
        }

        [Fact()]
        public void TestScan()
        {
            var reply1 = "*2\r\n$1\r\n0\r\n*3\r\n$5\r\ntest1\r\n$5\r\ntest2\r\n$5\r\ntest3\r\n";
            var reply2 = "*2\r\n$1\r\n0\r\n*0\r\n";
            var reply3 = "*2\r\n$1\r\n0\r\n*0\r\n";
            var reply4 = "*2\r\n$1\r\n0\r\n*0\r\n";
            using(var mock = new MockConnector("localhost", 9999, reply1, reply2, reply3, reply4))
            using(var redis = new RedisClient(mock))
            {
                var resp = (redis.Scan(0) as RedisScan<string>).GetEnumerator() as RedisScan<string>.Enumerator;
                for(int i = 0; i < 3; i++)
                {
                    resp.MoveNext();
                    Assert.Equal(3 - i - 1, resp.Items.Length);
                    Assert.Equal(0, resp.Cursor);
                    Assert.Equal("test" + (i + 1), resp.Current);
                }
                Assert.Equal("*2\r\n$4\r\nSCAN\r\n$1\r\n0\r\n", mock.GetMessage());

                redis.Scan(1, pattern: "pattern").ToArray();
                Assert.Equal("*4\r\n$4\r\nSCAN\r\n$1\r\n1\r\n$5\r\nMATCH\r\n$7\r\npattern\r\n", mock.GetMessage());

                redis.Scan(2, count: 5).ToArray();
                Assert.Equal("*4\r\n$4\r\nSCAN\r\n$1\r\n2\r\n$5\r\nCOUNT\r\n$1\r\n5\r\n", mock.GetMessage());

                redis.Scan(3, pattern: "pattern", count: 5).ToArray();
                Assert.Equal("*6\r\n$4\r\nSCAN\r\n$1\r\n3\r\n$5\r\nMATCH\r\n$7\r\npattern\r\n$5\r\nCOUNT\r\n$1\r\n5\r\n", mock.GetMessage());
            }


            this.RealCall(redis =>
            {
                var dict = new RedisDictionary();
                for(int i = 1; i < 21; i++)
                {
                    dict.Add("key" + i, "value" + i);
                }
                redis.MSet(dict);
                Assert.Equal(20, redis.Scan().Count());
            });
        }
    }
}
