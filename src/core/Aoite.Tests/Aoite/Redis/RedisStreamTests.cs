using Aoite.Redis.Commands;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace Aoite.Redis
{
    class RedisStreamTests
    {
        private IRedisClient CreateClient(bool flushAll = true)
        {
            var client = new RedisClient(6379);
            if(flushAll)
                client.FlushAll();
            return client;
        }
        private void SetKey(IRedisClient client, int count)
        {
            for(int i = 1; i <= count; i++)
            {
                client.Set("key" + i, count);
            }
        }

        [Fact()]
        public void MyTestMethod()
        {
            var client = this.CreateClient();
            client.Set("a", "b");
            Assert.Equal("b", (string)client.Get("a"));
            var value1 = new BinaryValue(TestModels.RootContainer.CreateFakeRoot());
            client.Set("x", value1);
            var value2 = client.Get("x");
            Assert.Equal(value1.ByteArray, value2.ByteArray);
            Assert.IsType<TestModels.RootContainer>(value2.ToModel());
        }
        [Fact()]
        public void MyTestMethod2()
        {
            var client = this.CreateClient();
            var s = client.Keys("*");
            Assert.Equal(new[] { "key:1", "key:2", "key:3", "key:4", "key:5" }, s.OrderBy(x => x).ToArray());
        }
        [Fact()]
        public void MyTestMethod4()
        {
            var client = this.CreateClient();
            SetKey(client, 20);
            var s = client.Scan().ToArray();
            Assert.Equal(20, s.Count());
            //Assert.Equal(new[] { "key:1", "key:2", "key:3", "key:4", "key:5" }, s.OrderBy(x => x).ToArray());
        }

        [Fact()]
        public void MyTestMethod3()
        {
            using(var mock = new MockConnector("localhost", 9999, "+OK\r\n", "+OK\r\n"))
            using(var redis = new RedisClient(mock))
            {
                Assert.True(redis.Set("key", "value"));
                Assert.Equal("*3\r\n$3\r\nSET\r\n$3\r\nkey\r\n$5\r\nvalue\r\n", mock.GetMessage());

                Assert.True(redis.Set("key", "value"));
                Assert.Equal("*3\r\n$3\r\nSET\r\n$3\r\nkey\r\n$5\r\nvalue\r\n", mock.GetMessage());
            }
        }

        [Fact()]
        public void TypeTest()
        {
            using(var client = this.CreateClient())
            {
                client.Set("a", new BinaryValue(null));
                var value = client.Get("a");
                Assert.Null(value.ToModel());
                Assert.Null((string)value);
                //client.Set("b", "6x");
                //Assert.Equal(RedisType.String, client.Type("a"));
                //Assert.Equal(RedisType.String, client.Type("b"));
            }
        }

        [Fact()]
        public void BRPopLPushTest()
        {
            using(var client = this.CreateClient(false))
            {
                var t = client.BRPopLPush("msg", "reply", 1);
                Assert.Equal("1", (string)t);
                t = client.BRPopLPush("msg", "reply", 1);
                Assert.Equal("2", (string)t);

                t = client.BRPopLPush("msg", "reply", 1);
                Assert.Null(t);
            }
        }
    }
}
