

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Aoite.Reflection;
using Xunit;
using Aoite.ReflectionTest.SampleModel.Animals;

namespace Aoite.ReflectionTest.Lookup
{

    public class MethodTest : BaseLookupTest
    {
        #region Single Method
        [Fact()]
        public void TestFindInstanceMethod()
        {
            MethodInfo method = typeof(object).Method("ID");
            Assert.Null(method);

            AnimalInstanceMethodNames.Select(s => typeof(Animal).Method(s)).ForEach(Assert.NotNull);
            SnakeInstanceMethodNames.Select(s => typeof(Snake).Method(s)).ForEach(Assert.NotNull);
        }

        [Fact()]
        public void TestFindMethodInstanceIgnoreCase()
        {
            Flags flags = Flags.InstanceAnyVisibility | Flags.IgnoreCase;

            AnimalInstanceMethodNames.Select(s => s.ToLower()).Select(s => typeof(Animal).Method(s)).ForEach(Assert.Null);
            AnimalInstanceMethodNames.Select(s => s.ToLower()).Select(s => typeof(Animal).Method(s, flags)).ForEach(Assert.NotNull);

            ReptileInstanceMethodNames.Select(s => s.ToLower()).Select(s => typeof(Reptile).Method(s)).ForEach(Assert.Null);
            ReptileInstanceMethodNames.Select(s => s.ToLower()).Select(s => typeof(Reptile).Method(s, flags)).ForEach(Assert.NotNull);

            SnakeInstanceMethodNames.Select(s => s.ToLower()).Select(s => typeof(Snake).Method(s)).ForEach(Assert.Null);
            SnakeInstanceMethodNames.Select(s => s.ToLower()).Select(s => typeof(Snake).Method(s, flags)).ForEach(Assert.NotNull);
        }

        [Fact()]
        public void TestFindMethodInstanceWithExcludeBackingMembers()
        {
            Flags flags = Flags.InstanceAnyVisibility | Flags.ExcludeBackingMembers;

            AnimalInstanceMethodNames.Select(s => typeof(Animal).Method(s)).ForEach(Assert.NotNull);
            AnimalInstanceMethodNames.Where(s => s.Contains("_")).Select(s => typeof(Animal).Method(s, flags)).ForEach(Assert.Null);
            AnimalInstanceMethodNames.Where(s => !s.Contains("_")).Select(s => typeof(Animal).Method(s, flags)).ForEach(Assert.NotNull);

            ReptileInstanceMethodNames.Select(s => typeof(Reptile).Method(s)).ForEach(Assert.NotNull);
            ReptileInstanceMethodNames.Where(s => s.Contains("_")).Select(s => typeof(Reptile).Method(s, flags)).ForEach(Assert.Null);
            ReptileInstanceMethodNames.Where(s => !s.Contains("_")).Select(s => typeof(Reptile).Method(s, flags)).ForEach(Assert.NotNull);

            SnakeInstanceMethodNames.Select(s => typeof(Snake).Method(s)).ForEach(Assert.NotNull);
            SnakeInstanceMethodNames.Where(s => s.Contains("_")).Select(s => typeof(Snake).Method(s, flags)).ForEach(Assert.Null);
            SnakeInstanceMethodNames.Where(s => !s.Contains("_")).Select(s => typeof(Snake).Method(s, flags)).ForEach(Assert.NotNull);
        }

        [Fact()]
        public void TestFindMethodInstanceWithParameterMatch()
        {
            Assert.Null(typeof(Snake).Method("Bite", new Type[] { }));
            Assert.Null(typeof(Snake).Method("Bite", new[] { typeof(object) }));
            Assert.NotNull(typeof(Snake).Method("Bite", null)); // should ignore flag when null is passed
            Assert.NotNull(typeof(Snake).Method("Bite", new[] { typeof(Animal) }));
            Assert.NotNull(typeof(Snake).Method("Bite", new[] { typeof(Mammal) }));
            Assert.NotNull(typeof(Snake).Method("Bite", new[] { typeof(Lion) }));

            Flags flags = Flags.InstanceAnyVisibility;
            Assert.Null(typeof(Snake).Method("Bite", new Type[] { }, flags));
            Assert.Null(typeof(Snake).Method("Bite", new[] { typeof(object) }, flags));
            Assert.NotNull(typeof(Snake).Method("Bite", null, flags)); // should ignore flag when null is passed
            Assert.NotNull(typeof(Snake).Method("Bite", new[] { typeof(Animal) }, flags));
            Assert.NotNull(typeof(Snake).Method("Bite", new[] { typeof(Mammal) }, flags));
            Assert.NotNull(typeof(Snake).Method("Bite", new[] { typeof(Lion) }, flags));
        }

        [Fact()]
        public void TestFindMethodInstanceWithExactBinding()
        {
            Flags flags = Flags.InstanceAnyVisibility | Flags.ExactBinding;

            Assert.Null(typeof(Snake).Method("Bite", new Type[] { }, flags));
            Assert.Null(typeof(Snake).Method("Bite", new[] { typeof(object) }, flags));
            Assert.NotNull(typeof(Snake).Method("Bite", null, flags)); // should ignore flag when null is passed
            Assert.NotNull(typeof(Snake).Method("Bite", new[] { typeof(Animal) }, flags));
            Assert.Null(typeof(Snake).Method("Bite", new[] { typeof(Mammal) }, flags));
            Assert.Null(typeof(Snake).Method("Bite", new[] { typeof(Lion) }, flags));
        }

        [Fact()]
        public void TestFindMethodInstanceWithDeclaredOnly()
        {
            Flags flags = Flags.InstanceAnyVisibility | Flags.DeclaredOnly;

            AnimalInstanceMethodNames.Select(s => typeof(Animal).Method(s, flags)).ForEach(Assert.NotNull);
            ReptileDeclaredInstanceMethodNames.Select(s => typeof(Reptile).Method(s, flags)).ForEach(Assert.NotNull);
            ReptileInstanceMethodNames.Where(s => !ReptileDeclaredInstanceMethodNames.Contains(s)).Select(s => typeof(Reptile).Method(s, flags)).ForEach(Assert.Null);
            SnakeDeclaredInstanceMethodNames.Select(s => typeof(Snake).Method(s, flags)).ForEach(Assert.NotNull);
            SnakeInstanceMethodNames.Where(s => !SnakeDeclaredInstanceMethodNames.Contains(s)).Select(s => typeof(Snake).Method(s, flags)).ForEach(Assert.Null);
        }
        #endregion

        #region Multiple Methods
        [Fact()]
        public void TestFindMethodsExcludesObjectMembers()
        {
            var methods = typeof(object).Methods();
            Assert.NotNull(methods);
            Assert.Equal(0, methods.Count);
        }

        [Fact()]
        public void TestFindMethodsInstance()
        {
            var methods = typeof(Animal).Methods();
            Assert.Equal(AnimalInstanceMethodNames.OrderBy(n => n), methods.Select(m => m.Name).OrderBy(n => n).ToList());

            methods = typeof(Reptile).Methods();
            Assert.Equal(ReptileInstanceMethodNames.OrderBy(n => n), methods.Select(m => m.Name).OrderBy(n => n).ToList());

            methods = typeof(Snake).Methods();
            Assert.Equal(SnakeInstanceMethodNames.OrderBy(n => n), methods.Select(m => m.Name.Contains(".") ? m.Name.Substring(m.Name.LastIndexOf(".") + 1) : m.Name).OrderBy(n => n).ToList());
        }

        [Fact()]
        public void TestFindMethodsWithExcludeBackingMembers()
        {
            Flags flags = Flags.InstanceAnyVisibility | Flags.ExcludeBackingMembers;

            var methods = typeof(Animal).Methods(flags);
            Assert.Equal(AnimalInstanceMethodNames.Where(s => !s.Contains("_")).Distinct().ToList(),
                                            methods.Select(m => m.Name).ToList());

            methods = typeof(Reptile).Methods(flags);
            Assert.Equal(ReptileInstanceMethodNames.Where(s => !s.Contains("_")).Distinct().ToList(),
                                            methods.Select(m => m.Name).ToList());

            methods = typeof(Snake).Methods(flags, "Move");
            Assert.Equal(1, methods.Count);
            Assert.Equal(typeof(Snake), methods[0].DeclaringType);
        }

        [Fact()]
        public void TestFindMethodsWithParameterList()
        {
            Flags flags = Flags.InstanceAnyVisibility;
            var methods = typeof(Animal).Methods(null, flags);
            Assert.Equal(AnimalInstanceMethodNames, methods.Select(m => m.Name).ToArray());

            // find methods with no arguments
            methods = typeof(Animal).Methods(new Type[0]);
            Assert.NotNull(methods);
            Assert.Equal(AnimalInstanceMethodNames.Where(n => n.StartsWith("get_")).Count(), methods.Count);
            methods = typeof(Animal).Methods(new Type[0], flags);
            Assert.Equal(AnimalInstanceMethodNames.Where(n => n.StartsWith("get_")).Count(), methods.Count);
            methods = typeof(Animal).Methods(new Type[0], flags, null);
            Assert.Equal(AnimalInstanceMethodNames.Where(n => n.StartsWith("get_")).Count(), methods.Count);
            methods = typeof(Animal).Methods(new Type[0], flags, new string[0]);
            Assert.Equal(AnimalInstanceMethodNames.Where(n => n.StartsWith("get_")).Count(), methods.Count);

            // find methods with single argument
            methods = typeof(Snake).Methods(new[] { typeof(Animal) });
            Assert.NotNull(methods);
            Assert.Equal(1, methods.Count);
            methods = typeof(Snake).Methods(new[] { typeof(Animal) }, flags);
            Assert.Equal(1, methods.Count);
            methods = typeof(Snake).Methods(new[] { typeof(Animal) }, flags, "B");
            Assert.Equal(0, methods.Count);
            methods = typeof(Snake).Methods(new[] { typeof(Animal) }, flags, "Bite");
            Assert.Equal(1, methods.Count);
        }

        [Fact()]
        public void TestFindMethodsInstanceWithDeclaredOnly()
        {
            var methods = typeof(Animal).Methods(Flags.InstanceAnyVisibility | Flags.DeclaredOnly);
            Assert.Equal(AnimalInstanceMethodNames.OrderBy(n => n), methods.Select(m => m.Name).OrderBy(n => n).ToList());

            methods = typeof(Reptile).Methods(Flags.InstanceAnyVisibility | Flags.DeclaredOnly);
            Assert.Equal(ReptileDeclaredInstanceMethodNames.OrderBy(n => n), methods.Select(m => m.Name).OrderBy(n => n).ToList());

            methods = typeof(Snake).Methods(Flags.InstanceAnyVisibility | Flags.DeclaredOnly);
            Assert.Equal(SnakeDeclaredInstanceMethodNames.OrderBy(n => n), methods.Select(m => m.Name.Contains(".") ? m.Name.Substring(m.Name.LastIndexOf(".") + 1) : m.Name).OrderBy(n => n).ToList());
        }

        [Fact()]
        public void TestFindMethodsInstanceWithPartialNameMatch()
        {
            Flags flags = Flags.InstanceAnyVisibility | Flags.PartialNameMatch;

            var methods = typeof(Animal).Methods(flags, "B");
            Assert.Equal(AnimalInstanceMethodNames.Where(s => s.Contains("B")).OrderBy(n => n).ToList(), methods.Select(m => m.Name).OrderBy(n => n).ToList());

            methods = typeof(Reptile).Methods(flags, "et_");
            Assert.Equal(ReptileInstanceMethodNames.Where(s => s.Contains("et_")).OrderBy(n => n).ToList(), methods.Select(m => m.Name).OrderBy(n => n).ToList());

            methods = typeof(Snake).Methods(flags, "get", "C");
            Assert.Equal(SnakeInstanceMethodNames.Where(s => s.Contains("get") || s.Contains("C")).OrderBy(n => n).ToList(), methods.Select(m => m.Name).OrderBy(n => n).ToList());

            methods = typeof(Snake).Methods(flags, "_");
            Assert.Equal(SnakeInstanceMethodNames.Where(s => s.Contains("_")).OrderBy(n => n).ToList(), methods.Select(m => m.Name).OrderBy(n => n).ToList());

            methods = typeof(Snake).Methods(flags);
            Assert.Equal(SnakeInstanceMethodNames.OrderBy(n => n), methods.Select(m => m.Name.TrimExplicitlyImplementedName()).OrderBy(n => n).ToList());
        }

        [Fact()]
        public void TestMethodsWithNameFilterList()
        {
            IList<MethodInfo> methods = typeof(object).Methods(AnimalInstanceMethodNames);
            Assert.Equal(0, methods.Count);

            methods = typeof(Animal).Methods(AnimalInstanceMethodNames);
            Assert.Equal(AnimalInstanceMethodNames, methods.Select(m => m.Name).ToArray());

            methods = typeof(Snake).Methods(AnimalInstanceMethodNames);
            Assert.Equal(AnimalInstanceMethodNames, methods.Select(m => m.Name).ToArray());

            var list = AnimalInstanceMethodNames.Where(s => s.Contains("C")).ToArray();
            methods = typeof(Animal).Methods(list);
            Assert.Equal(list, methods.Select(m => m.Name).ToArray());

            list = AnimalInstanceMethodNames.Where(s => s.Contains("B")).ToArray();
            methods = typeof(Snake).Methods(list);
            Assert.Equal(list, methods.Select(m => m.Name).ToArray());
        }
        #endregion
    }
}
