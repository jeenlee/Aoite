using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Aoite.LevelDB
{
    /// <summary>
    /// 表示一个数据库异常。
    /// </summary>
    public class LevelDBException : Exception
    {
        internal LevelDBException(string message) : base(message) { }

        /// <summary>
        /// 检查指定的异常句柄。如果存在异常则抛出异常。
        /// </summary>
        /// <param name="error">异常句柄。</param>
        internal static void Check(IntPtr error)
        {
            if(error != IntPtr.Zero)
            {
                try
                {
                    var message = Marshal.PtrToStringAnsi(error);
                    throw new LevelDBException(message);
                }
                finally
                {
                    LevelDBInterop.leveldb_free(error);
                }
            }
        }
    }
}
