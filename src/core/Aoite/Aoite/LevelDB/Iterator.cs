using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Aoite.LevelDB
{
    /// <summary>
    /// 表示一个键值迭代器。
    /// </summary>
    public class Iterator : LevelDBHandle
    {
        internal Iterator(IntPtr handle)
        {
            this._handle = handle;
        }

        /// <summary>
        /// 获取一个值，表示当前迭代器位置的是否包含有效的键值数据。
        /// </summary>
        /// <returns>有效返回 true，否则返回 false。</returns>
        public bool IsValid()
        {
            var result = LevelDBInterop.leveldb_iter_valid(this._handle) != 0;
            GC.KeepAlive(this);
            return result;
        }

        /// <summary>
        /// 设置迭代器到第一项位置。
        /// </summary>
        public void SeekToFirst()
        {
            LevelDBInterop.leveldb_iter_seek_to_first(this._handle);
            CheckLastError();
        }

        /// <summary>
        /// 设置迭代器到最后一项位置。
        /// </summary>
        public void SeekToLast()
        {
            LevelDBInterop.leveldb_iter_seek_to_last(this._handle);
            CheckLastError();
        }

        /// <summary>
        /// 设置迭代器到指定键的位置。
        /// </summary>
        /// <param name="key">键。</param>
        public void Seek(BinaryValue key)
        {
            var keyData = key.ByteArray;
            LevelDBInterop.leveldb_iter_seek(this._handle, keyData, new IntPtr(keyData.LongLength));
            CheckLastError();
        }

        /// <summary>
        /// 移动迭代器到下一位置。
        /// </summary>
        public void Next()
        {
            LevelDBInterop.leveldb_iter_next(this._handle);
            CheckLastError();
        }

        /// <summary>
        /// 移动迭代器到上一位置。
        /// </summary>
        public void Prev()
        {
            LevelDBInterop.leveldb_iter_prev(this._handle);
            CheckLastError();
        }

        /// <summary>
        /// 获取当前的键。
        /// </summary>
        /// <returns>返回一个的键。</returns>
        public BinaryValue GetKey()
        {
            var key = this.InnerGetKey();
            if(key == null) return null;
            return new BinaryValue(key);
        }

        /// <summary>
        /// 获取当前的字符串值。
        /// <returns>返回一个字符串的值。</returns>
        /// </summary>
        public BinaryValue GetValue()
        {
            var value = this.InnerGetValue();
            if(value == null) return null;
            return new BinaryValue(value);
        }

        private byte[] InnerGetKey()
        {
            IntPtr length;
            var key = LevelDBInterop.leveldb_iter_key(this._handle, out length);
            CheckLastError();
            if(key == IntPtr.Zero) return null;
            var bytes = new byte[(int)length];
            Marshal.Copy(key, bytes, 0, (int)length);
            GC.KeepAlive(this);
            return bytes;
        }
        private unsafe byte[] InnerGetValue()
        {
            IntPtr length;
            var value = LevelDBInterop.leveldb_iter_value(this._handle, out length);
            CheckLastError();
            if(value == IntPtr.Zero) return null;
            var bytes = new byte[(long)length];
            var valueNative = (byte*)value.ToPointer();
            for(long i = 0; i < (long)length; ++i)
                bytes[i] = valueNative[i];

            GC.KeepAlive(this);
            return bytes;
        }

        void CheckLastError()
        {
            IntPtr error;
            LevelDBInterop.leveldb_iter_get_error(this._handle, out error);
            LevelDBException.Check(error);
            GC.KeepAlive(this);
        }

        internal override void DestroyUnmanaged()
        {
            LevelDBInterop.leveldb_iter_destroy(this._handle);
        }
    }
}
