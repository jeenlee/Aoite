using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Aoite.Attributes
{
    public class AliasAttributeTests
    {
        [Fact()]
        public void CtorTest()
        {
            AliasAttribute attr = new AliasAttribute();
            Assert.Null(attr.Name);
            attr.Name = "a";
            Assert.Equal("a", attr.Name);
            attr = new AliasAttribute("b");
            Assert.Equal("b", attr.Name);
        }
    }
}
