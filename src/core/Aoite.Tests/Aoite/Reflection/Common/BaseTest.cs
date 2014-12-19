

using System;
using System.Linq;
using Aoite.Reflection;
using Xunit;

namespace Aoite.ReflectionTest.Common
{
    public abstract class BaseTest
    {
        protected readonly Type[] Types;

        protected BaseTest(Type[] types)
        {
            Types = types;
        }

        protected static void VerifyProperties(Type type, object sample)
        {
            var properties = sample.GetType().Properties();
            properties.ForEach(propInfo => Assert.Equal(propInfo.Get(sample),
                                                             type.GetPropertyValue(propInfo.Name.FirstCharUpper())));
        }

        protected static void VerifyProperties(object obj, object sample)
        {
            var properties = sample.GetType().Properties();
            properties.ForEach(propInfo => Assert.Equal(propInfo.Get(sample),
                                                             obj.GetPropertyValue(propInfo.Name.FirstCharUpper())));
        }

        protected static void VerifyFields(Type type, object sample)
        {
            var properties = sample.GetType().Properties();
            properties.ForEach(propInfo => Assert.Equal(propInfo.Get(sample), type.GetFieldValue(propInfo.Name.FirstCharLower())));
        }

        protected static void VerifyFields(object obj, object sample)
        {
            var properties = sample.GetType().Properties();
            properties.ForEach(propInfo => Assert.Equal(propInfo.Get(sample), obj.GetFieldValue(propInfo.Name.FirstCharLower())));
        }

        protected void RunWith(Action<Type> assertionAction)
        {
            Types.ForEach(assertionAction);
        }

        protected void RunWith(Action<object> assertionAction)
        {
            Types.Select(t => t.CreateInstance().WrapIfValueType()).ForEach(assertionAction);
        }
    }
}