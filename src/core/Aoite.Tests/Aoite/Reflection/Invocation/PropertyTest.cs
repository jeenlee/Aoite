#region License
// Copyright 2010 Buu Nguyen, Morten Mertner
// 
// Licensed under the Apache License, Version 2.0 (the "License"); 
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at 
// 
// http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, 
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
// See the License for the specific language governing permissions and 
// limitations under the License.
// 
// The latest version of this file can be found at http://Aoite.Reflection.codeplex.com/
#endregion

using System;
using System.Reflection;
using Aoite.Reflection;
using Aoite.ReflectionTest.SampleModel.People;
using Xunit;

namespace Aoite.ReflectionTest.Invocation
{

    public class PropertyTest : BaseInvocationTest
    {
        [Fact()]
        public void TestAccessStaticProperties()
        {
            RunWith((Type type) =>
                     {
                         int totalPeopleCreated = (int)type.GetPropertyValue("TotalPeopleCreated") + 1;
                         type.SetPropertyValue("TotalPeopleCreated", totalPeopleCreated);
                         VerifyProperties(type, new { totalPeopleCreated });
                     });
        }

        [Fact()]
        public void TestGetPublicStaticPropertyUnderNonPublicBindingFlags()
        {
            Assert.Throws<MissingMemberException>(() =>
            RunWith((Type type) => type.GetPropertyValue("TotalPeopleCreated", Flags.NonPublic | Flags.Instance)));
        }

        [Fact()]
        public void TestSetPublicStaticPropertyUnderNonPublicBindingFlags()
        {
            Assert.Throws<MissingMemberException>(() =>
            RunWith((Type type) => type.SetPropertyValue("TotalPeopleCreated", 2, Flags.NonPublic | Flags.Instance)));
        }

        [Fact()]
        public void TestAccessStaticPropertyViaPropertyInfo()
        {
            RunWith((Type type) =>
                     {
                         PropertyInfo propInfo = type.Property("TotalPeopleCreated", Flags.StaticAnyVisibility);
                         int totalPeopleCreated = (int)propInfo.Get() + 1;
                         propInfo.Set(totalPeopleCreated);
                         VerifyProperties(type, new { totalPeopleCreated });
                     });
        }

        [Fact()]
        public void TestAccessInstanceProperties()
        {
            RunWith((object person) =>
                     {
                         string name = (string)person.GetPropertyValue("Name") + " updated";
                         person.SetPropertyValue("Name", name);
                         VerifyProperties(person, new { name });
                     });
        }

        [Fact()]
        public void TestGetPublicPropertyUnderNonPublicBindingFlags()
        {
            Assert.Throws<MissingMemberException>(() =>
            RunWith((object person) => person.GetPropertyValue("Name", Flags.NonPublic | Flags.Instance)));
        }

        [Fact()]
        public void TestSetPublicPropertyUnderNonPublicBindingFlags()
        {
            Assert.Throws<MissingMemberException>(() =>
            RunWith((object person) => person.SetPropertyValue("Name", "John", Flags.NonPublic | Flags.Instance)));
        }

        [Fact()]
        public void TestAccessInstancePropertyViaPropertyInfo()
        {
            RunWith((object person) =>
                     {
                         PropertyInfo propInfo = person.UnwrapIfWrapped().GetType().Property("Name");
                         string name = (string)propInfo.Get(person) + " updated";
                         propInfo.Set(person, name);
                         VerifyProperties(person, new { name });
                     });
        }

        [Fact()]
        public void TestChainInstancePropertySetters()
        {
            RunWith((object person) =>
                     {
                         person.SetPropertyValue("Name", "John")
                             .SetPropertyValue("Age", 20)
                             .SetPropertyValue("MetersTravelled", 120d);
                         VerifyProperties(person, new { Name = "John", Age = 20, MetersTravelled = 120d });
                     });
        }

        [Fact()]
        public void TestAccessStaticPropertiesViaSubclassType()
        {
            int totalPeopleCreated = (int)EmployeeType.GetPropertyValue("TotalPeopleCreated") + 1;
            EmployeeType.SetPropertyValue("TotalPeopleCreated", totalPeopleCreated);
            VerifyProperties(EmployeeType, new { totalPeopleCreated });
        }

        [Fact()]
        public void TestSetNotExistentProperty()
        {
            Assert.Throws<MissingMemberException>(() =>
            RunWith((object person) => person.SetPropertyValue("not_exist", null)));
        }

        [Fact()]
        public void TestSetNotExistentStaticProperty()
        {
            Assert.Throws<MissingMemberException>(() =>
            RunWith((Type type) => type.SetPropertyValue("not_exist", null)));
        }

        [Fact()]
        public void TestGetNotExistentProperty()
        {
            Assert.Throws<MissingMemberException>(() =>
            RunWith((object person) => person.GetPropertyValue("not_exist")));
        }

        [Fact()]
        public void TestGetNotExistentStaticProperty()
        {
            Assert.Throws<MissingMemberException>(() =>
            RunWith((Type type) => type.GetPropertyValue("not_exist")));
        }

        [Fact()]
        public void TestSetInvalidValueType()
        {
            Assert.Throws<InvalidCastException>(() =>
            RunWith((object person) => person.SetPropertyValue("MetersTravelled", 1)));
        }

        [Fact()]
        public void TestSetNullToValueType()
        {
            Assert.Throws<NullReferenceException>(() =>
            RunWith((object person) => person.SetPropertyValue("Age", null)));
        }

        [Fact()]
        public void TestGetPropertyUsingMemberExpression()
        {
            var person = new Person("Arthur", 32);
            Assert.Equal("Arthur", person.GetPropertyValue(() => person.Name));
            person.SetPropertyValue(() => person.Name, "Ford");
            Assert.Equal("Ford", person.Name);
        }
    }
}