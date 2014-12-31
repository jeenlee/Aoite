using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace System.Core
{
    public class MeanTests
    {
        [Fact()]
        public void Test()
        {
            int x = 5;
            Mean<int> mean = new Mean<int>(() => x);
            Assert.Equal(5, mean.Value);
            x = 6;
            Assert.Equal(5, mean.Value);
            mean.Reset();
            Assert.Equal(6, mean.Value);
        }
    }
}
