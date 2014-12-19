

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Aoite.Reflection;
using Aoite.Reflection.Probing;
using Aoite.ReflectionTest.SampleModel.Animals;
using Xunit;

namespace Aoite.ReflectionTest.Probing
{

    public class MethodDispatcherTest
    {
        [Fact()]
        public void TestMethodDispatcherWithSingleMethod()
        {
            var obj = new Elephant();
            var dispatcher = new MethodDispatcher();
            dispatcher.AddMethod(typeof(Elephant).Methods("Eat").First());
            dispatcher.Invoke(obj, true, new { });
            Assert.Equal(1, obj.MethodInvoked);
        }

        [Fact()]
        public void TestMethodDispatcherWithMultipleMethods()
        {
            var obj = new Elephant();
            var dispatcher = new MethodDispatcher();
            typeof(Elephant).Methods(Flags.InstanceAnyVisibility | Flags.ExcludeBackingMembers).ForEach(dispatcher.AddMethod);
            dispatcher.Invoke(obj, true, new { });
            Assert.Equal(1, obj.MethodInvoked);
            dispatcher.Invoke(obj, true, new { count = 2.0, food = "hay", isHay = true });
            Assert.Equal(5, obj.MethodInvoked);
            dispatcher.Invoke(obj, true, new { count = 2, volume = 4 });
            Assert.Equal(11, obj.MethodInvoked);
        }

        #region Sample class with static overloads
        internal class StaticElephant
        {
#pragma warning disable 0169, 0649
            public static int MethodInvoked { get; private set; }
#pragma warning restore 0169, 0649

            public static void Eat()
            {
                MethodInvoked = 1;
            }
            public static void Eat(string food)
            {
                MethodInvoked = 2;
            }
            public static void Eat(int count)
            {
                MethodInvoked = 3;
            }
            public static void Eat(int count, string food)
            {
                MethodInvoked = 4;
            }
            public static void Eat(double count, string food, bool isHay)
            {
                MethodInvoked = 5;
            }
        }
        #endregion

        [Fact()]
        public void TestMethodDispatcherWithStaticMethods()
        {
            var type = typeof(StaticElephant);
            var dispatcher = new MethodDispatcher();
            typeof(StaticElephant).Methods(Flags.StaticAnyVisibility).ForEach(dispatcher.AddMethod);
            dispatcher.Invoke(type, true, new { });
            Assert.Equal(1, StaticElephant.MethodInvoked);
            dispatcher.Invoke(type, true, new { count = 2.0, food = "hay", isHay = true });
            Assert.Equal(5, StaticElephant.MethodInvoked);
            dispatcher.Invoke(type, false, new { count = 2, volume = 4 });
            Assert.Equal(3, StaticElephant.MethodInvoked);
        }
    }
}