

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Aoite.Reflection;
using Xunit;

namespace Aoite.ReflectionTest.Services
{

    public class CloneTest
    {
        #region Sample Reflection Classes
        private class Dynamic
        {
            public dynamic number;
            public dynamic person;
            public dynamic expando;

            public Dynamic()
            {
            }

            public Dynamic( dynamic number, dynamic person, dynamic expando )
            {
                this.number = number;
                this.person = person;
                this.expando = expando;
            }
        }

        private class Person
        {
            private DateTime lastModified = DateTime.Now;
            private Type myType = typeof(Person);
            public int Id { get; set; }
            public DateTime Birthday { get; set; }
            public string Name { get; set; }
            public int Age { get { return DateTime.Now.Year - Birthday.Year + (DateTime.Now.DayOfYear >= Birthday.DayOfYear ? 1 : 0); } }
            public DateTime LastModified { get { return lastModified; } }
            public string[] Roles { get; private set; }
            public Dictionary<string, Person> Family { get; private set; }

            public Person()
            {
            }
            public Person(int id, DateTime birthday, string name)
            {
                Id = id;
                Birthday = birthday;
                Name = name;
                Roles = new[] { "User", "Tester" };
                Family = new Dictionary<string, Person>();
            }
        }

        private class Employee : Person
        {
            private string initials;

            public string Initials { get { return initials; } }
            public Employee Manager { get; set; }

            public Employee()
            {
            }

            public Employee(int id, DateTime birthday, string name, string initials)
                : base(id, birthday, name)
            {
                this.initials = initials;
            }

            public Employee(int id, DateTime birthday, string name, string initials, Employee manager)
                : base(id, birthday, name)
            {
                this.initials = initials;
                Manager = manager;
            }
        }
        #endregion

        #region DeepClone Sample Classes
        class AddressPoco
        {
            public string Street { get; set; }
            public string City { get; set; }
            public string ZipCode { get; set; }
        }

        class PersonPoco
        {
            public string Name { get; set; }
            public int Ssn { get; set; }
            public string Employer { get; set; }
            public AddressPoco Address { get; set; }
        }

        class LibraryPoco : Dictionary<string, string>
        {
            public string Name { get; set; }
            public AddressPoco Address { get; set; }
            public List<PersonPoco> WaitingList { get; set; }
        }
        #endregion

        #region DeepClone
        [Fact()]
        public void TestDeepCloneSimpleObject()
        {
            DateTime birthday = new DateTime(1973, 1, 27);
            Person person = new Person(42, birthday, "Arthur Dent");
            Person clone = person.DeepClone();
            Verify(person, clone);
        }

        [Fact()]
        public void TestDeepCloneWithSelfReference()
        {
            DateTime birthday = new DateTime(1973, 1, 27);
            Employee employee = new Employee(42, birthday, "Arthur Dent", "AD");
            employee.Manager = employee;
            Employee clone = employee.DeepClone();
            Verify(employee, clone);
            Assert.Equal(employee.Initials, clone.Initials);
            Assert.NotSame(employee.Manager, clone.Manager);
            Verify(employee.Manager, clone.Manager);
            Assert.Equal(employee.Manager.Initials, clone.Manager.Initials);
        }

        [Fact()]
        public void TestDeepCloneWithCyclicObjectGraph()
        {
            DateTime birthday = new DateTime(1973, 1, 27);
            Employee manager = new Employee(1, birthday, "Ford Prefect", "FP");
            manager.Manager = manager;
            Employee employee = new Employee(2, birthday, "Arthur Dent", "AD", manager);

            Employee clone = employee.DeepClone();
            Verify(employee, clone);
            Verify(employee.Manager, clone.Manager);
            Verify(employee.Manager.Manager, clone.Manager.Manager);
            Assert.NotSame(employee.Manager, clone.Manager);
            Assert.Same(clone.Manager, clone.Manager.Manager);
        }

        [Fact()]
        public void TestDeepCloneWithComplexObjectGraph()
        {
            // arrange test objects
            var personAddress = new AddressPoco { City = "Copenhagen", ZipCode = "2300" };
            var libraryAddress = new AddressPoco { City = "London" };
            var otherAddress = new AddressPoco { City = "Berlin" };
            var arthur = new PersonPoco { Name = "Arthur Dent", Address = personAddress, Employer = "British Tea Company", Ssn = 123 };
            var trish = new PersonPoco { Name = "Trish", Address = personAddress, Employer = "", Ssn = 456 };
            var ford = new PersonPoco { Name = "Ford Prefect", Address = otherAddress, Employer = "Ursa Minor Publishing", Ssn = 789 };
            var library = new LibraryPoco
                          {
                              Address = libraryAddress,
                              Name = "The Library",
                              WaitingList = new List<PersonPoco> { arthur, trish, ford },
                          };
            library["foo"] = "bar";
            library["h2g2"] = "dont panic";

            library.Values.Contains("foo");
            // deep clone
            var clone = library.DeepClone();

            // verify clone
            Assert.Equal(2, clone.Keys.Count);
            Assert.Equal(3, clone.WaitingList.Count);
            Assert.Same(clone.WaitingList[0].Address, clone.WaitingList[1].Address);
            Assert.NotSame(library.Values, clone.Values);
            Assert.Equal(library.Values.Count, clone.Values.Count);
        }
        #endregion

        #region Verify Helpers
        private static void Verify(Person person, Person clone)
        {
            Assert.NotNull(clone);
            Assert.NotSame(person, clone);
            Assert.Equal(person.Id, clone.Id);
            Assert.Equal(person.Birthday, clone.Birthday);
            Assert.Equal(person.Name, clone.Name);
            Assert.Equal(person.LastModified, clone.LastModified);
            Assert.Equal(person.Roles, clone.Roles);
        }
        private static void Verify(Employee employee, Employee clone)
        {
            Verify(employee as Person, clone as Person);
            Assert.Equal(employee.Initials, clone.Initials);
            Assert.NotSame(employee.Manager, clone.Manager);
        }
        #endregion

        #region Dynamic
        [Fact()]
        public void TestDynamic()
        {
            var person = new Person(42, DateTime.Now, "Arthur Dent");
            dynamic expando = new ExpandoObject();
            expando.person = person;
            expando.number = 15;
            var dynamic = new Dynamic(15, person, expando);
            expando.dynamic = dynamic;
            var clone = dynamic.DeepClone();
            Verify(dynamic, clone);
            Verify(dynamic.expando.dynamic, clone.expando.dynamic);
        }

        private static void Verify(dynamic dynamic, dynamic clone)
        {
            Assert.Equal(dynamic.number, clone.number);
            Verify(dynamic.person, clone.person);
            Assert.Equal(dynamic.expando.number, clone.expando.number);
            Verify((Person)dynamic.expando.person, (Person)clone.expando.person);
        }
        #endregion
    }
}
