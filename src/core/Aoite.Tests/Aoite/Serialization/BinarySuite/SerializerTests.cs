using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using TestModels;
using Xunit;

namespace Aoite.Serialization.BinarySuite
{
    public class SerializerTests
    {
        class TestClass1
        {
            public TestClass2 TestClass2Property { get; set; }
            public int MyProperty { get; set; }
        }

        class TestClass2
        {
            public TestClass1 TestClass1Property { get; set; }
            public int IntProperty { get; set; }
            public Guid GuidProperty { get; set; }
            public DateTime DateTimeProperty { get; set; }
            public StringBuilder StringBuilderProperty { get; set; }
        }
        [Fact]
        public void TestMethod1()
        {
            TestClass1 tc1 = new TestClass1() { MyProperty = 564 };
            tc1.TestClass2Property = new TestClass2()
            {
                TestClass1Property = tc1,
                IntProperty = 5,
                GuidProperty = Guid.NewGuid(),
                DateTimeProperty = DateTime.Now,
                StringBuilderProperty = new StringBuilder("adfdsfas")
            };


            QuicklySerializer ser = new QuicklySerializer();
            var bytes = ser.FastWriteBytes(tc1);
            var tc2 = ser.FastReadBytes<TestClass1>(bytes);
            Assert.Equal(tc1.MyProperty, tc2.MyProperty);
            Assert.NotNull(tc2.TestClass2Property);
            Assert.Equal(tc1.TestClass2Property.TestClass1Property, tc1);
            Assert.Equal(tc1.TestClass2Property.IntProperty, tc2.TestClass2Property.IntProperty);
            Assert.Equal(tc1.TestClass2Property.GuidProperty, tc2.TestClass2Property.GuidProperty);
            Assert.Equal(tc1.TestClass2Property.DateTimeProperty, tc2.TestClass2Property.DateTimeProperty);
            Assert.Equal(tc1.TestClass2Property.StringBuilderProperty.ToString(), tc2.TestClass2Property.StringBuilderProperty.ToString());

        }


        [Fact]
        public void ProtectedTest()
        {
            QuicklySerializer ser = new QuicklySerializer();
            var bytes = ser.FastWriteBytes(Result.Successfully);
            var result = ser.FastReadBytes(bytes) as Result;
            Assert.NotNull(result);
        }
        public class TestModel
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }

        [Fact]
        public void ResultValueTest()
        {
            QuicklySerializer ser = new QuicklySerializer();
            var b = new Result<TestModel>();
            b.Value = new TestModel() { Id = "1", Name = "2" };
            var bytes = ser.FastWriteBytes(b);

            var a = ser.FastReadBytes<Result<TestModel>>(bytes);
            Assert.NotNull(a);
            Assert.True(a.IsSucceed);
            Assert.NotNull(a.Value);
            Assert.Equal(b.Value.Id, a.Value.Id);
            Assert.Equal(b.Value.Name, a.Value.Name);
        }


        public class RemotingResult
        {
            private object _Value;
            /// <summary>
            /// 获取或设置一个值，表示远程操作的返回值。
            /// </summary>
            public object Value
            {
                get
                {
                    return this._Value;
                }
                set
                {
                    this._Value = value;
                }
            }

            public RemotingResult() { }
        }
        [Fact]
        public void RemotingResultTest()
        {
            QuicklySerializer ser = new QuicklySerializer();
            var b = new Result<TestModel>() { Value = new TestModel() { Id = "1", Name = "2" } };
            RemotingResult rb = new RemotingResult();
            rb.Value = new object[2] { b, null };
            var bytes = ser.FastWriteBytes(rb);
            var ra = ser.FastReadBytes<RemotingResult>(bytes);
            Assert.NotNull(ra.Value);
            Assert.True(ra.Value is object[]);

            var a = (ra.Value as object[])[0] as Result<TestModel>;
            Assert.NotNull(ra);
            Assert.True(a.IsSucceed);
            Assert.NotNull(a.Value);
            Assert.Equal(b.Value.Id, a.Value.Id);
            Assert.Equal(b.Value.Name, a.Value.Name);
        }
        [Fact]
        public void TimeSpanTest()
        {
            QuicklySerializer ser = new QuicklySerializer();
            TimeSpan b = new TimeSpan(1, 2, 3, 4);
            var bytes = ser.FastWriteBytes(b);
            var a = ser.FastReadBytes<TimeSpan>(bytes);

            Assert.Equal(b, a);
        }
        [Fact]
        public void TimeSpanArrayTest()
        {
            QuicklySerializer ser = new QuicklySerializer();
            TimeSpan[] b = new TimeSpan[] { new TimeSpan(1, 2, 3, 4), new TimeSpan(1, 2, 3), new TimeSpan(125L), };
            var bytes = ser.FastWriteBytes(b);
            var a = ser.FastReadBytes<TimeSpan[]>(bytes);
            Assert.NotNull(a);
            Assert.Equal(b.Length, a.Length);
            for(int i = 0; i < b.Length; i++)
            {
                Assert.Equal(b[i], a[i]);
            }
        }

        public struct Point
        {
            public int X;
            public int Y;
        }

        [Fact]
        public void StructSTest()
        {
            QuicklySerializer ser = new QuicklySerializer();
            var point1 = new Point { X = 5, Y = 7 };
            var bytes = ser.FastWriteBytes(point1);
            var point2 = ser.FastReadBytes<Point>(bytes);
            Assert.Equal(point1.X, point2.X);
            Assert.Equal(point1.Y, point2.Y);
        }
        public class EntityModelBase
        {
            public int hashCode;
            public int key;
        }
        public class EntryModel : EntityModelBase
        {
            public int next;
        }
        [Fact]
        public void StructSTest2()
        {
            QuicklySerializer ser = new QuicklySerializer();
            var m1 = new EntryModel { hashCode = 44, key = 44, next = -1 };
            var bytes = ser.FastWriteBytes(m1);
            var m2 = ser.FastReadBytes<EntryModel>(bytes);
            Assert.Equal(m1.hashCode, m2.hashCode);
            Assert.Equal(m1.key, m2.key);
            Assert.Equal(m1.next, m2.next);
        }


        public class TestCl
        {
            public char[] SingleArrayOfChars { get; set; }
            public static TestCl Create()
            {
                return new TestCl()
                {

                    SingleArrayOfChars = new char[] { 'o', '\t', '\n', (char)0 }
                };
            }
        }

        [Fact]
        public void CharArrayTest()
        {
            QuicklySerializer ser = new QuicklySerializer();
            var m1 = TestCl.Create();
            var bytes = ser.FastWriteBytes(m1);
            var m2 = ser.FastReadBytes<TestCl>(bytes);
            for(int i = 0; i < m1.SingleArrayOfChars.Length; i++)
            {
                Assert.Equal(m1.SingleArrayOfChars[i], m2.SingleArrayOfChars[i]);
            }
        }

        [Fact]
        public void AdvTest()
        {
            ObjectComparer(RootContainer.CreateFakeRoot());
        }

        private T ObjectComparer<T>(T m1)
        {
            QuicklySerializer ser = new QuicklySerializer();
            var m1_bytes = ser.FastWriteBytes(m1);
            var m2 = ser.FastReadBytes<T>(m1_bytes);
            var m2_bytes = ser.FastWriteBytes(m2);
            Assert.Equal(m1_bytes.Length, m2_bytes.Length);
            for(int i = 0; i < m1_bytes.Length; i++)
            {
                Assert.Equal(m1_bytes[i], m2_bytes[i]);
            }
            return m2;
        }

        [Fact]
        public void GListTest()
        {
            var m1 = new List<string>() { "a", "b", "c" };
            ObjectComparer(m1);
        }
        [Fact]
        public void GCollectionTest()
        {
            var m1 = new System.Collections.ObjectModel.Collection<string>() { "a", "b", "c" };
            ObjectComparer(m1);
        }

        [Fact]
        public void GDictionaryTest()
        {
            var m1 = new Dictionary<string, int>() { { "A", 1 }, { "b", 2 } };
            var m2 = ObjectComparer(m1);
        }
        [Fact]
        public void GDictionaryTest2()
        {

            //var m2 = ObjectComparer(m1);
        }


        public class MultiDictionaryModel
        {
            public Dictionary<string, int> MyProperty1 { get; set; }
            public Dictionary<string, int> MyProperty2 { get; set; }
        }

        [Fact]
        public void GDictionaryTest3()
        {
            var dict = new Dictionary<string, int>() { { "A", 1 }, { "b", 2 } };
            MultiDictionaryModel m1 = new MultiDictionaryModel();
            m1.MyProperty1 = dict;
            m1.MyProperty2 = dict;
            var m2 = ObjectComparer(m1);
            Assert.Equal(m2.MyProperty1, m2.MyProperty2);
        }
        [Fact]
        public void GDictionaryTest4()
        {
            var dict1 = new Dictionary<string, int>() { { "A", 1 }, { "b", 2 } };
            var dict2 = new Dictionary<string, int>() { { "A", 1 }, { "b", 2 } };
            MultiDictionaryModel m1 = new MultiDictionaryModel();
            m1.MyProperty1 = dict1;
            m1.MyProperty2 = dict2;
            var m2 = ObjectComparer(m1);
            Assert.False(m2.MyProperty1 == m2.MyProperty2);
            Assert.Equal(m2.MyProperty1, m2.MyProperty2);
        }
        //[Fact()]
        //public void GDictionaryTest2()
        //{
        //    var m1 = new Dictionary<string, int>(StringComparer.CurrentCultureIgnoreCase) { { "A", 1 }, { "b", 2 } };
        //    var m2 = ObjectComparer(m1);

        //    // Assert.Equal(m2.Comparer, EqualityComparer<string>.Default);
        //    Assert.Equal(m1.Comparer.GetType().FullName, m2.Comparer.GetType().FullName);
        //}

        public class ListBase1 : List<string>
        {
            public int P1 { get; set; }
        }
        public class ListBase2 : ListBase1
        {
            public int P2 { get; set; }
        }

        [Fact]
        public void CustomGListTest1()
        {
            ListBase1 m1 = new ListBase1() { P1 = 5 };
            m1.Add("A");
            m1.Add("B");
            var m2 = ObjectComparer(m1);
            Assert.Equal(m1.P1, m2.P1);
            Assert.Equal(m1.Count, m2.Count);
            for(int i = 0; i < m1.Count; i++)
            {
                Assert.Equal(m1[i], m2[i]);
            }
        }
        [Fact]
        public void CustomGListTest2()
        {
            ListBase2 m1 = new ListBase2() { P1 = 5, P2 = 55 };
            m1.Add("A");
            m1.Add("B");
            var m2 = ObjectComparer(m1);
            Assert.Equal(m1.P1, m2.P1);
            Assert.Equal(m1.Count, m2.Count);
            for(int i = 0; i < m1.Count; i++)
            {
                Assert.Equal(m1[i], m2[i]);
            }
        }

        public class CollectionBase1 : System.Collections.ObjectModel.Collection<string>
        {
            public int P1 { get; set; }
        }
        public class CollectionBase2 : ListBase1
        {
            public int P2 { get; set; }
        }

        [Fact]
        public void CustomGCollectionTest1()
        {
            CollectionBase1 m1 = new CollectionBase1() { P1 = 5 };
            m1.Add("A");
            m1.Add("B");
            var m2 = ObjectComparer(m1);
            Assert.Equal(m1.P1, m2.P1);
            Assert.Equal(m1.Count, m2.Count);
            for(int i = 0; i < m1.Count; i++)
            {
                Assert.Equal(m1[i], m2[i]);
            }
        }

        [Fact]
        public void CustomGCollectionTest2()
        {
            CollectionBase2 m1 = new CollectionBase2() { P1 = 5, P2 = 55 };
            m1.Add("A");
            m1.Add("B");
            var m2 = ObjectComparer(m1);
            Assert.Equal(m1.P1, m2.P1);
            Assert.Equal(m1.Count, m2.Count);
            for(int i = 0; i < m1.Count; i++)
            {
                Assert.Equal(m1[i], m2[i]);
            }
        }


        public class DictionaryBase1 : Dictionary<string, int>
        {
            public int P1 { get; set; }

            public DictionaryBase1() { }
            protected DictionaryBase1(SerializationInfo info, StreamingContext context)
                : base(info, context)
            {
                P1 = info.GetInt32("P1");
            }
            public override void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                base.GetObjectData(info, context);
                info.AddValue("P1", P1);
            }
        }

        public class DictionaryBase2 : DictionaryBase1
        {
            public int P2 { get; set; }

            public DictionaryBase2() { }
            protected DictionaryBase2(SerializationInfo info, StreamingContext context)
                : base(info, context)
            {
                P2 = info.GetInt32("P2");
            }
            public override void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                base.GetObjectData(info, context);
                info.AddValue("P2", P2);
            }
        }


        [Fact]
        public void CustomGDictionaryTest1()
        {
            DictionaryBase1 m1 = new DictionaryBase1() { P1 = 5 };
            m1.Add("A", 1);
            m1.Add("B", 1);
            var m2 = ObjectComparer(m1);
            Assert.Equal(m1.P1, m2.P1);
            Assert.Equal(m1.Count, m2.Count);
            for(int i = 0; i < m1.Count; i++)
            {
                Assert.Equal(m1.ElementAt(i), m2.ElementAt(i));
            }
        }

        [Fact]
        public void CustomGDictionaryTest2()
        {
            DictionaryBase2 m1 = new DictionaryBase2() { P1 = 5, P2 = 55 };
            m1.Add("A", 1);
            m1.Add("B", 1);
            var m2 = ObjectComparer(m1);
            Assert.Equal(m1.P1, m2.P1);
            Assert.Equal(m1.Count, m2.Count);
            for(int i = 0; i < m1.Count; i++)
            {
                Assert.Equal(m1.ElementAt(i), m2.ElementAt(i));
            }
        }


        public class TypeTestClass1
        {
            public Type Type1 { get; set; }
        }

        [Fact]
        public void TypeTest1()
        {
            var m1 = new TypeTestClass1() { Type1 = Types.String };
            var m2 = ObjectComparer(m1);
            Assert.Equal(m1.Type1, m2.Type1);
        }
        public class TypeTestClass2 : TypeTestClass1
        {
            public Type Type2 { get; set; }
        }
        [Fact]
        public void TypeTest2()
        {
            var m1 = new TypeTestClass2() { Type1 = Types.String, Type2 = Types.String };
            var m2 = ObjectComparer(m1);
            Assert.Equal(m1.Type1, m2.Type1);
            Assert.Equal(m1.Type2, m2.Type2);
        }

        [Fact]
        public void TypeTest3()
        {
            var m1 = new TypeTestClass2() { Type1 = Types.String, Type2 = Types.Int32 };
            var m2 = ObjectComparer(m1);
            Assert.Equal(m1.Type1, m2.Type1);
        }

        [Fact]
        public void ResultSuccessfullyTest()
        {
            var m1 = Result.Successfully;
            QuicklySerializer ser = new QuicklySerializer();
            var m1_bytes = ser.FastWriteBytes(m1);
            Assert.Equal(1, m1_bytes.Length);
            var m2 = ser.FastReadBytes<Result>(m1_bytes);
            Assert.Equal(m1, m2);
        }

        [Fact]
        public void ResultTest1()
        {
            var m1 = new Result();
            QuicklySerializer ser = new QuicklySerializer();
            var m1_bytes = ser.FastWriteBytes(m1);
            var m2 = ser.FastReadBytes<Result>(m1_bytes);
            Assert.Equal(m1.Status, m2.Status);
            Assert.Equal(m1.Exception, m2.Exception);
        }
        [Fact]
        public void GResultTest1()
        {
            var m1 = new Result<int>() { Value = 333 };
            QuicklySerializer ser = new QuicklySerializer();
            var m1_bytes = ser.FastWriteBytes(m1);
            var m2 = ser.FastReadBytes<Result<int>>(m1_bytes);
            Assert.Equal(m1.Status, m2.Status);
            Assert.Equal(m1.Exception, m2.Exception);
            Assert.Equal(m1.Value, m2.Value);
        }
        [Fact]
        public void GResultTest2()
        {
            var m1 = new Result<int>().ToFailded("aaa", 100);
            QuicklySerializer ser = new QuicklySerializer();
            var m1_bytes = ser.FastWriteBytes(m1);
            var m2 = ser.FastReadBytes<Result<int>>(m1_bytes);
            Assert.Equal(m1.Status, m2.Status);
            Assert.Equal(m1.Exception.Message, m1.Exception.Message);
            Assert.Equal(m1.Value, m2.Value);
        }
    }
}
