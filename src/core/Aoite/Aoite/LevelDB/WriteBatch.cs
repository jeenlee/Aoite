using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.LevelDB
{
    /// <summary>
    /// 表示一个批量的写操作。
    /// </summary>
    public class WriteBatch : LevelDBHandle
    {
        /// <summary>
        /// 初始化一个 <see cref="Aoite.LevelDB.WriteBatch" /> 类的新实例。
        /// </summary>
        public WriteBatch()
        {
            _handle = LevelDBInterop.leveldb_writebatch_create();
        }

        /// <summary>
        /// 清空所有批量操作。
        /// </summary>
        public void Clear()
        {
            LevelDBInterop.leveldb_writebatch_clear(_handle);
        }

        /// <summary>
        /// 设置指定键的值。
        /// </summary>
        /// <param name="key">键。</param>
        /// <param name="value">值。</param>
        public WriteBatch Put(BinaryValue key, BinaryValue value)
        {
            var keyData = key.ByteArray;
            var valueData = value.ByteArray;
            LevelDBInterop.leveldb_writebatch_put(_handle, keyData, (IntPtr)keyData.LongLength, valueData, (IntPtr)valueData.LongLength);
            return this;
        }

        /// <summary>
        /// 删除指定键。
        /// </summary>
        /// <param name="key">键。</param>
        public WriteBatch Delete(BinaryValue key)
        {
            var keyData = key.ByteArray;
            LevelDBInterop.leveldb_writebatch_delete(_handle, keyData, (IntPtr)keyData.Length);
            return this;
        }

        /// <summary>
        /// Support for iterating over a batch.
        /// </summary>
        internal void Iterate(IntPtr state, Action<IntPtr, IntPtr, IntPtr, IntPtr, IntPtr> put, Action<IntPtr, IntPtr, IntPtr> deleted)
        {
            LevelDBInterop.leveldb_writebatch_iterate(_handle, state, put, deleted);
        }

        internal override void DestroyUnmanaged()
        {
            LevelDBInterop.leveldb_writebatch_destroy(_handle);
        }
    }
}
