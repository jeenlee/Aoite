using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.LevelDB
{
    /// <summary>
    /// 表示一个数据库快照。
    /// </summary>
    public class Snapshot : LevelDBHandle
    {
        // pointer to parent so that we can call ReleaseSnapshot(this) when disposed
        private WeakReference _parent;  // as DB

        internal Snapshot(IntPtr handle, LDB parent)
        {
            _handle = handle;
            _parent = new WeakReference(parent);
        }


        internal override void DestroyUnmanaged()
        {
            var parent = _parent.Target as LDB;
            if (parent != null)
                LevelDBInterop.leveldb_release_snapshot(parent._handle, _handle);
        }
    }
}
