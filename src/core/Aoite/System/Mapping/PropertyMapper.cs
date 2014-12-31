//using Aoite.Reflection;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Text;

//namespace System
//{
//    /// <summary>
//    /// 表示一个属性的映射器。
//    /// </summary>
//    public class PropertyMapper : DynamicProperty, IDynamicMapper
//    {
//        private string _Name;
//        /// <summary>
//        /// 获取或设置映射器的名称。
//        /// </summary>
//        public virtual string Name { get { return this._Name; } set { this._Name = value; } }

//        private ColumnAttribute _Column = EmptyColumnAttribute.Empty;
//        /// <summary>
//        /// 获取或设置成员的列特性。
//        /// </summary>
//        public ColumnAttribute Column { get { return this._Column; } set { this._Column = value ?? EmptyColumnAttribute.Empty; } }

//        private ITypeMapper _Entity;
//        /// <summary>
//        /// 获取属性所属的实体映射器。
//        /// </summary>
//        public ITypeMapper Entity { get { return this._Entity; } }

//        private bool _IsIgnore;
//        /// <summary>
//        /// 获取或设置一个值，该值指示当前成员是否已标识忽略标识。
//        /// </summary>
//        public bool IsIgnore { get { return this._IsIgnore; } set { this._IsIgnore = value; } }

//        private object _TypeDefaultValue;
//        /// <summary>
//        /// 获取类型的默认值。
//        /// </summary>
//        public object TypeDefaultValue { get { return this._TypeDefaultValue; } }

//        private MapperValueEventHandler _AppendValue;
//        /// <summary>
//        /// 获取或设置当需要生成值语句时发生的委托。
//        /// </summary>
//        public virtual MapperValueEventHandler AppendValue { get { return this._AppendValue; } set { this._AppendValue = value ?? this.DefaultAppendValue; } }

//        private bool _IsIdentityPK;
//        /// <summary>
//        /// 指定属性元数据，初始化一个 <see cref="Aoite.Data.PropertyMapper"/> 类的新实例。
//        /// </summary>
//        /// <param name="entity">实体的映射器。</param>
//        /// <param name="property">成员的属性元数据。</param>
//        public PropertyMapper(IEntityMapper entity, PropertyInfo property)
//            : base(property)
//        {
//            if(entity == null) throw new ArgumentNullException("entity");
//            this._AppendValue = this.DefaultAppendValue;
//            this._Entity = entity;
//            this._IsIgnore = property.GetAttribute<IgnoreAttribute>() != null;
//            this._TypeDefaultValue = property.PropertyType.GetDefaultValue();
//            this._Name = property.Name;

//            var columnAttr = property.GetAttribute<ColumnAttribute>();
//            if(columnAttr != null)
//            {
//                columnAttr.Initialization(this);
//                var pType = property.PropertyType;
//                this._IsIdentityPK = columnAttr.IsPrimaryKey && (pType == Types.Int64 || pType == Types.Int32);
//            }
//        }

//        private void AppendValueWithEmpty(MapperValueEventArgs e, object value)
//        {
//            var upperName = this._Name.ToUpper();
//            e.Builder
//                  .Append(e.ParameterSettings.PrefixWithText)
//                  .Append(upperName);
//            e.Parameters.Add(e.ParameterSettings.PrefixWithCollection + upperName, value ?? this.GetValue(e.Entity));
//        }

//        /// <summary>
//        /// 以默认的方式生成值语句。
//        /// </summary>
//        /// <param name="sender">事件对象。</param>
//        /// <param name="e">事件参数。</param>
//        /// <returns>返回执行的结果，为 null 值表示执行成功，否则表示执行的错误信息。</returns>
//        protected internal virtual void DefaultAppendValue(object sender, MapperValueEventArgs e)
//        {
//            var column = this._Column;
//            if(column is EmptyColumnAttribute) this.AppendValueWithEmpty(e, null);
//            else if(e.Runtime == MapperRuntime.Insert && !string.IsNullOrEmpty(column.InsertValueSql))
//            {
//                e.Builder.Append(column.InsertValueSql);
//            }
//            else if(e.Runtime == MapperRuntime.Update && !string.IsNullOrEmpty(column.UpdateValueSql))
//            {
//                e.Builder.Append(column.UpdateValueSql);
//            }
//            else
//            {
//                var value = this.GetValue(e.Entity);

//                var isNullValue = object.Equals(value, this._TypeDefaultValue);
//                if(isNullValue)
//                {
//                    if(e.Runtime == MapperRuntime.Insert && this._IsIdentityPK) return;
//                    if(column.DefaultValue != null) value = column.DefaultValue;
//                    else if(column.IsNotNull) throw new NotSupportedException("字段 [" + this._Name + "] 设定不允许为空！");
//                }

//                this.AppendValueWithEmpty(e, value);
//            }
//        }

//        /// <summary>
//        /// 以默认方式生成字段语句。
//        /// </summary>
//        /// <param name="builder">查询语句缓冲区。</param>
//        /// <param name="parameterSettings">参数的配置信息。</param>
//        protected internal virtual void DefaultAppendField(StringBuilder builder, DefaultParameterSettings parameterSettings)
//        {
//            builder.Append(parameterSettings.EscapeName(this._Name));
//        }
//    }
//}
