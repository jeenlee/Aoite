using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Xunit;

namespace Aoite.Serialization.BinarySuite
{
    public class BasisFormattersTests : FormattersBase
    {
        static byte[] WriteTest(Serializer ser, object value)
        {
            return ser.FastWriteBytes(value);
        }
        static T ReadTest<T>(Serializer ser, byte[] value)
        {
            return ser.FastReadBytes<T>(value);
        }
        static void SerializerTest<T>(T rb)
        {
            Console.WriteLine("开始测试 {0}：", rb);
            const int LoopCount = 1 * 10000;
            Console.WriteLine("===>写测试：");
            Stopwatch watch = new Stopwatch();
            for(int i = 0; i < 3; i++)
            {
                Console.WriteLine("--->第 {0} 次：", i + 1);

                watch.Restart();
                for(int x = 0; x < LoopCount; x++) WriteTest(new QuicklySerializer(), rb);
                watch.Stop();

                Console.WriteLine("Fast Binary Write : {0}", watch.Elapsed);

                watch.Restart();
                for(int x = 0; x < LoopCount; x++) WriteTest(new QuicklySerializer(), rb);
                watch.Stop();
                Console.WriteLine("Quick Binary Write : {0}", watch.Elapsed);
            }
            Console.WriteLine("===>读测试：");
            var fbBytes = WriteTest(new QuicklySerializer(), rb);
            var hBytes = WriteTest(new QuicklySerializer(), rb);
            Console.WriteLine("Fast bytes length：{0}", fbBytes.Length);
            Console.WriteLine("Quick bytes length：{0}", hBytes.Length);
            for(int i = 0; i < 3; i++)
            {
                Console.WriteLine("--->第 {0} 次：", i + 1);

                watch.Restart();
                for(int x = 0; x < LoopCount; x++) ReadTest<T>(new QuicklySerializer(), fbBytes);
                watch.Stop();
                Console.WriteLine("Fast Binary  Read : {0}", watch.Elapsed);

                watch.Restart();
                for(int x = 0; x < LoopCount; x++) ReadTest<T>(new QuicklySerializer(), hBytes);
                watch.Stop();
                Console.WriteLine("Quick Binary  Read : {0}", watch.Elapsed);
            }
        }

        [Fact]
        public void Test()
        {
            //SerializerTest(HelloWorldApp.BusinessObjects.RootContainer.CreateFakeRoot());
            SerializerTest(new SerializerTests.TestModel() { Id = "1", Name = "aa" });
        }

        [Fact]
        public void ObjectArrayTest()
        {
            var expected = new object[] { "A", 1, 1D, 1L, new SerializerTests.TestModel() { Id = "1", Name = "aa" } };


            using(var writer = CreateWriter())
            {
                writer.Serialize(expected);
                writer.Stream.Position = 0;
                var reader = this.CreateReader(writer.Stream);
                var actual = reader.Deserialize() as object[];
                Assert.Equal(expected.Length, actual.Length);
                for(int i = 0; i < expected.Length - 1; i++)
                {
                    Assert.Equal(expected[i], actual[i]);
                }
                var m1 = expected[expected.Length - 1] as SerializerTests.TestModel;
                var m2 = actual[actual.Length - 1] as SerializerTests.TestModel;
                Assert.Equal(m1.Id, m2.Id);
                Assert.Equal(m1.Name, m2.Name);
            }
        }

        [Fact]
        public void SimpleTest()
        {
            this.AssertObjct(null);
            this.AssertObjct(DBNull.Value);
            this.AssertObjct(Result.Successfully);
        }

        [Fact]
        public void HybridDictionaryTest()
        {
            using(var writer = CreateWriter())
            {
                HybridDictionary expected = new HybridDictionary();
                for(int i = 0; i < 100; i++)
                {
                    expected.Add("a" + i, i);
                }
                writer.Serialize(expected);
                writer.Stream.Position = 0;
                var reader = this.CreateReader(writer.Stream);
                var actual = reader.Deserialize() as HybridDictionary;

                Assert.Equal(expected.Count, actual.Count);
                for(int i = 0; i < 100; i++)
                {
                    Assert.Equal(expected["a" + i], actual["a" + i]);
                }
            }
        }
        [Fact]
        public void HybridDictionaryTest2()
        {
            using(var writer = CreateWriter())
            {
                HybridDictionary expected = new HybridDictionary(true);
                for(int i = 0; i < 100; i++)
                {
                    expected.Add("a" + i, i);
                }
                writer.Serialize(expected);
                writer.Stream.Position = 0;
                var reader = this.CreateReader(writer.Stream);
                var actual = reader.Deserialize() as HybridDictionary;

                Assert.Equal(expected.Count, actual.Count);
                for(int i = 0; i < 100; i++)
                {
                    Assert.Equal(expected["A" + i], actual["A" + i]);
                }
            }
        }
        [Fact]
        public void TypeTest()
        {
            this.AssertObjct(Types.String);
            this.AssertObjct(Types.Int32);
            this.AssertObjct(Types.DataTable);
            this.AssertObjct(Types.IGEnumerable);

            this.AssertObjct(new Type[] { Types.IGEnumerable });
            this.AssertObjct(new Type[] { Types.String, Types.Int32, Types.String, Types.Int64, Types.Int16 });
            this.AssertObjct(new Type[] { Types.String, Types.IGEnumerable, Types.String, Types.Int64, Types.Int16 });
            this.AssertObjct(new Type[] { Types.String, Types.Int32, Types.String, Types.IGEnumerable, Types.Int16 });
        }

        [Fact]
        public void GuidTest()
        {
            this.AssertObjct(Guid.NewGuid());
            this.AssertObjct(Guid.Empty);
            this.AssertObjct(Guid.NewGuid());

            this.AssertObjct(new Guid[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() });
            this.AssertObjct(new Guid[] { });
            this.AssertObjct(new Guid[] { Guid.NewGuid() });
        }

        [Fact]
        public void BooleanTest()
        {
            this.AssertObjct(true);
            this.AssertObjct(false);

            this.AssertObjct(new Boolean[] { true, true, false });
            this.AssertObjct(new Boolean[] { });
            this.AssertObjct(new Boolean[] { false, false, false });
        }

        [Fact]
        public void DateTimeTest()
        {
            this.AssertObjct(DateTime.Now);
            this.AssertObjct(DateTime.UtcNow);

            this.AssertObjct(new DateTime[] { DateTime.Now, DateTime.Now.AddYears(-1), DateTime.Now.AddMinutes(1) });
            this.AssertObjct(new DateTime[] { });
            this.AssertObjct(new DateTime[] { DateTime.MinValue, DateTime.MaxValue });
        }

        [Fact]
        public void TimeSpanTest()
        {
            this.AssertObjct(DateTime.Now.AddYears(1) - DateTime.Now);
            this.AssertObjct(TimeSpan.MaxValue);
            this.AssertObjct(TimeSpan.MinValue);

            this.AssertObjct(new TimeSpan[] { TimeSpan.MaxValue, DateTime.Now - DateTime.Now.AddYears(-1), DateTime.Now.AddMinutes(1) - DateTime.Now });
            this.AssertObjct(new TimeSpan[] { });
            this.AssertObjct(new TimeSpan[] { TimeSpan.MinValue, TimeSpan.MaxValue });
        }

    }
}
