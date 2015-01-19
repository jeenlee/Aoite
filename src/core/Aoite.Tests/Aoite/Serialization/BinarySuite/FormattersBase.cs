using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace Aoite.Serialization.BinarySuite
{

    public abstract class FormattersBase
    {
        internal ObjectWriter CreateWriter() { return new ObjectWriter(new MemoryStream(), Encoding.UTF8); }

        internal ObjectReader CreateReader(Stream stream) { return new ObjectReader(stream, Encoding.UTF8); }

        public void TestArray<T>(T[] test1, T[] test2)
        {
            Assert.Equal(test1.Length, test2.Length);
            for(int i = 0; i < test1.Length; i++) Assert.Equal(test1[i], test2[i]);
        }

        public void AssertObjct<T>(T[] expected)
        {
            using(var writer = CreateWriter())
            {
                writer.Serialize(expected);
                writer.Stream.Position = 0;
                var reader = this.CreateReader(writer.Stream);
                this.TestArray(expected, reader.Deserialize() as T[]);
            }
        }

        public void AssertObjct(object expected)
        {
            using(var writer = CreateWriter())
            {
                writer.Serialize(expected);
                writer.Stream.Position = 0;
                var reader = this.CreateReader(writer.Stream);
                Assert.Equal(expected, reader.Deserialize());
            }
        }
    }
}
