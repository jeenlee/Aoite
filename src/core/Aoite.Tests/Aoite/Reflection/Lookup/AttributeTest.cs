

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Aoite.Reflection;
using Aoite.ReflectionTest.SampleModel.Animals;
using Aoite.ReflectionTest.SampleModel.Animals.Attributes;
using Aoite.ReflectionTest.SampleModel.Animals.Enumerations;
using Xunit;

namespace Aoite.ReflectionTest.Lookup
{

    public class AttributeTest
    {
        #region Enumerations
        #region Attribute<T>()
        [Fact()]
        public void TestFindSingleAttributeOnEnum_Generic()
        {
            CodeAttribute attr = typeof(Climate).Attribute<CodeAttribute>();
            Assert.NotNull(attr);
            Assert.Equal("Temperature", attr.Code);
        }

        [Fact()]
        public void TestFindSingleAttributeOnEnumField_Generic()
        {
            CodeAttribute attr = Climate.Hot.Attribute<CodeAttribute>();
            Assert.NotNull(attr);
            Assert.Equal("Hot", attr.Code);
            attr = Climate.Any.Attribute<CodeAttribute>();
            Assert.NotNull(attr);
            Assert.Equal("Any", attr.Code);
        }
        #endregion

        #region Attributes()
        [Fact()]
        public void TestFindAllAttributesOnEnum()
        {
            IList<Attribute> attrs = typeof(Climate).Attributes();
            Assert.NotNull(attrs);
            Assert.Equal(2, attrs.Count);
        }

        [Fact()]
        public void TestFindSpecificAttributesOnEnum()
        {
            IList<Attribute> attrs = typeof(Climate).Attributes(typeof(CodeAttribute));
            Assert.NotNull(attrs);
            Assert.Equal(1, attrs.Count);
        }

        [Fact()]
        public void TestFindSpecificAttributesOnEnum_Generic()
        {
            IList<CodeAttribute> attrs = typeof(Climate).Attributes<CodeAttribute>();
            Assert.NotNull(attrs);
            Assert.Equal(1, attrs.Count);
        }

        [Fact()]
        public void TestFindAllAttributesOnEnumField()
        {
            IList<Attribute> attrs = MovementCapabilities.Land.Attributes();
            Assert.NotNull(attrs);
            Assert.Equal(0, attrs.Count);

            attrs = Climate.Cold.Attributes();
            Assert.NotNull(attrs);
            Assert.Equal(1, attrs.Count);
            var codes = attrs.Cast<CodeAttribute>();
            Assert.NotNull(codes.FirstOrDefault(a => a.Code == "Cold"));
        }

        [Fact()]
        public void TestFindSpecificAttributesOnEnumField()
        {
            IList<Attribute> attrs = Climate.Hot.Attributes(typeof(ConditionalAttribute));
            Assert.NotNull(attrs);
            Assert.Equal(0, attrs.Count);

            attrs = Climate.Hot.Attributes(typeof(CodeAttribute));
            Assert.NotNull(attrs);
            Assert.Equal(1, attrs.Count);
            var codes = attrs.Cast<CodeAttribute>();
            Assert.NotNull(codes.FirstOrDefault(a => a.Code == "Hot"));
        }

        [Fact()]
        public void TestFindSpecificAttributesOnEnumField_Generic()
        {
            IList<ConditionalAttribute> empty = Climate.Hot.Attributes<ConditionalAttribute>();
            Assert.NotNull(empty);
            Assert.Equal(0, empty.Count);

            IList<CodeAttribute> attrs = Climate.Hot.Attributes<CodeAttribute>();
            Assert.NotNull(attrs);
            Assert.Equal(1, attrs.Count);
            Assert.NotNull(attrs.FirstOrDefault(a => a.Code == "Hot"));
        }
        #endregion
        #endregion

        #region Find attributes on types
        #region Attribute() and Attribute<T>()
        [Fact()]
        public void TestFindFirstAttributeOnType()
        {
            Attribute attr = typeof(Giraffe).Attribute();
            Assert.NotNull(attr);
            Assert.IsType(typeof(ZoneAttribute), attr);
        }

        [Fact()]
        public void TestFindSpecificAttributeOnType()
        {
            CodeAttribute code = typeof(Lion).Attribute<CodeAttribute>();
            Assert.Null(code);

            ZoneAttribute zone = typeof(Lion).Attribute<ZoneAttribute>();
            Assert.NotNull(zone);
            Assert.Equal(Zone.Savannah, zone.Zone);
        }
        #endregion

        #region Attributes()
        [Fact()]
        public void TestFindAllAttributesOnType()
        {
            IList<Attribute> attrs = typeof(Lion).Attributes();
            Assert.NotNull(attrs);
            Assert.Equal(3, attrs.Count);
            Assert.True(attrs[0] is ZoneAttribute || attrs[0] is SerializableAttribute || attrs[0] is DebuggerDisplayAttribute);
            Assert.True(attrs[1] is ZoneAttribute || attrs[1] is SerializableAttribute || attrs[1] is DebuggerDisplayAttribute);
            Assert.True(attrs[2] is ZoneAttribute || attrs[2] is SerializableAttribute || attrs[2] is DebuggerDisplayAttribute);
        }

        [Fact()]
        public void TestFindSpecificAttributesOnType()
        {
            IList<Attribute> attrs = typeof(Lion).Attributes(typeof(CodeAttribute));
            Assert.NotNull(attrs);
            Assert.Equal(0, attrs.Count);

            attrs = typeof(Lion).Attributes(typeof(ZoneAttribute));
            Assert.NotNull(attrs);
            Assert.Equal(1, attrs.Count);
            Assert.NotNull(attrs.Cast<ZoneAttribute>().FirstOrDefault(a => a.Zone == Zone.Savannah));
        }

        [Fact()]
        public void TestFindSpecificAttributesOnType_Generic()
        {
            IList<CodeAttribute> empty = typeof(Lion).Attributes<CodeAttribute>();
            Assert.NotNull(empty);
            Assert.Equal(0, empty.Count);

            IList<ZoneAttribute> attrs = typeof(Lion).Attributes<ZoneAttribute>();
            Assert.NotNull(attrs);
            Assert.Equal(1, attrs.Count);
            Assert.NotNull(attrs.FirstOrDefault(a => a.Zone == Zone.Savannah));
        }
        #endregion

        #region TypesWith()
        [Fact()]
        public void TestFindTypesWith()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            IList<Type> types = assembly.TypesWith(typeof(CodeAttribute));
            Assert.NotNull(types);
            Assert.Equal(1, types.Count);
            Assert.Equal(typeof(Climate), types[0]);
        }

        [Fact()]
        public void TestFindTypesWithGeneric()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            IList<Type> types = assembly.TypesWith<CodeAttribute>();
            Assert.NotNull(types);
            Assert.Equal(1, types.Count);
            Assert.Equal(typeof(Climate), types[0]);
        }
        #endregion

        #region FieldsAndPropertiesWith()
        [Fact()]
        public void TestFindFieldsAndPropertiesWith_NoMatchShouldReturnEmptyList()
        {
            Type type = typeof(Lion);
            IList<MemberInfo> members = type.FieldsAndPropertiesWith(Flags.InstanceAnyVisibility, typeof(ZoneAttribute));
            Assert.NotNull(members);
            Assert.Equal(0, members.Count);
        }

        [Fact()]
        public void TestFindFieldsAndPropertiesWith_InstanceFieldShouldIncludeInheritedPrivateField()
        {
            Type type = typeof(Lion);
            IList<MemberInfo> members = type.FieldsAndPropertiesWith(Flags.InstanceAnyVisibility, typeof(DefaultValueAttribute));
            Assert.NotNull(members);
            Assert.Equal(2, members.Count);
            Assert.NotNull(members.FirstOrDefault(m => m.Name == "Name"));
            Assert.NotNull(members.FirstOrDefault(m => m.Name == "birthDay"));

            members = type.FieldsAndPropertiesWith(Flags.Instance | Flags.NonPublic, typeof(CodeAttribute));
            Assert.NotNull(members);
            Assert.Equal(2, members.Count);
            Assert.NotNull(members.FirstOrDefault(m => m.Name == "id"));
            Assert.NotNull(members.FirstOrDefault(m => m.Name == "lastMealTime"));
        }

        [Fact()]
        public void TestFinFieldsAndPropertiesWithWith_DefaultFlagsShouldBeInstanceAnyVisibility()
        {
            Type type = typeof(Lion);
            IList<MemberInfo> members = type.FieldsAndPropertiesWith(typeof(DefaultValueAttribute));
            Assert.NotNull(members);
            Assert.Equal(2, members.Count);
            Assert.NotNull(members.FirstOrDefault(m => m.Name == "Name"));
            Assert.NotNull(members.FirstOrDefault(m => m.Name == "birthDay"));
        }
        #endregion

        #region MembersWith()
        [Fact()]
        public void TestFindMembersWith_EmptyOrNullAttributeTypeListShouldReturnAll()
        {
            Type type = typeof(Lion);
            IList<MemberInfo> members = type.MembersWith(MemberTypes.Field, Flags.InstanceAnyVisibility);
            Assert.NotNull(members);
            Assert.Equal(7, members.Count);

            members = type.MembersWith(MemberTypes.Field, Flags.InstanceAnyVisibility, null);
            Assert.NotNull(members);
            Assert.Equal(7, members.Count);
        }

        [Fact()]
        public void TestFindMembersWith_InstanceFieldShouldIncludeInheritedPrivateField()
        {
            Type type = typeof(Lion);
            IList<MemberInfo> members = type.MembersWith(MemberTypes.Field, Flags.InstanceAnyVisibility, typeof(CodeAttribute));
            Assert.NotNull(members);
            Assert.Equal(2, members.Count);
            Assert.NotNull(members.FirstOrDefault(m => m.Name == "id"));
            Assert.NotNull(members.FirstOrDefault(m => m.Name == "lastMealTime"));

            members = type.MembersWith(MemberTypes.Field, Flags.NonPublic | Flags.Instance, typeof(DefaultValueAttribute));
            Assert.NotNull(members);
            Assert.Equal(1, members.Count);
            Assert.NotNull(members.FirstOrDefault(m => m.Name == "birthDay"));
        }

        [Fact()]
        public void TestFindMembersWith_InstanceFieldShouldIncludeInheritedPrivateField_Generic()
        {
            Type type = typeof(Lion);
            IList<MemberInfo> members = type.MembersWith<CodeAttribute>(MemberTypes.Field, Flags.InstanceAnyVisibility);
            Assert.NotNull(members);
            Assert.Equal(2, members.Count);
            Assert.NotNull(members.FirstOrDefault(m => m.Name == "id"));
            Assert.NotNull(members.FirstOrDefault(m => m.Name == "lastMealTime"));
        }

        [Fact()]
        public void TestFindMembersWith_DefaultFlagsShouldBeInstanceAnyVisibility()
        {
            Type type = typeof(Lion);
            IList<MemberInfo> members = type.MembersWith(MemberTypes.Field, typeof(CodeAttribute));
            Assert.NotNull(members);
            Assert.Equal(2, members.Count);
            Assert.NotNull(members.FirstOrDefault(m => m.Name == "id"));
            Assert.NotNull(members.FirstOrDefault(m => m.Name == "lastMealTime"));
        }
        #endregion

        #region MembersAndAttributes()
        [Fact()]
        public void TestFindMembersAndAttributes()
        {
            var members = typeof(Lion).MembersAndAttributes(MemberTypes.Field, Flags.InstanceAnyVisibility, typeof(CodeAttribute));
            Assert.NotNull(members);
            Assert.Equal(2, members.Count);
            foreach(var item in members)
            {
                Assert.True(item.Key.Name == "id" || item.Key.Name == "lastMealTime");
                Assert.Equal(1, item.Value.Count);
                Assert.True(item.Value[0] is CodeAttribute);
            }
        }

        [Fact()]
        public void TestFindMembersAndAttributes_DefaultFlagsShouldBeInstanceAnyVisibility()
        {
            var expectedMembers = typeof(Lion).MembersAndAttributes(MemberTypes.Field, Flags.InstanceAnyVisibility, typeof(CodeAttribute));
            Assert.NotNull(expectedMembers);
            Assert.Equal(2, expectedMembers.Count);
            var members = typeof(Lion).MembersAndAttributes(MemberTypes.Field, typeof(CodeAttribute));
            Assert.NotNull(expectedMembers);
            foreach(var item in expectedMembers)
            {
                Assert.True(members.ContainsKey(item.Key));
                Assert.Equal(item.Value, members[item.Key]);
            }
        }
        #endregion
        #endregion

        #region Find attributes on members
        #region Attribute<T>()
        [Fact()]
        public void TestFindSpecificAttributeOnField()
        {
            // declared field
            FieldInfo field = typeof(Lion).Field("lastMealTime", Flags.InstanceAnyVisibility | Flags.DeclaredOnly);
            Assert.NotNull(field);
            Assert.NotNull(field.Attribute<CodeAttribute>());
            // inherited field
            field = typeof(Lion).Field("birthDay", Flags.InstanceAnyVisibility);
            Assert.NotNull(field);
            field = typeof(Lion).Field("birthDay", Flags.InstanceAnyVisibility);
            Assert.NotNull(field);
            Assert.Null(field.Attribute<CodeAttribute>());
            Assert.NotNull(field.Attribute<DefaultValueAttribute>());
        }

        [Fact()]
        public void TestFindSpecificAttributeOnProperty()
        {
            // inherited property (without attributes)
            PropertyInfo info = typeof(Lion).Property("ID");
            Assert.NotNull(info);
            Assert.Null(info.Attribute<CodeAttribute>());
            // declared property
            info = typeof(Lion).Property("Name");
            Assert.NotNull(info);
            Assert.NotNull(info.Attribute<CodeAttribute>());
            Assert.NotNull(info.Attribute<DefaultValueAttribute>());
            // inherited property
            info = typeof(Lion).Property("MovementCapabilities");
            Assert.NotNull(info);
            Assert.NotNull(info.Attribute<CodeAttribute>());
        }
        #endregion

        #region Attributes()
        [Fact()]
        public void TestFindAllAttributesOnField()
        {
            // declared field
            FieldInfo info = typeof(Lion).Field("lastMealTime");
            Assert.NotNull(info);
            Assert.Equal(1, info.Attributes().Count);
            // inherited field
            info = typeof(Lion).Field("id");
            Assert.NotNull(info);
            Assert.Equal(1, info.Attributes().Count);
        }

        [Fact()]
        public void TestFindSpecificAttributesOnField()
        {
            // declared field
            FieldInfo info = typeof(Lion).Field("lastMealTime");
            Assert.NotNull(info);
            Assert.Equal(1, info.Attributes<CodeAttribute>().Count);
            // inherited field
            info = typeof(Lion).Field("id");
            Assert.NotNull(info);
            Assert.Equal(1, info.Attributes<CodeAttribute>().Count);
        }

        [Fact()]
        public void TestFindAllAttributesOnProperty()
        {
            // inherited property (without attributes)
            PropertyInfo info = typeof(Lion).Property("ID");
            Assert.NotNull(info);
            Assert.Equal(0, info.Attributes().Count);
            // declared property
            info = typeof(Lion).Property("Name");
            Assert.NotNull(info);
            Assert.Equal(2, info.Attributes().Count);
            // inherited property
            info = typeof(Lion).Property("MovementCapabilities");
            Assert.NotNull(info);
            Assert.Equal(1, info.Attributes().Count);
        }

        [Fact()]
        public void TestFindSpecificAttributesOnProperty()
        {
            // inherited property (without attributes)
            PropertyInfo info = typeof(Lion).Property("ID");
            Assert.NotNull(info);
            Assert.Equal(0, info.Attributes<CodeAttribute>().Count);
            // declared property
            info = typeof(Lion).Property("Name");
            Assert.NotNull(info);
            Assert.Equal(1, info.Attributes<CodeAttribute>().Count);
            Assert.Equal(1, info.Attributes<DefaultValueAttribute>().Count);
            // inherited property
            info = typeof(Lion).Property("MovementCapabilities");
            Assert.NotNull(info);
            Assert.Equal(1, info.Attributes<CodeAttribute>().Count);
        }
        #endregion

        #region HasAttribute and relatives
        [Fact()]
        public void TestHasAttributeOnField()
        {
            FieldInfo info = typeof(Lion).Field("lastMealTime");
            Assert.NotNull(info);
            Assert.False(info.HasAttribute(typeof(DefaultValueAttribute)));
            Assert.True(info.HasAttribute(typeof(CodeAttribute)));
        }

        [Fact()]
        public void TestHasAttributeOnField_Generic()
        {
            FieldInfo info = typeof(Lion).Field("lastMealTime");
            Assert.NotNull(info);
            Assert.False(info.HasAttribute<DefaultValueAttribute>());
            Assert.True(info.HasAttribute<CodeAttribute>());
        }

        [Fact()]
        public void TestHasAnyAttribute()
        {
            FieldInfo field = typeof(Lion).Field("lastMealTime");
            Assert.NotNull(field);
            Assert.True(field.HasAnyAttribute(null));
            Assert.True(field.HasAnyAttribute(typeof(CodeAttribute)));
            Assert.False(field.HasAnyAttribute(typeof(DefaultValueAttribute)));
            Assert.True(field.HasAnyAttribute(typeof(CodeAttribute), typeof(DefaultValueAttribute)));

            PropertyInfo property = typeof(Lion).Property("Name");
            Assert.NotNull(property);
            Assert.True(property.HasAnyAttribute(typeof(CodeAttribute), typeof(DefaultValueAttribute)));
        }

        [Fact()]
        public void TestHasAllAttributes()
        {
            FieldInfo field = typeof(Lion).Field("lastMealTime");
            Assert.NotNull(field);
            Assert.True(field.HasAllAttributes());
            Assert.True(field.HasAllAttributes(null));
            Assert.False(field.HasAllAttributes(typeof(CodeAttribute), typeof(DefaultValueAttribute)));

            PropertyInfo property = typeof(Lion).Property("Name");
            Assert.NotNull(property);
            Assert.True(property.HasAllAttributes(typeof(CodeAttribute), typeof(DefaultValueAttribute)));
        }
        #endregion
        #endregion
    }
}


