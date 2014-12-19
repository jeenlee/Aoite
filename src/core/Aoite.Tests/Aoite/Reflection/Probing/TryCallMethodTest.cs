
using System;
using Aoite.Reflection;
using Aoite.ReflectionTest.SampleModel.Animals;
using Xunit;

namespace Aoite.ReflectionTest.Probing
{

    public class TryCallMethodTest
    {
        [Fact()]
        public void TestTryCallWithEmptyArgumentShouldInvokeMethod1()
        {
            var obj = new Elephant();
            obj.TryCallMethod("Eat", true, new { });
            Assert.Equal(1, obj.MethodInvoked);
            // check that we also work when passing in unused parameters
            obj.TryCallMethod("Eat", false, new { size = 1 });
            Assert.Equal(1, obj.MethodInvoked);
        }

        [Fact()]
        public void TestTryCallWithFoodArgumentShouldInvokeMethod2()
        {
            var obj = new Elephant();
            obj.TryCallMethod("Eat", true, new { food = "hay" });
            Assert.Equal(2, obj.MethodInvoked);
        }

        [Fact()]
        public void TestTryCallWithCountArgumentsShouldInvokeMethod3()
        {
            var obj = new Elephant();
            obj.TryCallMethod("Eat", true, new { count = 2 });
            Assert.Equal(3, obj.MethodInvoked);
        }

        [Fact()]
        public void TestTryCallWithCountAndFoodArgumentsShouldInvokeMethod4()
        {
            var obj = new Elephant();
            obj.TryCallMethod("Eat", true, new { count = 2, food = "hay" });
            Assert.Equal(4, obj.MethodInvoked);
        }

        [Fact()]
        public void TestTryCallWithCountAndFoodAndIsHayArgumentsShouldInvokeMethod5()
        {
            var obj = new Elephant();
            obj.TryCallMethod("Eat", true, new { count = 2.0, food = "hay", isHay = true });
            Assert.Equal(5, obj.MethodInvoked);
            // try with argument that must be type converted
            obj.TryCallMethod("Eat", true, new { count = 2, food = "hay", isHay = true });
            Assert.Equal(5, obj.MethodInvoked);
        }

        [Fact()]
        public void TestTryCallWithNonMatchShouldThrow()
        {
            var obj = new Elephant();
            Assert.Throws<MissingMethodException>(() =>
            obj.TryCallMethod("Eat", true, new { size = 1 }));
        }
    }
}