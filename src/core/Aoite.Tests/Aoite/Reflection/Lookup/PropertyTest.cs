

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Aoite.Reflection;
using Xunit;
using Aoite.ReflectionTest.SampleModel.Animals;

namespace Aoite.ReflectionTest.Lookup
{

    public class PropertyTest : BaseLookupTest
    {
        #region Single Property
        [Fact()]
        public void TestPropertyInstance()
        {
            PropertyInfo property = typeof(object).Property("ID");
            Assert.Null(property);

            AnimalInstancePropertyNames.Select(s => typeof(Animal).Property(s)).ForEach(Assert.NotNull);
            LionInstancePropertyNames.Select(s => typeof(Lion).Property(s)).ForEach(Assert.NotNull);
        }

        [Fact()]
        public void TestPropertyInstanceIgnoreCase()
        {
            Flags flags = Flags.InstanceAnyVisibility | Flags.IgnoreCase;

            AnimalInstancePropertyNames.Select(s => s.ToLower()).Select(s => typeof(Animal).Property(s)).ForEach(Assert.Null);
            AnimalInstancePropertyNames.Select(s => s.ToLower()).Select(s => typeof(Animal).Property(s, flags)).ForEach(Assert.NotNull);

            LionInstancePropertyNames.Select(s => s.ToLower()).Select(s => typeof(Lion).Property(s)).ForEach(Assert.Null);
            LionInstancePropertyNames.Select(s => s.ToLower()).Select(s => typeof(Lion).Property(s, flags)).ForEach(Assert.NotNull);
        }

        [Fact()]
        public void TestPropertyInstanceDeclaredOnly()
        {
            Flags flags = Flags.InstanceAnyVisibility | Flags.DeclaredOnly;

            AnimalInstancePropertyNames.Select(s => typeof(Animal).Property(s, flags)).ForEach(Assert.NotNull);
            LionDeclaredInstancePropertyNames.Select(s => typeof(Lion).Property(s, flags)).ForEach(Assert.NotNull);
        }

        [Fact()]
        public void TestPropertyByPartialName()
        {
            Flags flags = Flags.InstanceAnyVisibility | Flags.PartialNameMatch;

            var expectedName = AnimalInstancePropertyNames.Where(s => s.Contains("C")).First();
            var property = typeof(Animal).Property("C", flags);
            Assert.NotNull(property);
            Assert.Equal(expectedName, property.Name);

            expectedName = AnimalInstancePropertyNames.Where(s => s.Contains("B")).First();
            property = typeof(Animal).Property("B", flags);
            Assert.NotNull(property);
            Assert.Equal(expectedName, property.Name);
        }

        [Fact()]
        public void TestPropertyWithExcludeExplicitlyImplemented()
        {
            Flags flags = Flags.InstanceAnyVisibility | Flags.ExcludeExplicitlyImplemented;

            // using explicit name
            var property = typeof(Giraffe).Property("Aoite.ReflectionTest.SampleModel.Animals.Interfaces.ISwim.SwimDistance");
            Assert.NotNull(property);
            property = typeof(Giraffe).Property("Aoite.ReflectionTest.SampleModel.Animals.Interfaces.ISwim.SwimDistance", flags);
            Assert.Null(property);

            // using short name
            property = typeof(Giraffe).Property("SwimDistance", Flags.InstanceAnyVisibility | Flags.TrimExplicitlyImplemented);
            Assert.NotNull(property);
            property = typeof(Giraffe).Property("SwimDistance", flags | Flags.TrimExplicitlyImplemented);
            Assert.Null(property);
        }

        [Fact()]
        public void TestPropertyStatic()
        {
            Flags flags = Flags.StaticAnyVisibility;

            AnimalInstancePropertyNames.Select(s => typeof(Animal).Property(s, flags)).ForEach(Assert.Null);

            AnimalStaticPropertyNames.Select(s => typeof(Animal).Property(s, flags)).ForEach(Assert.NotNull);
            AnimalStaticPropertyNames.Select(s => typeof(Lion).Property(s, flags)).ForEach(Assert.NotNull);
        }

        [Fact()]
        public void TestPropertyStaticDeclaredOnly()
        {
            Flags flags = Flags.StaticAnyVisibility | Flags.DeclaredOnly;

            AnimalStaticPropertyNames.Select(s => typeof(Animal).Property(s, flags)).ForEach(Assert.NotNull);
            AnimalStaticPropertyNames.Select(s => typeof(Lion).Property(s, flags)).ForEach(Assert.Null);
        }
        #endregion

        #region Multiple Properties
        [Fact()]
        public void TestPropertiesInstance()
        {
            IList<PropertyInfo> properties = typeof(object).Properties();
            Assert.NotNull(properties);
            Assert.Equal(0, properties.Count);

            properties = typeof(Animal).Properties();
            Assert.Equal(AnimalInstancePropertyNames, properties.Select(p => p.Name).ToArray());
            Assert.Equal(AnimalInstancePropertyTypes, properties.Select(p => p.PropertyType).ToArray());

            properties = typeof(Mammal).Properties();
            Assert.Equal(AnimalInstanceFieldNames.Length, properties.Count);

            properties = typeof(Lion).Properties();
            Assert.Equal(LionInstancePropertyNames, properties.Select(p => p.Name).ToArray());
            Assert.Equal(LionInstancePropertyTypes, properties.Select(p => p.PropertyType).ToArray());
        }

        [Fact()]
        public void TestPropertiesInstanceWithDeclaredOnlyFlag()
        {
            IList<PropertyInfo> properties = typeof(object).Properties(Flags.InstanceAnyVisibility | Flags.DeclaredOnly);
            Assert.NotNull(properties);
            Assert.Equal(0, properties.Count);

            properties = typeof(Animal).Properties(Flags.InstanceAnyVisibility | Flags.DeclaredOnly);
            Assert.Equal(AnimalInstancePropertyNames, properties.Select(p => p.Name).ToArray());
            Assert.Equal(AnimalInstancePropertyTypes, properties.Select(p => p.PropertyType).ToArray());

            properties = typeof(Mammal).Properties(Flags.InstanceAnyVisibility | Flags.DeclaredOnly);
            Assert.Equal(0, properties.Count);

            properties = typeof(Lion).Properties(Flags.InstanceAnyVisibility | Flags.DeclaredOnly);
            Assert.Equal(LionDeclaredInstancePropertyNames, properties.Select(p => p.Name).ToArray());
            Assert.Equal(LionDeclaredInstancePropertyTypes, properties.Select(p => p.PropertyType).ToArray());
        }

        [Fact()]
        public void TestPropertiesStatic()
        {
            IList<PropertyInfo> properties = typeof(object).Properties(Flags.StaticAnyVisibility);
            Assert.NotNull(properties);
            Assert.Equal(0, properties.Count);

            properties = typeof(Animal).Properties(Flags.StaticAnyVisibility);
            Assert.Equal(AnimalStaticPropertyNames, properties.Select(p => p.Name).ToArray());
            Assert.Equal(AnimalStaticPropertyTypes, properties.Select(p => p.PropertyType).ToArray());

            properties = typeof(Mammal).Properties(Flags.StaticAnyVisibility);
            Assert.Equal(AnimalStaticPropertyNames.Length, properties.Count);

            properties = typeof(Lion).Properties(Flags.StaticAnyVisibility);
            Assert.Equal(AnimalStaticPropertyNames, properties.Select(p => p.Name).ToArray());
            Assert.Equal(AnimalStaticPropertyTypes, properties.Select(p => p.PropertyType).ToArray());
        }

        [Fact()]
        public void TestPropertiesStaticWithDeclaredOnlyFlag()
        {
            IList<PropertyInfo> properties = typeof(object).Properties(Flags.StaticAnyVisibility | Flags.DeclaredOnly);
            Assert.NotNull(properties);
            Assert.Equal(0, properties.Count);

            properties = typeof(Animal).Properties(Flags.StaticAnyVisibility | Flags.DeclaredOnly);
            Assert.Equal(AnimalStaticPropertyNames, properties.Select(p => p.Name).ToArray());
            Assert.Equal(AnimalStaticPropertyTypes, properties.Select(p => p.PropertyType).ToArray());

            properties = typeof(Mammal).Properties(Flags.StaticAnyVisibility | Flags.DeclaredOnly);
            Assert.Equal(0, properties.Count);

            properties = typeof(Lion).Properties(Flags.StaticAnyVisibility | Flags.DeclaredOnly);
            Assert.Equal(0, properties.Count);
        }

        [Fact()]
        public void TestPropertiesWithNameFilterList()
        {
            IList<PropertyInfo> properties = typeof(object).Properties(AnimalInstancePropertyNames);
            Assert.Equal(0, properties.Count);

            properties = typeof(Animal).Properties(AnimalInstancePropertyNames);
            Assert.Equal(AnimalInstancePropertyNames, properties.Select(p => p.Name).ToArray());

            properties = typeof(Lion).Properties(AnimalInstancePropertyNames);
            Assert.Equal(AnimalInstancePropertyNames, properties.Select(p => p.Name).ToArray());
            Assert.Equal(AnimalInstancePropertyTypes, properties.Select(p => p.PropertyType).ToArray());

            var list = AnimalInstancePropertyNames.Where(s => s.Contains("C")).ToArray();
            properties = typeof(Animal).Properties(list);
            Assert.Equal(list, properties.Select(p => p.Name).ToArray());

            list = AnimalInstancePropertyNames.Where(s => s.Contains("B")).ToArray();
            properties = typeof(Lion).Properties(list);
            Assert.Equal(list, properties.Select(p => p.Name).ToArray());
        }

        [Fact()]
        public void TestPropertiesWithExcludeBackingMembers()
        {
            Flags flags = Flags.InstanceAnyVisibility | Flags.ExcludeBackingMembers;

            IList<PropertyInfo> properties = typeof(Snake).Properties("SlideDistance");
            Assert.Equal(2, properties.Count);
            Assert.Equal(typeof(Snake), properties.First().DeclaringType);

            properties = typeof(Snake).Properties(flags, "SlideDistance");
            Assert.Equal(1, properties.Count);
            Assert.Equal(typeof(Snake), properties.First().DeclaringType);
        }
        #endregion
    }
}
