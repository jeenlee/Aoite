
using System;
using System.Collections.Generic;
using System.Reflection;
using Aoite.Reflection;
using Xunit;

namespace Aoite.ReflectionTest.Lookup
{

    public class AssemblyTest
    {
        #region Types()
        [Fact()]
        public void TestFindTypes()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(int));
            IList<Type> types = assembly.Types();
            Assert.NotNull(types);
            Assert.True(types.Count > 1000);
        }

        [Fact()]
        public void TestFindTypesByEmptyNameListShouldReturnAllTypes()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(int));
            IList<Type> types = assembly.Types(new string[0]);
            Assert.NotNull(types);
            Assert.True(types.Count > 1000);

            types = assembly.Types(null);
            Assert.NotNull(types);
            Assert.True(types.Count > 1000);
        }

        [Fact()]
        public void TestFindTypesShouldReturnEmptyListIfNotFound()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(int));
            IList<Type> types = assembly.Types("UrzgHafn");
            Assert.NotNull(types);
            Assert.Equal(0, types.Count);
        }

        [Fact()]
        public void TestFindTypesByName()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(int));
            IList<Type> types = assembly.Types("Int32");
            Assert.NotNull(types);
            Assert.Equal(1, types.Count);
        }

        [Fact()]
        public void TestFindTypesByPartialName()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(int));
            IList<Type> types = assembly.Types(Flags.PartialNameMatch, "Engine");
            Assert.NotNull(types);
            Assert.Equal(2, types.Count);
        }
        #endregion
    }
}


