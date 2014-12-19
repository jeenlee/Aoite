

using System;
using System.Reflection;
using Aoite.Reflection;
using Aoite.ReflectionTest.SampleModel.Animals;
using Aoite.ReflectionTest.SampleModel.Animals.Interfaces;
using Aoite.ReflectionTest.SampleModel.People;
using Xunit;

namespace Aoite.ReflectionTest.Invocation
{

    public class ConstructorTest : BaseInvocationTest
    {
        [Fact()]
        public void TestInvokeNoArgCtor()
        {
            var person = PersonType.CreateInstance();
            Assert.NotNull(person);
        }

        [Fact()]
        public void TestInvokeCtorWithCorrectBindingFlags()
        {
            RunWith(type => type.CreateInstance(Flags.Instance | Flags.NonPublic));
        }

        [Fact()]
        public void TestInvokeCtorWithIncorrectBindingFlags()
        {
            Assert.Throws<MissingMemberException>(() =>
            RunWith(type => type.CreateInstance(Flags.Public | Flags.Instance)));
        }

        [Fact()]
        public void TestInvokeCtorWithPrimitiveArguments()
        {
            RunWith(type =>
               {
                   var person = type.CreateInstance("John", 10).WrapIfValueType();
                   VerifyFields(person, new { name = "John", age = 10 });
               });
        }

        [Fact()]
        public void TestInvokeCtorWithComplexArgument()
        {
            RunWith(type =>
               {
                   var person = type.CreateInstance("John", 10);
                   var copy = type.CreateInstance(person).WrapIfValueType();
                   VerifyFields(copy, new { name = "John", age = 10 });
               });
        }

        [Fact()]
        public void TestInvokeCtorWithComplexArgumentCoveriant()
        {
            var employee = EmployeeType.CreateInstance("John", 10);
            var copy = PersonType.CreateInstance(employee).WrapIfValueType();
            VerifyFields(copy, new { name = "John", age = 10 });
        }

        [Fact()]
        public void TestInvokeCtorWithOutArgument()
        {
            RunWith(type =>
               {
                   var arguments = new object[] { "John", 10, 0 };
                   var person = type.CreateInstance(new[] { typeof(string), typeof(int), typeof(int).MakeByRefType() },
                                                     arguments).WrapIfValueType();
                   VerifyFields(person, new { name = "John", age = 10 });
                   Assert.True((int)arguments[2] > 0);
               });
        }

        [Fact()]
        public void TestInvokeCtorWithArrayArgument()
        {
            var employee = EmployeeType.CreateInstance(new[] { EmployeeType.MakeArrayType() },
                                                        new[] { new Employee[0] });
            Assert.NotNull(employee);
            Assert.Equal(0, employee.GetPropertyValue("Subordinates").GetPropertyValue("Length"));
        }

        [Fact()]
        public void TestInvokeMissingCtor()
        {
            Assert.Throws<MissingMemberException>(() =>
            RunWith(type => type.CreateInstance("oneStringArgument")));
        }

        [Fact()]
        public void TestInvokeCtorWithNullParametersTheRightWay()
        {
            RunWith(type =>
               {
                   var person = type.CreateInstance(new[] { typeof(string), typeof(int) },
                                                     null, 10).WrapIfValueType();
                   VerifyFields(person, new { name = (string)null, age = 10 });
               });
        }

        [Fact()]
        public void TestInvokeNoArgCtorViaConstructorInfo()
        {
            ConstructorInfo ctorInfo = PersonType.Constructor();
            var person = ctorInfo.CreateInstance().WrapIfValueType();
            VerifyFields(person, new { name = string.Empty, age = 0 });
        }

        [Fact()]
        public void TestInvokeCtorViaConstructorInfo()
        {
            RunWith(type =>
               {
                   ConstructorInfo ctorInfo = type.Constructor(typeof(string), typeof(int));
                   var person = ctorInfo.CreateInstance(null, 10).WrapIfValueType();
                   VerifyFields(person, new { name = (string)null, age = 10 });
               });
        }

        [Fact()]
        public void TestInvokeViaAssemblyScanner()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var list1 = assembly.CreateInstances<ISwimmable>();
            Assert.Equal(1, list1.Count);
            Assert.IsAssignableFrom<ISwimmable>(list1[0]);

            var list2 = assembly.CreateInstances<ISlide>();
            Assert.Equal(1, list2.Count);
            Assert.IsAssignableFrom<ISlide>(list2[0]);

            var list3 = assembly.CreateInstances<Mammal>();
            Assert.Equal(2, list3.Count); // Elephant + Lion (Giraffe has no default ctor)
            list3.ForEach(o => Assert.IsAssignableFrom<Mammal>(o));
        }
    }
}