using Aoite.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System
{
    /// <summary>
    /// 表示一个属性的映射器。
    /// </summary>
    public class PropertyMapper : DynamicProperty
    {
        /// <summary>
        /// 获取或设置映射器的名称。
        /// </summary>
        public virtual string Name { get; set; }
        /// <summary>
        /// 获取或设置一个值，指示是否为唯一标识。
        /// </summary>
        public virtual bool IsKey { get; set; }

        /// <summary>
        /// 获取属性所属的类型映射器。
        /// </summary>
        public virtual TypeMapper TypeMapper { get; private set; }

        /// <summary>
        /// 获取或设置一个值，该值指示当前成员是否已标识忽略标识。
        /// </summary>
        public virtual bool IsIgnore { get; set; }

        private Lazy<object> _LazyTypeDefaultValue;
        /// <summary>
        /// 获取类型的默认值。
        /// </summary>
        public object TypeDefaultValue { get { return this._LazyTypeDefaultValue.Value; } }

        /// <summary>
        /// 指定属性元数据，初始化一个 <see cref="System.PropertyMapper"/> 类的新实例。
        /// </summary>
        /// <param name="typeMapper">类型的映射器。</param>
        /// <param name="property">成员的属性元数据。</param>
        public PropertyMapper(TypeMapper typeMapper, PropertyInfo property)
            : base(property)
        {
            if(typeMapper == null) throw new ArgumentNullException("typeMapper");
            this.TypeMapper = typeMapper;
            this.IsIgnore = property.GetAttribute<IgnoreAttribute>() != null;
            this._LazyTypeDefaultValue = new Lazy<object>(property.PropertyType.GetDefaultValue);

            var aliasAttr = property.GetAttribute<IAliasAttribute>();
            this.Name = aliasAttr != null && aliasAttr.Name != null
                ? aliasAttr.Name
                : property.Name;

            var keyAttr = property.GetAttribute<IKeyAttribute>();
            this.IsKey = keyAttr != null && keyAttr.IsKey;
        }
    }
}
