using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Aoite.Data
{
    /// <summary>
    /// 表示一个数据源查询与交互的注入器的基类。
    /// </summary>
    public abstract class DbInjectorBase : IDbInjector
    {
        /// <summary>
        /// 获取用于创建提供程序对数据源类的实现的实例。
        /// </summary>
        public abstract DbProviderFactory Factory { get; }
        /// <summary>
        /// 获取一个值，表示当前数据操作的数据提供程序的类型。
        /// </summary>
        public abstract DbEngineProvider Provider { get; }
        /// <summary>
        /// 获取参数的配置。
        /// </summary>
        public virtual DefaultParameterSettings ParameterSettings { get { return DefaultParameterSettings.Default; } }

        /// <summary>
        /// 描述指定的 <see cref="System.Data.Common.DbParameter"/>。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="parameter">一个 <see cref="System.Data.Common.DbParameter"/> 的实例。</param>
        /// <returns>返回参数描述的 <see cref="System.String"/> 值。</returns>
        public abstract string DescribeParameter(IDbEngine engine, DbParameter parameter);

        /// <summary>
        /// 创建一个分页组件。
        /// </summary>
        /// <returns>返回一个分页组件。</returns>
        public abstract PaginationBase CreatePagination(IDbEngine engine);

        /// <summary>
        /// 创建并返回一个与给定数据源关联的命令对象。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="command">查询命令。</param>
        /// <param name="conn">指定数据源。</param>
        /// <returns>返回一个命令对象。</returns>
        public DbCommand CreateDbCommand(IDbEngine engine, ExecuteCommand command, DbConnection conn = null)
        {
            if(engine == null) throw new ArgumentNullException("engine");
            if(command == null) throw new ArgumentNullException("exeCommand");
            if(conn == null) conn = this.CreateConnection(engine);

            var dbCommand = this.CreateDbCommand(engine, command.CommandText);
            dbCommand.Connection = conn;

            if(command.CommandType.HasValue) dbCommand.CommandType = command.CommandType.Value;
            if(command.Timeout.HasValue) dbCommand.CommandTimeout = command.Timeout.Value;
            if(command.Count > 0) this.FillParameters(engine, dbCommand, command.Parameters);

            command.SetRuntimeObject(engine, dbCommand);

            return dbCommand;
        }

        /// <summary>
        /// 创建并返回一个与给定数据连接关联的命令对象。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="commandText">Transact-SQL 语句。第一个字符为“>”时，表示一个存储过程。</param>
        /// <returns>返回一个命令对象。</returns>
        protected virtual DbCommand CreateDbCommand(IDbEngine engine, string commandText)
        {
            if(string.IsNullOrEmpty(commandText)) throw new ArgumentNullException("commandText");

            DbCommand comm = this.Factory.CreateCommand();

            var firstChar = commandText[0];
            if(firstChar == '>')//存储过程
            {
                commandText = commandText.RemoveStarts();
                comm.CommandType = CommandType.StoredProcedure;
            }
            comm.CommandText = commandText;
            return comm;
        }

        /// <summary>
        /// 将参数集合填充到 <see cref="System.Data.Common.DbCommand"/>。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="command">命令对象。</param>
        /// <param name="parameters">参数集合。</param>
        protected virtual void FillParameters(IDbEngine engine, DbCommand command, ExecuteParameterCollection parameters)
        {
            foreach(var p in parameters) command.Parameters.Add(p.CreateParameter(command));
        }

        /// <summary>
        /// 创建并返回一个到数据源的连接。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <returns>返回一个到数据源的连接。</returns>
        public virtual DbConnection CreateConnection(IDbEngine engine)
        {
            var conn = this.Factory.CreateConnection();
            conn.ConnectionString = engine.Owner.ConnectionString;
            return conn;
        }

        /// <summary>
        /// 创建一个数据源查询与交互的执行器。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="command">执行查询的命令。</param>
        /// <returns>返回一个执行器。</returns>
        public virtual IDbExecutor CreateExecutor(IDbEngine engine, ExecuteCommand command)
        {
            return new DefaultDbExecutor(engine, command);
        }

        /// <summary>
        /// 指定类型映射器创建一个获取最后递增序列值的命令。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="mapper">类型映射器。</param>
        /// <returns>返回一个查询命令。</returns>
        public virtual ExecuteCommand CreateLastIdentityCommand(IDbEngine engine, TypeMapper mapper)
        {
            return new ExecuteCommand("SELECT @@IDENTITY");
        }

        /// <summary>
        /// 指定实体创建一个插入的命令。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="mapper">类型映射器。</param>
        /// <param name="entity">实体的实例对象。</param>
        /// <param name="tableName">实体的实际表名称，可以为 null 值。</param>
        /// <returns>返回一个查询命令。</returns>
        public virtual ExecuteCommand CreateInsertCommand(IDbEngine engine, TypeMapper mapper, object entity, string tableName = null)
        {
            if(engine == null) throw new ArgumentNullException("engine");
            if(mapper == null) throw new ArgumentNullException("mapper");
            if(entity == null) throw new ArgumentNullException("entity");
            if(mapper.Count == 0) throw new NotSupportedException("{0} 的插入操作没有找到任何属性。".Fmt(entity.GetType().FullName));

            var parameterSettings = this.ParameterSettings;
            var fieldsBuilder = new StringBuilder("INSERT INTO ")
                                .Append(parameterSettings.EscapeName(tableName ?? mapper.Name))
                                .Append('(');
            var valueBuilder = new StringBuilder(")VALUES(");
            var ps = new ExecuteParameterCollection(mapper.Count);
            int index = 0;
            foreach(var property in FindProperties(mapper, ref entity))
            {
                if(property.IsIgnore) continue;
                var value = property.GetValue(entity);
                if(property.IsKey && object.Equals(value, property.TypeDefaultValue)) continue;

                if(index++ > 0)
                {
                    fieldsBuilder.Append(',');
                    valueBuilder.Append(',');
                }
                DefaultAppendField(property, fieldsBuilder, parameterSettings);
                DefaultAppendValue(property, valueBuilder, parameterSettings, value, ps);
            }
            return new ExecuteCommand(fieldsBuilder.Append(valueBuilder.Append(')').ToString()).ToString(), ps);
        }

        /// <summary>
        /// 指定实体创建一个更新的命令。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="mapper">类型映射器。</param>
        /// <param name="entity">实体的实例对象。</param>
        /// <param name="tableName">实体的实际表名称，可以为 null 值。</param>
        /// <returns>返回一个查询命令。</returns>
        public virtual ExecuteCommand CreateUpdateCommand(IDbEngine engine, TypeMapper mapper, object entity, string tableName = null)
        {
            if(engine == null) throw new ArgumentNullException("engine");
            if(mapper == null) throw new ArgumentNullException("mapper");
            if(entity == null) throw new ArgumentNullException("entity");

            var parameterSettings = this.ParameterSettings;
            var setBuilder = new StringBuilder("UPDATE ")
                                .Append(parameterSettings.EscapeName(tableName ?? mapper.Name))
                                .Append(" SET ");
            var whereBuilder = new StringBuilder();
            var ps = new ExecuteParameterCollection(mapper.Count);

            int index = 0;
            foreach(var property in FindProperties(mapper, ref entity))
            {
                if(property.IsIgnore) continue;

                StringBuilder builder;
                if(property.IsKey)
                {
                    builder = whereBuilder;
                    if(builder.Length > 0) builder.Append(" AND ");
                }
                else
                {
                    builder = setBuilder;
                    if(index++ > 0) builder.Append(',');
                }

                DefaultAppendField(property, builder, parameterSettings);
                builder.Append('=');
                var value = property.GetValue(entity);
                DefaultAppendValue(property, builder, parameterSettings, value, ps);
            }

            if(whereBuilder.Length == 0) throw new NotSupportedException("{0} 的更新操作没有找到主键。".Fmt(entity.GetType().FullName));
            setBuilder.Append(" WHERE ").Append(whereBuilder.ToString());
            return new ExecuteCommand(setBuilder.ToString(), ps);
        }

        /// <summary>
        /// 指定实体创建一个删除的命令。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="mapper">类型映射器。</param>
        /// <param name="entityOrPKValue">实体的实例对象（引用类型）或一个主键的值（值类型）。</param>
        /// <param name="tableName">实体的实际表名称，可以为 null 值。</param>
        /// <returns>返回一个查询命令。</returns>
        public virtual ExecuteCommand CreateDeleteCommand(IDbEngine engine, TypeMapper mapper, object entityOrPKValue, string tableName = null)
        {
            if(engine == null) throw new ArgumentNullException("engine");
            if(mapper == null) throw new ArgumentNullException("mapper");
            if(entityOrPKValue == null) throw new ArgumentNullException("entityOrPKValue");

            var type = entityOrPKValue.GetType();
            if(type.IsPrimitive || type == Types.Decimal || type == Types.Guid) return CreateDeleteCommandWithPK(engine, mapper, entityOrPKValue, tableName);

            if(entityOrPKValue is Array) return CreateDeleteCommandWithPK(engine, mapper, entityOrPKValue, tableName);

            if(entityOrPKValue is System.Collections.IEnumerable)
            {
                List<object> items = new List<object>();
                foreach(var item in (System.Collections.IEnumerable)entityOrPKValue) items.Add(item);
                return CreateDeleteCommand(engine, mapper, items.ToArray(), tableName);
            }
            return CreateDeleteCommandWithEntity(engine, mapper, entityOrPKValue, tableName);
        }

        private ExecuteCommand CreateDeleteCommandWithPK(IDbEngine engine, TypeMapper mapper, object value, string tableName)
        {
            var parSettings = this.ParameterSettings;
            var whereBuilder = new StringBuilder();
            var ps = new ExecuteParameterCollection(mapper.Count);

            foreach(var property in mapper.Properties)
            {
                if(property.IsKey)
                {
                    var arrayValue = value as Array;
                    var isArrayValue = arrayValue != null;
                    int index = 0;
                    var fName = property.Name;
                ARRAY_LABEL:
                    var pName = fName;
                    if(isArrayValue) pName += index;

                    DefaultAppendField(property, whereBuilder, parSettings);
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
            return this.CreateDeleteCommand(mapper, whereBuilder, parSettings, ps, tableName);
        }

        private ExecuteCommand CreateDeleteCommand(TypeMapper mapper, StringBuilder whereBuilder, DefaultParameterSettings parSettings, ExecuteParameterCollection ps, string tableName)
        {
            if(whereBuilder.Length == 0) throw new NotSupportedException("{0} 的删除操作没有找到主键。".Fmt(mapper.Type.FullName));

            whereBuilder.Insert(0, " WHERE ");
            whereBuilder.Insert(0, parSettings.EscapeName(tableName ?? mapper.Name));
            whereBuilder.Insert(0, "DELETE ");
            return new ExecuteCommand(whereBuilder.ToString(), ps);
        }

        private ExecuteCommand CreateDeleteCommandWithEntity(IDbEngine engine, TypeMapper mapper, object entity, string tableName)
        {
            var parSettings = engine.Owner.Injector.ParameterSettings;
            var whereBuilder = new StringBuilder();
            var ps = new ExecuteParameterCollection(mapper.Count);

            int index = 0;
            foreach(var property in FindProperties(mapper, ref entity))
            {
                if(property.IsKey)
                {
                    if(whereBuilder.Length > 0) whereBuilder.Append(" AND ");

                    if(index++ > 0) whereBuilder.Append(',');
                    DefaultAppendField(property, whereBuilder, parSettings);
                    whereBuilder.Append('=');
                    var value = property.GetValue(entity);
                    DefaultAppendValue(property, whereBuilder, parSettings, value, ps);
                }
            }
            return this.CreateDeleteCommand(mapper, whereBuilder, parSettings, ps, tableName);
        }
        private IEnumerable<PropertyMapper> FindProperties(TypeMapper mapper, ref object entity)
        {
            IEnumerable<PropertyMapper> pms = mapper.Properties;
            var entityType = entity.GetType();
            if(entityType.IsAssignableFrom(mapper.Type)) return pms;

            var mapper2 = TypeMapper.Create(entityType);

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

        private void DefaultAppendField(PropertyMapper property, StringBuilder builder, DefaultParameterSettings parameterSettings)
        {
            builder.Append(parameterSettings.EscapeName(property.Name));
        }
        private void DefaultAppendValue(PropertyMapper property, StringBuilder builder, DefaultParameterSettings parameterSettings
            , object value
            , ExecuteParameterCollection ps)
        {
            var upperName = property.Name.ToUpper();
            builder.Append(parameterSettings.PrefixWithText)
                   .Append(upperName);
            ps.Add(parameterSettings.PrefixWithCollection + upperName, value);
        }

    }
}
