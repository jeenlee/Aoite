using System;
using System.Collections.Generic;
using System.Text;

namespace Aoite.LevelDB
{
    /// <summary>
    /// Options that control write operations.
    /// </summary>
    internal class WriteOptions : LevelDBHandle
    {
        public WriteOptions(bool sync)
        {
            _handle = LevelDBInterop.leveldb_writeoptions_create();
            if(sync)
            {
                LevelDBInterop.leveldb_writeoptions_set_sync(_handle, sync ? (byte)1 : (byte)0);
            }
        }

        internal override void DestroyUnmanaged()
        {
            LevelDBInterop.leveldb_writeoptions_destroy(_handle);
        }
    }
}
