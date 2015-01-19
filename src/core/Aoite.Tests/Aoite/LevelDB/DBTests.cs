using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Aoite.LevelDB
{
    public class DBTests : IDisposable
    {
        LDB Database { get; set; }
        string DatabasePath { get; set; }

        public DBTests()
        {
            var tempPath = Path.GetTempPath();
            var randName = Path.GetRandomFileName();
            DatabasePath = Path.Combine(tempPath, randName);
            var options = new Options()
            {
                CreateIfMissing = true
            };
            Database = new LDB(DatabasePath, options);
        }
  

        public void Dispose()
        {
            // some test-cases tear-down them self
            if(Database != null)
            {
                Database.Dispose();
            }
            if(Directory.Exists(DatabasePath))
            {
                Directory.Delete(DatabasePath, true);
            }
        }

        [Fact]
        public void Close()
        {
            // test double close
            Database.Dispose();
        }

        [Fact]
        public void DisposeChecks()
        {
            Assert.Throws<ObjectDisposedException>(() =>
            {
                Database.Dispose();
                Database.Get("key1");
            });
        }

        [Fact]
        public void Error()
        {
            Assert.Throws<LevelDBException>(() =>
            {
                var options = new Options()
                {
                    CreateIfMissing = false
                };
                var db = new LDB("non-existent", options);
                Assert.Fail();
                db.Get("key1");
            });
        }

        [Fact]
        public void Put()
        {
            Database.Put("key1", "value1");
            Database.Put("key2", "value2");
            Database.Put("key3", "value3");

            Database.Put("key4", "value4", true);
        }

        [Fact]
        public void Get()
        {
            Database.Put("key1", "value1");
            var value1 = Database.Get("key1");
            Assert.Equal("value1", (string)value1);

            Database.Put("key2", "value2");
            var value2 = Database.Get("key2");
            Assert.Equal("value2", (string)value2);

            Database.Put("key3", "value3");
            var value3 = Database.Get("key3");
            Assert.Equal("value3", (string)value3);

            // verify checksum
            var options = new ReadOptions()
            {
                VerifyCheckSums = true
            };
            value1 = Database.Get("key1", options);
            Assert.Equal("value1", (string)value1);

            // no fill cache
            options = new ReadOptions()
            {
                FillCache = false
            };
            value2 = Database.Get("key2", options);
            Assert.Equal("value2", (string)value2);
        }

        [Fact]
        public void PutMutli()
        {
            const int COUNT = 100;
            Task[] tasks = new Task[COUNT];
            for(int i = 0; i < COUNT; i++)
            {
                tasks[i] = Task.Factory.StartNew(o =>
                {
                    Database.Put("key" + o, "value" + o);
                }, i);
            }

            Task.WaitAll(tasks);
            for(int i = 0; i < COUNT; i++)
            {
                Assert.Equal("value" + i, (string)Database.Get("key" + i));
            }
        }

        //[Fact]
        //public void Lock()
        //{
        //    string key = "IdentityKey";
        //    Database.Put(key, BitConverter.GetBytes(0));
        //    const int COUNT = 100;
        //    Task[] tasks = new Task[COUNT];
        //    for(int i = 0; i < COUNT; i++)
        //    {
        //        tasks[i] = Task.Factory.StartNew(o =>
        //        {

        //        }, i);
        //    }
        //}
        [Fact]
        public void Delete()
        {
            Database.Put("key1", "value1");
            var value1 = Database.Get("key1");
            Assert.Equal("value1", (string)value1);
            Database.Delete("key1");
            value1 = Database.Get("key1");
            Assert.Null(value1);
        }

        [Fact]
        public void WriteBatch()
        {
            Database.Put("key1", "value1");

            var writeBatch = new WriteBatch().
                Delete("key1").
                Put("key2", "value2");
            Database.Write(writeBatch);

            string value1 = Database.Get("key1");
            Assert.Null(value1);
            string value2 = Database.Get("key2");
            Assert.Equal("value2", value2);

            writeBatch.Delete("key2").Clear();
            Database.Write(writeBatch);
            value2 = Database.Get("key2");
            Assert.Equal("value2", value2);
        }

        [Fact]
        public void IsValid()
        {
            Database.Put("key1", "value1");

            var iter = Database.CreateIterator();
            iter.SeekToLast();
            Assert.True(iter.IsValid());

            iter.Next();
            Assert.False(iter.IsValid());
        }

        [Fact]
        public void IsValid2()
        {
            Database.Put("key1", "value1");

            var iter = Database.CreateIterator();
            iter.SeekToLast();
            Assert.True(iter.IsValid());

            iter.Next();
            Assert.False(iter.IsValid());
        }
 
        [Fact]
        public void Enumerator()
        {
            Database.Put("key1", "value1");
            Database.Put("key2", "value2");
            Database.Put("key3", "value3");

            var entries = new List<KeyValuePair<string, string>>();
            foreach(var entry in Database)
            {
                entries.Add(new KeyValuePair<string, string>(entry.Key, entry.Value));
            }

            Assert.Equal(3, entries.Count);
            Assert.Equal("key1", entries[0].Key);
            Assert.Equal("value1", entries[0].Value);
            Assert.Equal("key2", entries[1].Key);
            Assert.Equal("value2", entries[1].Value);
            Assert.Equal("key3", entries[2].Key);
            Assert.Equal("value3", entries[2].Value);
        }

        [Fact]
        public void Cache()
        {
            Database.Dispose();

            // open the DB with a cache that is not owned by LevelDB, then
            // close DB and then free the cache
            var options = new Options()
            {
                CacheSize = 64
            };
            Database = new LDB(DatabasePath, options);
            options = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Database.Put("key1", "value1");
            Database.Dispose();
            Database = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        [Fact]
        public void Snapshot()
        {
            // modify db
            Database.Put("key1", "value1");

            // create snapshot
            var snapshot = Database.CreateSnapshot();

            // modify db again
            Database.Put("key2", "value2");

            // read from snapshot
            var readOptions = new ReadOptions()
            {
                Snapshot = snapshot
            };
            string val1 = Database.Get("key1", readOptions);
            Assert.Equal("value1", val1);
            string val2 = Database.Get("key2", readOptions);
            Assert.Null(val2);

            // read from non-snapshot
            readOptions.Snapshot = null;
            val1 = Database.Get("key1", readOptions);
            Assert.Equal("value1", val1);
            val2 = Database.Get("key2", readOptions);
            Assert.Equal("value2", val2);

            // release snapshot
            // GC calls ~Snapshot() for us
        }


        [Fact]
        public void Destroy()
        {
            Database.Dispose();
            Database = null;

            LDB.Destroy(DatabasePath);
        }

        [Fact]
        public void Repair()
        {
            Database.Dispose();

            LDB.Repair(DatabasePath);
        }

        [Fact]
        public void Property()
        {
            var property = Database.PropertyValue("leveldb.stats");
            Assert.NotNull(property);
            Console.WriteLine("LevelDB stats: {0}", property);
        }
    }
}
