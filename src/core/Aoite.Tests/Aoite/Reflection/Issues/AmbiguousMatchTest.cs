using System;
using System.Collections;
using System.Linq;
using Aoite.Reflection;
using Xunit;

namespace Aoite.ReflectionTest.Issues
{

    public class AmbiguousMatchTest
    {
        #region Sample Classes
        private class Foo
        {
            public object Property { get; set; }
        }
        private class Bar : Foo
        {
            public new string Property { get; set; }
        }
        #endregion

        [Fact()]
        public void Test_PropertyLookupWithNameAndEXHFlagShouldNotThrowAmbiguousMatchException()
        {
            var propertyInfo = typeof(Bar).Property("Property", Flags.InstanceAnyVisibility | Flags.ExcludeHiddenMembers);
            Assert.NotNull(propertyInfo);
            Assert.Equal(typeof(Bar), propertyInfo.DeclaringType);
        }

        [Fact()]
        public void Test_PropertiesLookupWithNameAndEXHFlagShouldFindSingleResult()
        {
            var propertyInfo = typeof(Bar).Properties(Flags.InstanceAnyVisibility | Flags.ExcludeHiddenMembers, "Property").Single();
            Assert.NotNull(propertyInfo);
            Assert.Equal(typeof(Bar), propertyInfo.DeclaringType);
        }
    }
}
