
using Aoite.Reflection;
using Aoite.ReflectionTest.SampleModel.Animals;
using Xunit;

namespace Aoite.ReflectionTest.Lookup
{

    public class TryGetSetTest
    {
        [Fact()]
        public void TestTryGetSetField()
        {
            Lion lion = new Lion(42, "Scar");
            // tryget
            Assert.Null(lion.TryGetFieldValue("name"));
            Assert.Null(lion.TryGetFieldValue("ID"));
            Assert.Equal(42, lion.TryGetFieldValue("id"));
            Assert.Equal(42, lion.TryGetFieldValue("ID", Flags.InstanceAnyVisibility | Flags.IgnoreCase));
            // tryset
            Assert.False(lion.TrySetFieldValue("missing", false));
            Assert.True(lion.TrySetFieldValue("id", 43));
            Assert.Equal(43, lion.ID);
        }

        [Fact()]
        public void TestTryGetSetProperty()
        {
            Lion lion = new Lion(42, "Scar");
            // tryget
            Assert.Null(lion.TryGetPropertyValue("missing"));
            Assert.Equal(42, lion.TryGetPropertyValue("ID"));
            Assert.Equal("Scar", lion.TryGetPropertyValue("Name"));
            // tryset
            Assert.False(lion.TrySetPropertyValue("missing", false));
            Assert.True(lion.TrySetPropertyValue("Name", "Simba"));
            Assert.Equal("Simba", lion.Name);
        }

        [Fact()]
        public void TestTryGetSetMember()
        {
            Lion lion = new Lion(42, "Scar");
            // tryget
            Assert.Null(lion.TryGetValue("missing"));
            Assert.Equal(42, lion.TryGetValue("id"));
            Assert.Equal("Scar", lion.TryGetValue("Name"));
            // tryset
            Assert.False(lion.TrySetValue("missing", false));
            Assert.True(lion.TrySetValue("id", 43));
            Assert.Equal(43, lion.ID);
            Assert.True(lion.TrySetValue("ID", 44, Flags.InstanceAnyVisibility | Flags.IgnoreCase));
            Assert.True(lion.TrySetValue("Name", "Simba"));
            Assert.Equal(44, lion.ID);
            Assert.Equal("Simba", lion.Name);
        }
    }
}
