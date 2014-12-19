

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Aoite.Reflection;
using Xunit;

namespace Aoite.ReflectionTest.Lookup
{

    public class ConstructorTest
    {
        #region Sample Classes
        private class PersonClass
        {
            public int Age;
            public int? Id;
            public string Name;
            public object Data;
            public PersonClass Peer;
            public PersonClass[] Peers;

            private PersonClass() { }
            internal PersonClass(out int i, out string s)
            {
                i = 1;
                s = "changed";
            }
            internal PersonClass(int age) { Age = age; }
            internal PersonClass(int? id) { Id = id; }
            protected PersonClass(string name) { Name = name; }
            protected PersonClass(object data) { Data = data; }
            public PersonClass(PersonClass peer) { Peer = peer; }
            public PersonClass(PersonClass[] peers) { Peers = peers; }
            internal PersonClass(int age, int? id, string name, PersonClass peer, PersonClass[] peers)
            {
                Age = age;
                Id = id;
                Name = name;
                Peer = peer;
                Peers = peers;
            }
        }

        private class PersonStruct
        {
            public int Age;
            public int? Id;
            public string Name;
            public object Data;
            public PersonClass Peer;
            public PersonClass[] Peers;

            private PersonStruct() { }
            internal PersonStruct(out int i, out string s)
            {
                i = 1;
                s = "changed";
            }
            internal PersonStruct(int age) { Age = age; }
            internal PersonStruct(int? id) { Id = id; }
            protected PersonStruct(string name) { Name = name; }
            protected PersonStruct(object data) { Data = data; }
            public PersonStruct(PersonClass peer) { Peer = peer; }
            public PersonStruct(PersonClass[] peers) { Peers = peers; }
            internal PersonStruct(int age, int? id, string name, PersonClass peer, PersonClass[] peers)
            {
                Age = age;
                Id = id;
                Name = name;
                Peer = peer;
                Peers = peers;
            }
        }

        class Employee : PersonClass
        {
            internal Employee(int age) : base(age) { }
        }

        private static readonly List<Type> TypeList = new List<Type>
                {
                    typeof(PersonClass), 
                    typeof(PersonStruct)
                };
        #endregion

        #region Constructor Lookup Tests
        [Fact()]
        public void TestFindAllConstructorsOnPersonClassShouldFindNine()
        {
            IList<ConstructorInfo> constructors = typeof(PersonClass).Constructors().ToList();
            Assert.NotNull(constructors);
            Assert.Equal(9, constructors.Count);
        }

        [Fact()]
        public void TestFindSpecificConstructorOnPersonClassShouldReturnNullForNoMatch()
        {
            Type[] paramTypes = new[] { typeof(int), typeof(int) };
            Assert.Null(typeof(PersonClass).Constructor(paramTypes));
        }

        [Fact()]
        public void TestFindSpecificConstructorOnPersonClassShouldReturnSingleForMatch()
        {
            Type[] paramTypes = new[] { typeof(int) };
            ConstructorInfo constructor = typeof(PersonClass).Constructor(paramTypes);
            Assert.NotNull(constructor);
            Assert.Equal("age", constructor.GetParameters()[0].Name);

            paramTypes = new[] { typeof(int?) };
            constructor = typeof(PersonClass).Constructor(paramTypes);
            Assert.NotNull(constructor);
            Assert.Equal("id", constructor.GetParameters()[0].Name);
        }

        [Fact()]
        public void TestFindSpecificConstructorsOnPersonClass()
        {
            IList<ConstructorInfo> constructors = typeof(PersonClass).Constructors().ToList();
            Assert.NotNull(constructors);
            Assert.Equal(9, constructors.Count);
        }

        [Fact()]
        public void TestConstructorLookupOnEmployeeShouldFindOne()
        {
            IList<ConstructorInfo> constructors = typeof(Employee).Constructors().ToList();
            Assert.NotNull(constructors);
            Assert.Equal(1, constructors.Count);
        }
        #endregion
    }
}
