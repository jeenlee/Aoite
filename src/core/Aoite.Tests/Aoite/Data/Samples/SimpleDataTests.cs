using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Xunit;

namespace Aoite.Data.Samples
{
    public class SimpleDataTests
    {
        private TestManagerBase CreateManager()
        {
            return new MsCeTestManager();
            //return new MsSqlTestManager("Data Source=localhost;Initial Catalog=master;Integrated Security=True;");
        }

        private void CreateTable(TestManagerBase manager)
        {
            var sql = ("CREATE TABLE TestTable(ID bigint PRIMARY KEY identity(1,1),UserName nvarchar(255))");
            manager.Engine
                .Execute(sql)
                .ToNonQuery()
                .ThrowIfFailded();

        }
        private void InsertRows(TestManagerBase manager, int rowCount = 10)
        {
            using(var dbContext = manager.Engine.ContextTransaction)
            {
                for(int i = 0; i < rowCount; i++)
                {
                    dbContext.Execute("INSERT INTO TestTable(UserName) VALUES (@username)", "@username", "user" + i)
                        .ToNonQuery()
                        .ThrowIfFailded();
                }
                dbContext.Commit();
            }
        }

        [Fact()]
        public void ToNonQueryTest()
        {
            using(var manager = this.CreateManager())
            {
                CreateTable(manager);
            }
        }

        [Fact()]
        public void ToScalarTest()
        {
            using(var manager = this.CreateManager())
            {
                CreateTable(manager);
                Assert.Equal((object)0
                        , manager.Engine
                            .Execute("SELECT COUNT(*) FROM TestTable")
                            .ToScalar()
                            .UnsafeValue
                        );


                Assert.Equal(0
                        , manager.Engine
                            .Execute("SELECT COUNT(*) FROM TestTable")
                            .ToScalar<int>()
                            .UnsafeValue
                        );

                Assert.Equal(0L
                        , manager.Engine
                            .Execute("SELECT COUNT(*) FROM TestTable")
                            .ToScalar<long>()
                            .UnsafeValue
                        );
            }
        }

        [Fact()]
        public void TransactionTest()
        {
            using(var manager = this.CreateManager())
            {
                CreateTable(manager);
                InsertRows(manager);
                Assert.Equal(10
                       , manager.Engine
                           .Execute("SELECT COUNT(*) FROM TestTable")
                           .ToScalar<int>()
                           .UnsafeValue
                       );
                using(var dbContext = manager.Engine.ContextTransaction)
                {
                    for(int i = 0; i < 10; i++)
                    {
                        dbContext.Execute("INSERT INTO TestTable(UserName) VALUES (@username)", "@username", "user" + i)
                            .ToNonQuery()
                            .ThrowIfFailded();
                        if(i == 4)
                        {
                            dbContext.Rollback();
                            dbContext.OpenTransaction();
                        }
                    }
                    dbContext.Commit();
                }
                Assert.Equal(15
               , manager.Engine
                   .Execute("SELECT COUNT(*) FROM TestTable")
                   .ToScalar<int>()
                   .UnsafeValue
               );
            }
        }

        private void InsertPerformanceTest(IDbEngine engine)
        {
            for(int i = 0; i < 10000; i++)
            {
                engine.Execute("INSERT INTO TestTable(UserName) VALUES (@username)", "@username", "user" + i)
                    .ToNonQuery()
                    .ThrowIfFailded();
            }
        }

        private void InsertCountTest(IDbEngine engine)
        {
            Assert.Equal(10000
                     , engine
                         .Execute("SELECT COUNT(*) FROM TestTable")
                         .ToScalar<int>()
                         .UnsafeValue
                     );
        }

#if TEST_MSSQL
        [Fact()]
        public void InsertPerformanceTest_Ado()
        {
            using(var manager = this.CreateManager())
            {
                CreateTable(manager);
                using(var context = manager.Engine.Context)
                {
                    Stopwatch watch = new Stopwatch();
                    watch.Start();
                    var conn = manager.Engine.CreateConnection();
                    conn.Open();
                    for(int i = 0; i < 10000; i++)
                    {
                        var command = conn.CreateCommand();
                        command.CommandText = "INSERT INTO TestTable(UserName) VALUES (@username)";
                        var p = command.CreateParameter();
                        p.ParameterName = "@username";
                        p.Value = "user" + i;
                        command.Parameters.Add(p);
                        command.ExecuteNonQuery();
                    }
                    conn.Close();
                    watch.Stop();
                    Console.WriteLine(watch.Elapsed);
                    InsertCountTest(manager.Engine);
                }
                //if(elapsed.TotalMilliseconds< 1000)
            }
        }

        [Fact()]
        public void InsertPerformanceTest_Engine()
        {
            using(var manager = this.CreateManager())
            {
                CreateTable(manager);
                Stopwatch watch = new Stopwatch();
                watch.Start();
                InsertPerformanceTest(manager.Engine);
                watch.Stop();
                Console.WriteLine(watch.Elapsed);
                InsertCountTest(manager.Engine);
            }
        }

        [Fact()]
        public void InsertPerformanceTest_Context()
        {
            using(var manager = this.CreateManager())
            {
                CreateTable(manager);
                Stopwatch watch = new Stopwatch();
                watch.Start();
                using(var context = manager.Engine.Context)
                {
                    InsertPerformanceTest(context);
                }

                watch.Stop();
                Console.WriteLine(watch.Elapsed);
                InsertCountTest(manager.Engine);
            }
        }

        [Fact()]
        public void InsertPerformanceTest_ContextT()
        {
            using(var manager = this.CreateManager())
            {
                CreateTable(manager);
                Stopwatch watch = new Stopwatch();
                watch.Start();
                using(var context = manager.Engine.ContextTransaction)
                {
                    InsertPerformanceTest(context);
                    context.Commit().ThrowIfFailded();
                }
                watch.Stop();
                Console.WriteLine(watch.Elapsed);
                InsertCountTest(manager.Engine);
                //if(elapsed.TotalMilliseconds< 1000)
            }
        }
#endif
        [Fact()]
        public void ToTableTest()
        {
            using(var manager = this.CreateManager())
            {
                CreateTable(manager);
                InsertRows(manager);
                var r = manager.Engine
                    .Execute("SELECT ID,UserName FROM TestTable WHERE ID=@id", "@id", 1)
                    .ToTable()
                    .ThrowIfFailded();
                Assert.Equal(1, r.Value.Rows.Count);
                Assert.Equal(r.Value.Rows[0][1], "user0");
            }
        }

        [Fact()]
        public void ToTablePageTest()
        {
            using(var manager = this.CreateManager())
            {
                int rowCount = 1108;
                int pageSize = 7;
                int pageNumber = 8;
                CreateTable(manager);
                InsertRows(manager, rowCount);

                using(var r = manager.Engine.Execute("SELECT ID,UserName FROM TestTable ORDER BY Id DESC")
                    .ToTable(pageNumber, pageSize))
                {
                    r.ThrowIfFailded();

                    Assert.Equal(pageSize, r.Value.Rows.Count);
                    Assert.Equal(rowCount, r.Value.TotalRowCount);
                    for(int i = 0; i < pageSize; i++)
                    {
                        Assert.Equal("user" + (rowCount + pageSize - (pageSize * pageNumber) - i - 1), r.Value.Rows[i][1]);
                    }
                }
            }
        }

        class TestTable
        {
            public long ID { get; set; }
            public string UserName { get; set; }
        }
        class NameTable
        {
            public string UserName { get; set; }
        }

        [Fact()]
        public void ToEntityTest()
        {
            using(var manager = this.CreateManager())
            {
                CreateTable(manager);
                InsertRows(manager);
                var r = manager.Engine
                    .Execute("SELECT ID,UserName FROM TestTable WHERE ID=@id", "@id", 1)
                    .ToEntity<TestTable>()
                    .ThrowIfFailded();
                Assert.Equal(r.Value.UserName, "user0");
            }
        }


        [Fact()]
        public void FineOneTest()
        {
            using(var manager = this.CreateManager())
            {
                CreateTable(manager);
                InsertRows(manager);
                Aoite.Data.Profilers.DbProfiler.Add(manager.Engine);
                Aoite.Data.Profilers.DbProfiler.Executing += (ss, ee) =>
                {
                    Console.WriteLine(ee.Command.ToString());
                };
                var r = manager.Engine.FindOne<TestTable>(5);
                Assert.NotNull(r.UnsafeValue);
                var r2 = manager.Engine.FindOne<TestTable, NameTable>(5);
                Assert.NotNull(r2.UnsafeValue);
            }
        }

        [Fact()]
        public void FineOneWhereTest()
        {
            using(var manager = this.CreateManager())
            {
                CreateTable(manager);
                InsertRows(manager);
                var r2 = manager.Engine.FindOneWhere<TestTable, NameTable>(new { username = "user4", id = 5 });
                Assert.NotNull(r2.UnsafeValue);
            }
        }
        [Fact()]
        public void FineAllWhereTest()
        {
            using(var manager = this.CreateManager())
            {
                CreateTable(manager);
                InsertRows(manager);
                var r2 = manager.Engine.FindAllWhere<TestTable, NameTable>("id<=@uid", new ExecuteParameterCollection("@uid", 5));
                Assert.Equal(5, r2.UnsafeValue.Count);
            }
        }
        [Fact()]
        public void ToEntitiesTest()
        {
            using(var manager = this.CreateManager())
            {
                CreateTable(manager);
                InsertRows(manager);
                var r = manager.Engine
                    .Execute("SELECT ID,UserName FROM TestTable WHERE ID=@id", "@id", 1)
                    .ToEntities<TestTable>()
                    .ThrowIfFailded();
                Assert.Equal(1, r.Value.Count);
                Assert.Equal(r.Value[0].UserName, "user0");
            }
        }

        [Fact()]
        public void ToEntitiesPageTest()
        {
            using(var manager = this.CreateManager())
            {
                int rowCount = 1108;
                int pageSize = 7;
                int pageNumber = 8;
                CreateTable(manager);
                InsertRows(manager, rowCount);

                using(var r = manager.Engine.Execute("SELECT ID,UserName FROM TestTable ORDER BY Id DESC")
                    .ToEntities<TestTable>(pageNumber, pageSize))
                {
                    r.ThrowIfFailded();

                    Assert.Equal(pageSize, r.Value.Rows.Length);
                    Assert.Equal(rowCount, r.Value.Total);
                    for(int i = 0; i < pageSize; i++)
                    {
                        Assert.Equal("user" + (rowCount + pageSize - (pageSize * pageNumber) - i - 1), r.Value[i].UserName);
                    }
                }
            }
        }

        [Fact()]
        public void ToDynamicEntityTest()
        {
            using(var manager = this.CreateManager())
            {
                CreateTable(manager);
                InsertRows(manager);
                var r = manager.Engine
                    .Execute("SELECT ID,UserName FROM TestTable WHERE ID=@id", "@id", 1)
                    .ToEntity()
                    .ThrowIfFailded();
                Assert.Equal(r.Value.Id, 1L);
                Assert.Equal(r.Value.username, "user0");
            }
        }

        [Fact()]
        public void ToDynamicEntitiesTest()
        {
            using(var manager = this.CreateManager())
            {
                CreateTable(manager);
                InsertRows(manager);
                var r = manager.Engine
                    .Execute("SELECT ID,UserName FROM TestTable WHERE ID=@id", "@id", 1)
                    .ToEntities()
                    .ThrowIfFailded();
                Assert.Equal(1, r.Value.Count);
                Assert.Equal(r.Value[0].UserName, "user0");
            }
        }

        [Fact()]
        public void ToDynamicEntitiesPageTest()
        {
            using(var manager = this.CreateManager())
            {
                int rowCount = 1108;
                int pageSize = 7;
                int pageNumber = 8;
                CreateTable(manager);
                InsertRows(manager, rowCount);

                using(var r = manager.Engine.Execute("SELECT ID,UserName FROM TestTable ORDER BY Id DESC")
                    .ToEntities(pageNumber, pageSize))
                {
                    r.ThrowIfFailded();

                    Assert.Equal(pageSize, r.Value.Rows.Length);
                    Assert.Equal(rowCount, r.Value.Total);
                    for(int i = 0; i < pageSize; i++)
                    {
                        Assert.Equal("user" + (rowCount + pageSize - (pageSize * pageNumber) - i - 1), r.Value[i].UserName);
                    }
                }
            }
        }

        [Fact()]
        public void CalrTest()
        {
            /* PS：两倍性能损耗，非常棒！*/
            Stopwatch watch = new Stopwatch();
            watch.Start();
            using(var manager = this.CreateManager())
            {
                CreateTable(manager);
                InsertRows(manager);
                for(int i = 0; i < 100; i++)
                {
                    var r = manager.Engine
                        .Execute("SELECT ID,UserName FROM TestTable WHERE ID=@id", "@id", 1)
                        .ToEntity<TestTable>()
                        .ThrowIfFailded();
                    Assert.Equal(r.Value.ID, 1L);
                    Assert.Equal(r.Value.UserName, "user0");
                }
            }
            watch.Stop();
            Console.WriteLine(watch.Elapsed);


            watch.Start(); 
            using(var manager = this.CreateManager())
            {
                CreateTable(manager);
                InsertRows(manager);
                for(int i = 0; i < 100; i++)
                {
                    var r = manager.Engine
                        .Execute("SELECT ID,UserName FROM TestTable WHERE ID=@id", "@id", 1)
                        .ToEntity()
                        .ThrowIfFailded();
                    Assert.Equal(r.Value.Id, 1L);
                    Assert.Equal(r.Value.username, "user0");
                }
            }
            watch.Stop();
            Console.WriteLine(watch.Elapsed);
        }
    }
}
