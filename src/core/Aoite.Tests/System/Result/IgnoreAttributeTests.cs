using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace System
{
    public class IgnoreAttributeTests
    {
        [Fact()]
        public void Type()
        {
            Assert.Equal(typeof(IgnoreAttribute), IgnoreAttribute.Type);
        }
    }
}
