using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace System.Extensions
{
    public class StringExtensionsTests
    {
        [Fact()]
        public void ToMd5Test()
        {
            Assert.Equal("E10ADC3949BA59ABBE56E057F20F883E", "123456".ToMd5());
        }

        [Fact()]
        public void ToCamelCaseTest()
        {
            Assert.Equal("id", "Id".ToCamelCase());
            Assert.Equal("a", "A".ToCamelCase());
            Assert.Equal("a", "a".ToCamelCase());
        }

        [Fact()]
        public void ToBytesTest()
        {
            Assert.NotNull("123456".ToBytes());
        }
        [Fact()]
        public void iEqualsTest()
        {
            Assert.True("abc".iEquals("AbC"));
        }
        [Fact()]
        public void iStartsWithTest()
        {
            Assert.True("abcdefg".iStartsWith("AbC"));
        }
        [Fact()]
        public void iEndsWithTest()
        {
            Assert.True("abcdefg".iEndsWith("EfG"));
        }
        [Fact()]
        public void iContainsTest()
        {
            Assert.True("abcdefg".iContains("cDE"));
        }
        [Fact()]
        public void ToLikingTest()
        {
            Assert.Equal("%abcdefg%", "abcdefg".ToLiking());
        }
        [Fact()]
        public void FmtTest()
        {
            Assert.Equal("abcdefg", "ab{0}efg".Fmt("cd"));
        }
        [Fact()]
        public void ToStringOrEmptyTest()
        {
            Assert.Equal(string.Empty, ((string)null).ToStringOrEmpty());
        }
        [Fact()]
        public void IsNullTest()
        {
            string s = null;
            Assert.True(s.IsNull());
            s = string.Empty;
            Assert.True(s.IsNull());
            s = "\t ";
            Assert.True(s.IsNull());
            s = "a";
            Assert.False(s.IsNull());
        }
        [Fact()]
        public void CutStringTest()
        {
            var s = "0123456789";
            Assert.Equal(s, s.CutString(10));
            Assert.Equal("012345...", s.CutString(9));
        }
        [Fact()]
        public void StartsTest()
        {
            var s = "0123456789";
            Assert.Equal("012", s.Starts(3));
        }
        [Fact()]
        public void EndsTest()
        {
            var s = "0123456789";
            Assert.Equal("789", s.Ends(3));
        }
        [Fact()]
        public void RemoveStartsTest()
        {
            var s = "0123456789";
            Assert.Equal("3456789", s.RemoveStarts(3));
        }
        [Fact()]
        public void RemoveEndsTest()
        {
            var s = "0123456789";
            Assert.Equal("0123456", s.RemoveEnds(3));
        }
    }
}
