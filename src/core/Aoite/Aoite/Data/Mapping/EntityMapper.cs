using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Aoite.Data
{
    /// <summary>
    /// 表示一个实体的映射器。
    /// </summary>
    public class EntityMapper : IEntityMapper
    {
        /// <summary>
        /// 获取或设置表的特性。
        /// </summary>
        public TableAttribute Table { get; set; }
        /// <summary>
        /// 初始化一个 <see cref="Aoite.Data.EntityMapper"/> 类的新实例。
        /// </summary>
        public EntityMapper() : this(0) { }

        internal EntityMapper(int capacity)
        {
            this._Properties = new Dictionary<string, PropertyMapper>(capacity, StringComparer.CurrentCultureIgnoreCase);
        }

        private string _Name;
        /// <summary>
        /// 获取或设置映射器的名称。
        /// </summary>
        public virtual string Name { get { return this._Name; } set { this._Name = value; } }

        private Type _Type;
        /// <summary>
        /// 获取实体的类型。
        /// </summary>
        public virtual Type Type { get { return this._Type; } }

        /// <summary>
        /// 实体的属性映射集合。
        /// </summary>
        protected readonly Dictionary<string, PropertyMapper> _Properties;
        /// <summary>
        /// 获取实体的属性映射集合。
        /// </summary>
        public IEnumerable<PropertyMapper> Properties { get { return this._Properties.Values; } }

        /// <summary>
        /// 指定属性名，判断指定的属性是否存在。
        /// </summary>
        /// <param name="propertyName">属性名称。</param>
        /// <returns>存在返回 true，否则返回 false。</returns>
        public bool ContainsProperty(string propertyName)
        {
            return this._Properties.ContainsKey(propertyName);
        }

        /// <summary>
        /// 获取指定属性名称的属性映射。
        /// </summary>
        /// <param name="propertyName">属性名称。</param>
        public PropertyMapper this[string propertyName]
        {
            get
            {
                return this._Properties.TryGetValue(propertyName);
            }
        }

        /// <summary>
        /// 获取实体的属性映射集合的元素数。
        /// </summary>
        public int Count { get { return this._Properties.Count; } }

        /// <summary>
        /// 检查指定实例的属性值。
        /// </summary>
        /// <param name="instance">实例。</param>
        /// <param name="property">属性。</param>
        /// <param name="value">属性的值。</param>
        /// <returns>返回新的属性值。</returns>
        protected virtual object CheckValue(object instance, PropertyInfo property, object value) { return value; }

        /// <summary>
        /// 检查指定数据行的列值。
        /// </summary>
        /// <param name="row">数据行。</param>
        /// <param name="column">列。</param>
        /// <param name="value">列的值。</param>
        /// <returns>返回新的列值。</returns>
        protected virtual object CheckValue(DataRow row, DataColumn column, object value) { return value; }

        private void SetPropertyValue(object ownerInstance, string name, Type valueType, object value)
        {
            PropertyMapper pMapper;

            if(!this._Properties.TryGetValue(name, out pMapper))
            {
                if(DbEngine.IgnoreUnfoundEntityProperty) return;
                throw new KeyNotFoundException("填充数据时候发生错误，无法在 " + ownerInstance.GetType().FullName + " 找到字段 " + name + " 对应的属性成员。");
            }
            var property = pMapper.Property;
            var propertyType = property.PropertyType;

            if(Convert.IsDBNull(value)) value = propertyType.GetDefaultValue();
            else if(valueType != propertyType)
            {
                try
                {
                    value = propertyType.ChangeType(value);
                }
                catch(Exception ex)
                {
                    throw new InvalidCastException("列 {0} 无法将值类型 {1} 转换为类型 {2}。".Fmt(name,valueType.FullName,propertyType.FullName), ex);
                }
            }

            pMapper.SetValue(ownerInstance, this.CheckValue(ownerInstance, property, value));
        }

        /// <summary>
        /// 将指定的数据行填充到实体。
        /// </summary>
        /// <param name="form">数据行。</param>
        /// <param name="to">实例。</param>
        public virtual void FillEntity(DataRow form, object to)
        {
            if(to != null)
                foreach(DataColumn column in form.Table.Columns)
                    this.SetPropertyValue(to, column.ColumnName, column.DataType, form[column]);
        }

        /// <summary>
        /// 将指定的数据读取器填充到实体。
        /// </summary>
        /// <param name="form">数据读取器。</param>
        /// <param name="to">实例。</param>
        public virtual void FillEntity(IDataReader form, object to)
        {
            if(to != null)
                for(int i = 0; i < form.FieldCount; i++)
                    this.SetPropertyValue(to, form.GetName(i), form.GetFieldType(i), form.GetValue(i));
        }

        /// <summary>
        /// 将指定的实体填充到数据行。
        /// </summary>
        /// <param name="form">实例。</param>
        /// <param name="to">数据行。</param>
        public virtual void FillRow(object form, DataRow to)
        {
            if(form == null) return;
            PropertyMapper pMapper;
            string name;
            object rowValue, entityValue;

            foreach(DataColumn column in to.Table.Columns)
            {
                name = column.ColumnName;
                rowValue = to[column];

                if(!this._Properties.TryGetValue(name, out pMapper))
                {
                    if(DbEngine.IgnoreUnfoundEntityProperty) continue;
                    throw new KeyNotFoundException("填充数据时候发生错误，无法在 " + form.GetType().FullName + " 找到字段 " + name + " 对应的属性成员。");
                }
                entityValue = pMapper.GetValue(form) ?? DBNull.Value;
                if(object.Equals(entityValue, rowValue)) continue;
                to[column] = this.CheckValue(to, column, entityValue);
            }
        }

        internal static IEnumerable<PropertyMapper> FindProperties(IEntityMapper mapper, ref object entity)
        {
            IEnumerable<PropertyMapper> pms = mapper.Properties;
            var entityType = entity.GetType();
            if(entityType.IsAssignableFrom(mapper.Type)) return pms;

            var mapper2 = Create(entityType);

            var query = from op in pms
                        join np in mapper2.Properties on op.Property.Name.ToLower() equals np.Name.ToLower()
                        select new { op, np };
            var entity2 = Activator.CreateInstance(mapper.Type, true);

            List<PropertyMapper> pms2 = new List<PropertyMapper>();
            foreach(var item in query)
            {
                pms2.Add(item.op);
                item.op.SetValue(entity2, item.np.GetValue(entity));
            }
            entity = entity2;
            return pms2;
        }

        /// <summary>
        /// 指定实体创建一个插入的命令。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="entity">实体的实例对象。</param>
        /// <param name="tableName">实体的实际表名称，可以为 null 值。</param>
        /// <returns>返回一个查询结果。</returns>
        public ExecuteCommand CreateInsertCommand(IDbEngine engine, object entity, string tableName = null)
        {
            return engine.Owner.Injector.CreateInsertCommand(engine, this, entity, tableName);
        }

        /// <summary>
        /// 指定实体创建一个更新的命令。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="entity">实体的实例对象。</param>
        /// <param name="tableName">实体的实际表名称，可以为 null 值。</param>
        /// <returns>返回一个命令结果。</returns>
        public ExecuteCommand CreateUpdateCommand(IDbEngine engine, object entity, string tableName = null)
        {
            if(engine == null) throw new ArgumentNullException("engine");
            if(entity == null) throw new ArgumentNullException("entity");

            var parSettings = engine.Owner.Injector.ParameterSettings;
            var setBuilder = new StringBuilder("UPDATE ")
                                .Append(parSettings.EscapeName(tableName ?? this._Name))
                                .Append(" SET ");
            var whereBuilder = new StringBuilder();
            var ps = new ExecuteParameterCollection(this.Count);

            int index = 0;
            foreach(var property in FindProperties(this, ref entity))
            {
                if(property.IsIgnore) continue;

                StringBuilder builder;
                if(property.Column.IsPrimaryKey)
                {
                    builder = whereBuilder;
                    if(builder.Length > 0) builder.Append(" AND ");
                }
                else
                {
                    builder = setBuilder;
                    if(index++ > 0) builder.Append(',');
                }

                property.DefaultAppendField(builder, parSettings);
                builder.Append('=');
                property.AppendValue(property, new MapperValueEventArgs(engine, builder, parSettings, ps, entity, MapperRuntime.Update));
            }

            if(whereBuilder.Length == 0) throw new NotSupportedException("{0} 的更新操作没有找到主键。".Fmt(entity.GetType().FullName));
            setBuilder.Append(" WHERE ").Append(whereBuilder.ToString());
            return new ExecuteCommand(setBuilder.ToString(), ps);
        }

        /// <summary>
        /// 指定实体创建一个删除的命令。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="entityOrPKValue">实体的实例对象（引用类型）或一个主键的值（值类型）。</param>
        /// <param name="tableName">实体的实际表名称，可以为 null 值。</param>
        /// <returns>返回一个命令结果。</returns>
        public ExecuteCommand CreateDeleteCommand(IDbEngine engine, object entityOrPKValue, string tableName = null)
        {
            if(engine == null) throw new ArgumentNullException("engine");
            if(entityOrPKValue == null) throw new ArgumentNullException("entityOrPKValue");

            var type = entityOrPKValue.GetType();
            if(type == Types.Guid) return CreateDeleteCommandWithPK(engine, entityOrPKValue, tableName);

            switch(Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                case TypeCode.Byte:
                case TypeCode.Char:
                case TypeCode.DateTime:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.String:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return CreateDeleteCommandWithPK(engine, entityOrPKValue, tableName);
            }
            if(entityOrPKValue is Array) return CreateDeleteCommandWithPK(engine, entityOrPKValue, tableName);

            if(entityOrPKValue is System.Collections.IEnumerable)
            {
                List<object> items = new List<object>();
                foreach(var item in (System.Collections.IEnumerable)entityOrPKValue) items.Add(item);
                return CreateDeleteCommand(engine, items.ToArray(), tableName);
            }
            return CreateDeleteCommandWithEntity(engine, entityOrPKValue, tableName);
        }

        private ExecuteCommand CreateDeleteCommandWithPK(IDbEngine engine, object value, string tableName)
        {
            var parSettings = engine.Owner.Injector.ParameterSettings;
            var whereBuilder = new StringBuilder();
            var ps = new ExecuteParameterCollection(this.Count);

            foreach(var property in this._Properties.Values)
            {
                if(property.Column.IsPrimaryKey)
                {
                    var arrayValue = value as Array;
                    var isArrayValue = arrayValue != null;
                    int index = 0;
                    var fName = property.Name;
                ARRAY_LABEL:
                    var pName = fName;
                    if(isArrayValue) pName += index;

                    property.DefaultAppendField(whereBuilder, parSettings);
                    whereBuilder.Append('=');
                    whereBuilder
                          .Append(parSettings.PrefixWithText)
                          .Append(pName);
                    if(isArrayValue)
                    {
                        ps.Add(parSettings.PrefixWithCollection + pName, arrayValue.GetValue(index++));
                        if(index < arrayValue.Length)
                        {
                            whereBuilder.Append(" OR ");
                            goto ARRAY_LABEL;
                        }
                    }
                    else ps.Add(parSettings.PrefixWithCollection + pName, value);
                    break;
                }
            }
            return this.CreateDeleteCommand(whereBuilder, parSettings, ps, tableName);
        }

        private ExecuteCommand CreateDeleteCommand(StringBuilder whereBuilder, DefaultParameterSettings parSettings, ExecuteParameterCollection ps, string tableName)
        {
            if(whereBuilder.Length == 0) throw new NotSupportedException("{0} 的删除操作没有找到主键。".Fmt(this._Type.FullName));

            whereBuilder.Insert(0, " WHERE ");
            whereBuilder.Insert(0, parSettings.EscapeName(tableName ?? this._Name));
            whereBuilder.Insert(0, "DELETE ");
            return new ExecuteCommand(whereBuilder.ToString(), ps);
        }

        private ExecuteCommand CreateDeleteCommandWithEntity(IDbEngine engine, object entity, string tableName)
        {
            var parSettings = engine.Owner.Injector.ParameterSettings;
            var whereBuilder = new StringBuilder();
            var ps = new ExecuteParameterCollection(this.Count);

            int index = 0;
            foreach(var property in FindProperties(this, ref entity))
            {
                if(property.Column.IsPrimaryKey)
                {
                    if(whereBuilder.Length > 0) whereBuilder.Append(" AND ");

                    if(index++ > 0) whereBuilder.Append(',');
                    property.DefaultAppendField(whereBuilder, parSettings);
                    whereBuilder.Append('=');
                    property.AppendValue(property, new MapperValueEventArgs(engine, whereBuilder, parSettings, ps, entity, MapperRuntime.Update));
                }
            }
            return this.CreateDeleteCommand(whereBuilder, parSettings, ps, tableName);
        }

        private static readonly Dictionary<Type, IEntityMapper> Cacher = new Dictionary<Type, IEntityMapper>();

        /// <summary>
        /// 指定实例的数据类型，创建或从缓存读取一个实体的映射器。
        /// </summary>
        /// <param name="type">实例的数据类型。</param>
        /// <returns>返回一个实体映射器。</returns>
        public static IEntityMapper Create(Type type)
        {
            var attr = type.GetAttribute<EntityMapperAttribute>();
            IEntityMapper mapper;
            lock(type)
            {
                if(Cacher.TryGetValue(type, out mapper)) return mapper;

                var tableAttr = type.GetAttribute<TableAttribute>();
                if(attr != null)
                {
                    mapper = Activator.CreateInstance(attr.MapperType, type) as IEntityMapper;
                    if(tableAttr != null) tableAttr.Initialization(mapper);
                }
                else
                {
                    var ps = type.GetProperties();
                    var mapper2 = new EntityMapper(ps.Length) { _Type = type, _Name = type.Name };
                    mapper = mapper2;
                    if(tableAttr != null) tableAttr.Initialization(mapper);
                    foreach(var p in ps)
                    {
                        var propertyMapper = new PropertyMapper(mapper2, p);
                        mapper2._Properties.Add(propertyMapper.Name, propertyMapper);
                        EntityMapper.Initialization(p, propertyMapper);
                    }
                }
                Initialization(type, mapper);
                Cacher.Add(type, mapper);
                return mapper;
            }
        }

        internal static void Initialization(MemberInfo member, IMapper mapper)
        {
            foreach(var attrMapper in member.GetAttributes<IMapperAttribute>()) attrMapper.Initialization(mapper);

        }

        /// <summary>
        /// 实体映射器泛型单例模式。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        public static class Instance<TEntity>
        {
            /// <summary>
            /// 获取实体的映射器。
            /// </summary>
            public readonly static IEntityMapper Mapper = EntityMapper.Create(typeof(TEntity));
        }

    }
}
