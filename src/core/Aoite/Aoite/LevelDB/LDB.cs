using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Aoite.LevelDB
{
    /// <summary>
    /// 表示一个数据库。
    /// </summary>
    public class LDB : LevelDBHandle, IEnumerable<KeyValuePair<BinaryValue, BinaryValue>>
    {
        Options Options;
        private string _DbFolder;
        /// <summary>
        /// 获取数据库的目录。
        /// </summary>
        public string DbFolder { get { return _DbFolder; } }

        /// <summary>
        /// 指定数据库目录，初始化一个 <see cref="Aoite.LevelDB.LDB"/> 类的新实例。
        /// </summary>
        /// <param name="dbFolder">数据库的目录。</param>
        public LDB(string dbFolder)
            : this(dbFolder, new Options { CreateIfMissing = true }) { }

        /// <summary>
        /// 指定数据库目录和选项，初始化一个 <see cref="Aoite.LevelDB.LDB"/> 类的新实例。
        /// </summary>
        /// <param name="dbFolder">数据库的目录。</param>
        /// <param name="options">数据库的选项。</param>
        public LDB(string dbFolder, Options options)
        {
            if(dbFolder == null) throw new ArgumentNullException("dbFolder");

            Options = options ?? new Options();
            IntPtr error;
            _handle = LevelDBInterop.leveldb_open(Options._handle, dbFolder, out error);
            LevelDBException.Check(error);
            GC.KeepAlive(Options);
            this._DbFolder = dbFolder;
        }

        /// <summary>
        /// 返回数据库的目录的哈希代码。
        /// </summary>
        /// <returns>32 位带符号整数哈希代码。</returns>
        public override int GetHashCode()
        {
            return this._DbFolder.GetHashCode();
        }

        /// <summary>
        /// 修复指定目录的数据库。
        /// </summary>
        /// <param name="dbFolder">数据库的目录。</param>
        public static void Repair(string dbFolder)
        {
            Repair(dbFolder, new Options());
        }

        /// <summary>
        /// 修复指定目录的数据库。
        /// </summary>
        /// <param name="dbFolder">数据库的目录。</param>
        /// <param name="options">数据库的选项。</param>
        public static void Repair(string dbFolder, Options options)
        {
            IntPtr error;
            LevelDBInterop.leveldb_repair_db(options._handle, dbFolder, out error);
            LevelDBException.Check(error);
            GC.KeepAlive(options);
        }

        /// <summary>
        /// 摧毁指定目录的数据库。
        /// </summary>
        /// <param name="dbFolder">数据库的目录。</param>
        public static void Destroy(string dbFolder)
        {
            Destroy(dbFolder, new Options());
        }

        /// <summary>
        /// 摧毁指定目录的数据库。
        /// </summary>
        /// <param name="dbFolder">数据库的目录。</param>
        /// <param name="options">数据库的选项。</param>
        public static void Destroy(string dbFolder, Options options)
        {
            IntPtr error;
            LevelDBInterop.leveldb_destroy_db(options._handle, dbFolder, out error);
            LevelDBException.Check(error);
            GC.KeepAlive(options);
        }

        /// <summary>
        /// 设置指定键的值。
        /// </summary>
        /// <param name="key">键。</param>
        /// <param name="value">值。</param>
        /// <param name="sync">指示是否同步操作。</param>
        public void Put(BinaryValue key, BinaryValue value, bool sync = false)
        {
            this.ThrowWhenDisposed();

            var options = new WriteOptions(sync);
            IntPtr error;
            var keyData = key.ByteArray;
            var valueData = value.ByteArray;
            LevelDBInterop.leveldb_put(this._handle, options._handle, keyData, (IntPtr)keyData.LongLength, valueData, (IntPtr)valueData.LongLength, out error);
            LevelDBException.Check(error);
            GC.KeepAlive(options);
            GC.KeepAlive(this);
        }

        /// <summary>
        /// 删除指定键。
        /// </summary>
        /// <param name="key">键。</param>
        /// <param name="sync">指示是否同步操作。</param>
        public void Delete(BinaryValue key, bool sync = false)
        {
            this.ThrowWhenDisposed();

            var options = new WriteOptions(sync);
            var keyData = key.ByteArray;
            IntPtr error;
            LevelDBInterop.leveldb_delete(this._handle, options._handle, keyData, (IntPtr)keyData.LongLength, out error);
            LevelDBException.Check(error);
            GC.KeepAlive(options);
            GC.KeepAlive(this);
        }

        /// <summary>
        /// 写入批量的操作。
        /// </summary>
        /// <param name="batch">批量的操作。</param>
        /// <param name="sync">指示是否同步操作。</param>
        public void Write(WriteBatch batch, bool sync = false)
        {
            this.ThrowWhenDisposed();

            var options = new WriteOptions(sync);
            IntPtr error;
            LevelDBInterop.leveldb_write(this._handle, options._handle, batch._handle, out error);
            LevelDBException.Check(error);
            GC.KeepAlive(batch);
            GC.KeepAlive(options);
            GC.KeepAlive(this);
        }
        /// <summary>
        /// 获取指定键的值。
        /// </summary>
        /// <param name="key">键。</param>
        /// <param name="options">读的选项。</param>
        /// <returns>返回一个值，如果值不存在则返回 null 值。</returns>
        public unsafe BinaryValue Get(BinaryValue key, ReadOptions options = null)
        {
            this.ThrowWhenDisposed();

            if(options == null) options = new ReadOptions();
            var keyData = key.ByteArray;
            IntPtr error;
            IntPtr lengthPtr;
            var valuePtr = LevelDBInterop.leveldb_get(this._handle, options._handle, keyData, (IntPtr)keyData.LongLength, out lengthPtr, out error);
            LevelDBException.Check(error);
            if(valuePtr == IntPtr.Zero)
                return null;
            try
            {
                var length = (long)lengthPtr;
                var value = new byte[length];
                var valueNative = (byte*)valuePtr.ToPointer();
                for(long i = 0; i < length; ++i)
                    value[i] = valueNative[i];
                return new BinaryValue(value);
            }
            finally
            {
                LevelDBInterop.leveldb_free(valuePtr);
                GC.KeepAlive(options);
                GC.KeepAlive(this);
            }
        }

        /// <summary>
        /// 创建一个数据库迭代器。
        /// </summary>
        /// <remarks>返回数据库迭代器。</remarks>
        public Iterator CreateIterator()
        {
            return this.CreateIterator(new ReadOptions());
        }

        /// <summary>
        /// 创建一个数据库迭代器。
        /// </summary>
        /// <param name="options">读的选项。</param>
        /// <remarks>返回数据库迭代器。</remarks>
        public Iterator CreateIterator(ReadOptions options)
        {
            var result = new Iterator(LevelDBInterop.leveldb_create_iterator(this._handle, options._handle));
            GC.KeepAlive(options);
            GC.KeepAlive(this);
            return result;
        }

        /// <summary>
        /// 创建数据库快照，可用于快速读取数据库最新的数据。
        /// </summary>
        /// <returns>返回一个快照。</returns>
        public Snapshot CreateSnapshot()
        {
            var result = new Snapshot(LevelDBInterop.leveldb_create_snapshot(this._handle), this);
            GC.KeepAlive(this);
            return result;
        }

        // <summary>
        // DB implementations can export properties about their state
        // via this method.  If "property" is a valid property understood by this
        // DB implementation, fills "*value" with its current value and returns
        // true.  Otherwise returns false.
        //
        // Valid property names include:
        //
        //  "leveldb.num-files-at-level<N>" - return the number of files at level <N>,
        //     where <N> is an ASCII representation of a level number (e.g. "0").
        //  "leveldb.stats" - returns a multi-line string that describes statistics
        //     about the internal operation of the DB.
        // </summary>
        /// <summary>
        /// 获取数据库属性。
        /// </summary>
        /// <param name="name">属性名称。</param>
        /// <returns>一个属性的值。</returns>
        public string PropertyValue(string name)
        {
            var ptr = LevelDBInterop.leveldb_property_value(this._handle, name);
            if(ptr == IntPtr.Zero)
                return null;
            try
            {
                return Marshal.PtrToStringAnsi(ptr);
            }
            finally
            {
                LevelDBInterop.leveldb_free(ptr);
                GC.KeepAlive(this);
            }
        }

        internal override void DestroyUnmanaged()
        {
            if(this._handle != default(IntPtr))
                LevelDBInterop.leveldb_close(this._handle);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// 返回一个循环访问集合的枚举器。
        /// </summary>
        public IEnumerator<KeyValuePair<BinaryValue, BinaryValue>> GetEnumerator()
        {
            using(var sn = this.CreateSnapshot())
            using(var iterator = this.CreateIterator(new ReadOptions { Snapshot = sn }))
            {
                iterator.SeekToFirst();
                while(iterator.IsValid())
                {
                    yield return new KeyValuePair<BinaryValue, BinaryValue>(iterator.GetKey(), iterator.GetValue());
                    iterator.Next();
                }
            }
        }
    }
}
