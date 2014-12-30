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
                commandText = commandText.RemoveStart();
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
        /// 指定实体创建一个插入的命令。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="mapper">实体映射器。</param>
        /// <param name="entity">实体的实例对象。</param>
        /// <param name="tableName">实体的实际表名称，可以为 null 值。</param>
        /// <returns>返回一个查询结果。</returns>
        public virtual ExecuteCommand CreateInsertCommand(IDbEngine engine, IEntityMapper mapper, object entity, string tableName = null)
        {
            if(mapper == null) throw new ArgumentNullException("mapper");
            if(entity == null) throw new ArgumentNullException("entity");
            if(entity == null) throw new ArgumentNullException("entity");
            if(mapper.Count == 0) throw new NotSupportedException("{0} 的插入操作没有找到任何属性。".Fmt(entity.GetType().FullName));

            var parSettings = this.ParameterSettings;
            var fieldsBuilder = new StringBuilder("INSERT INTO ")
                                .Append(parSettings.EscapeName(tableName ?? mapper.Name))
                                .Append('(');
            var valueBuilder = new StringBuilder(")VALUES(");
            var ps = new ExecuteParameterCollection(mapper.Count);
            int index = 0;
            foreach(var property in EntityMapper.FindProperties(mapper, ref entity))
            {
                this.OnPropertyInsertMapper(engine, property, ref index, fieldsBuilder, valueBuilder, ps, parSettings, entity);
            }
            return new ExecuteCommand(fieldsBuilder.Append(valueBuilder.Append(')').ToString()).ToString(), ps);
        }

        internal virtual void OnPropertyInsertMapper(IDbEngine engine
            , PropertyMapper property
            , ref int index
            , StringBuilder fieldsBuilder
            , StringBuilder valueBuilder
            , ExecuteParameterCollection ps
            , DefaultParameterSettings parSettings
            , object entity)
        {
            if(property.IsIgnore) return;

            if(index++ > 0)
            {
                fieldsBuilder.Append(',');
                valueBuilder.Append(',');
            }
            property.DefaultAppendField(fieldsBuilder, parSettings);
            property.AppendValue(property, new MapperValueEventArgs(engine, valueBuilder, parSettings, ps, entity, MapperRuntime.Insert));
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
        /// <returns></returns>
        public virtual IDbExecutor CreateExecutor(IDbEngine engine, ExecuteCommand command)
        {
            return new DefaultDbExecutor(engine, command);
        }
    }
}
