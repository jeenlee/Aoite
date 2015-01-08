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
    public class ColumnAttribute : AliasAttribute, IKeyAttribute
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
        /// <param name="isKey">指示当前属性是否为主要成员。</param>
        public ColumnAttribute(string name, bool isKey)
            : base(name)
        {
            this.IsKey = isKey;
        }

        /// <summary>
        /// 初始化一个空的 <see cref="Aoite.Data.ColumnAttribute"/> 类的新实例。
        /// </summary>
        public ColumnAttribute() { }

        /// <summary>
        /// 获取或设置一个值，该值指示当前属性是否为主键。
        /// </summary>
        public bool IsKey { get; set; }
    }
}
