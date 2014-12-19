

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Aoite.Reflection;
using Aoite.ReflectionTest.SampleModel.Animals;
using Xunit;

namespace Aoite.ReflectionTest.Lookup
{

    public class FieldTest : BaseLookupTest
    {
        #region Single Field
        [Fact()]
        public void TestFieldInstance()
        {
            FieldInfo field = typeof(object).Field("id");
            Assert.Null(field);

            AnimalInstanceFieldNames.Select(s => typeof(Animal).Field(s)).ForEach(Assert.NotNull);
            LionInstanceFieldNames.Select(s => typeof(Lion).Field(s)).ForEach(Assert.NotNull);
        }

        [Fact()]
        public void TestFieldInstanceIgnoreCase()
        {
            Flags flags = Flags.InstanceAnyVisibility | Flags.IgnoreCase;

            AnimalInstanceFieldNames.Select(s => s.ToUpper()).Select(s => typeof(Animal).Field(s, flags)).ForEach(Assert.NotNull);
            LionInstanceFieldNames.Select(s => s.ToUpper()).Select(s => typeof(Lion).Field(s, flags)).ForEach(Assert.NotNull);
        }

        [Fact()]
        public void TestFieldInstanceDeclaredOnly()
        {
            Flags flags = Flags.InstanceAnyVisibility | Flags.DeclaredOnly;

            AnimalInstanceFieldNames.Select(s => typeof(Animal).Field(s, flags)).ForEach(Assert.NotNull);
            LionDeclaredInstanceFieldNames.Select(s => typeof(Lion).Field(s, flags)).ForEach(Assert.NotNull);
        }

        [Fact()]
        public void TestFieldByPartialName()
        {
            Flags flags = Flags.InstanceAnyVisibility | Flags.PartialNameMatch;

            var expectedName = AnimalInstanceFieldNames.Where(s => s.Contains("i")).First();
            var field = typeof(Animal).Field("i", flags);
            Assert.NotNull(field);
            Assert.Equal(expectedName, field.Name);

            expectedName = AnimalInstanceFieldNames.Where(s => s.Contains("bi")).First();
            field = typeof(Animal).Field("bi", flags);
            Assert.NotNull(field);
            Assert.Equal(expectedName, field.Name);
        }

        [Fact()]
        public void TestFieldWithPartialNameMatchAndExcludeBackingMembers()
        {
            Flags flags = Flags.InstanceAnyVisibility | Flags.PartialNameMatch;

            var expectedName = AnimalInstanceFieldNames.Where(s => s.Contains("Movement")).First();
            var field = typeof(Animal).Field("Movement", flags);
            Assert.NotNull(field);
            Assert.Equal(expectedName, field.Name);

            field = typeof(Animal).Field("Movement", flags | Flags.ExcludeBackingMembers);
            Assert.Null(field);
        }

        [Fact()]
        public void TestFieldStatic()
        {
            Flags flags = Flags.StaticAnyVisibility;

            AnimalInstanceFieldNames.Select(s => typeof(Animal).Field(s, flags)).ForEach(Assert.Null);

            AnimalStaticFieldNames.Select(s => typeof(Animal).Field(s, flags)).ForEach(Assert.NotNull);
            AnimalStaticFieldNames.Select(s => typeof(Lion).Field(s, flags)).ForEach(Assert.NotNull);
        }

        [Fact()]
        public void TestFieldStaticDeclaredOnly()
        {
            Flags flags = Flags.StaticAnyVisibility | Flags.DeclaredOnly;

            AnimalStaticFieldNames.Select(s => typeof(Animal).Field(s, flags)).ForEach(Assert.NotNull);
            AnimalStaticFieldNames.Select(s => typeof(Lion).Field(s, flags)).ForEach(Assert.Null);
        }
        #endregion

        #region Multiple Fields
        [Fact()]
        public void TestFieldsInstance()
        {
            IList<FieldInfo> fields = typeof(object).Fields();
            Assert.NotNull(fields);
            Assert.Equal(0, fields.Count);

            fields = typeof(Animal).Fields();
            Assert.Equal(AnimalInstanceFieldNames, fields.Select(f => f.Name).ToArray());
            Assert.Equal(AnimalInstanceFieldTypes, fields.Select(f => f.FieldType).ToArray());

            fields = typeof(Mammal).Fields();
            Assert.Equal(AnimalInstanceFieldNames.Length, fields.Count);

            fields = typeof(Lion).Fields();
            Assert.Equal(LionInstanceFieldNames, fields.Select(f => f.Name).ToArray());
            Assert.Equal(LionInstanceFieldTypes, fields.Select(f => f.FieldType).ToArray());
        }

        [Fact()]
        public void TestFieldsInstanceWithDeclaredOnlyFlag()
        {
            IList<FieldInfo> fields = typeof(object).Fields(Flags.InstanceAnyVisibility | Flags.DeclaredOnly);
            Assert.NotNull(fields);
            Assert.Equal(0, fields.Count);

            fields = typeof(Animal).Fields(Flags.InstanceAnyVisibility | Flags.DeclaredOnly);
            Assert.Equal(AnimalInstanceFieldNames, fields.Select(f => f.Name).ToArray());
            Assert.Equal(AnimalInstanceFieldTypes, fields.Select(f => f.FieldType).ToArray());

            fields = typeof(Mammal).Fields(Flags.InstanceAnyVisibility | Flags.DeclaredOnly);
            Assert.Equal(0, fields.Count);

            fields = typeof(Lion).Fields(Flags.InstanceAnyVisibility | Flags.DeclaredOnly);
            Assert.Equal(LionDeclaredInstanceFieldNames, fields.Select(f => f.Name).ToArray());
            Assert.Equal(LionDeclaredInstanceFieldTypes, fields.Select(f => f.FieldType).ToArray());
        }

        [Fact()]
        public void TestFieldsStatic()
        {
            IList<FieldInfo> fields = typeof(object).Fields(Flags.StaticAnyVisibility);
            Assert.NotNull(fields);
            Assert.Equal(0, fields.Count);

            fields = typeof(Animal).Fields(Flags.StaticAnyVisibility);
            Assert.Equal(AnimalStaticFieldNames, fields.Select(f => f.Name).ToArray());
            Assert.Equal(AnimalStaticFieldTypes, fields.Select(f => f.FieldType).ToArray());

            fields = typeof(Lion).Fields(Flags.StaticAnyVisibility);
            Assert.Equal(AnimalStaticFieldNames, fields.Select(f => f.Name).ToArray());
            Assert.Equal(AnimalStaticFieldTypes, fields.Select(f => f.FieldType).ToArray());
        }

        [Fact()]
        public void TestFieldsStaticWithDeclaredOnlyFlag()
        {
            IList<FieldInfo> fields = typeof(object).Fields(Flags.StaticAnyVisibility | Flags.DeclaredOnly);
            Assert.NotNull(fields);
            Assert.Equal(0, fields.Count);

            fields = typeof(Animal).Fields(Flags.StaticAnyVisibility | Flags.DeclaredOnly);
            Assert.Equal(AnimalStaticFieldNames, fields.Select(f => f.Name).ToArray());
            Assert.Equal(AnimalStaticFieldTypes, fields.Select(f => f.FieldType).ToArray());

            fields = typeof(Mammal).Fields(Flags.StaticAnyVisibility | Flags.DeclaredOnly);
            Assert.Equal(0, fields.Count);

            fields = typeof(Lion).Fields(Flags.StaticAnyVisibility | Flags.DeclaredOnly);
            Assert.Equal(0, fields.Count);
        }
        #endregion

        #region Enum Field(s)
        enum Sample { One = 1, Two = 2, Three = 3 }

        [Fact()]
        public void TestEnumFieldValueAccess()
        {
            var sample = typeof(Sample);
            var value = sample.GetFieldValue("Two");
            Assert.Equal(Sample.Two, value);
        }
        #endregion
    }
}
