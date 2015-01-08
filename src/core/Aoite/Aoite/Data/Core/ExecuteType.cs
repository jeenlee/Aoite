using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Data
{
    /// <summary>
    /// 数据源查询的操作类型。
    /// </summary>
    public enum ExecuteType
    {
        /// <summary>
        /// 未知类型。执行的通常是一个非 Execute 作为起始的数据源查询与交互函数。
        /// </summary>
        Unknow = 0,
        /// <summary>
        /// 无值查询。
        /// </summary>
        NoQuery = 1,
        /// <summary>
        /// 单值查询。
        /// </summary>
        Scalar = 2,
        /// <summary>
        /// 读取器查询。
        /// </summary>
        Reader = 4,
        /// <summary>
        /// 数据集查询。
        /// </summary>
        DataSet = 8,
        /// <summary>
        /// 数据表查询。
        /// </summary>
        Table = 16
    }
}
