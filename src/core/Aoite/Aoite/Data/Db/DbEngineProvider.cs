using System;

namespace System
{
    /// <summary>
    /// 表示一个数据源查询与交互引擎的提供程序。
    /// </summary>
    [Serializable]
    public enum DbEngineProvider
    {
        /// <summary>
        /// 表示一个 Microsoft SQL Server 的数据源查询与交互引擎的实例。
        /// </summary>
        MicrosoftSqlServer = 0,
        /// <summary>
        /// 表示一个 Microsoft SQL Server 的数据源查询与交互引擎的实例。
        /// </summary>
        MicrosoftSqlServerCompact = 6,
        ///// <summary>
        ///// 表示一个 Microsoft OleDb 2003(含) 之前的数据源查询与交互引擎的实例。
        ///// </summary>
        //MicrosoftOleDb2003 = 1,
        ///// <summary>
        ///// 表示一个 Microsoft OleDb 2007(含) 之后的数据源查询与交互引擎的实例。
        ///// </summary>
        //MicrosoftOleDb2007 = 2,
        ///// <summary>
        ///// 表示一个 SQLite 数据源查询与交互引擎的实例。
        ///// </summary>
        //SQLite = 3,
        ///// <summary>
        ///// 表示一个 Oracle 数据源查询与交互引擎的实例。
        ///// </summary>
        //Oracle = 4,
        ///// <summary>
        ///// 表示一个 MySql 数据源查询与交互引擎的实例。
        ///// </summary>
        //MySql = 5,
        ///// <summary>
        ///// 表示一个自定义数据源。
        ///// </summary>
        //Custom = 7,
    }
}
