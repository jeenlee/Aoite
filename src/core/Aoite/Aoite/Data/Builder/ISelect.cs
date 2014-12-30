using System;
using System.Collections.Generic;
using System.Text;

namespace Aoite.Data
{
    /// <summary>
    /// 一个 SELECT SQL 语句生成的实现。
    /// </summary>
    public interface ISelect : IBuilder
    {
        /// <summary>
        /// 添加 SELECT 的字段。
        /// </summary>
        /// <param name="fields">字段的集合。</param>
        /// <returns>返回 <see cref="Aoite.Data.ISelect"/> 的实例。</returns>
        ISelect Select(params string[] fields);
        /// <summary>
        /// 添加 FROM 后的 SQL 语句。
        /// </summary>
        /// <param name="fromTables">SQL 语句。</param>
        /// <returns>返回 <see cref="Aoite.Data.ISelect"/> 的实例。</returns>
        ISelect From(string fromTables);
        /// <summary>
        /// 添加一个参数。
        /// </summary>
        /// <param name="name">参数名称。</param>
        /// <param name="value">参数值。</param>
        /// <returns>返回 <see cref="Aoite.Data.IWhere"/> 的实例。</returns>
        ISelect Parameter(string name, object value);
        /// <summary>
        /// 进入 WHERE SQL 实现。
        /// </summary>
        /// <returns>返回 <see cref="Aoite.Data.IWhere"/> 的实例。</returns>
        IWhere Where();
        /// <summary>
        /// 进入 WHERE SQL 实现。
        /// </summary>
        /// <param name="expression">逻辑表达式（如：“t1.x IS NULL”）。</param>
        /// <returns>返回 <see cref="Aoite.Data.IWhere"/> 的实例。</returns>
        IWhere Where(string expression);
        /// <summary>
        /// 进入 WHERE SQL 实现。
        /// </summary>
        /// <param name="expression">逻辑表达式（如：“t1.x=@x”）。</param>
        /// <param name="name">参数的名称。</param>
        /// <param name="value">参数的值。</param>
        /// <returns>返回 <see cref="Aoite.Data.IWhere"/> 的实例。</returns>
        IWhere Where(string expression, string name, object value);
        /// <summary>
        /// 进入 WHERE SQL 实现，添加表达式“(fieldName=@namePrefix0 OR fieldName=@namePrefix1)”。
        /// </summary>
        /// <typeparam name="T">要枚举的实例的类型。</typeparam>
        /// <param name="fieldName">字段的名称。</param>
        /// <param name="namePrefix">参数的名称前缀。</param>
        /// <param name="values">参数值集合。</param>
        /// <returns>返回 <see cref="Aoite.Data.IWhere"/> 的实例。</returns>
        IWhere Where<T>(string fieldName, string namePrefix, T[] values);
        /// <summary>
        /// 进入 WHERE SQL 实现，添加表达式“fieldName IN (@namePrefix0, @namePrefix1)”。
        /// </summary>
        /// <typeparam name="T">要枚举的实例的类型。</typeparam>
        /// <param name="fieldName">字段的名称。</param>
        /// <param name="namePrefix">参数的名称前缀。</param>
        /// <param name="values">参数值集合。</param>
        /// <returns>返回 <see cref="Aoite.Data.IWhere"/> 的实例。</returns>
        IWhere WhereIn<T>(string fieldName, string namePrefix, T[] values);
        /// <summary>
        /// 进入 WHERE SQL 实现，添加表达式“fieldName NOT IN (@namePrefix0, @namePrefix1)”。
        /// </summary>
        /// <typeparam name="T">要枚举的实例的类型。</typeparam>
        /// <param name="fieldName">字段的名称。</param>
        /// <param name="namePrefix">参数的名称前缀。</param>
        /// <param name="values">参数值集合。</param>
        /// <returns>返回 <see cref="Aoite.Data.IWhere"/> 的实例。</returns>
        IWhere WhereNotIn<T>(string fieldName, string namePrefix, T[] values);
    }
}
