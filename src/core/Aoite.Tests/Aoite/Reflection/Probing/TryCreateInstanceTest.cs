

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

    public class TryCreateInstanceTest
    {
        #region TryCreateInstance Constructor Matching Tests
        [Fact()]
        public void TestTryCreateInstanceWithMatchingEmptyArgumentShouldInvokeConstructor1()
        {
            Lion animal = typeof(Lion).TryCreateInstance(new { }) as Lion;
            Verify(animal, 1, Animal.LastID, "Simba", null);

            animal = typeof(Lion).TryCreateInstance(null) as Lion;
            Verify(animal, 1, Animal.LastID, "Simba", null);
            animal = typeof(Lion).TryCreateInstance(new Dictionary<string, object>()) as Lion;
            Verify(animal, 1, Animal.LastID, "Simba", null);

            animal = typeof(Lion).TryCreateInstance(null, null) as Lion;
            Verify(animal, 1, Animal.LastID, "Simba", null);
            animal = typeof(Lion).TryCreateInstance(new string[0], new object[0]) as Lion;
            Verify(animal, 1, Animal.LastID, "Simba", null);

            animal = typeof(Lion).TryCreateInstance(null, null, null) as Lion;
            Verify(animal, 1, Animal.LastID, "Simba", null);
            animal = typeof(Lion).TryCreateInstance(new string[0], new Type[0], new object[0]) as Lion;
            Verify(animal, 1, Animal.LastID, "Simba", null);
        }

        [Fact()]
        public void TestTryCreateInstanceWithMatchingSingleArgumentShouldInvokeConstructor2()
        {
            Lion animal = typeof(Lion).TryCreateInstance(new { Name = "Scar" }) as Lion;
            Verify(animal, 2, Animal.LastID, "Scar", null);

            animal = typeof(Lion).TryCreateInstance(new Dictionary<string, object> { { "Name", "Scar" } }) as Lion;
            Verify(animal, 2, Animal.LastID, "Scar", null);

            animal = typeof(Lion).TryCreateInstance(new[] { "Name" }, new object[] { "Scar" }) as Lion;
            Verify(animal, 2, Animal.LastID, "Scar", null);

            animal = typeof(Lion).TryCreateInstance(new[] { "Name" }, new[] { typeof(string) }, new object[] { "Scar" }) as Lion;
            Verify(animal, 2, Animal.LastID, "Scar", null);
        }

        [Fact()]
        public void TestTryCreateInstanceWithMatchingSingleArgumentShouldInvokeConstructor3()
        {
            Lion animal = typeof(Lion).TryCreateInstance(new { Id = 42 }) as Lion;
            Verify(animal, 3, 42, "Simba", null);

            animal = typeof(Lion).TryCreateInstance(new Dictionary<string, object> { { "Id", 42 } }) as Lion;
            Verify(animal, 3, 42, "Simba", null);

            animal = typeof(Lion).TryCreateInstance(new[] { "Id" }, new object[] { 42 }) as Lion;
            Verify(animal, 3, 42, "Simba", null);

            animal = typeof(Lion).TryCreateInstance(new[] { "Id" }, new[] { typeof(string) }, new object[] { 42 }) as Lion;
            Verify(animal, 3, 42, "Simba", null);
        }

        [Fact()]
        public void TestTryCreateInstanceWithMatchingDoubleArgumentShouldInvokeConstructor4()
        {
            Lion animal = typeof(Lion).TryCreateInstance(new { Id = 42, Name = "Scar" }) as Lion;
            Verify(animal, 4, 42, "Scar", null);
        }

        [Fact()]
        public void TestTryCreateInstanceWithPartialMatchShouldInvokeConstructor3AndSetProperty()
        {
            DateTime? birthday = new DateTime(1973, 1, 27);
            Lion animal = typeof(Lion).TryCreateInstance(new { Id = 42, Birthday = birthday }) as Lion;
            Verify(animal, 3, 42, "Simba", birthday);
        }

        [Fact()]
        public void TestTryCreateInstanceWithPartialMatchShouldInvokeConstructor4AndIgnoreExtraArgs()
        {
            DateTime? birthday = new DateTime(1973, 1, 27);
            Lion animal = typeof(Lion).TryCreateInstance(new { Id = 42, Name = "Scar", Birthday = birthday, Dummy = 0 }) as Lion;
            Verify(animal, 4, 42, "Scar", birthday);
        }

        [Fact()]
        public void TestTryCreateInstanceWithConvertibleArgumentTypeShouldUseConstructor3()
        {
            Lion animal = typeof(Lion).TryCreateInstance(new { Id = "2" }) as Lion;
            Verify(animal, 3, 2, "Simba", null);
        }

        private static void Verify(Lion animal, int constructorInstanceUsed, int id, string name, DateTime? birthday)
        {
            Assert.NotNull(animal);
            Assert.Equal(constructorInstanceUsed, animal.ConstructorInstanceUsed);
            Assert.Equal(id, animal.ID);
            Assert.Equal(name, animal.Name);
            if(birthday.HasValue)
                Assert.Equal(birthday, animal.BirthDay);
            else
                Assert.Null(animal.BirthDay);
        }

        [Fact()]
        public void TestTryCreateInstanceWithInvalidArgumentTypeShouldThrow()
        {
            Assert.Throws<ArgumentException>(() =>
            typeof(Lion).TryCreateInstance(new { Id = "Incompatible Argument Type" }));
        }

        [Fact()]
        public void TestTryCreateInstanceWithoutMatchShouldThrow()
        {
            Assert.Throws<MissingMethodException>(() =>
            typeof(Giraffe).TryCreateInstance(new { Id = 42 }));
        }
        #endregion

        #region TryCreateInstance with XML input
        #region Book class
        private class Book
        {
#pragma warning disable 0169, 0649
            private int _id;
            public string Author { get; private set; }
            public string Title { get; private set; }
            public double Rating { get; private set; }
#pragma warning restore 0169, 0649

            public Book(string author, string title, double rating)
            {
                Author = author;
                Title = title;
                Rating = rating;
            }
        }
        #endregion

        [Fact()]
        public void TestConvertFromString()
        {
            XElement xml = new XElement("Books",
                new XElement("Book",
                    new XAttribute("id", 1),
                    new XAttribute("author", "Douglad Adams"),
                    new XAttribute("title", "The Hitchhikers Guide to the Galaxy"),
                    new XAttribute("rating", 4.8)
                ),
                new XElement("Book",
                    new XAttribute("id", 2),
                    new XAttribute("author", "Iain M. Banks"),
                    new XAttribute("title", "The Player of Games"),
                    new XAttribute("rating", 4.9)
                ),
                new XElement("Book",
                    new XAttribute("id", 3),
                    new XAttribute("author", "Raymond E. Feist"),
                    new XAttribute("title", "Magician"),
                    new XAttribute("rating", 4.2)
                )
            );

            // now lets try to create instances of the Book class using these values
            var data = from book in xml.Elements("Book")
                       select new
                       {
                           id = book.Attribute("id"),
                           author = book.Attribute("author"),
                           title = book.Attribute("title"),
                           rating = book.Attribute("rating")
                       };

            IList<Book> books = data.Select(b => typeof(Book).TryCreateInstance(b) as Book).ToList();
            Assert.Equal(3, books.Count);

            Assert.Equal(1, books[0].GetFieldValue("_id"));
            Assert.Equal("Douglad Adams", books[0].Author);
            Assert.Equal("The Hitchhikers Guide to the Galaxy", books[0].Title);
            Assert.Equal(4.8, books[0].Rating);

            Assert.Equal(2, books[1].GetFieldValue("_id"));
            Assert.Equal("Iain M. Banks", books[1].Author);
            Assert.Equal("The Player of Games", books[1].Title);
            Assert.Equal(4.9, books[1].Rating);

            Assert.Equal(3, books[2].GetFieldValue("_id"));
            Assert.Equal("Raymond E. Feist", books[2].Author);
            Assert.Equal("Magician", books[2].Title);
            Assert.Equal(4.2, books[2].Rating);
        }
        #endregion

        #region Test Hash Code
        [Fact()]
        public void TestParameterHashGenerator_SameTypeShouldGiveIdenticalHash()
        {
            object source1 = new { Id = 42 };
            SourceInfo sample1 = SourceInfo.CreateFromType(source1.GetType());

            object source2 = new { Id = 5 };
            SourceInfo sample2 = SourceInfo.CreateFromType(source2.GetType());

            Assert.Equal(sample1.GetHashCode(), sample2.GetHashCode());
        }

        [Fact()]
        public void TestParameterHashGenerator_DifferentTypeShouldGiveUniqueHash()
        {
            int[] hashes = GetSampleHashes();
            hashes.GroupBy(k => k);
            Assert.Equal(hashes.Length, hashes.Distinct().Count());
        }

        private static int[] GetSampleHashes()
        {
            var sources = new object[]
			              {
			              	new { }, new { Id = 42 }, new { Name = "Scar" }, new { Id = 42, Name = "Scar" },
			              	new { Id = 42, Birthday = DateTime.Now },
			              	new { Id = 42, Name = "Scar", Birthday = DateTime.Now, Dummy = 0 }
			              };
            int index = 0;
            var infos = new SourceInfo[sources.Length];
            Array.ForEach(sources, s => { infos[index++] = SourceInfo.CreateFromType(s.GetType()); });
            index = 0;
            int[] hashes = new int[sources.Length];
            Array.ForEach(infos, i => { hashes[index++] = i.GetHashCode(); });
            return hashes;
        }
        #endregion
    }
}