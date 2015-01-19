using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Aoite.LevelDB.Tests
{
    public class DatabaseTest : IDisposable
    {
        const string DatabasePath = "mytestdb";
        public void Dispose()
        {
            GA.IO.DeleteDirectory(DatabasePath);
        }
        [Fact]
        public void Intro()
        {
            using(var database = new LDB(DatabasePath, new Options() { CreateIfMissing = true }))
            {
                database.Put("key1", "value1");
                Assert.Equal("value1", (string)database.Get("key1"));
                Assert.True(database.Get("key1") != null);
                database.Delete("key1");
                Assert.False(database.Get("key1") != null);
                Assert.Null(database.Get("key1"));
            }
        }

        [Fact]
        public void ComparatorTest()
        {
            using(var database = new LDB(DatabasePath, new Options() { CreateIfMissing = true }))
            {
                for(int i = 0; i < 20; i++)
                {
                    database.Put("AA:" + i, "valueA" + i);
                    database.Put("BB:" + i, "valueB" + i);
                    database.Put("CC:" + i, "valueC" + i);
                    database.Put("BBX:" + i, "valueBX" + i);
                }
            }

            using(var database = new LDB(DatabasePath, new Options()))
            {
                using(var iter = database.CreateIterator())
                {
                    int testCount = 0;
                    iter.Seek("BB:");
                    while(iter.IsValid())
                    {
                        string key = iter.GetKey();
                        if(!key.StartsWith("BB:")) break;

                        string value = iter.GetValue();
                        testCount++;
                        Console.WriteLine("{0} -> {1}", key, value);

                        iter.Next();
                    }

                    Assert.Equal(20, testCount);
                }
            }

        }
    }
}
