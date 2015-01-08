using System;
using System.Collections.Generic;
using System.Text;

namespace Aoite.Data
{
    /// <summary>
    /// 一个 WHERE SQL 语句生成的实现。
    /// </summary>
    public interface IWhere : IBuilder
    {
        /// <summary>
        /// 添加自定义 SQL 语句。
        /// </summary>
        /// <param name="content">SQL 语句内容。</param>
        /// <returns>返回 <see cref="Aoite.Data.IWhere"/> 的实例。</returns>
        IWhere Sql(string content);
        /// <summary>
        /// 添加一个参数。
        /// </summary>
        /// <param name="name">参数名称。</param>
        /// <param name="value">参数值。</param>
        /// <returns>返回 <see cref="Aoite.Data.IWhere"/> 的实例。</returns>
        IWhere Parameter(string name, object value);
        /// <summary>
        /// 添加一个 AND 语句。
        /// </summary>
        /// <returns>返回 <see cref="Aoite.Data.IWhere"/> 的实例。</returns>
        IWhere And();
        /// <summary>
        /// 添加一个 OR 语句。
        /// </summary>
        /// <returns>返回 <see cref="Aoite.Data.IWhere"/> 的实例。</returns>
        IWhere Or();
        /// <summary>
        /// 添加一个开始括号。
        /// </summary>
        /// <returns>返回 <see cref="Aoite.Data.IWhere"/> 的实例。</returns>
        IWhere BeginGroup();
        /// <summary>
        /// 添加一个开始括号，并添加 SQL 表达式。
        /// </summary>
        /// <param name="expression">逻辑表达式（如：“t1.x=@x”）。</param>
        /// <param name="name">参数的名称。</param>
        /// <param name="value">参数的值。</param>
        /// <returns>返回 <see cref="Aoite.Data.IWhere"/> 的实例。</returns>
        IWhere BeginGroup(string expression, string name, object value);
        /// <summary>
        /// 添加一个结束括号。
        /// </summary>
        /// <returns>返回 <see cref="Aoite.Data.IWhere"/> 的实例。</returns>
        IWhere EndGroup();
        /// <summary>
        /// 添加 AND 表达式。
        /// </summary>
        /// <param name="expression">逻辑表达式（如：“t1.x IS NULL”）。</param>
        /// <returns>返回 <see cref="Aoite.Data.IWhere"/> 的实例。</returns>
        IWhere And(string expression);
        /// <summary>
        /// 添加 AND 表达式。
        /// </summary>
        /// <param name="expression">逻辑表达式（如：“t1.x=@x”）。</param>
        /// <param name="name">参数的名称。</param>
        /// <param name="value">参数的值。</param>
        /// <returns>返回 <see cref="Aoite.Data.IWhere"/> 的实例。</returns>
        IWhere And(string expression, string name, object value);
        /// <summary>
        /// 添加 OR 表达式。
        /// </summary>
        /// <param name="expression">逻辑表达式（如：“t1.x IS NULL”）。</param>
        /// <returns>返回 <see cref="Aoite.Data.IWhere"/> 的实例。</returns>
        IWhere Or(string expression);
        /// <summary>
        /// 添加 OR 表达式。
        /// </summary>
        /// <param name="expression">逻辑表达式（如：“t1.x=@x”）。</param>
        /// <param name="name">参数的名称。</param>
        /// <param name="value">参数的值。</param>
        /// <returns>返回 <see cref="Aoite.Data.IWhere"/> 的实例。</returns>
        IWhere Or(string expression, string name, object value);
        /// <summary>
        /// 添加 AND 表达式“AND (fieldName=@namePrefix0 OR fieldName=@namePrefix1)”。
        /// </summary>
        /// <typeparam name="T">要枚举的实例的类型。</typeparam>
        /// <param name="fieldName">字段的名称。</param>
        /// <param name="namePrefix">参数的名称前缀。</param>
        /// <param name="values">参数值集合。</param>
        /// <returns>返回 <see cref="Aoite.Data.IWhere"/> 的实例。</returns>
        IWhere And<T>(string fieldName, string namePrefix, T[] values);
        /// <summary>
        /// 添加 OR 表达式“OR (fieldName=@namePrefix0 OR fieldName=@namePrefix1)”。
        /// </summary>
        /// <typeparam name="T">要枚举的实例的类型。</typeparam>
        /// <param name="fieldName">字段的名称。</param>
        /// <param name="namePrefix">参数的名称前缀。</param>
        /// <param name="values">参数值集合。</param>
        /// <returns>返回 <see cref="Aoite.Data.IWhere"/> 的实例。</returns>
        IWhere Or<T>(string fieldName, string namePrefix, T[] values);
        /// <summary>
        /// 添加 AND 表达式“AND fieldName IN (@namePrefix0, @namePrefix1)”。
        /// </summary>
        /// <typeparam name="T">要枚举的实例的类型。</typeparam>
        /// <param name="fieldName">字段的名称。</param>
        /// <param name="namePrefix">参数的名称前缀。</param>
        /// <param name="values">参数值集合。</param>
        /// <returns>返回 <see cref="Aoite.Data.IWhere"/> 的实例。</returns>
        IWhere AndIn<T>(string fieldName, string namePrefix, T[] values);
        /// <summary>
        /// 添加 AND 表达式“AND fieldName NOT IN (@namePrefix0, @namePrefix1)”。
        /// </summary>
        /// <typeparam name="T">要枚举的实例的类型。</typeparam>
        /// <param name="fieldName">字段的名称。</param>
        /// <param name="namePrefix">参数的名称前缀。</param>
        /// <param name="values">参数值集合。</param>
        /// <returns>返回 <see cref="Aoite.Data.IWhere"/> 的实例。</returns>
        IWhere AndNotIn<T>(string fieldName, string namePrefix, T[] values);
        /// <summary>
        /// 添加 OR 表达式“OR fieldName IN (@namePrefix0, @namePrefix1)”。
        /// </summary>
        /// <typeparam name="T">要枚举的实例的类型。</typeparam>
        /// <param name="fieldName">字段的名称。</param>
        /// <param name="namePrefix">参数的名称前缀。</param>
        /// <param name="values">参数值集合。</param>
        /// <returns>返回 <see cref="Aoite.Data.IWhere"/> 的实例。</returns>
        IWhere OrIn<T>(string fieldName, string namePrefix, T[] values);
        /// <summary>
        /// 添加 OR 表达式“OR fieldName NOT IN (@namePrefix0, @namePrefix1)”。
        /// </summary>
        /// <typeparam name="T">要枚举的实例的类型。</typeparam>
        /// <param name="fieldName">字段的名称。</param>
        /// <param name="namePrefix">参数的名称前缀。</param>
        /// <param name="values">参数值集合。</param>
        /// <returns>返回 <see cref="Aoite.Data.IWhere"/> 的实例。</returns>
        IWhere OrNotIn<T>(string fieldName, string namePrefix, T[] values);
    }
}
