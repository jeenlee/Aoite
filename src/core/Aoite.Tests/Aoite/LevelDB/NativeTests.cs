using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Aoite.LevelDB
{
    public class NativeTests:IDisposable
    {
        IntPtr Database { get; set; }
        string DatabasePath { get; set; }
        public NativeTests()
        {
            var tempPath = Path.GetTempPath();
            var randName = Path.GetRandomFileName();
            DatabasePath = Path.Combine(tempPath, randName);
            var options = LevelDBInterop.leveldb_options_create();
            LevelDBInterop.leveldb_options_set_create_if_missing(options, 1);
            IntPtr error;
            Database = LevelDBInterop.leveldb_open(options, DatabasePath, out error);
            LevelDBException.Check(error);
        }


        public void Dispose()
        {
            if(Database != IntPtr.Zero)
            {
                LevelDBInterop.leveldb_close(Database);
                Database = IntPtr.Zero;
            }
            if(Directory.Exists(DatabasePath))
            {
                Directory.Delete(DatabasePath, true);
            }
        }

        private void InnerPut(IntPtr options, string key, string value)
        {
            var keyData = Encoding.UTF8.GetBytes(key);
            var valueData = Encoding.UTF8.GetBytes(value);

            IntPtr error;
            LevelDBInterop.leveldb_put(Database, options, keyData, (IntPtr)keyData.Length, valueData, (IntPtr)valueData.LongLength, out error);
            LevelDBException.Check(error);
            GC.KeepAlive(options);
            GC.KeepAlive(this);
        }

        private unsafe string GetValue(IntPtr valuePtr, IntPtr lengthPtr)
        {
            var length = (long)lengthPtr;
            var value = new byte[length];
            var valueNative = (byte*)valuePtr.ToPointer();
            for(long i = 0; i < length; ++i)
                value[i] = valueNative[i];
            return Encoding.UTF8.GetString(value);
        }
        private string InnerGet(IntPtr readOptions, string key)
        {
            var keyData = Encoding.UTF8.GetBytes(key);
            IntPtr error;
            IntPtr lengthPtr;
            var valuePtr = LevelDBInterop.leveldb_get(this.Database, readOptions, keyData, (IntPtr)keyData.Length, out lengthPtr, out error);
            LevelDBException.Check(error);
            if(valuePtr == IntPtr.Zero)
                return null;
            try
            {
                return GetValue(valuePtr, lengthPtr);
            }
            finally
            {
                LevelDBInterop.leveldb_free(valuePtr);
                GC.KeepAlive(readOptions);
                GC.KeepAlive(this);
            }
        }

        private void InnerDelete(IntPtr writeOptions, string key)
        {
            var keyData = Encoding.UTF8.GetBytes(key);
            IntPtr error;
            LevelDBInterop.leveldb_delete(Database, writeOptions, keyData, (IntPtr)keyData.Length, out error);
            LevelDBException.Check(error);
        }
        private void InnerBatchPut(IntPtr batch, string key, string value)
        {
            var keyData = Encoding.UTF8.GetBytes(key);
            var valueData = Encoding.UTF8.GetBytes(value);

            LevelDBInterop.leveldb_writebatch_put(batch, keyData, (IntPtr)keyData.Length, valueData, (IntPtr)valueData.Length);
        }
        private void InnerBatchDelete(IntPtr batch, string key)
        {
            var keyData = Encoding.UTF8.GetBytes(key);
            LevelDBInterop.leveldb_writebatch_delete(batch, keyData, (IntPtr)keyData.Length);
        }
        [Fact]
        public void Reopen()
        {
            LevelDBInterop.leveldb_close(Database);
            Database = IntPtr.Zero;

            var options = LevelDBInterop.leveldb_options_create();
            IntPtr error;
            Database = LevelDBInterop.leveldb_open(options, DatabasePath, out error);
            LevelDBException.Check(error);
            var readOptions = LevelDBInterop.leveldb_readoptions_create();
            this.InnerGet(readOptions, "key1");
            LevelDBInterop.leveldb_readoptions_destroy(readOptions);
        }

        [Fact]
        public void Put()
        {
            var options = LevelDBInterop.leveldb_writeoptions_create();
            this.InnerPut(options, "key1", "value1");
            this.InnerPut(options, "key2", "value2");
            this.InnerPut(options, "key3", "value3");

            // sync
            LevelDBInterop.leveldb_writeoptions_set_sync(options, 1);
            this.InnerPut(options, "key4", "value4");
        }

        [Fact]
        public void Get()
        {
            var options = LevelDBInterop.leveldb_readoptions_create();

            this.InnerPut(options, "key1", "value1");
            var value1 = this.InnerGet(options, "key1");
            Assert.Equal("value1", value1);

            this.InnerPut(options, "key2", "value2");
            var value2 = this.InnerGet(options, "key2");
            Assert.Equal("value2", value2);

            this.InnerPut(options, "key3", "value3");
            var value3 = this.InnerGet(options, "key3");
            Assert.Equal("value3", value3);

            // verify checksums
            LevelDBInterop.leveldb_readoptions_set_verify_checksums(options, 1);
            value1 = this.InnerGet(options, "key1");
            Assert.Equal("value1", value1);

            // no fill cache
            LevelDBInterop.leveldb_readoptions_set_fill_cache(options, 0);
            value2 = this.InnerGet(options, "key2");
            Assert.Equal("value2", value2);

            LevelDBInterop.leveldb_readoptions_destroy(options);
        }

        [Fact]
        public void Delete()
        {
            var writeOptions = LevelDBInterop.leveldb_writeoptions_create();
            this.InnerPut(writeOptions, "key1", "value1");

            var readOptions = LevelDBInterop.leveldb_readoptions_create();
            var value1 = this.InnerGet(readOptions, "key1");
            Assert.Equal("value1", value1);
            this.InnerDelete(writeOptions, "key1");
            value1 = this.InnerGet(readOptions, "key1");
            Assert.Null(value1);

            LevelDBInterop.leveldb_writeoptions_destroy(writeOptions);
            LevelDBInterop.leveldb_readoptions_destroy(readOptions);
        }


        [Fact]
        public void WriteBatch()
        {
            var writeOptions = LevelDBInterop.leveldb_writeoptions_create();
            this.InnerPut(writeOptions, "key1", "value1");

            var writeBatch = LevelDBInterop.leveldb_writebatch_create();
            this.InnerBatchDelete(writeBatch, "key1");
            this.InnerBatchPut(writeBatch, "key2", "value2");
            IntPtr error;
            LevelDBInterop.leveldb_write(Database, writeOptions, writeBatch, out error);
            LevelDBException.Check(error);

            var readOptions = LevelDBInterop.leveldb_readoptions_create();
            var value1 = this.InnerGet(readOptions, "key1");
            Assert.Null(value1);
            var value2 = this.InnerGet(readOptions, "key2");
            Assert.Equal("value2", value2);

            this.InnerBatchDelete(writeBatch, "key2");
            LevelDBInterop.leveldb_writebatch_clear(writeBatch);
            LevelDBInterop.leveldb_write(Database, writeOptions, writeBatch, out error);
            LevelDBException.Check(error);
            value2 = this.InnerGet(readOptions, "key2");
            Assert.Equal("value2", value2);

            LevelDBInterop.leveldb_writebatch_destroy(writeBatch);
            LevelDBInterop.leveldb_writeoptions_destroy(writeOptions);
            LevelDBInterop.leveldb_writeoptions_destroy(readOptions);
        }

        [Fact]
        public void IsValid()
        {
            var writeOptions = LevelDBInterop.leveldb_writeoptions_create();
            this.InnerPut(writeOptions, "key1", "value1");

            var readOptions = LevelDBInterop.leveldb_readoptions_create();
            IntPtr iter = LevelDBInterop.leveldb_create_iterator(Database, readOptions);

            LevelDBInterop.leveldb_iter_seek_to_last(iter);
            Assert.True(LevelDBInterop.leveldb_iter_valid(iter) == 1);

            LevelDBInterop.leveldb_iter_next(iter);
            Assert.False(LevelDBInterop.leveldb_iter_valid(iter) == 1);
        }

        [Fact]
        public void Enumerator()
        {
            var writeOptions = LevelDBInterop.leveldb_writeoptions_create();
            this.InnerPut(writeOptions, "key1", "value1");
            this.InnerPut(writeOptions, "key2", "value2");
            this.InnerPut(writeOptions, "key3", "value3");

            var entries = new List<KeyValuePair<string, string>>();
            var readOptions = LevelDBInterop.leveldb_readoptions_create();
            IntPtr iter = LevelDBInterop.leveldb_create_iterator(Database, readOptions);
            for(LevelDBInterop.leveldb_iter_seek_to_first(iter);
                 LevelDBInterop.leveldb_iter_valid(iter) == 1;
                 LevelDBInterop.leveldb_iter_next(iter))
            {
                IntPtr len;
                string key = GetValue(LevelDBInterop.leveldb_iter_key(iter, out len), len);
                string value = GetValue(LevelDBInterop.leveldb_iter_value(iter, out len), len);
                var entry = new KeyValuePair<string, string>(key, value);
                entries.Add(entry);
            }
            LevelDBInterop.leveldb_iter_destroy(iter);
            LevelDBInterop.leveldb_readoptions_destroy(readOptions);

            Assert.Equal(3, entries.Count);
            Assert.Equal("key1", entries[0].Key);
            Assert.Equal("value1", entries[0].Value);
            Assert.Equal("key2", entries[1].Key);
            Assert.Equal("value2", entries[1].Value);
            Assert.Equal("key3", entries[2].Key);
            Assert.Equal("value3", entries[2].Value);

            LevelDBInterop.leveldb_writeoptions_destroy(writeOptions);
        }

        [Fact]
        public void Cache()
        {
            LevelDBInterop.leveldb_close(Database);
            Database = IntPtr.Zero;

            // open the DB with a cache that is not owned by LevelDB, then
            // close DB and then free the cache
            var options = LevelDBInterop.leveldb_options_create();
            var cache = LevelDBInterop.leveldb_cache_create_lru((IntPtr)64);
            LevelDBInterop.leveldb_options_set_cache(options, cache);
            IntPtr error;
            Database = LevelDBInterop.leveldb_open(options, DatabasePath, out error);
            LevelDBException.Check(error);
            LevelDBInterop.leveldb_close(Database);
            Database = IntPtr.Zero;

            LevelDBInterop.leveldb_cache_destroy(cache);
            LevelDBInterop.leveldb_options_destroy(options);
        }

        [Fact]
        public void Snapshot()
        {
            // modify db
            var writeOptions = LevelDBInterop.leveldb_writeoptions_create();
            this.InnerPut(writeOptions, "key1", "value1");
            LevelDBInterop.leveldb_writeoptions_destroy(writeOptions);

            // create snapshot
            var snapshot = LevelDBInterop.leveldb_create_snapshot(Database);

            // modify db again
            writeOptions = LevelDBInterop.leveldb_writeoptions_create();
            this.InnerPut(writeOptions, "key2", "value2");
            LevelDBInterop.leveldb_writeoptions_destroy(writeOptions);

            // read from snapshot
            var readOptions = LevelDBInterop.leveldb_readoptions_create();
            LevelDBInterop.leveldb_readoptions_set_snapshot(readOptions, snapshot);
            var val1 = this.InnerGet(readOptions, "key1");
            Assert.Equal("value1", val1);
            var val2 = this.InnerGet(readOptions, "key2");
            Assert.Null(val2);
            LevelDBInterop.leveldb_readoptions_destroy(readOptions);

            // release snapshot
            LevelDBInterop.leveldb_release_snapshot(Database, snapshot);
            snapshot = IntPtr.Zero;
        }

        [Fact]
        public void Destroy()
        {
            LevelDBInterop.leveldb_close(Database);
            Database = IntPtr.Zero;

            var options = LevelDBInterop.leveldb_options_create();
            IntPtr error;
            LevelDBInterop.leveldb_destroy_db(options, DatabasePath, out error);
            LevelDBException.Check(error);
            LevelDBInterop.leveldb_options_destroy(options);
        }

        [Fact]
        public void Repair()
        {
            LevelDBInterop.leveldb_close(Database);
            Database = IntPtr.Zero;

            var options = LevelDBInterop.leveldb_options_create();
            IntPtr error;
            LevelDBInterop.leveldb_repair_db(options, DatabasePath, out error);
            LevelDBException.Check(error);
            LevelDBInterop.leveldb_options_destroy(options);
        }

        [Fact]
        public void Property()
        {
            var property = LevelDBInterop.leveldb_property_value(Database, "leveldb.stats");
            Assert.NotNull(property);
            Console.WriteLine("LevelDB stats: {0}", property);
        }
    }
}
