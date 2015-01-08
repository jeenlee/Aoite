using System;
using System.Collections.Generic;
using System.Text;

namespace Aoite.Data
{
    /// <summary>
    /// 默认的参数配置信息。
    /// </summary>
    public class DefaultParameterSettings
    {
        /// <summary>
        /// 获取默认的参数配置信息。
        /// </summary>
        public static readonly DefaultParameterSettings Default = new DefaultParameterSettings();

        /// <summary>
        /// 获取参数名称的前缀（T-SQL 文本）。
        /// </summary>
        public virtual string PrefixWithText
        {
            get
            {
                return "@";
            }
        }

        /// <summary>
        /// 获取参数名称的前缀（参数集合）。
        /// </summary>
        public virtual string PrefixWithCollection
        {
            get
            {
                return "@";
            }
        }

        /// <summary>
        /// 转义指定的名称。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <returns>返回转义后的名称。</returns>
        public virtual string EscapeName(string name) { return "[" + name + "]"; }

        /// <summary>
        /// 初始化一个 <see cref="Aoite.Data.DefaultParameterSettings"/> 类的新实例。
        /// </summary>
        protected DefaultParameterSettings() { }
    }

    /// <summary>
    /// 基于 Oracle 的参数配置信息。
    /// </summary>
    public class OracleParameterSettings : DefaultParameterSettings
    {
        /// <summary>
        /// 获取基于 Oracle 的参数配置信息。
        /// </summary>
        public static readonly OracleParameterSettings Oracle = new OracleParameterSettings();

        /// <summary>
        /// 初始化一个 <see cref="Aoite.Data.OracleParameterSettings"/> 类的新实例。
        /// </summary>
        protected OracleParameterSettings() { }
        /// <summary>
        /// 获取参数名称的前缀（T-SQL 文本）。
        /// </summary>
        public override string PrefixWithText
        {
            get
            {
                return ":";
            }
        }

        /// <summary>
        /// 获取参数名称的前缀（参数集合）。在 Oracle 中，并不需要指定参数名称的前缀。
        /// </summary>
        public override string PrefixWithCollection
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 转义指定的名称。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <returns>返回转义后的名称。</returns>
        public override string EscapeName(string name) { return name.ToUpper(); }
    }
}
