
using System;
using Aoite.Reflection;
using Aoite.ReflectionTest.Common;
using Aoite.ReflectionTest.SampleModel.People;
using Xunit;

namespace Aoite.ReflectionTest.Invocation
{

    public class ArrayTest : BaseInvocationTest
    {
        public ArrayTest() : base(typeof(Person[]), typeof(PersonStruct[])) { }

        [Fact()]
        public void TestConstructArrays()
        {
            RunWith((Type type) =>
            {
                var obj = type.CreateInstance(10);
                Assert.NotNull(obj);
                Assert.Equal(10, obj.GetPropertyValue("Length"));
            });
        }

        [Fact()]
        public void TestGetSetElements()
        {
            RunWith((Type type) =>
            {
                var array = type.CreateInstance(10);
                var instance = type.GetElementType().CreateInstance().WrapIfValueType();
                instance.SetFieldValue("name", "John");
                array.SetElement(1, instance.UnwrapIfWrapped());
                VerifyFields(array.GetElement(1).WrapIfValueType(), new { name = "John" });
            });
        }

        [Fact()]
        public void TestGetSetElementsOnIntArray()
        {
            var array = typeof(int[]).CreateInstance(20);
            array.SetElement(5, 10);
            Assert.Equal(10, array.GetElement(5));
        }

        [Fact()]
        public void TestGetSetElementsOnArrayProperty()
        {
            var employee = EmployeeType.CreateInstance();
            employee.SetPropertyValue("Subordinates", new Employee[10]);
            var subordinates = employee.GetPropertyValue("Subordinates");
            subordinates.SetElement(5, employee);
            Assert.Equal(employee, subordinates.GetElement(5));
        }
    }
}
