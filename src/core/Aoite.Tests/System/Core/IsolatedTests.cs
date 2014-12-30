using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace System
{
    public class IsolatedTests
    {
        public class TE : MarshalByRefObject
        {
            public static int X { get; set; }

            public int GetStaticX()
            {
                return X;
            }
        }
        [Fact()]
        public void Test()
        {
            using(var iso = new Isolated<TE>())
            {
                TE.X = 5;
                Assert.Equal(5, new TE().GetStaticX());
                Assert.Equal(0, iso.Instance.GetStaticX());
            }
        }
    }

}
