
using Aoite.Reflection;
using Xunit;

namespace Aoite.ReflectionTest.Internal
{

    public class FlagsTest
    {
        [Fact()]
        public void TestFlagsToString()
        {
            Assert.Equal(string.Empty, Flags.None.ToString());
            Assert.Equal("Public", Flags.Public.ToString());
            Assert.Equal("Instance", Flags.Instance.ToString());
            Assert.Equal("Public", (Flags.None | Flags.Public).ToString());
            Assert.Equal("Instance | Public", (Flags.Instance | Flags.Public).ToString());
            Assert.Equal("Instance | NonPublic | Public", (Flags.Instance | Flags.Public | Flags.NonPublic).ToString());
            Assert.Equal("Instance | NonPublic | Public", Flags.InstanceAnyVisibility.ToString());
        }
    }
}


