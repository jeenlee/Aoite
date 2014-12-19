

using System;
using Aoite.Reflection;
using Aoite.ReflectionTest.SampleModel.People;
using Xunit;

namespace Aoite.ReflectionTest.Invocation
{

    public class DelegateTest : BaseInvocationTest
    {
        [Fact()]
        public void TestDelegateRetrievalMethodsReturnCorrectDelegateType()
        {
            RunWith((Type type) =>
               {
                   var funcs = new Func<Delegate>[]
                               {
                                   () => type.MakeArrayType().DelegateForGetElement(),
                                   () => type.MakeArrayType().DelegateForSetElement(),
                                   () => type.DelegateForCreateInstance(),
                                   () => type.DelegateForCreateInstance( new[] { typeof(string), typeof(int) } ),
                                   () =>
                                   type.DelegateForCallMethod( "AdjustTotalPeopleCreated", new[] { typeof(int) } ),
                                   () => type.DelegateForCallMethod( "GetTotalPeopleCreated" ),
                                   () => type.DelegateForSetFieldValue( "totalPeopleCreated" ),
                                   () => type.DelegateForGetFieldValue( "totalPeopleCreated" ),
                                   () => type.DelegateForSetPropertyValue( "TotalPeopleCreated" ),
                                   () => type.DelegateForGetPropertyValue( "TotalPeopleCreated" ),
                                   () => type.DelegateForGetIndexer( new[] { typeof(string) } ),
                                   () =>
                                   type.DelegateForSetIndexer( new[]
                                                               {
                                                                   typeof(string),
                                                                   type == typeof(Person) ? type : typeof(PersonStruct?)
                                                               } )
                                   ,
                                   () => type.DelegateForGetIndexer( new[] { typeof(string), typeof(int) } ),
                                   () => type.DelegateForSetFieldValue( "name" ),
                                   () => type.DelegateForGetFieldValue( "name" ),
                                   () => type.DelegateForSetPropertyValue( "Age" ),
                                   () => type.DelegateForGetPropertyValue( "Age" ),
                                   () => type.DelegateForCallMethod( "Walk", new[] { typeof(double) } ),
                                   () =>
                                   type.DelegateForCallMethod( "Walk",
                                                           new[] { typeof(double), typeof(double).MakeByRefType() } ),
                               };
                   var types = new[]
                               {
                                   typeof(ArrayElementGetter),
                                   typeof(ArrayElementSetter),
                                   typeof(ConstructorInvoker),
                                   typeof(ConstructorInvoker),
                                   typeof(MethodInvoker),
                                   typeof(MethodInvoker),
                                   typeof(MemberSetter),
                                   typeof(MemberGetter),
                                   typeof(MemberSetter),
                                   typeof(MemberGetter),
                                   typeof(MethodInvoker),
                                   typeof(MethodInvoker),
                                   typeof(MethodInvoker),
                                   typeof(MemberSetter),
                                   typeof(MemberGetter),
                                   typeof(MemberSetter),
                                   typeof(MemberGetter),
                                   typeof(MethodInvoker),
                                   typeof(MethodInvoker)
                               };

                   for(int i = 0; i < funcs.Length; i++)
                   {
                       var result = funcs[i]();
                       Assert.NotNull(result);
                       Assert.IsType(types[i], result);
                   }
               });
        }
    }
}