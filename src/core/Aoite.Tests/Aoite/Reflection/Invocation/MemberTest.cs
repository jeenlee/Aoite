
using System;
using Aoite.Reflection;
using Xunit;

namespace Aoite.ReflectionTest.Invocation
{
    
    public class MemberTest : BaseInvocationTest
    {
        [Fact()]
        public void TestAccessStaticMemberViaMemberInfo()
        {
            RunWith((Type type) =>
               {
                   var memberInfo = type.Member("TotalPeopleCreated", Flags.StaticAnyVisibility);
                   var totalPeopleCreated = (int)memberInfo.Get() + 1;
                   memberInfo.Set(totalPeopleCreated);
                   VerifyProperties(type, new { totalPeopleCreated });
               });
        }

        [Fact()]
        public void TestAccessInstanceMemberViaMemberInfo()
        {
            RunWith((object person) =>
            {
                var memberInfo = person.UnwrapIfWrapped().GetType().Member("Name");
                var name = (string)memberInfo.Get(person) + " updated";
                memberInfo.Set(person, name);
                VerifyProperties(person, new { name });
            });
        }
    }
}
