using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace System
{
    public class CollectionsExtensionsTests
    {
        [Fact()]
        public void ToGridTest()
        {
            Assert.Throws<ArgumentNullException>(() => CollectionsExtensions.ToGrid((int[])null));

            var grid = CollectionsExtensions.ToGrid(new int[] { 1, 2, 3 }, 5);
            Assert.Equal(5, grid.Total);
            Assert.Equal(new int[] { 1, 2, 3 }, grid.Rows);
        }

        [Fact()]
        public void TryGetValueTest()
        {
            Dictionary<int, string> dict = null;
            Assert.Throws<ArgumentNullException>(() => CollectionsExtensions.TryGetValue(dict, 1));

            dict = new Dictionary<int, string>() { { 1, "a" } };
            Assert.Equal("a", CollectionsExtensions.TryGetValue(dict, 1));
        }

        [Fact()]
        public void EqualsBytesTest()
        {
            byte[] b1 = null, b2 = null;
            Assert.True(CollectionsExtensions.EqualsBytes(b1, b2));
            b1 = new byte[] { 1, 2, 3, 4, 5 };
            Assert.False(CollectionsExtensions.EqualsBytes(b1, b2));
            b2 = new byte[] { 1, 2, 3, 4, 5 };
            Assert.True(CollectionsExtensions.EqualsBytes(b1, b2));

            b1 = new byte[2048];
            b2 = new byte[2048];
            FastRandom.Instance.NextBytes(b1);
            FastRandom.Instance.NextBytes(b2);
            Assert.False(CollectionsExtensions.EqualsBytes(b1, b2));
        }
        [Fact()]
        public void RandomAnyTest()
        {
            int[] a = { 1, 2, 3, 4, 5 };
            foreach(var item in a.RandomAny())
            {
                Assert.True(a.Contains(item));
            }
        }

        [Fact()]
        public void RandomOneTest()
        {
            int[] a = { 1, 2, 3, 4, 5 };
            Assert.True(a.Contains(a.RandomOne()));
        }

        [Fact()]
        public void JoinTest()
        {
            int[] a = { 1, 2, 3, 4, 5 };
            Assert.Equal("y1x2x3x4x5z", a.Join("x", start: "y", end: "z"));
        }

        [Fact()]
        public void IndexOfTest()
        {
            string[] s = { "A", "B", "C" };
            Assert.Equal(1, s.IndexOf("b", StringComparer.CurrentCultureIgnoreCase));
        }
    }
}
