using System;
using System.Collections.Generic;
using System.Text;

namespace Aoite.LevelDB
{
    // A Cache is an interface that maps keys to values.  It has internal
    // synchronization and may be safely accessed concurrently from
    // multiple threads.  It may automatically evict entries to make room
    // for new entries.  Values have a specified charge against the cache
    // capacity.  For example, a cache where the values are variable
    // length strings, may use the length of the string as the charge for
    // the string.
    //
    // A builtin cache implementation with a least-recently-used eviction
    // policy is provided.  Clients may use their own implementations if
    // they want something more sophisticated (like scan-resistance, a
    // custom eviction policy, variable cache sizing, etc.)
 
    /// <summary>
    /// 表示一个提供数据库的读取高速缓存。
    /// </summary>
    internal class Cache : LevelDBHandle
    {
        /// <summary>
        /// 初始化一个 <see cref="Aoite.LevelDB.Cache"/> 类的新实例。
        /// </summary>
        /// <param name="capacity">缓存的容量。</param>
        public Cache(long capacity)
        {
            _handle = LevelDBInterop.leveldb_cache_create_lru((IntPtr)capacity);
        }

        internal override void DestroyUnmanaged()
        {
            LevelDBInterop.leveldb_cache_destroy(_handle);
        }
    }
}
