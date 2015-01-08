using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Aoite.Data.Builder
{
    public class WhereCommandBuilderTests
    {
        private WhereCommandBuilder GetWhereSelect()
        {
            return new WhereCommandBuilder(new SelectCommandBuilder(new MsSqlEngine("")));
        }
        [Fact()]
        public void SqlTest()
        {
            var where = this.GetWhereSelect();
            var builder = where.Select;
            where.Sql("a=b");
            Assert.Equal("SELECT * FROM  WHERE a=b", builder.CommandText);
            where.Sql(" AND c=d");
            Assert.Equal("SELECT * FROM  WHERE a=b AND c=d", builder.CommandText);
        }

        [Fact()]
        public void ParameterTest()
        {
            var where = this.GetWhereSelect();
            var builder = where.Select;
            where.Parameter("@a", 1);
            where.Parameter("@b", 2);
            Assert.Equal(2, builder.Parameters.Count);
        }

        [Fact()]
        public void AndTest()
        {
            var where = this.GetWhereSelect();
            var builder = where.Select;
            where.And();
            Assert.Equal("SELECT * FROM ", builder.CommandText);
            where.And("a=@a", "@a", 1)
                 .And("b=@a");
            Assert.Equal("SELECT * FROM  WHERE a=@a AND b=@a", builder.CommandText);
        }

        [Fact()]
        public void OrTest()
        {
            var where = this.GetWhereSelect();
            var builder = where.Select;
            where.Or();
            Assert.Equal("SELECT * FROM ", builder.CommandText);
            where.Or("a=@a", "@a", 1)
                 .Or("b=@a");
            Assert.Equal("SELECT * FROM  WHERE a=@a OR b=@a", builder.CommandText);
        }

        [Fact()]
        public void BeginGroupTest()
        {
            var where = this.GetWhereSelect();
            var builder = where.Select;
            where.BeginGroup();
            Assert.Equal("SELECT * FROM  WHERE (", builder.CommandText);
        }

        [Fact()]
        public void BeginGroup_SSOTest()
        {
            var where = this.GetWhereSelect();
            var builder = where.Select;
            where.BeginGroup("a=@a", "@a", 1);
            Assert.Equal("SELECT * FROM  WHERE (a=@a", builder.CommandText);
        }

        [Fact()]
        public void EndGroupTest()
        {
            var where = this.GetWhereSelect();
            var builder = where.Select;
            where.EndGroup();
            Assert.Equal("SELECT * FROM  WHERE )", builder.CommandText);
        }

        [Fact()]
        public void And_STest()
        {
            var where = this.GetWhereSelect();
            var builder = where.Select;
            where.And("a=1");
            Assert.Equal("SELECT * FROM  WHERE a=1", builder.CommandText);
            where.And("b=1");
            Assert.Equal("SELECT * FROM  WHERE a=1 AND b=1", builder.CommandText);
        }

        [Fact()]
        public void And_SSOTest()
        {
            var where = this.GetWhereSelect();
            var builder = where.Select;
            where.And("a=@a", "@a", 1);
            Assert.Equal("SELECT * FROM  WHERE a=@a", builder.CommandText);
            where.And("b=@b", "@b", 1);
            Assert.Equal("SELECT * FROM  WHERE a=@a AND b=@b", builder.CommandText);
        }

        [Fact()]
        public void And_SSTTest()
        {
            var where = this.GetWhereSelect();
            var builder = where.Select;

            where.And("a", "@a", new int[] { 1, 2, 3, });
            Assert.Equal("SELECT * FROM  WHERE (a=@a0 OR a=@a1 OR a=@a2)", builder.CommandText);
            Assert.Equal(3, builder.Parameters.Count);

            where.And("b", "@b", new int[] { 1 });
            Assert.Equal("SELECT * FROM  WHERE (a=@a0 OR a=@a1 OR a=@a2) AND (b=@b0)", builder.CommandText);
            Assert.Equal(4, builder.Parameters.Count);
        }

        [Fact()]
        public void Or_STest()
        {
            var where = this.GetWhereSelect();
            var builder = where.Select;
            where.Or("a=1");
            Assert.Equal("SELECT * FROM  WHERE a=1", builder.CommandText);
            where.Or("b=1");
            Assert.Equal("SELECT * FROM  WHERE a=1 OR b=1", builder.CommandText);
        }

        [Fact()]
        public void Or_SSOTest()
        {
            var where = this.GetWhereSelect();
            var builder = where.Select;
            where.Or("a=@a", "@a", 1);
            Assert.Equal("SELECT * FROM  WHERE a=@a", builder.CommandText);
            where.Or("b=@b", "@b", 1);
            Assert.Equal("SELECT * FROM  WHERE a=@a OR b=@b", builder.CommandText);
        }

        [Fact()]
        public void Or_SSTTest()
        {
            var where = this.GetWhereSelect();
            var builder = where.Select;

            where.Or("a", "@a", new int[] { 1, 2, 3, });
            Assert.Equal("SELECT * FROM  WHERE (a=@a0 OR a=@a1 OR a=@a2)", builder.CommandText);
            Assert.Equal(3, builder.Parameters.Count);

            where.Or("b", "@b", new int[] { 1 });
            Assert.Equal("SELECT * FROM  WHERE (a=@a0 OR a=@a1 OR a=@a2) OR (b=@b0)", builder.CommandText);
            Assert.Equal(4, builder.Parameters.Count);
        }

        [Fact()]
        public void AndInTest()
        {
            var where = this.GetWhereSelect();
            var builder = where.Select;

            where.AndIn("a", "@a", new int[] { 1, 2, 3, });
            Assert.Equal("SELECT * FROM  WHERE a IN (@a0, @a1, @a2)", builder.CommandText);
            Assert.Equal(3, builder.Parameters.Count);
            where.AndIn("b", "@b", new int[] { 1 });
            Assert.Equal("SELECT * FROM  WHERE a IN (@a0, @a1, @a2) AND b IN (@b0)", builder.CommandText);
            Assert.Equal(4, builder.Parameters.Count);
        }

        [Fact()]
        public void AndNotInTest()
        {
            var where = this.GetWhereSelect();
            var builder = where.Select;
            where.AndNotIn("a", "@a", new int[] { 1, 2, 3, });
            Assert.Equal("SELECT * FROM  WHERE a NOT IN (@a0, @a1, @a2)", builder.CommandText);
            Assert.Equal(3, builder.Parameters.Count);
            where.AndNotIn("b", "@b", new int[] { 1 });
            Assert.Equal("SELECT * FROM  WHERE a NOT IN (@a0, @a1, @a2) AND b NOT IN (@b0)", builder.CommandText);
            Assert.Equal(4, builder.Parameters.Count);
        }

        [Fact()]
        public void OrInTest()
        {
            var where = this.GetWhereSelect();
            var builder = where.Select;

            where.OrIn("a", "@a", new int[] { 1, 2, 3, });
            Assert.Equal("SELECT * FROM  WHERE a IN (@a0, @a1, @a2)", builder.CommandText);
            Assert.Equal(3, builder.Parameters.Count);
            where.OrIn("b", "@b", new int[] { 1 });
            Assert.Equal("SELECT * FROM  WHERE a IN (@a0, @a1, @a2) OR b IN (@b0)", builder.CommandText);
            Assert.Equal(4, builder.Parameters.Count);
        }

        [Fact()]
        public void OrNotInTest()
        {
            var where = this.GetWhereSelect();
            var builder = where.Select;
            where.OrNotIn("a", "@a", new int[] { 1, 2, 3, });
            Assert.Equal("SELECT * FROM  WHERE a NOT IN (@a0, @a1, @a2)", builder.CommandText);
            Assert.Equal(3, builder.Parameters.Count);
            where.OrNotIn("b", "@b", new int[] { 1 });
            Assert.Equal("SELECT * FROM  WHERE a NOT IN (@a0, @a1, @a2) OR b NOT IN (@b0)", builder.CommandText);
            Assert.Equal(4, builder.Parameters.Count);
        }
    }
}
