

using System;
using System.Collections;
using Aoite.Reflection;
using Aoite.Reflection.Emitter;
using Aoite.ReflectionTest.SampleModel.People;
using Xunit;

namespace Aoite.ReflectionTest.Invocation
{
    
    public class DelegateCacheTest : BaseInvocationTest
    {
        private readonly object[] objectTypes = {
                                                    typeof(Person).CreateInstance(),
                                                    typeof(PersonStruct).CreateInstance().WrapIfValueType()
                                                };

        private IDictionary delegateMap;

        public DelegateCacheTest()
        {
            delegateMap = (IDictionary)typeof(BaseEmitter).GetFieldValue("cache").GetFieldValue("entries");
            delegateMap.Clear();
        }

        private void ExecuteCacheTest( params Action[] actions )
        {
            int delCount = delegateMap.Count;
            foreach( var action in actions )
            {
                for( int i = 0; i < 20; i++ )
                {
                    action();
                }
                Assert.Equal( ++delCount, delegateMap.Count );
            }
        }

        [Fact()]
        public void TestDelegateIsProperlyCachedForFields()
        {
            objectTypes.ForEach(
                                   obj =>
                                   ExecuteCacheTest(
                                                       () => obj.SetFieldValue( "name", "John" ),
                                                       () => obj.GetFieldValue( "age" ) ) );
            Types.ForEach( type => ExecuteCacheTest(
                                                       () => type.SetFieldValue( "totalPeopleCreated", 1 ),
                                                       () => type.GetFieldValue( "totalPeopleCreated" ) ) );
        }

        [Fact()]
        public void TestDelegateIsProperlyCachedForProperties()
        {
            objectTypes.ForEach(
                                   obj =>
                                   ExecuteCacheTest(
                                                       () =>
                                                       obj.SetPropertyValue( "Name", "John" ),
                                                       () => obj.GetPropertyValue( "Age" ) ) );
            Types.ForEach( type => ExecuteCacheTest(
                                                       () => type.SetPropertyValue( "TotalPeopleCreated", 1 ),
                                                       () => type.GetPropertyValue( "TotalPeopleCreated" ) ) );
        }

        [Fact()]
        public void TestDelegateIsProperlyCachedForConstructors()
        {
            RunWith( ( Type type ) => ExecuteCacheTest(
                                                    () => type.CreateInstance(),
                                                    () => type.CreateInstance( "John", 10 ) ) );
        }

        [Fact()]
        public void TestDelegateIsProperlyCachedForMethods()
        {
            var arguments = new object[] { 100d, null };
            objectTypes.ForEach(
                                   obj =>
                                   ExecuteCacheTest( () => obj.CallMethod( "Walk", 100d ),
                                                     () => obj.CallMethod( "Walk",
                                                                       new[]
                                                                       {
                                                                           typeof(double), typeof(double).MakeByRefType()
                                                                       }, arguments ) ) );
            Types.ForEach( type => ExecuteCacheTest(
                                                       () => type.CallMethod( "GetTotalPeopleCreated" ),
                                                       () => type.CallMethod( "AdjustTotalPeopleCreated", 10 ) ) );
        }

        [Fact()]
        public void TestDelegateIsProperlyCachedForIndexers()
        {
            for( int i = 0; i < Types.Length; i++ )
            {
                var emptyDictionary = Types[ i ].Field( "friends" ).FieldType.CreateInstance();
                objectTypes[ i ].SetFieldValue( "friends", emptyDictionary );
                ExecuteCacheTest( () => objectTypes[ i ].SetIndexer(
                                                                       new[]
                                                                       {
                                                                           typeof(string),
                                                                           Types[ i ] == typeof(Person)
                                                                               ? typeof(Person)
                                                                               : typeof(PersonStruct?)
                                                                       },
                                                                       "John", null ),
                                  () => objectTypes[ i ].GetIndexer( "John" ),
                                  () => objectTypes[ i ].GetIndexer( "John", 10 ) );
            }
        }
    }
}