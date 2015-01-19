using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace System.Extensions
{
    public class CommonExtensionsTests
    {
        public class TestDemo1
        {
            public string MyProperty1 { get; set; }
            public int MyProperty2 { get; set; }
            public double MyProperty3 { get; set; }
        }

        public class TestDemo2
        {
            public string MyProperty1 { get; set; }
            public int MyProperty2 { get; set; }
            public double MyProperty3 { get; set; }
        }

        [Fact()]
        public void CopyToTest1()
        {
            TestDemo1 t1 = new TestDemo1() { MyProperty1 = "AA", MyProperty2 = 22, MyProperty3 = 11.1 };
            var t2 = t1.CopyTo<TestDemo2>();
            Assert.Equal(t1.MyProperty1, t2.MyProperty1);
            Assert.Equal(t1.MyProperty2, t2.MyProperty2);
            Assert.Equal(t1.MyProperty3, t2.MyProperty3);
        }
        public class TestDemo3 : TestDemo2
        {

        }
        [Fact()]
        public void CopyToTest2()
        {
            TestDemo1 t1 = new TestDemo1() { MyProperty1 = "AA", MyProperty2 = 22, MyProperty3 = 11.1 };
            var t2 = t1.CopyTo<TestDemo3>();
            Assert.Equal(t1.MyProperty1, t2.MyProperty1);
            Assert.Equal(t1.MyProperty2, t2.MyProperty2);
            Assert.Equal(t1.MyProperty3, t2.MyProperty3);
        }
        [Fact()]
        public void CopyFromTest()
        {
            TestDemo1 t1 = new TestDemo1() { MyProperty1 = "AA", MyProperty2 = 22, MyProperty3 = 11.1 };
            var t2 = new TestDemo3();
            t2.CopyFrom(t1);
            Assert.Equal(t1.MyProperty1, t2.MyProperty1);
            Assert.Equal(t1.MyProperty2, t2.MyProperty2);
            Assert.Equal(t1.MyProperty3, t2.MyProperty3);
        }

        [Fact()]
        public void CopyEnumTest()
        {
            FromClass1 f1 = new FromClass1() { Id = 3 };
            FromClass2 f2 = new FromClass2() { Id = 3 };
            FromClass3 f3 = new FromClass3() { Id = TypeCode.Boolean };
            FromClass4 f4 = new FromClass4() { Id = TypeCode.Boolean };

            Assert.Equal(TypeCode.Boolean, f1.CopyTo<FromClass3>().Id);
            Assert.Equal(TypeCode.Boolean, f1.CopyTo<FromClass4>().Id);

            Assert.Equal(TypeCode.Boolean, f2.CopyTo<FromClass3>().Id);
            Assert.Equal(TypeCode.Boolean, f2.CopyTo<FromClass4>().Id);

            Assert.Equal(3, f3.CopyTo<FromClass1>().Id);
            Assert.Equal(3, f3.CopyTo<FromClass2>().Id);

            Assert.Equal(3, f4.CopyTo<FromClass1>().Id);
            Assert.Equal(3, f4.CopyTo<FromClass2>().Id);
        }

        [Fact()]
        public void CopyToNullableTest()
        {
            FromClass2 f2_null = new FromClass2() { Id = null };
            Assert.Throws<ArgumentNullException>(() => f2_null.CopyTo<FromClass3>());
        }
        [Fact()]
        public void CopyToEnumTest()
        {
            FromClass4 f4_null = new FromClass4() { Id = null };
            Assert.Throws<ArgumentNullException>(() => f4_null.CopyTo<FromClass3>());
        }

        public class FromClass1
        {
            public int Id { get; set; }
        }
        public class FromClass2
        {
            public int? Id { get; set; }
        }
        public class FromClass3
        {
            public TypeCode Id { get; set; }
        }
        public class FromClass4
        {
            public TypeCode? Id { get; set; }
        }
    }
}
