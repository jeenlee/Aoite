
using System;
using System.Linq;
using Aoite.Reflection.Probing;
using Xunit;
using Aoite.ReflectionTest.SampleModel.Animals.Enumerations;

namespace Aoite.ReflectionTest.Probing
{

    public class TypeConverterTest
    {
        #region Enum Conversions
        [Fact()]
        public void TestConvertFromEnum()
        {
            Assert.Equal(2, TypeConverter.Get(typeof(int), Climate.Cold));
            Assert.Equal(2f, TypeConverter.Get(typeof(float), Climate.Cold));
            Assert.Equal(2d, TypeConverter.Get(typeof(double), Climate.Cold));
            Assert.Equal("Cold", TypeConverter.Get(typeof(string), Climate.Cold));
        }

        [Fact()]
        public void TestConvertToEnum()
        {
            Assert.Equal(Climate.Cold, TypeConverter.Get(typeof(Climate), 2));
            Assert.Equal(Climate.Cold, TypeConverter.Get(typeof(Climate), "2"));
            Assert.Equal(Climate.Cold, TypeConverter.Get(typeof(Climate), "Cold"));
            Assert.Equal(Climate.Cold, TypeConverter.Get(typeof(Climate), (object)"Cold"));
        }
        #endregion

        #region Guid Conversions
        [Fact()]
        public void TestConvertWithEmptyGuid()
        {
            string emptyGuidString = string.Empty.PadRight(16, '\0');
            string textualGuid = string.Empty.PadRight(32, '0');
            var emptyGuidBuffer = new byte[16];
            Assert.Equal(Guid.Empty, TypeConverter.Get(typeof(Guid), (object)emptyGuidString));
            Assert.Equal(Guid.Empty, TypeConverter.Get(typeof(Guid), emptyGuidBuffer));
            Assert.Equal(Guid.Empty, TypeConverter.Get(typeof(Guid), textualGuid));
        }

        [Fact()]
        public void TestConvertWithNonEmptyGuid()
        {
            Guid guid = Guid.NewGuid();
            string binaryStringGuid = TypeConverter.GuidToBinaryString(guid);
            string readableStringGuid = guid.ToString();
            byte[] binaryGuid = guid.ToByteArray();
            // test direct to guid
            Assert.Equal(guid, TypeConverter.Get(typeof(Guid), binaryStringGuid));
            Assert.Equal(guid, TypeConverter.Get(typeof(Guid), readableStringGuid));
            Assert.Equal(guid, TypeConverter.Get(typeof(Guid), binaryGuid));
            // test direct from guid
            Assert.Equal(binaryStringGuid, TypeConverter.Get(typeof(string), guid));
            Assert.True(binaryGuid.SequenceEqual((byte[])TypeConverter.Get(typeof(byte[]), guid)));
        }
        #endregion
    }
}