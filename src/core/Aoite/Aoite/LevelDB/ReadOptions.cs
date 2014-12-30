using System;
using System.Collections.Generic;
using System.Text;

namespace Aoite.LevelDB
{
    /// <summary>
    /// 表示一个读取的配置。
    /// </summary>
    public class ReadOptions : LevelDBHandle
    {
        /// <summary>
        /// 初始化一个 <see cref="Aoite.LevelDB.ReadOptions"/> 类的新实例。
        /// </summary>
        public ReadOptions()
        {
            _handle = LevelDBInterop.leveldb_readoptions_create();
        }

        private bool _VerifyCheckSums = false;
        /// <summary>
        /// 设置或获取一个值，指示是否启用数据校验。
        /// </summary>
        public bool VerifyCheckSums
        {
            get { return this._VerifyCheckSums; }
            set
            {
                this._VerifyCheckSums = value;
                LevelDBInterop.leveldb_readoptions_set_verify_checksums(this._handle, (byte)(value ? 1 : 0));
            }
        }

        private bool _FillCache = true;
        /// <summary>
        /// 设置或获取一个值，指示是否将数据缓存到内存中。
        /// </summary>
        public bool FillCache
        {
            get { return this._FillCache; }
            set
            {
                this._FillCache = value;
                LevelDBInterop.leveldb_readoptions_set_fill_cache(this._handle, (byte)(value ? 1 : 0));
            }
        }

        private Snapshot _SnapShot;
        /// <summary>
        /// 设置或获取快照。
        /// </summary>
        public Snapshot Snapshot
        {
            get { return this._SnapShot; }
            set
            {
                this._SnapShot = value;
                if(value == null)
                    LevelDBInterop.leveldb_readoptions_set_snapshot(_handle, IntPtr.Zero);
                else
                    LevelDBInterop.leveldb_readoptions_set_snapshot(_handle, value._handle);
            }
        }

        internal override void DestroyUnmanaged()
        {
            LevelDBInterop.leveldb_readoptions_destroy(_handle);
        }
    }
}
