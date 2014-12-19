#region License
// Copyright 2010 Buu Nguyen, Morten Mertner
// 
// Licensed under the Apache License, Version 2.0 (the "License"); 
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at 
// 
// http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, 
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
// See the License for the specific language governing permissions and 
// limitations under the License.
// 
// The latest version of this file can be found at http://Aoite.Reflection.codeplex.com/
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Aoite.Reflection;
using Aoite.ReflectionTest.SampleModel.Animals;
using Aoite.ReflectionTest.SampleModel.Generics;
using Xunit;

namespace Aoite.ReflectionTest.Lookup
{

    public class TypeTest
    {
        #region Implements
        [Fact()]
        public void TestImplements()
        {
            Assert.True(typeof(int).Implements<IComparable>());
            Assert.False(typeof(Stack).Implements<IComparable>());
            Assert.True(typeof(int[]).Implements<ICollection>());
            Assert.True(typeof(List<>).Implements<ICollection>());
            Assert.True(typeof(List<>).Implements(typeof(IEnumerable<>)));
            Assert.True(typeof(List<>).Implements(typeof(ICollection<>)));
            Assert.True(typeof(List<>).Implements(typeof(IList<>)));
            Assert.True(typeof(IList<>).Implements(typeof(ICollection<>)));
            Assert.True(typeof(List<int>).Implements<ICollection>());
            Assert.True(typeof(List<int>).Implements(typeof(ICollection<>)));
            Assert.True(typeof(List<int>).Implements<IList<int>>());
            Assert.False(typeof(List<int>).Implements<IList<string>>());
            Assert.False(typeof(List<int>).Implements<IEnumerator>());
            Assert.True(typeof(IList).Implements<ICollection>());
            Assert.True(typeof(IList<int>).Implements<ICollection<int>>());
            Assert.False(typeof(ICollection).Implements<IList>());
            Assert.False(typeof(ICollection<int>).Implements<IList<int>>());
        }
        #endregion

        #region Inherits
        [Fact()]
        public void TestInherits()
        {
            Assert.False(typeof(Concrete).Inherits(typeof(Concrete)));
            Assert.True(typeof(Concrete).Inherits(typeof(GenericBase<int>)));
            Assert.True(typeof(Concrete).Inherits(typeof(GenericBase<>)));
            Assert.True(typeof(Concrete).Inherits(typeof(AbstractGenericBase<>)));
            Assert.False(typeof(Concrete).Inherits(typeof(GenericBase<long>)));
            Assert.True(typeof(Concrete).Inherits(typeof(AbstractGenericBase<int>)));
            Assert.True(typeof(GenericBase<>).Inherits(typeof(AbstractGenericBase<>)));
            Assert.True(typeof(GenericBase<int>).Inherits(typeof(AbstractGenericBase<>)));
            Assert.True(typeof(GenericBase<int>).Inherits(typeof(AbstractGenericBase<int>)));
            Assert.False(typeof(GenericBase<>).Inherits(typeof(AbstractGenericBase<int>)));
            Assert.False(typeof(GenericBase<>).Inherits(typeof(GenericBase<>)));
        }
        #endregion

        #region InheritsOrImplements
        [Fact()]
        public void TestInheritsOrImplements()
        {
            Assert.True(typeof(int).InheritsOrImplements<IComparable>());
            Assert.False(typeof(Stack).InheritsOrImplements<IComparable>());
            Assert.True(typeof(int[]).InheritsOrImplements<ICollection>());
            Assert.True(typeof(List<>).InheritsOrImplements<ICollection>());
            Assert.True(typeof(List<>).InheritsOrImplements(typeof(IEnumerable<>)));
            Assert.True(typeof(List<>).InheritsOrImplements(typeof(ICollection<>)));
            Assert.True(typeof(List<>).InheritsOrImplements(typeof(IList<>)));
            Assert.True(typeof(IList<>).InheritsOrImplements(typeof(ICollection<>)));
            Assert.True(typeof(List<int>).InheritsOrImplements<ICollection>());
            Assert.True(typeof(List<int>).InheritsOrImplements(typeof(ICollection<>)));
            Assert.True(typeof(List<int>).InheritsOrImplements<IList<int>>());
            Assert.False(typeof(List<int>).InheritsOrImplements<IList<string>>());
            Assert.False(typeof(List<int>).InheritsOrImplements<IEnumerator>());
            Assert.True(typeof(IList).InheritsOrImplements<ICollection>());
            Assert.True(typeof(IList<int>).InheritsOrImplements<ICollection<int>>());
            Assert.False(typeof(ICollection).InheritsOrImplements<IList>());
            Assert.False(typeof(ICollection<int>).InheritsOrImplements<IList<int>>());

            Assert.False(typeof(Concrete).InheritsOrImplements(typeof(Concrete)));
            Assert.True(typeof(Concrete).InheritsOrImplements(typeof(GenericBase<int>)));
            Assert.True(typeof(Concrete).InheritsOrImplements(typeof(GenericBase<>)));
            Assert.True(typeof(Concrete).InheritsOrImplements(typeof(AbstractGenericBase<>)));
            Assert.False(typeof(Concrete).InheritsOrImplements(typeof(GenericBase<long>)));
            Assert.True(typeof(Concrete).InheritsOrImplements(typeof(AbstractGenericBase<int>)));
            Assert.True(typeof(GenericBase<>).InheritsOrImplements(typeof(AbstractGenericBase<>)));
            Assert.True(typeof(GenericBase<int>).InheritsOrImplements(typeof(AbstractGenericBase<>)));
            Assert.True(typeof(GenericBase<int>).InheritsOrImplements(typeof(AbstractGenericBase<int>)));
            Assert.False(typeof(GenericBase<>).InheritsOrImplements(typeof(AbstractGenericBase<int>)));
            Assert.False(typeof(GenericBase<>).InheritsOrImplements(typeof(GenericBase<>)));
        }
        #endregion

        #region IsFrameworkType
        [Fact()]
        public void TestIsFrameworkType()
        {
            Assert.True(typeof(int).IsFrameworkType());
            Assert.True(typeof(Assembly).IsFrameworkType());
            Assert.True(typeof(BindingFlags).IsFrameworkType());
            Assert.True(typeof(List<>).IsFrameworkType());
            Assert.True(typeof(int).IsFrameworkType());
            Assert.False(typeof(Flags).IsFrameworkType());
            Assert.False(typeof(Lion).IsFrameworkType());
        }
        #endregion

        #region Name
        [Fact()]
        public void TestNameWithNonGenericTypes()
        {
            Assert.Equal("string", typeof(string).Name());
            Assert.Equal("int", typeof(Int32).Name());
            Assert.Equal("decimal", typeof(Decimal).Name());
            Assert.Equal("bool[]", typeof(Boolean[]).Name());
            Assert.Equal("int[][]", typeof(int[][]).Name());
            Assert.Equal("StringBuilder", typeof(StringBuilder).Name());
        }

        [Fact()]
        public void TestNameWithNullableTypes()
        {
            Assert.Equal("T?", typeof(Nullable<>).Name());
            Assert.Equal("bool?", typeof(bool?).Name());
            Assert.Equal("int?", typeof(int?).Name());
            Assert.Equal("decimal?[]", typeof(decimal?[]).Name());
            Assert.Equal("int?[][]", typeof(int?[][]).Name());
        }

        [Fact()]
        public void TestNameWithGenericTypes()
        {
            Assert.Equal("StringBuilder", typeof(StringBuilder).Name());
            Assert.Equal("List<T>", typeof(List<>).Name());
            Assert.Equal("Dictionary<TKey,TValue>", typeof(Dictionary<,>).Name());
            Assert.Equal("Dictionary<int,List<string>>", typeof(Dictionary<int, List<string>>).Name());
        }
        #endregion
    }
}