using System;
using System.Collections.Generic;
using System.Text;

namespace Aoite.LevelDB
{
    /// <summary>
    /// 表示一个数据库选项。
    /// </summary>
    public class Options : LevelDBHandle
    {
        #region Properties

        /*
                private Env _Env;
                /// <summary>
                /// 设置或获取数据库的操作系统环境。默认为 null 值。
                /// </summary>
                internal Env Env
                {
                    get { return this._Env; }
                    set
                    {
                        if(value == null) throw new ArgumentNullException("value");
                        this._Env = value;
                        LevelDBInterop.leveldb_options_set_env(Handle, value.Handle);
                    }
                }


                private Cache _Cache;
                /// <summary>
                /// 设置或获取数据库的高速缓存。如果没有设置，将默认 8MB 容量的缓存。
                /// </summary>
                public Cache Cache
                {
                    get { return _Cache; }
                    set
                    {
                        if(value == null) throw new ArgumentNullException("value");
                        this._Cache = value;
                        LevelDBInterop.leveldb_options_set_cache(_handle, value._handle);
                    }
                }
        
        private Comparator _Comparator;
        /// <summary>
        /// 获取或设置键的比较器。默认为 null 值。
        /// </summary>
        public Comparator Comparator
        {
            get { return _Comparator; }
            set
            {
                if(value == null) throw new ArgumentNullException("value");
                LevelDBInterop.leveldb_options_set_comparator(this._handle, value._handle);
                this._Comparator = value;
            }
        }
        
        */
        private bool _CreateIfMissing;
        /// <summary>
        /// 设置或获取一个值，指示当数据库不存在时，是否自动创建。默认为 false。
        /// </summary>
        public bool CreateIfMissing
        {
            get { return this._CreateIfMissing; }
            set
            {
                this._CreateIfMissing = value;
                LevelDBInterop.leveldb_options_set_create_if_missing(_handle, value ? (byte)1 : (byte)0);
            }
        }

        private bool _ErrorIfExists;
        /// <summary>
        /// 设置或获取一个值，指示数据库存在时是否抛出异常。默认为 false。
        /// </summary>
        public bool ErrorIfExists
        {
            get { return this._ErrorIfExists; }
            set
            {
                this._ErrorIfExists = value;
                LevelDBInterop.leveldb_options_set_error_if_exists(_handle, value ? (byte)1 : (byte)0);
            }
        }

        private bool _ParanoidChecks;
        /// <summary>
        /// 设置或获取一个值，指示是否开启数据库强制性检查。
        /// </summary>
        public bool ParanoidChecks
        {
            get { return this._ParanoidChecks; }
            set
            {
                this._ParanoidChecks = value;
                LevelDBInterop.leveldb_options_set_paranoid_checks(_handle, value ? (byte)1 : (byte)0);
            }
        }

        private long _WriteBufferSize = 4 * 1024 * 1024;
        /// <summary>
        /// 设置或获取数据库写的缓存大小。默认为 4MB。
        /// </summary>
        public long WriteBufferSize
        {
            get { return this._WriteBufferSize; }
            set
            {
                this._WriteBufferSize = value;
                LevelDBInterop.leveldb_options_set_write_buffer_size(_handle, value);
            }
        }

        private int _MaxOpenFiles = 1000;
        /// <summary>
        /// 设置或获取打开数据库文件的数量（每个文件约 2MB）。默认为 1000。
        /// </summary>
        public int MaxOpenFiles
        {
            get { return this._MaxOpenFiles; }
            set
            {
                this._MaxOpenFiles = value;
                LevelDBInterop.leveldb_options_set_max_open_files(_handle, value);
            }
        }

        private long _BlockSize = 4 * 1024;
        /// <summary>
        /// 设置或获取数据包大小。此参数可以动态改变。默认为 4KB。
        /// </summary>
        public long BlockSize
        {
            get { return this._BlockSize; }
            set
            {
                this._BlockSize = value;
                LevelDBInterop.leveldb_options_set_block_size(_handle, value);
            }
        }

        private long _CacheSize = 8 * 1024 * 1024;
        /// <summary>
        /// 设置或获取缓存的容量。默认为 8MB。
        /// </summary>
        public long CacheSize
        {
            get { return this._CacheSize; }
            set
            {
                if(value < 0) throw new ArgumentOutOfRangeException("value");
                this.TryDestroyCacheHandle();
                this._CacheSize = value;
                this._cacheHandle = LevelDBInterop.leveldb_cache_create_lru((IntPtr)value);
                LevelDBInterop.leveldb_options_set_cache(this._handle, this._cacheHandle);
            }
        }

        private int _RestartInterval = 16;
        /// <summary>
        /// Number of keys between restart points for delta encoding of keys.
        /// This parameter can be changed dynamically.  
        /// Most clients should leave this parameter alone.
        ///
        /// Default: 16
        /// </summary>
        public int RestartInterval
        {
            get { return this._RestartInterval; }
            set
            {
                this._RestartInterval = value;
                LevelDBInterop.leveldb_options_set_block_restart_interval(_handle, value);
            }
        }

        private CompressionLevel _CompressionLevel = CompressionLevel.Snappy;
        /// <summary>
        /// 设置或获取一个值，指示数据库压缩的方式。默认为 <see cref="Aoite.LevelDB.CompressionLevel.Snappy"/>。压缩大概可以节省 1/3~1/2 的磁盘容量。
        /// </summary>
        public CompressionLevel CompressionLevel
        {
            get { return this._CompressionLevel; }
            set
            {
                this._CompressionLevel = value;
                LevelDBInterop.leveldb_options_set_compression(_handle, (int)value);
            }
        }

        #endregion

        /// <summary>
        /// 初始化一个 <see cref="Aoite.LevelDB.Options"/> 类的新实例。
        /// </summary>
        public Options()
        {
            _handle = LevelDBInterop.leveldb_options_create();
        }

        IntPtr _cacheHandle;

        private void TryDestroyCacheHandle()
        {
            if(this._cacheHandle != IntPtr.Zero)
            {
                LevelDBInterop.leveldb_cache_destroy(this._cacheHandle);
                this._cacheHandle = IntPtr.Zero;
            }
        }
        internal override void DestroyUnmanaged()
        {
            this.TryDestroyCacheHandle();
            LevelDBInterop.leveldb_options_destroy(this._handle);
        }

    }
}
