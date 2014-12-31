using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace System
{
    public class BinaryValueTests
    {
        [Fact()]
        public void BytesTest()
        {
            byte[] value = { 1, 2, 3 };
            BinaryValue bv = value;
            Assert.Equal(value, (byte[])bv);
        }

        [Fact()]
        public void DecimalTest()
        {
            Decimal value = 5M;
            BinaryValue bv = value;
            Assert.Equal(value, (Decimal)bv);
        }

        [Fact()]
        public void GuidTest()
        {
            Guid value = Guid.NewGuid();
            BinaryValue bv = value;
            Assert.Equal(value, (Guid)bv);
        }
        [Fact()]
        public void StringTest()
        {
            String value = Guid.NewGuid().ToString();
            BinaryValue bv = value;
            Assert.Equal(value, (String)bv);
        }

        [Fact()]
        public void DateTimeTest()
        {
            DateTime value = DateTime.Now;
            BinaryValue bv = value;
            Assert.Equal(value, (DateTime)bv);
        }

        [Fact()]
        public void TimeSpanTest()
        {
            TimeSpan value = DateTime.Now - DateTime.Now.AddDays(-5);
            BinaryValue bv = value;
            Assert.Equal(value, (TimeSpan)bv);
        }

        [Fact()]
        public void BooleanTest()
        {
            Boolean value = true;
            BinaryValue bv = value;
            Assert.Equal(value, (Boolean)bv);
        }
        [Fact()]
        public void CharTest()
        {
            Char value = 'a';
            BinaryValue bv = value;
            Assert.Equal(value, (Char)bv);
        }
        [Fact()]
        public void DoubleTest()
        {
            Double value = 0.5;
            BinaryValue bv = value;
            Assert.Equal(value, (Double)bv);
        }

        [Fact()]
        public void Int16Test()
        {
            Int16 value = Int16.MaxValue;
            BinaryValue bv = value;
            Assert.Equal(value, (Int16)bv);
        }

        [Fact()]
        public void Int32Test()
        {
            Int32 value = Int32.MaxValue;
            BinaryValue bv = value;
            Assert.Equal(value, (Int32)bv);
        }

        [Fact()]
        public void Int64Test()
        {
            Int64 value = Int64.MaxValue;
            BinaryValue bv = value;
            Assert.Equal(value, (Int64)bv);
        }

        [Fact()]
        public void UInt16Test()
        {
            UInt16 value = UInt16.MaxValue;
            BinaryValue bv = value;
            Assert.Equal(value, (UInt16)bv);
        }

        [Fact()]
        public void UInt32Test()
        {
            UInt32 value = UInt32.MaxValue;
            BinaryValue bv = value;
            Assert.Equal(value, (UInt32)bv);
        }

        [Fact()]
        public void UInt64Test()
        {
            UInt64 value = UInt64.MaxValue;
            BinaryValue bv = value;
            Assert.Equal(value, (UInt64)bv);
        }
    }
}
