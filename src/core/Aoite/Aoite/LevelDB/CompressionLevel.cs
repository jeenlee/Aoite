using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.LevelDB
{
    /// <summary>
    /// 定义数据库压缩的方式。
    /// </summary>
    public enum CompressionLevel
    {
        /// <summary>
        /// 表示采用无压缩方式。
        /// </summary>
        No = 0,
        /// <summary>
        /// 表示轻快压缩方式。
        /// </summary>
        Snappy = 1
    }
}
