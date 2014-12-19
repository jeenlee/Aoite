
using Aoite.Reflection;
using Xunit;
using Aoite.ReflectionTest.SampleModel.Animals;

namespace Aoite.ReflectionTest.Lookup
{

    public class ParameterTest
    {
        [Fact()]
        public void TestFindParameterDefaultValue()
        {
            var method = typeof(Snake).Method("Move");
            Assert.NotNull(method);
            var parameters = method.Parameters();
            Assert.NotNull(parameters);
            Assert.Equal(1, parameters.Count);
            var parameter = parameters[0];
            Assert.True(parameter.HasDefaultValue());
            Assert.Equal(100d, parameter.DefaultValue());
        }

        [Fact()]
        public void TestParameterHasName()
        {
            // Zoo.RegisterClass( string @class, string _section, string __name, int size )
            var method = typeof(Zoo).Method("RegisterClass");
            Assert.NotNull(method);
            var parameters = method.Parameters();
            Assert.NotNull(parameters);
            Assert.Equal(4, parameters.Count);
            Assert.True(parameters[0].HasName("class"));
            Assert.True(parameters[0].HasName("_class"));
            Assert.True(parameters[1].HasName("section"));
            Assert.True(parameters[1].HasName("_section"));
            Assert.False(parameters[1].HasName("__section"));
            Assert.False(parameters[2].HasName("name"));
            Assert.False(parameters[2].HasName("_name"));
            Assert.True(parameters[2].HasName("__name"));
        }

        [Fact()]
        public void TestParameterIsNullable()
        {
            var method = typeof(Snake).Method("Move");
            Assert.NotNull(method);
            var parameters = method.Parameters();
            Assert.NotNull(parameters);
            Assert.Equal(1, parameters.Count);
            Assert.False(parameters[0].IsNullable());

            method = typeof(Snake).Method("Bite");
            Assert.NotNull(method);
            parameters = method.Parameters();
            Assert.NotNull(parameters);
            Assert.Equal(1, parameters.Count);
            Assert.True(parameters[0].IsNullable());
        }
    }
}
