using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Data
{
    /// <summary>
    /// 表示一个具有列的特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ColumnAttribute : AliasAttribute
    {
        /// <summary>
        /// 指示当前属性是否为主要成员，初始化一个 <see cref="Aoite.Data.ColumnAttribute"/> 类的新实例。
        /// </summary>
        /// <param name="isPrimaryKey">指示当前属性是否为主要成员。</param>
        public ColumnAttribute(bool isPrimaryKey) : this(null, isPrimaryKey) { }

        /// <summary>
        /// 指定名称，初始化一个 <see cref="Aoite.Data.ColumnAttribute"/> 类的新实例。
        /// </summary>
        /// <param name="name">名称。</param>
        public ColumnAttribute(string name) : this(name, false) { }

        /// <summary>
        /// 指定名称和指示当前属性是否为主要成员，初始化一个 <see cref="Aoite.Data.ColumnAttribute"/> 类的新实例。
        /// </summary>
        /// <param name="name">名称。</param>
        /// <param name="isPrimaryKey">指示当前属性是否为主要成员。</param>
        public ColumnAttribute(string name, bool isPrimaryKey)
            : base(name)
        {
            this.IsPrimaryKey = isPrimaryKey;
        }

        /// <summary>
        /// 初始化一个空的 <see cref="Aoite.Data.ColumnAttribute"/> 类的新实例。
        /// </summary>
        public ColumnAttribute() { }

        private bool _IsPrimaryKey;
        /// <summary>
        /// 获取或设置一个值，该值指示当前属性是否为主键。
        /// </summary>
        public bool IsPrimaryKey { get { return this._IsPrimaryKey; } set { this._IsPrimaryKey = value; } }

        /// <summary>
        /// 获取或设置默认值（该值与 LINQ 无关）。
        /// </summary>
        public object DefaultValue { get; set; }

        /// <summary>
        /// 获取或设置属性的值插入 SQL 语句（该值与 LINQ 无关）。
        /// <para>当前列如果已设置值，则当前配置将被忽略。</para>
        /// </summary>
        public string InsertValueSql { get; set; }

        /// <summary>
        /// 获取或设置属性的值更新 SQL 语句（该值与 LINQ 无关）。
        /// <para>当前列如果已设置值，则当前配置将被忽略。</para>
        /// </summary>
        public string UpdateValueSql { get; set; }

        /// <summary>
        /// 获取或设置一个值，该值指示列是否不可包含 null 值。
        /// </summary>
        public bool IsNotNull { get; set; }

        /// <summary>
        /// 初始化映射器。
        /// </summary>
        /// <param name="mapper">映射器。</param>
        public void Initialization(PropertyMapper mapper)
        {
            if(this.Name != null) mapper.Name = this.Name;
            mapper.Column = this;
        }

    }
}
