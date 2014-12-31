using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Aoite.Data.Samples
{
    public class BuilderTests
    {
        public BuilderTests()
        {
            Db.SetEngine(new MsSqlCeEngine(""));
        }
        class TestTable
        {
            public int ID { get; set; }
            public string UserName { get; set; }
        }

        [Fact]
        public void CreateWhereTest()
        {
            var where = DbExtensions.CreateWhere(Db.Engine, new ExecuteParameterCollection("id", 5));
            Assert.Equal("id=@id", where);
            where = DbExtensions.CreateWhere(Db.Engine, new ExecuteParameterCollection("id", 5, "un", "a"));
            Assert.Equal("id=@id AND un=@un", where);
        }

        [Fact]
        public void WhereInTest1()
        {
            var command = Db.Select<TestTable>().WhereIn("ID", "@id", new int[] { 1, 2, 3 }).End();
            Assert.NotNull(command);
            Assert.Equal("SELECT * FROM TestTable WHERE ID IN (@id0, @id1, @id2)", command.CommandText);
        }

        [Fact]
        public void WhereInTest2()
        {
            var command = Db.Select<TestTable>()
                .Where("UserName=@username", "@username", "abc")
                .AndIn("ID", "@id", new int[] { 1, 2, 3 }).End();
            Assert.NotNull(command);
            Assert.Equal("SELECT * FROM TestTable WHERE UserName=@username AND ID IN (@id0, @id1, @id2)", command.CommandText);
        }


        [Fact]
        public void WhereInTest3()
        {
            var command = Db.Select<TestTable>()
                .Where("UserName=@username", "@username", "abc")
                .AndNotIn("ID", "@id", new int[] { 1, 2, 3 }).End();
            Assert.NotNull(command);
            Assert.Equal("SELECT * FROM TestTable WHERE UserName=@username AND ID NOT IN (@id0, @id1, @id2)", command.CommandText);
        }

        [Fact]
        public void SelectTest()
        {
            var command = Db.Select<TestTable>().End();
            Assert.NotNull(command);
            Assert.Equal("SELECT * FROM TestTable", command.CommandText);
        }

        [Fact]
        public void SelectByFieldsTest()
        {
            var command = Db.Select<TestTable>("Name").End();
            Assert.NotNull(command);
            Assert.Equal("SELECT Name FROM TestTable", command.CommandText);
        }

        [Fact]
        public void SelectWhereTest()
        {
            var command = Db.Select<TestTable>()
                            .Where("ID=@id", "@id", 5)
                            .End();
            Assert.NotNull(command);
            Assert.Equal("SELECT * FROM TestTable WHERE ID=@id", command.CommandText);
            Assert.Equal(1, command.Count);
            Assert.Equal("@id", command[0].Name);
            Assert.Equal(5, command[0].Value);
        }

        [Fact]
        public void SelectWhereGroupsTest()
        {
            var command = Db.Select<TestTable>()
                            .Where("ID=@id", "@id", 5)
                            .And("Name=@name", "@name", "abc")
                            .Or()
                                .BeginGroup("ID=@id2", "@id2", 6)
                                .Or("ID=@id3", "@id3", 7)
                                .EndGroup()
                            .End();
            Assert.NotNull(command);
            Assert.Equal("SELECT * FROM TestTable WHERE ID=@id AND Name=@name OR (ID=@id2 OR ID=@id3)", command.CommandText);
            Assert.Equal(4, command.Count);
        }

        [Fact]
        public void SelectSomeWhere()
        {
            var command = Db.Select<TestTable>()
                            .Where("ID", "@id", new int[] { 1, 2, 3 })
                            .End();
            Assert.NotNull(command);
            Assert.Equal("SELECT * FROM TestTable WHERE (ID=@id0 OR ID=@id1 OR ID=@id2)", command.CommandText);
            Assert.Equal(3, command.Count);
        }
    }
}
