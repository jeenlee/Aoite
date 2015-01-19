using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Aoite.LevelDB.Tests
{
    public class ReactiveTests : IDisposable
    {
        public ReactiveTests()
        {
            var tempPath = Path.GetTempPath();
            var randName = Path.GetRandomFileName();
            DatabasePath = Path.Combine(tempPath, randName);
        }
        string CleanTestDB()
        {
            return DatabasePath;
        }
        string DatabasePath { get; set; }
        public void Dispose()
        {
            // some test-cases tear-down them self
            if(Directory.Exists(DatabasePath))
            {
                Directory.Delete(DatabasePath, true);
            }
        }
        [Fact]
        public void TestOpen()
        {
            var path = CleanTestDB();
            Assert.Throws<LevelDBException>(() =>
            {
                using(var db = new LDB(path, new Options { CreateIfMissing = true }))
                {
                }

                using(var db = new LDB(path, new Options { ErrorIfExists = true }))
                {
                }
            });
        }

        [Fact]
        public void TestCRUD()
        {
            var path = CleanTestDB();

            using(var db = new LDB(path, new Options { CreateIfMissing = true }))
            {
                db.Put("Tampa", "green");
                db.Put("London", "red");
                db.Put("New York", "blue");

                Assert.Equal((string)db.Get("Tampa"), "green");
                Assert.Equal((string)db.Get("London"), "red");
                Assert.Equal((string)db.Get("New York"), "blue");

                db.Delete("New York");

                Assert.Null(db.Get("New York"));

                db.Delete("New York");
            }
        }

        [Fact]
        public void TestRepair()
        {
            TestCRUD();
            LDB.Repair(DatabasePath, new Options());
        }

        [Fact]
        public void TestIterator()
        {
            var path = CleanTestDB();

            using(var db = new LDB(path, new Options { CreateIfMissing = true }))
            {
                db.Put("Tampa", "green");
                db.Put("London", "red");
                db.Put("New York", "blue");

                var expected = new[] { "London", "New York", "Tampa" };

                var actual = new List<string>();
                using(var iterator = db.CreateIterator(new ReadOptions()))
                {
                    iterator.SeekToFirst();
                    while(iterator.IsValid())
                    {
                        var key = iterator.GetKey();
                        actual.Add(key);
                        iterator.Next();
                    }
                }

                Assert.Equal(expected, actual);

            }
        }

        [Fact]
        public void TestEnumerable()
        {
            var path = CleanTestDB();

            using(var db = new LDB(path, new Options { CreateIfMissing = true }))
            {
                db.Put("Tampa", "green");
                db.Put("London", "red");
                db.Put("New York", "blue");

                var expected = new[] { "London", "New York", "Tampa" };
                var actual = from kv in db
                             select (string)kv.Key;

                Assert.Equal(expected, actual.ToArray());
            }
        }

        [Fact]
        public void TestSnapshot()
        {
            var path = CleanTestDB();

            using(var db = new LDB(path, new Options { CreateIfMissing = true }))
            {
                db.Put("Tampa", "green");
                db.Put("London", "red");
                db.Delete("New York");

                using(var snapShot = db.CreateSnapshot())
                {
                    var readOptions = new ReadOptions { Snapshot = snapShot };

                    db.Put("New York", "blue");

                    Assert.Equal((string)db.Get("Tampa", readOptions), "green");
                    Assert.Equal((string)db.Get("London", readOptions), "red");

                    // Snapshot taken before key was updates
                    Assert.Null(db.Get("New York", readOptions));
                }

                // can see the change now
                Assert.Equal((string)db.Get("New York"), "blue");

            }
        }

        [Fact]
        public void TestGetProperty()
        {
            var path = CleanTestDB();

            using(var db = new LDB(path, new Options { CreateIfMissing = true }))
            {
                var r = new Random(0);
                var data = "";
                for(var i = 0; i < 1024; i++)
                {
                    data += 'a' + r.Next(26);
                }

                for(int i = 0; i < 5 * 1024; i++)
                {
                    db.Put(string.Format("row{0}", i), data);
                }

                var stats = db.PropertyValue("leveldb.stats");

                Assert.NotNull(stats);
                Assert.True(stats.Contains("Compactions"));
            }
        }

        [Fact]
        public void TestWriteBatch()
        {
            var path = CleanTestDB();

            using(var db = new LDB(path, new Options { CreateIfMissing = true }))
            {
                db.Put("NA", "Na");

                using(var batch = new WriteBatch())
                {
                    batch.Delete("NA")
                         .Put("Tampa", "Green")
                         .Put("London", "red")
                         .Put("New York", "blue");
                    db.Write(batch);
                }

                var expected = new[] { "London", "New York", "Tampa" };
                var actual = from kv in db
                             select (string)kv.Key;

                Assert.Equal(expected, actual.ToArray());
            }
        }
    }
}
