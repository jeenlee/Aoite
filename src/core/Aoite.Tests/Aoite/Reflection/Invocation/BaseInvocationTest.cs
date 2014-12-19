
using System;
using Aoite.ReflectionTest.Common;
using Aoite.ReflectionTest.SampleModel.People;

namespace Aoite.ReflectionTest.Invocation
{
    public abstract class BaseInvocationTest : BaseTest
    {
        protected static readonly Type EmployeeType = typeof(Employee);
        protected static readonly Type PersonType = typeof(Person);
        protected static readonly Type PersonStructType = typeof(PersonStruct);

        protected BaseInvocationTest()
            : base(new[] { PersonType, PersonStructType })
        {
        }

        protected BaseInvocationTest(Type classType, Type structType)
            : base(new[] { classType, structType })
        {
        }
    }
}