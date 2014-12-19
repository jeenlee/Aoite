

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Aoite.Reflection;
using Aoite.ReflectionTest.SampleModel.Animals;
using Xunit;

namespace Aoite.ReflectionTest.Lookup
{

    public class MemberTest : BaseLookupTest
    {
        #region Single Member
        [Fact()]
        public void TestMemberInstance()
        {
            MemberInfo member = typeof(object).Member("id");
            Assert.Null(member);

            AnimalInstanceMemberNames.Select(s => typeof(Animal).Member(s)).ForEach(Assert.NotNull);
            LionInstanceMemberNames.Select(s => typeof(Lion).Member(s)).ForEach(Assert.NotNull);
        }

        [Fact()]
        public void TestMemberInstanceIgnoreCase()
        {
            Flags flags = Flags.InstanceAnyVisibility | Flags.IgnoreCase;

            AnimalInstanceMemberNames.Select(s => s.ToUpper()).Select(s => typeof(Animal).Member(s, flags)).ForEach(Assert.NotNull);
            LionInstanceMemberNames.Select(s => s.ToUpper()).Select(s => typeof(Lion).Member(s, flags)).ForEach(Assert.NotNull);
        }

        [Fact()]
        public void TestMemberInstanceDeclaredOnly()
        {
            Flags flags = Flags.InstanceAnyVisibility | Flags.DeclaredOnly;

            AnimalInstanceMemberNames.Select(s => typeof(Animal).Member(s, flags)).ForEach(Assert.NotNull);
            LionDeclaredInstanceMemberNames.Select(s => typeof(Lion).Member(s, flags)).ForEach(Assert.NotNull);
        }

        [Fact()]
        public void TestMemberStatic()
        {
            Flags flags = Flags.StaticAnyVisibility;

            AnimalInstanceMemberNames.Select(s => typeof(Animal).Member(s, flags)).ForEach(Assert.Null);

            AnimalStaticMemberNames.Select(s => typeof(Animal).Member(s, flags)).ForEach(Assert.NotNull);
            AnimalStaticMemberNames.Select(s => typeof(Lion).Member(s, flags)).ForEach(Assert.NotNull);
        }

        [Fact()]
        public void TestMemberStaticDeclaredOnly()
        {
            Flags flags = Flags.StaticAnyVisibility | Flags.DeclaredOnly;

            AnimalStaticMemberNames.Select(s => typeof(Animal).Member(s, flags)).ForEach(Assert.NotNull);
            AnimalStaticMemberNames.Select(s => typeof(Lion).Member(s, flags)).ForEach(Assert.Null);
        }
        #endregion

        #region Multiple Members
        [Fact()]
        public void TestMembersInstance()
        {
            IList<MemberInfo> members = typeof(object).Members(Flags.InstanceAnyVisibility).OrderBy(m => m.Name).ToList();
            Assert.NotNull(members);
            Assert.Equal(0, members.Count);

            members = typeof(Animal).Members(Flags.InstanceAnyVisibility);
            Assert.Equal(AnimalInstanceMemberNames.Length, members.Count);
            Assert.Equal(AnimalInstanceMemberNames.OrderBy(n => n), members.Select(m => m.Name).OrderBy(n => n).ToArray());
            Assert.Equal(AnimalInstanceMemberTypes.OrderBy(t => t), members.Select(m => m.MemberType).OrderBy(t => t).ToArray());

            members = typeof(Mammal).Members(Flags.InstanceAnyVisibility);
            Assert.Equal(AnimalInstanceMemberNames.Length + MammalDeclaredInstanceMemberNames.Length, members.Count);

            members = typeof(Lion).Members(Flags.InstanceAnyVisibility);
            Assert.Equal(LionInstanceMemberNames.Length, members.Count);
            Assert.Equal(LionInstanceMemberNames.OrderBy(n => n), members.Select(m => m.Name).OrderBy(n => n).ToArray());
            Assert.Equal(LionInstanceMemberTypes.OrderBy(t => t), members.Select(m => m.MemberType).OrderBy(t => t).ToArray());
        }

        [Fact()]
        public void TestMembersInstanceWithDeclaredOnlyFlag()
        {
            IList<MemberInfo> members = typeof(object).Members(Flags.InstanceAnyVisibility | Flags.DeclaredOnly);
            Assert.NotNull(members);
            Assert.Equal(0, members.Count);

            members = typeof(Animal).Members(Flags.InstanceAnyVisibility | Flags.DeclaredOnly);
            Assert.Equal(AnimalInstanceMemberNames.Length, members.Count);
            Assert.Equal(AnimalInstanceMemberNames.OrderBy(n => n), members.Select(m => m.Name).OrderBy(n => n).ToArray());
            Assert.Equal(AnimalInstanceMemberTypes.OrderBy(t => t), members.Select(m => m.MemberType).OrderBy(t => t).ToArray());

            members = typeof(Mammal).Members(Flags.InstanceAnyVisibility | Flags.DeclaredOnly);
            Assert.Equal(MammalDeclaredInstanceMemberNames.Length, members.Count);

            members = typeof(Lion).Members(Flags.InstanceAnyVisibility | Flags.DeclaredOnly);
            Assert.Equal(LionDeclaredInstanceMemberNames.Length, members.Count);
            Assert.Equal(LionDeclaredInstanceMemberNames.OrderBy(n => n), members.Select(m => m.Name).OrderBy(n => n).ToArray());
            Assert.Equal(LionDeclaredInstanceMemberTypes.OrderBy(t => t), members.Select(m => m.MemberType).OrderBy(t => t).ToArray());
        }

        [Fact()]
        public void TestMembersStatic()
        {
            IList<MemberInfo> members = typeof(object).Members(Flags.StaticAnyVisibility);
            Assert.NotNull(members);
            Assert.Equal(0, members.Count);

            members = typeof(Animal).Members(Flags.StaticAnyVisibility);
            Assert.Equal(AnimalStaticMemberNames.Length, members.Count);
            Assert.Equal(AnimalStaticMemberNames.OrderBy(n => n), members.Select(m => m.Name).OrderBy(n => n).ToArray());
            Assert.Equal(AnimalStaticMemberTypes.OrderBy(n => n), members.Select(m => m.MemberType).OrderBy(n => n).ToArray());

            members = typeof(Lion).Members(Flags.StaticAnyVisibility);
            Assert.Equal(AnimalStaticMemberNames.Length, members.Count);
            Assert.Equal(AnimalStaticMemberNames.OrderBy(n => n), members.Select(m => m.Name).OrderBy(n => n).ToArray());
            Assert.Equal(AnimalStaticMemberTypes.OrderBy(n => n), members.Select(m => m.MemberType).OrderBy(n => n).ToArray());
        }

        [Fact()]
        public void TestMembersStaticWithDeclaredOnlyFlag()
        {
            IList<MemberInfo> members = typeof(object).Members(Flags.StaticAnyVisibility | Flags.DeclaredOnly);
            Assert.NotNull(members);
            Assert.Equal(0, members.Count);

            members = typeof(Animal).Members(Flags.StaticAnyVisibility | Flags.DeclaredOnly);
            Assert.Equal(AnimalStaticMemberNames.Length, members.Count);
            Assert.Equal(AnimalStaticMemberNames.OrderBy(n => n), members.Select(m => m.Name).OrderBy(n => n).ToArray());
            Assert.Equal(AnimalStaticMemberTypes.OrderBy(n => n), members.Select(m => m.MemberType).OrderBy(n => n).ToArray());

            members = typeof(Mammal).Members(Flags.StaticAnyVisibility | Flags.DeclaredOnly);
            Assert.Equal(0, members.Count);

            members = typeof(Lion).Members(Flags.StaticAnyVisibility | Flags.DeclaredOnly);
            Assert.Equal(0, members.Count);
        }
        #endregion

        #region ExcludeBackingMembers and ExcludeHiddenMembers
        public class Person
        {
            public virtual string Name { get; protected set; }
            public virtual void Foo() { }
        }
        public class Employee : Person
        {
            public override string Name { get; protected set; }
            new public virtual void Foo() { }
            public void Foo(int foo) { }
        }
        public class Manager : Employee
        {
            public override void Foo() { }
            public void Foo(int foo, int bar) { }
        }

        [Fact()]
        public void TestWithExcludeBackingMembers()
        {
            Flags flags = Flags.InstanceAnyVisibility | Flags.ExcludeBackingMembers;

            IList<PropertyInfo> properties = typeof(Employee).Properties("Name");
            Assert.Equal(2, properties.Count);
            Assert.Equal(typeof(Employee), properties.First().DeclaringType);

            properties = typeof(Employee).Properties(flags, "Name");
            Assert.Equal(1, properties.Count);
            Assert.Equal(typeof(Employee), properties.First().DeclaringType);

            MemberTypes memberTypes = MemberTypes.Method | MemberTypes.Field | MemberTypes.Property;
            IList<MemberInfo> members = typeof(Employee).Members(memberTypes);
            Assert.Equal(11, members.Count);
            Assert.Equal(typeof(Employee), members.First(m => m.Name == "Foo").DeclaringType);
            Assert.Equal(typeof(Employee), members.First(m => m.Name == "Name").DeclaringType);

            members = typeof(Employee).Members(memberTypes, flags);
            Assert.Equal(3, members.Count);
            Assert.Equal(typeof(Employee), members.First(m => m.Name == "Name").DeclaringType);
        }

        [Fact()]
        public void TestWithExcludeHiddenMembers()
        {
            Flags flags = Flags.InstanceAnyVisibility | Flags.ExcludeHiddenMembers;

            IList<PropertyInfo> properties = typeof(Manager).Properties("Name");
            Assert.Equal(2, properties.Count);
            Assert.Equal(typeof(Employee), properties.First().DeclaringType);

            properties = typeof(Manager).Properties(flags, "Name");
            Assert.Equal(1, properties.Count);
            Assert.Equal(typeof(Employee), properties.First().DeclaringType);

            MemberTypes memberTypes = MemberTypes.Method | MemberTypes.Field | MemberTypes.Property;
            IList<MemberInfo> members = typeof(Manager).Members(memberTypes);
            Assert.Equal(13, members.Count);
            Assert.Equal(typeof(Manager), members.First().DeclaringType);

            members = typeof(Manager).Members(memberTypes, flags);
            Assert.Equal(7, members.Count);
            Assert.Equal(typeof(Manager), members.First().DeclaringType);

            members = typeof(Manager).Members(memberTypes, flags | Flags.ExcludeBackingMembers);
            Assert.Equal(4, members.Count);
            Assert.Equal(typeof(Manager), members.First().DeclaringType);

        }
        #endregion

        #region Member Helpers (HasName, Type, IsReadable/IsWritable)
        [Fact()]
        public void TestMemberHasName()
        {
            var member = typeof(Lion).Member("lastMealTime");
            Assert.NotNull(member);
            Assert.True(member.HasName("lastMealTime"));
            Assert.True(member.HasName("_lastMealTime"));
        }

        [Fact()]
        public void TestMemberType()
        {
            var member = typeof(Lion).Member("lastMealTime");
            Assert.NotNull(member);
            Assert.Equal(typeof(DateTime), member.Type());
        }

        [Fact()]
        public void TestMemberIsReadableIsWritable()
        {
            // normal instance field
            var member = typeof(Lion).Member("lastMealTime");
            Assert.NotNull(member);
            Assert.True(member.IsReadable());
            Assert.True(member.IsWritable());
            // normal instance property
            member = typeof(Lion).Member("Name");
            Assert.NotNull(member);
            Assert.True(member.IsReadable());
            Assert.True(member.IsWritable());

            // const field
            member = typeof(Zoo).Member("FirstId", Flags.StaticAnyVisibility);
            Assert.NotNull(member);
            Assert.True(member.IsReadable());
            Assert.False(member.IsWritable());
            // static field
            member = typeof(Zoo).Member("nextId", Flags.StaticAnyVisibility);
            Assert.NotNull(member);
            Assert.True(member.IsReadable());
            Assert.True(member.IsWritable());
            // readonly instance field
            member = typeof(Zoo).Member("name");
            Assert.NotNull(member);
            Assert.True(member.IsReadable());
            Assert.False(member.IsWritable());
            // read-only instance property
            member = typeof(Zoo).Member("Name");
            Assert.NotNull(member);
            Assert.True(member.IsReadable());
            Assert.False(member.IsWritable());
            // write-only instance property
            member = typeof(Zoo).Member("Alias");
            Assert.NotNull(member);
            Assert.False(member.IsReadable());
            Assert.True(member.IsWritable());
        }
        #endregion
    }
}
