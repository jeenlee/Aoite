

using System;
using System.Linq;
using Aoite.Reflection;
using Aoite.ReflectionTest.SampleModel.People;
using Xunit;

namespace Aoite.ReflectionTest.Invocation
{

    public class IndexerTest : BaseInvocationTest
    {
        [Fact()]
        public void TestInvokeOneArgIndexer()
        {
            InvokeIndexer((person, sample) => person.GetIndexer(sample[0]));
        }

        [Fact()]
        public void TestInvokeTwoArgsIndexer()
        {
            InvokeIndexer((person, sample) => person.GetIndexer(sample[0], sample[1]));
        }

        private void InvokeIndexer(Func<object, object[], object> getIndexerAction)
        {
            RunWith((personBase, elementType, elementTypeNullable) =>
               {
                   var people = new[] { new object[] { "John", 10 }, new object[] { "Jane", 20 } };
                   people.ForEach(sample =>
                                   {
                                       var person = elementType.CreateInstance(sample[0], sample[1]);
                                       var name = person.WrapIfValueType().GetFieldValue("name");
                                       personBase.SetIndexer(new[] { typeof(string), elementTypeNullable }, name,
                                                              person);
                                   });
                   people.ForEach(sample =>
                                   {
                                       var person = getIndexerAction(personBase, sample);
                                       VerifyFields(person.WrapIfValueType(),
                                                     new { name = sample[0], age = sample[1] });
                                   });
               });
        }

        private void RunWith(Action<object, Type, Type> assertionAction)
        {
            Types.Select(t =>
                          {
                              var emptyDictionary = t.Field("friends").FieldType.CreateInstance();
                              var person = t.CreateInstance().WrapIfValueType();
                              person.SetFieldValue("friends", emptyDictionary);
                              return person;
                          }).ForEach(
                                         person =>
                                         assertionAction(
                                                            person,
                                                            !person.IsWrapped() ? typeof(Person) : typeof(PersonStruct),
                                                            person is Person ? typeof(Person) : typeof(PersonStruct?)));
        }
    }
}