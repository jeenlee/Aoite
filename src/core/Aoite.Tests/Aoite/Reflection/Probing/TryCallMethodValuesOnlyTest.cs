
using System;
using System.Reflection;
using Aoite.Reflection;
using Aoite.ReflectionTest.SampleModel.Animals;
using Xunit;

namespace Aoite.ReflectionTest.Probing
{

    public class TryCallMethodValuesOnlyTest
    {
        // Elephant method overload summary
        // 1: public void Eat()
        // 2: public void Eat( string food )
        // 3: public void Eat( int count )
        // 4: public void Eat( int count, string food )
        // 5: public void Eat( double count, string food, bool isHay )

        [Fact()]
        public void TestTryCallWithValuesOnlyAndFixedParameterOrdering()
        {
            var obj = new Elephant();
            obj.TryCallMethodWithValues("Eat", 1, "foo");
            Assert.Equal(4, obj.MethodInvoked);
            obj.TryCallMethodWithValues("Eat", 1.0, "foo", false);
            Assert.Equal(5, obj.MethodInvoked);
            obj.TryCallMethodWithValues("Eat", "foo");
            Assert.Equal(2, obj.MethodInvoked);
            obj.TryCallMethodWithValues("Eat", 'f');
            Assert.Equal(2, obj.MethodInvoked);
            obj.TryCallMethodWithValues("Eat", null); // this invokes 1 and not 2 because null implies the params array parameter is null == no arguments
            Assert.Equal(1, obj.MethodInvoked);
            obj.TryCallMethodWithValues("Eat", 1, null);
            Assert.Equal(4, obj.MethodInvoked);
            obj.TryCallMethodWithValues("Eat", 1, null, false);
            Assert.Equal(5, obj.MethodInvoked);
            Assert.Equal(5, obj.MethodInvoked);
            obj.TryCallMethodWithValues("Accept", 'c');
            Assert.Equal(12, obj.MethodInvoked);
            obj.TryCallMethodWithValues("Accept", "a");
            Assert.Equal(12, obj.MethodInvoked);
            obj.TryCallMethodWithValues("AcceptParams");
            Assert.Equal(13, obj.MethodInvoked);
            obj.TryCallMethodWithValues("AcceptParams", null);
            Assert.Equal(13, obj.MethodInvoked);
            obj.TryCallMethodWithValues("AcceptParams", 1);
            Assert.Equal(13, obj.MethodInvoked);
            obj.TryCallMethodWithValues("AcceptParams", 1, "str");
            Assert.Equal(13, obj.MethodInvoked);
        }

        [Fact()]
        public void TestTryCallWithValuesOnlyAndFixedParameterOrderingOnString()
        {
            Assert.Equal("abc", "abc ".TryCallMethodWithValues("TrimEnd"));
            Assert.Equal("abc", "abc ".TryCallMethodWithValues("TrimEnd", null));
            Assert.Equal("ab", "abc ".TryCallMethodWithValues("TrimEnd", ' ', 'c'));
        }
    }
}