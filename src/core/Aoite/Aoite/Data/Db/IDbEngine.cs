using Aoite.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace System
{
    /// <summary>
    /// 定义数据源查询与交互引擎的方法。
    /// </summary>
    public interface IDbEngine
    {
        /// <summary>
        /// 获取当前上下文的所属引擎。
        /// </summary>
        DbEngine Owner { get; }
        /// <summary>
        /// 创建一个到数据源的连接。
        /// </summary>
        /// <returns>返回一个到数据源的连接。</returns>
        DbConnection CreateConnection();
        /// <summary>
        /// 创建一个有关当前上下文的数据源事务。
        /// </summary>
        /// <returns>返回一个有关当前上下文的数据源事务，可以是一个 null 值。</returns>
        DbTransaction CreateTransaction();
    }
}