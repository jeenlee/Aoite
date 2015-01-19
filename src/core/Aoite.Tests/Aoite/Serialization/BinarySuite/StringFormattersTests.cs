using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Aoite.Serialization.BinarySuite
{
    public class StringFormattersTests : FormattersBase
    {
        [Fact]
        public void CharTest()
        {
            this.AssertObjct('a');
            this.AssertObjct(Char.MinValue);
            this.AssertObjct(Char.MaxValue);

            this.AssertObjct(new Char[] { 'a', 'b', 'A', 'B' });
            this.AssertObjct(new Char[] { 'a', 'b', 'A', 'B', Char.MinValue });
            this.AssertObjct(new Char[] { 'a', 'b', 'A', 'B', Char.MaxValue });
        }

        [Fact]
        public void StringTest()
        {
            this.AssertObjct(string.Empty);
            this.AssertObjct("");
            this.AssertObjct("fdsfsdafasf56");

            this.AssertObjct(new string[] { "fdsafdsaf", "fdsafdsaf" });
            this.AssertObjct(new string[] { "fdsafdsaf", "fdsafdsaf", null, string.Empty, "aaaa", string.Empty, "a" });
            this.AssertObjct(new string[] { "a", "A", "a", "A", "a", "A", "a", "A" });
            this.AssertObjct(new string[] { null, null });
            this.AssertObjct(new string[] { string.Empty, string.Empty });
            this.AssertObjct(new string[] { null, string.Empty, string.Empty });
        }

        private void AssertObjct(StringBuilder[] expected)
        {
            using(var writer = CreateWriter())
            {
                writer.Serialize(expected);
                writer.Stream.Position = 0;
                var reader = this.CreateReader(writer.Stream);
                var actual = reader.Deserialize() as StringBuilder[];

                Assert.Equal(expected.Length, actual.Length);
                for(int i = 0; i < expected.Length; i++)
                {
                    if(expected[i] == null)
                    {
                        Assert.Null(actual[i]);
                        continue;
                    }
                    Assert.Equal(expected[i].ToString(), actual[i].ToString());
                }
            }
        }

        private void AssertObjct(StringBuilder expected)
        {
            using(var writer = CreateWriter())
            {
                writer.Serialize(expected);
                writer.Stream.Position = 0;
                var reader = this.CreateReader(writer.Stream);
                Assert.Equal(Convert.ToString(expected), Convert.ToString(reader.Deserialize()));
            }
        }

        private void AssertStringBuilder(string value)
        {
            this.AssertObjct(new StringBuilder(value));
        }

        private void AssertStringBuilder(string[] value)
        {
            StringBuilder[] builders = new StringBuilder[value.Length];
            for(int i = 0; i < value.Length; i++) builders[i] = new StringBuilder(value[i]);
            this.AssertObjct(builders);
        }
        [Fact]
        public void StringBuilderTest()
        {
            this.AssertStringBuilder(string.Empty);
            this.AssertStringBuilder("");
            this.AssertStringBuilder("fdsfsdafasf56");

            this.AssertStringBuilder(new string[] { "fdsafdsaf", "fdsafdsaf" });
            this.AssertStringBuilder(new string[] { "fdsafdsaf", "fdsafdsaf", null, string.Empty, "aaaa", string.Empty, "a" });
            this.AssertStringBuilder(new string[] { "a", "A", "a", "A", "a", "A", "a", "A" });
            this.AssertStringBuilder(new string[] { null, null });
            this.AssertStringBuilder(new string[] { string.Empty, string.Empty });
            this.AssertStringBuilder(new string[] { null, string.Empty, string.Empty });

            this.AssertObjct(new StringBuilder[] { null, null, null, new StringBuilder("A"), null, null, new StringBuilder("A"), null, null, new StringBuilder("B") });
        }
    }
}
