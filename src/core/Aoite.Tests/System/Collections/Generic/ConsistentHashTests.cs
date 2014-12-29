using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace System.Collections.Generic
{
    public class ConsistentHashTests
    {
        [Fact()]
        public void Test()
        {
            string[] array = { "AAA", "BBB", "CCC", "DDD" };
            string[] array2 = { array[0], array[1], array[2] };
            ConsistentHash<string> s = new ConsistentHash<string>(array);
            s.Add(array[3]);
            for(int i = 0; i < 100; i++)
            {
                Assert.Contains(s.GetNode(i.ToString()), array);
            }

        }
    }
}
