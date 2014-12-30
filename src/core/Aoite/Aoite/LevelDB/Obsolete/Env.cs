using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.LevelDB
{
    /// <summary>
    /// A default environment to access operating system functionality like 
    /// the filesystem etc of the current operating system.
    /// </summary>
    internal class Env : LevelDBHandle
    {
        public Env()
        {
            _handle = LevelDBInterop.leveldb_create_default_env();
        }

        internal override void DestroyUnmanaged()
        {
            LevelDBInterop.leveldb_env_destroy(_handle);
        }
    }
}
