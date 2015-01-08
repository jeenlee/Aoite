using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace System.Extensions
{
    public class TypeExtensionsTests
    {
        [Fact()]
        public void GetDefaultValueTest()
        {
            Assert.Equal(0, typeof(Int32).GetDefaultValue());
            Assert.Equal(0L, typeof(Int64).GetDefaultValue());
            Assert.Equal(null, typeof(String).GetDefaultValue());
        }

        [Fact()]
        public void IsDataTypeTest()
        {
            Assert.True(typeof(System.Data.DataTable).IsDataType());
            Assert.True(typeof(System.Data.DataSet).IsDataType());
        }
        [Fact()]
        public void IsAnonymousTest()
        {
            Assert.False(typeof(System.Data.DataTable).IsAnonymous());
            var a = new { };
            Assert.True(a.GetType().IsAnonymous());
            var b = new { c = "a" };
            Assert.True(b.GetType().IsAnonymous());
        }
        [Fact()]
        public void IsNullableTest()
        {
            Assert.True(typeof(Int32?).IsNullable());
            Assert.False(typeof(Int32).IsNullable());
        }
        [Fact()]
        public void GetNullableTypeTest()
        {
            Assert.Equal(typeof(Int32), typeof(Int32?).GetNullableType());
            Assert.Equal(typeof(Int32), typeof(Int32).GetNullableType());
        }

        [Fact()]
        public void HasStringConverterTest()
        {
            Assert.True(typeof(Int32).HasStringConverter());
        }
        [Fact()]
        public void IsXXXTypeTest()
        {
            Assert.True(typeof(Int32).IsSimpleType());
            Assert.True(typeof(Decimal).IsSimpleType());
            Assert.True(typeof(Int32).IsNumber());
            Assert.True(typeof(Double).IsNumberFloat());
        }
        [Fact()]
        public void GetSimpleTypeTest()
        {
            Assert.Equal("int?", typeof(Int32).GetSimpleType(0, 0, true));
        }

        [Aoite.Data.Table("abcdefg")]
        public class TestTable { }
        [Fact()]
        public void GetAttributesTests()
        {
            Assert.Equal("abcdefg", typeof(TestTable).GetAttribute<Aoite.Data.TableAttribute>().Name);
            Assert.Equal("abcdefg", typeof(TestTable).GetAttribute<System.AliasAttribute>().Name);
        }
        [Fact()]
        public void ToEnumValueTest()
        {
            var type = typeof(TypeCode);
            Assert.Equal(TypeCode.Object, type.ToEnumValue(1));
            Assert.Equal(TypeCode.Object, type.ToEnumValue("1"));
            Assert.Equal(TypeCode.Object, type.ToEnumValue("01"));
            Assert.Equal(TypeCode.Object, type.ToEnumValue("Object"));
            Assert.Equal(TypeCode.Object, type.ToEnumValue(TypeCode.Object));
        }
    }
}
