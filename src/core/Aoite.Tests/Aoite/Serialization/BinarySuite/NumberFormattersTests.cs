using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Aoite.Serialization.BinarySuite
{
    public class NumberFormattersTests : FormattersBase
    {
        #region Integers

        [Fact]
        public void SByteTest()
        {
            this.AssertObjct(5);
            this.AssertObjct(SByte.MinValue);
            this.AssertObjct(SByte.MaxValue);

            this.AssertObjct(new SByte[] { 5 });
            this.AssertObjct(new SByte[] { 5, SByte.MinValue });
            this.AssertObjct(new SByte[] { 5, SByte.MaxValue });
        }

        [Fact]
        public void ByteTest()
        {

            this.AssertObjct(5);
            this.AssertObjct(Byte.MinValue);
            this.AssertObjct(Byte.MaxValue);

            this.AssertObjct(new Byte[] { 5 });
            this.AssertObjct(new Byte[] { 5, Byte.MinValue });
            this.AssertObjct(new Byte[] { 5, Byte.MaxValue });
        }

        [Fact]
        public void Int16Test()
        {
            this.AssertObjct((Int16)5);
            this.AssertObjct(Int16.MinValue);
            this.AssertObjct(Int16.MaxValue);

            this.AssertObjct(new Int16[] { 5 });
            this.AssertObjct(new Int16[] { 5, Int16.MinValue });
            this.AssertObjct(new Int16[] { 5, Int16.MaxValue });
        }

        [Fact]
        public void UInt16Test()
        {
            this.AssertObjct((UInt16)5);
            this.AssertObjct(UInt16.MinValue);
            this.AssertObjct(UInt16.MaxValue);

            this.AssertObjct(new UInt16[] { 5 });
            this.AssertObjct(new UInt16[] { 5, UInt16.MinValue });
            this.AssertObjct(new UInt16[] { 5, UInt16.MaxValue });
        }

        [Fact]
        public void Int32Test()
        {
            this.AssertObjct(5);
            this.AssertObjct(Int32.MinValue);
            this.AssertObjct(Int32.MaxValue);

            this.AssertObjct(new Int32[] { 5 });
            this.AssertObjct(new Int32[] { 5, Int32.MinValue });
            this.AssertObjct(new Int32[] { 5, Int32.MaxValue });
        }

        [Fact]
        public void UInt32Test()
        {
            this.AssertObjct((UInt32)5);
            this.AssertObjct(UInt32.MinValue);
            this.AssertObjct(UInt32.MaxValue);

            this.AssertObjct(new UInt32[] { 5 });
            this.AssertObjct(new UInt32[] { 5, UInt32.MinValue });
            this.AssertObjct(new UInt32[] { 5, UInt32.MaxValue });
        }

        [Fact]
        public void Int64Test()
        {
            this.AssertObjct(5L);
            this.AssertObjct(Int64.MinValue);
            this.AssertObjct(Int64.MaxValue);

            this.AssertObjct(new Int64[] { 5 });
            this.AssertObjct(new Int64[] { 5, Int64.MinValue });
            this.AssertObjct(new Int64[] { 5, Int64.MaxValue });
        }

        [Fact]
        public void UInt64Test()
        {
            this.AssertObjct((UInt64)5);
            this.AssertObjct(UInt64.MinValue);
            this.AssertObjct(UInt64.MaxValue);

            this.AssertObjct(new UInt64[] { 5 });
            this.AssertObjct(new UInt64[] { 5, UInt64.MinValue });
            this.AssertObjct(new UInt64[] { 5, UInt64.MaxValue });
        }

        #endregion

        #region Decimals

        [Fact]
        public void SingleTest()
        {
            this.AssertObjct((Single)5);
            this.AssertObjct(Single.MinValue);
            this.AssertObjct(Single.MaxValue);

            this.AssertObjct(new Single[] { 5 });
            this.AssertObjct(new Single[] { 5, Single.MinValue });
            this.AssertObjct(new Single[] { 5, Single.MaxValue });
        }

        [Fact]
        public void DoubleTest()
        {
            this.AssertObjct(5D);
            this.AssertObjct(Double.MinValue);
            this.AssertObjct(Double.MaxValue);

            this.AssertObjct(new Double[] { 5 });
            this.AssertObjct(new Double[] { 5, Double.MinValue });
            this.AssertObjct(new Double[] { 5, Double.MaxValue });
        }

        [Fact]
        public void DecimalTest()
        {
            this.AssertObjct(5M);
            this.AssertObjct(Decimal.MinValue);
            this.AssertObjct(Decimal.MaxValue);

            this.AssertObjct(new Decimal[] { 5 });
            this.AssertObjct(new Decimal[] { 5, Decimal.MinValue });
            this.AssertObjct(new Decimal[] { 5, Decimal.MaxValue });
        }

        #endregion
    }
}
