

using System;
using System.Linq;
using Aoite.Reflection;
using Xunit;

namespace Aoite.ReflectionTest.Invocation
{

    public class MethodTest : BaseInvocationTest
    {
        [Fact()]
        public void TestInvokeInstanceMethod()
        {
            RunWith((object person) =>
               {
                   var elements = new[] { 1d, 2d, 3d, 4d, 5d };
                   elements.ForEach(element => person.CallMethod("Walk", element));
                   Assert.Equal(elements.Sum(), person.GetFieldValue("metersTravelled"));
               });
        }

        [Fact()]
        public void TestInvokePrivateInstanceMethodUnderNonPublicBindingFlags()
        {
            RunWith((object person) => person.CallMethod("Walk", Flags.NonPublic | Flags.Instance, 10d));
        }

        [Fact()]
        public void TestInvokePublicStaticMethodUnderStaticBindingFlags()
        {
            Assert.Throws<MissingMethodException>(() =>
            RunWith((object person) => person.CallMethod("Walk", Flags.StaticAnyVisibility, 10d)));
        }

        [Fact()]
        public void TestInvokePrivateInstanceMethodUnderPublicBindingFlags()
        {
            Assert.Throws<MissingMethodException>(() =>
            RunWith((object person) => person.CallMethod("Walk", Flags.Public | Flags.Instance, 10d)));
        }

        [Fact()]
        public void TestInvokeInstanceMethodViaMethodInfo()
        {
            RunWith((object person) =>
               {
                   var elements = new[] { 1d, 2d, 3d, 4d, 5d };
                   var methodInfo = person.UnwrapIfWrapped().GetType().Method("Walk", new[] { typeof(int) }, Flags.InstanceAnyVisibility);
                   elements.ForEach(element => methodInfo.Call(person, element));
                   Assert.Equal(elements.Sum(), person.GetFieldValue("metersTravelled"));
               });
        }

        [Fact()]
        public void TestInvokeWithCoVariantReturnAndParamType()
        {
            var person = PersonType.CreateInstance();
            var friend = EmployeeType.CreateInstance();
            var result = person.CallMethod("AddFriend", friend);
            Assert.Same(friend, result);
        }

        [Fact()]
        public void TestInvokeMethodWithOutArgument()
        {
            RunWith((object person) =>
               {
                   var arguments = new object[] { 10d, null };
                   person.CallMethod("Walk", new[] { typeof(double), typeof(double).MakeByRefType() }, arguments);
                   Assert.Equal(person.GetFieldValue("metersTravelled"), arguments[1]);
               });
        }

        [Fact()]
        public void TestInvokeExplicitlyImplementedMethod()
        {
            var employee = EmployeeType.CreateInstance();
            var currentMeters = (double)employee.GetFieldValue("metersTravelled");
            employee.CallMethod("Swim", Flags.InstanceAnyVisibility | Flags.TrimExplicitlyImplemented, 100d);
            VerifyFields(employee, new { metersTravelled = currentMeters + 100 });
        }

        [Fact()]
        public void TestInvokeBaseClassMethods()
        {
            var employee = EmployeeType.CreateInstance();
            var currentMeters = (double)employee.GetFieldValue("metersTravelled");
            employee.CallMethod("Walk", 100d);
            VerifyFields(employee, new { metersTravelled = currentMeters + 100 });
        }

        [Fact()]
        public void TestInvokeStaticMethod()
        {
            RunWith((Type type) =>
               {
                   var totalPeopleCreated = (int)type.GetFieldValue("totalPeopleCreated");
                   Assert.Equal(totalPeopleCreated, type.CallMethod("GetTotalPeopleCreated"));
               });
        }

        [Fact()]
        public void TestInvokePublicStaticMethodUnderNonPublicBindingFlags()
        {
            Assert.Throws<MissingMethodException>(() =>
            RunWith((Type type) => type.CallMethod("GetTotalPeopleCreated", Flags.NonPublic | Flags.Static)));
        }

        [Fact()]
        public void TestInvokePublicStaticMethodUnderInstanceBindingFlags()
        {
            Assert.Throws<MissingMethodException>(() =>
            RunWith((Type type) => type.CallMethod("GetTotalPeopleCreated", Flags.InstanceAnyVisibility)));
        }

        [Fact()]
        public void TestInvokePublicStaticMethodUnderPublicBindingFlags()
        {
            RunWith((Type type) => type.CallMethod("GetTotalPeopleCreated", Flags.Public | Flags.Static));
        }

        [Fact()]
        public void TestInvokeStaticMethodViaMethodInfo()
        {
            RunWith((Type type) =>
               {
                   var totalPeopleCreated = (int)type.GetFieldValue("totalPeopleCreated");
                   Assert.Equal(totalPeopleCreated,
                                    type.Method("GetTotalPeopleCreated", Flags.StaticAnyVisibility).Call());
               });
        }

        [Fact()]
        public void TestInvokeStaticMethodsWithArgument()
        {
            RunWith((Type type) =>
               {
                   var totalPeopleCreated = (int)type.GetFieldValue("totalPeopleCreated");
                   Assert.Equal(totalPeopleCreated + 20, type.CallMethod("AdjustTotalPeopleCreated", 20));
               });
        }

        [Fact()]
        public void TestInvokeNonExistentInstanceMethod()
        {
            Assert.Throws<MissingMethodException>(() =>
            RunWith((object person) => person.CallMethod("not_exist")));
        }

        [Fact()]
        public void TestInvokeNonExistentStaticMethod()
        {
            Assert.Throws<MissingMethodException>(() =>
            RunWith((Type type) => type.CallMethod("not_exist")));
        }
    }
}