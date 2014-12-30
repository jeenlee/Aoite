using Aoite.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// 表示一个 WHERE 的条件参数。
    /// </summary>
    public class WhereParameters
    {
        /// <summary>
        /// 获取或设置 WHERE 的语句。可以为 null 值。
        /// </summary>
        public string Where { get; set; }
        /// <summary>
        /// 获取或设置 WHERE 的参数集合。可以为 null 值。
        /// </summary>
        public ExecuteParameterCollection Parameters { get; set; }
        /// <summary>
        /// 初始化一个空的 <see cref="System.WhereParameters"/> 类的新实例。
        /// </summary>
        public WhereParameters() { }
        /// <summary>
        /// 初始化一个 <see cref="System.WhereParameters"/> 类的新实例。
        /// </summary>
        /// <param name="where">WHERE 的语句。</param>
        /// <param name="ps">WHERE 的参数集合。</param>
        public WhereParameters(string where, ExecuteParameterCollection ps = null)
        {
            this.Where = where;
            this.Parameters = ps;
        }

        /// <summary>
        /// 解析匿名对象参数集合，并用 AND 符拼接 WHERE 语句。
        /// </summary>
        /// <param name="objectInstance">匿名参数集合实例。</param>
        /// <param name="binary">二元运算符。</param>
        public static WhereParameters Parse(object objectInstance, string binary = "AND")
        {
            return Parse(new ExecuteParameterCollection(objectInstance), binary);
        }
        /// <summary>
        /// 解析参数集合，并用 AND 符拼接 WHERE 语句。
        /// </summary>
        /// <param name="ps">参数集合。</param>
        /// <param name="binary">二元运算符。</param>
        public static WhereParameters Parse(ExecuteParameterCollection ps = null, string binary = "AND")
        {
            return new WhereParameters(Db.Context.CreateWhere(ps, binary), ps);
        }
    }
}
