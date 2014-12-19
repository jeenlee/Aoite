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

    public class FieldTest : BaseInvocationTest
    {
        [Fact()]
        public void TestAccessStaticFields()
        {
            RunWith((Type type) =>
                     {
                         int totalPeopleCreated = (int)type.GetFieldValue("totalPeopleCreated") + 1;
                         type.SetFieldValue("totalPeopleCreated", totalPeopleCreated);
                         VerifyFields(type, new { totalPeopleCreated });
                     });
        }

        [Fact()]
        public void TestGetPrivateStaticFieldUnderPublicBindingFlags()
        {
            Assert.Throws<MissingFieldException>(() =>
            RunWith((Type type) => type.GetFieldValue("totalPeopleCreated", Flags.Public | Flags.Instance)));
        }

        [Fact()]
        public void TestSetPrivateStaticFieldUnderPublicBindingFlags()
        {
            Assert.Throws<MissingFieldException>(() =>
            RunWith((Type type) => type.SetFieldValue("totalPeopleCreated", 2, Flags.Public | Flags.Instance)));
        }

        [Fact()]
        public void TestAccessStaticFieldViaFieldInfo()
        {
            RunWith((Type type) =>
                     {
                         FieldInfo fieldInfo = type.Field("totalPeopleCreated", Flags.StaticAnyVisibility);
                         int totalPeopleCreated = (int)fieldInfo.Get() + 1;
                         fieldInfo.Set(totalPeopleCreated);
                         VerifyFields(type, new { totalPeopleCreated });
                     });
        }

        [Fact()]
        public void TestAccessInstanceFields()
        {
            RunWith((object person) =>
                     {
                         string name = (string)person.GetFieldValue("name") + " updated";
                         person.SetFieldValue("name", name);
                         VerifyFields(person, new { name });
                     });
        }

        [Fact()]
        public void TestAccessPrivateFieldUnderNonPublicBindingFlags()
        {
            RunWith((object person) =>
                     {
                         string name = (string)person.GetFieldValue("name", Flags.NonPublic | Flags.Instance) + " updated";
                         person.SetFieldValue("name", name, Flags.NonPublic | Flags.Instance);
                         VerifyFields(person, new { name });
                     });
        }

        [Fact()]
        public void TestGetPrivateFieldUnderPublicBindingFlags()
        {
            Assert.Throws<MissingFieldException>(() =>
            RunWith((object person) => person.GetFieldValue("name", Flags.Public | Flags.Instance)));
        }

        [Fact()]
        public void TestSetPrivateFieldUnderPublicBindingFlags()
        {
            Assert.Throws<MissingFieldException>(() =>
            RunWith((object person) => person.SetFieldValue("name", "John", Flags.Public | Flags.Instance)));
        }

        [Fact()]
        public void TestAccessInstanceFieldViaFieldInfo()
        {
            RunWith((object person) =>
                     {
                         FieldInfo fieldInfo = person.UnwrapIfWrapped().GetType().Field("name");
                         string name = (string)fieldInfo.Get(person) + " updated";
                         fieldInfo.Set(person, name);
                         VerifyFields(person, new { name });
                     });
        }

        [Fact()]
        public void TestChainInstanceFieldSetters()
        {
            RunWith((object person) =>
                     {
                         person.SetFieldValue("name", "John")
                             .SetFieldValue("age", 20)
                             .SetFieldValue("metersTravelled", 120d);
                         VerifyFields(person, new { name = "John", age = 20, metersTravelled = 120d });
                     });
        }

        [Fact()]
        public void TestAccessStaticFieldsViaSubclassType()
        {
            int totalPeopleCreated = (int)EmployeeType.GetFieldValue("totalPeopleCreated") + 1;
            EmployeeType.SetFieldValue("totalPeopleCreated", totalPeopleCreated);
            VerifyFields(EmployeeType, new { totalPeopleCreated });
        }

        [Fact()]
        public void TestSetNotExistentField()
        {
            Assert.Throws<MissingFieldException>(() =>
            RunWith((object person) => person.GetFieldValue("not_exist", Flags.Public | Flags.Instance)));
        }

        [Fact()]
        public void TestSetNotExistentStaticField()
        {
            Assert.Throws<MissingFieldException>(() =>
            RunWith((Type type) => type.SetFieldValue("not_exist", null)));
        }

        [Fact()]
        public void TestGetNotExistentField()
        {
            Assert.Throws<MissingFieldException>(() =>
            RunWith((object person) => person.GetFieldValue("not_exist")));
        }

        [Fact()]
        public void TestGetNotExistentStaticField()
        {
            Assert.Throws<MissingFieldException>(() =>
            RunWith((Type type) => type.GetFieldValue("not_exist")));
        }

        [Fact()]
        public void TestSetInvalidValueType()
        {
            Assert.Throws<InvalidCastException>(() =>
            RunWith((object person) => person.SetFieldValue("metersTravelled", 1)));
        }

        [Fact()]
        public void TestSetNullToValueType()
        {
            Assert.Throws<NullReferenceException>(() =>
            RunWith((object person) => person.SetFieldValue("age", null)));
        }
    }
}