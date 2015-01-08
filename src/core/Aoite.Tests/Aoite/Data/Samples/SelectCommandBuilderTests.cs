using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Aoite.Data.Builder
{
    public class SelectCommandBuilderTests
    {
        private SelectCommandBuilder GetSelect()
        {
            return new SelectCommandBuilder(new MsSqlEngine(""));
        }
        [Fact()]
        public void SelectTest()
        {
            var builder = this.GetSelect();
            builder.Select("a, b");
            Assert.Equal("SELECT a, b FROM ", builder.CommandText);
            builder.Select("c", "d");
            Assert.Equal("SELECT a, b, c, d FROM ", builder.CommandText);
        }

        [Fact()]
        public void FromTest()
        {
            var builder = this.GetSelect();
            builder.From("a");
            Assert.Equal("SELECT * FROM a", builder.CommandText);
            builder.Select("c", "d");
            Assert.Equal("SELECT c, d FROM a", builder.CommandText);
            builder.From("b");
            Assert.Equal("SELECT c, d FROM b", builder.CommandText);
        }

        [Fact()]
        public void OrderByTest()
        {
            var builder = this.GetSelect();
            builder.OrderBy("a");
            Assert.Equal("SELECT * FROM  ORDER BY a", builder.CommandText);
            builder.OrderBy("b");
            Assert.Equal("SELECT * FROM  ORDER BY a, b", builder.CommandText);
        }

        [Fact()]
        public void GroupByTest()
        {
            var builder = this.GetSelect();
            builder.GroupBy("a");
            Assert.Equal("SELECT * FROM  GROUP BY a", builder.CommandText);
            builder.GroupBy("b");
            Assert.Equal("SELECT * FROM  GROUP BY a, b", builder.CommandText);
        }

        [Fact()]
        public void WhereTest()
        {
            var builder = this.GetSelect();
            builder.Where();
            Assert.Equal("SELECT * FROM ", builder.CommandText);
            builder.Where("a=1");
            Assert.Equal("SELECT * FROM  WHERE a=1", builder.CommandText);
        }

        [Fact()]
        public void Where_STest()
        {
            var builder = this.GetSelect();
            builder.Where("a=1");
            Assert.Equal("SELECT * FROM  WHERE a=1", builder.CommandText);
            builder.Where("b=2");
            Assert.Equal("SELECT * FROM  WHERE a=1 AND b=2", builder.CommandText);
        }

        [Fact()]
        public void Where_SSTTest()
        {
            var builder = this.GetSelect();
            builder.Where("a", "@a", new int[] { 1, 2, 3, });
            Assert.Equal("SELECT * FROM  WHERE (a=@a0 OR a=@a1 OR a=@a2)", builder.CommandText);
            Assert.Equal(3, builder.Parameters.Count);

            builder.Where("b", "@b", new int[] { 1 });
            Assert.Equal("SELECT * FROM  WHERE (a=@a0 OR a=@a1 OR a=@a2) AND (b=@b0)", builder.CommandText);
            Assert.Equal(4, builder.Parameters.Count);
        }

        [Fact()]
        public void Where_SSOTest()
        {
            var builder = this.GetSelect();
            builder.Where("a=@a", "@a", 1);
            Assert.Equal("SELECT * FROM  WHERE a=@a", builder.CommandText);
            Assert.Equal(1, builder.Parameters.Count);
            builder.Where("b=@b", "@b", 2);
            Assert.Equal("SELECT * FROM  WHERE a=@a AND b=@b", builder.CommandText);
            Assert.Equal(2, builder.Parameters.Count);
        }

        [Fact()]
        public void WhereInTest()
        {
            var builder = this.GetSelect();
            builder.WhereIn("a", "@a", new int[] { 1, 2, 3, });
            Assert.Equal("SELECT * FROM  WHERE a IN (@a0, @a1, @a2)", builder.CommandText);
            Assert.Equal(3, builder.Parameters.Count);
            builder.WhereIn("b", "@b", new int[] { 1 });
            Assert.Equal("SELECT * FROM  WHERE a IN (@a0, @a1, @a2) AND b IN (@b0)", builder.CommandText);
            Assert.Equal(4, builder.Parameters.Count);
        }

        [Fact()]
        public void WhereNotInTest()
        {
            var builder = this.GetSelect();
            builder.WhereNotIn("a", "@a", new int[] { 1, 2, 3, });
            Assert.Equal("SELECT * FROM  WHERE a NOT IN (@a0, @a1, @a2)", builder.CommandText);
            Assert.Equal(3, builder.Parameters.Count);
            builder.WhereNotIn("b", "@b", new int[] { 1 });
            Assert.Equal("SELECT * FROM  WHERE a NOT IN (@a0, @a1, @a2) AND b NOT IN (@b0)", builder.CommandText);
            Assert.Equal(4, builder.Parameters.Count);
        }
    }
}
