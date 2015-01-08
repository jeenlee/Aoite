using System;
using System.Collections.Generic;
using System.Text;

namespace Aoite.Data
{
    /// <summary>
    /// 一个 SQL 语句生成的实现。
    /// </summary>
    public interface IBuilder : IDbExecutor
    {
        /// <summary>
        /// 获取当前的查询参数集合。
        /// </summary>
        ExecuteParameterCollection Parameters { get; }
        /// <summary>
        /// 生成并获取当前的查询语句。
        /// </summary>
        string CommandText { get; }
        /// <summary>
        /// 生成执行数据源查询与交互的执行器。
        /// </summary>
        /// <returns>返回执行数据源查询与交互的执行器。</returns>
        IDbExecutor Execute();
        /// <summary>
        /// 结束并生成命令。
        /// </summary>
        /// <returns>返回一个命令。</returns>
        ExecuteCommand End();

        /// <summary>
        /// 添加 ORDER BY 的字段。
        /// </summary>
        /// <param name="fields">字段的集合。</param>
        /// <returns>返回 <see cref="Aoite.Data.ISelect"/> 的实例。</returns>
        ISelect OrderBy(params string[] fields);
        /// <summary>
        /// 添加 GROUP BY 的字段。
        /// </summary>
        /// <param name="fields">字段的集合。</param>
        /// <returns>返回 <see cref="Aoite.Data.ISelect"/> 的实例。</returns>
        ISelect GroupBy(params string[] fields);
    }
}
