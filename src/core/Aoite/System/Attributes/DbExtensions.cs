using Aoite.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.UI.WebControls;

namespace System
{
    /// <summary>
    /// 提供数据源操作的实用工具方法。
    /// </summary>
    public static class DbExtensions
    {
        #region Ado

        /// <summary>
        /// 添加一个参数。
        /// </summary>
        /// <param name="dbCommand">数据源命令。</param>
        /// <param name="name">参数名称。</param>
        /// <param name="value">参数值。</param>
        public static void AddParameter(this DbCommand dbCommand, string name, object value)
        {
            if(dbCommand == null) throw new ArgumentNullException("dbCommand");
            var p = dbCommand.CreateParameter();
            p.ParameterName = name;
            p.Value = value;
            dbCommand.Parameters.Add(p);
        }

        /// <summary>
        /// 获取指定参数索引的值。
        /// </summary>
        /// <typeparam name="T">值的数据类型。</typeparam>
        /// <param name="dbCommand">数据源命令。</param>
        /// <param name="index">参数索引。</param>
        /// <returns>返回强类型的值。</returns>
        public static T GetValue<T>(this DbCommand dbCommand, int index)
        {
            if(dbCommand == null) throw new ArgumentNullException("dbCommand");
            return dbCommand.Parameters[index].Value.CastTo<T>();
        }
        /// <summary>
        /// 获取指定参数名称的值。
        /// </summary>
        /// <typeparam name="T">值的数据类型。</typeparam>
        /// <param name="dbCommand">数据源命令。</param>
        /// <param name="name">参数名称。</param>
        /// <returns>返回强类型的值。</returns>
        public static T GetValue<T>(this DbCommand dbCommand, string name)
        {
            if(dbCommand == null) throw new ArgumentNullException("dbCommand");
            return dbCommand.Parameters[name].Value.CastTo<T>();
        }
        /// <summary>
        /// 将指定的执行命令转换成完整字符串形式。
        /// </summary>
        /// <param name="command">执行命令。</param>
        /// <returns>返回一个完整执行命令的字符串形式。</returns>
        public static string ToFullString(this ExecuteCommand command)
        {
            if(command == null) throw new ArgumentNullException("command");
            StringBuilder builder = new StringBuilder(command.CommandText);
            if(command.Parameters != null)
            {
                builder.AppendLine();
                builder.AppendLine("*** Parameters ***");
                foreach(var item in command.Parameters)
                {
                    builder.AppendFormat("{0}\t=\t{1}\r\n", item.Name + (item.Value == null ? string.Empty : " <" + item.Value.GetType().Name + ">"), (item.Value == null || Convert.IsDBNull(item.Value)) ? "<NULL>" : item.Value);
                }
                builder.AppendLine("*** Parameters ***");
            }
            return builder.ToString();
        }

        /// <summary>
        /// 执行查询，转换并返回结果集转换后的实体集合。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="dbCommand">数据源命令。</param>
        /// <returns>返回一个实体集合</returns>
        public static List<TEntity> ExecuteEntities<TEntity>(this DbCommand dbCommand)
        {
            if(dbCommand == null) throw new ArgumentNullException("dbCommand");
            var mapper = EntityMapper.Instance<TEntity>.Mapper;
            using(var reader = dbCommand.ExecuteReader())
            {
                List<TEntity> value = new List<TEntity>();
                while(reader.Read())
                {
                    var model = (TEntity)Activator.CreateInstance(typeof(TEntity), true);
                    mapper.FillEntity(reader, model);
                    value.Add(model);
                }
                return value;
            }
        }

        /// <summary>
        /// 执行查询，转换并返回结果集转换后的实体集合。
        /// </summary>
        /// <param name="dbCommand">数据源命令。</param>
        /// <returns>返回一个实体集合</returns>
        public static List<dynamic> ExecuteEntities(this DbCommand dbCommand)
        {
            if(dbCommand == null) throw new ArgumentNullException("dbCommand");
            List<dynamic> value = new List<dynamic>();
            using(var reader = dbCommand.ExecuteReader())
            {
                while(reader.Read())
                {
                    value.Add(new DynamicEntityValue(reader));
                }
            }
            return value;
        }

        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列的强类型值。所有其他的列和行将被忽略。
        /// </summary>
        /// <typeparam name="TValue">值的数据类型。</typeparam>
        /// <param name="dbCommand">数据源命令。</param>
        /// <returns>结果集中第一行的第一列</returns>
        public static TValue ExecuteScalar<TValue>(this DbCommand dbCommand)
        {
            if(dbCommand == null) throw new ArgumentNullException("dbCommand");
            var value = dbCommand.ExecuteScalar();
            if(Convert.IsDBNull(value)) value = default(TValue);
            else if(!(value is TValue)) value = value.CastTo<TValue>();
            return (TValue)value;
        }

        /// <summary>
        /// 执行查询，并返回一张表。
        /// </summary>
        /// <param name="dbCommand">数据库命令。</param>
        /// <param name="dataAdpater">数据源适配器。</param>
        /// <returns>返回一张表。</returns>
        public static AoiteTable ExecuteTable(this DbCommand dbCommand, DbDataAdapter dataAdpater)
        {
            if(dbCommand == null) throw new ArgumentNullException("command");
            if(dataAdpater == null) throw new ArgumentNullException("dataAdpater");

            AoiteTable table = new AoiteTable();
            dataAdpater.Fill(table);
            table.TotalRowCount = table.Rows.Count;
            return table;

        }

        /// <summary>
        /// 执行查询，并返回一个数据集。
        /// </summary>
        /// <typeparam name="TDataSet">数据集的数据类型。</typeparam>
        /// <param name="dbCommand">数据库命令。</param>
        /// <param name="dataAdpater">数据源适配器。</param>
        /// <returns>返回一个数据集。</returns>
        public static TDataSet ExecuteDataSet<TDataSet>(this DbCommand dbCommand, DbDataAdapter dataAdpater)
            where TDataSet : DataSet, new()
        {
            if(dbCommand == null) throw new ArgumentNullException("command");
            if(dataAdpater == null) throw new ArgumentNullException("dataAdpater");

            TDataSet dataSet = new TDataSet();
            dataAdpater.Fill(dataSet);
            return dataSet;
        }

        /// <summary>
        /// 当数据源的状态为关闭时，打开连接。
        /// </summary>
        /// <param name="connection">数据库的连接。</param>
        public static void OpenWhenClosed(this DbConnection connection)
        {
            if(connection != null && connection.State == ConnectionState.Closed) connection.Open();
        }

        /// <summary>
        /// 当数据源的状态为打开时，尝试关闭连接。该方法可以避免异常的抛出。
        /// </summary>
        /// <param name="connection">数据库的连接。</param>
        public static void TryClose(this DbConnection connection)
        {
            if(connection != null && connection.State != ConnectionState.Closed)
            {
                try
                {
                    connection.Close();
                }
                catch(Exception) { }
            }
        }

        #endregion

        #region Execute

        /// <summary>
        /// 生成执行数据源查询与交互的执行器。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="command">执行查询的命令。</param>
        /// <returns>返回执行数据源查询与交互的执行器。</returns>
        public static IDbExecutor Execute(this IDbEngine engine, ExecuteCommand command)
        {
            if(engine == null) throw new ArgumentNullException("engine");
            if(command == null) throw new ArgumentNullException("command");
            return engine.Owner.Injector.CreateExecutor(engine, command);
        }

        /// <summary>
        /// 生成执行数据源查询与交互的执行器。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="commandText">Transact-SQL 语句。第一个字符为“>”时，表示一个存储过程。</param>
        /// <returns>返回执行数据源查询与交互的执行器。</returns>
        public static IDbExecutor Execute(this IDbEngine engine, string commandText)
        {
            return engine.Execute(new ExecuteCommand(commandText));
        }

        /// <summary>
        /// 生成执行数据源查询与交互的执行器。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="commandText">Transact-SQL 语句。第一个字符为“>”时，表示一个存储过程。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns>返回执行数据源查询与交互的执行器。</returns>
        public static IDbExecutor Execute(this IDbEngine engine, string commandText, ExecuteParameterCollection parameters)
        {
            return engine.Execute(new ExecuteCommand(commandText, parameters));
        }

        /// <summary>
        /// 生成执行数据源查询与交互的执行器。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="commandText">Transact-SQL 语句。第一个字符为“>”时，表示一个存储过程。</param>
        /// <param name="parameters">匹配 Name/Value 的参数集合或 数组。</param>
        /// <returns>返回执行数据源查询与交互的执行器。</returns>
        public static IDbExecutor Execute(this IDbEngine engine, string commandText, params object[] parameters)
        {
            return engine.Execute(new ExecuteCommand(commandText, parameters));
        }

        /// <summary>
        /// 生成执行数据源查询与交互的执行器。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="commandText">Transact-SQL 语句。第一个字符为“>”时，表示一个存储过程。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns>返回执行数据源查询与交互的执行器。</returns>
        public static IDbExecutor Execute(this IDbEngine engine, string commandText, params ExecuteParameter[] parameters)
        {
            return engine.Execute(new ExecuteCommand(commandText, parameters));
        }

        /// <summary>
        /// 生成执行数据源查询与交互的执行器。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="commandText">Transact-SQL 语句。第一个字符为“>”时，表示一个存储过程。</param>
        /// <param name="objectInstance">任意类型的实例。</param>
        /// <returns>返回执行数据源查询与交互的执行器。</returns>
        public static IDbExecutor Execute(this IDbEngine engine, string commandText, object objectInstance)
        {
            return engine.Execute(new ExecuteCommand(commandText, new ExecuteParameterCollection(objectInstance)));
        }

        #endregion

        #region Adv

        private static DbResult<int> ExecuteAED<TEntity>(this IDbEngine engine, Func<IEntityMapper, Result<ExecuteCommand>> callback)
        {
            var commandResult = callback(EntityMapper.Instance<TEntity>.Mapper);
            if(commandResult.IsSucceed) return engine.Execute(commandResult.Value).ToNonQuery();

            return new DbResult<int>().ToFailded(commandResult.Exception);
        }

        /// <summary>
        /// 创建指定视图类型的字段列表。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <returns>返回一个字段的列表。</returns>
        public static string CreateFields<TEntity, TView>(this IDbEngine engine)
        {
            string fields = "*";
            var m1 = EntityMapper.Instance<TEntity>.Mapper;
            if(m1.Type != typeof(TView))
            {
                var parSetting = engine.Owner.Injector.ParameterSettings;
                var m2 = EntityMapper.Instance<TView>.Mapper;
                List<string> fieldList = new List<string>();
                foreach(var mp2 in m2.Properties)
                {
                    if(mp2.IsIgnore) continue;
                    //if(!m1.ContainsProperty(mp2.Name)) continue;
                    fieldList.Add(parSetting.EscapeName(mp2.Name));
                }
                fields = fieldList.ToArray().Join();
            }
            return fields;
        }

        /// <summary>
        /// 创建一个条件查询语句。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="ps">参数集合实例。</param>
        /// <param name="binary">二元运算符。</param>
        /// <returns>返回一个条件查询语句。</returns>
        public static string CreateWhere(this IDbEngine engine, ExecuteParameterCollection ps, string binary = "AND")
        {
            if(ps == null || ps.Count == 0) return null;
            var parSetting = engine.Owner.Injector.ParameterSettings;
            var builder = new StringBuilder();
            int index = 0;
            foreach(var p in ps)
            {
                if(index++ > 0) builder.Append(' ').Append(binary).Append(' ');
                builder
                    .Append(p.Name)
                    .Append('=').Append(parSetting.PrefixWithText)
                    .Append(p.Name);
            }
            return builder.ToString();
        }

        /// <summary>
        /// 执行 @@IDENTITY 并获取值。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <returns>返回一个结果。</returns>
        public static Result<long> GetLastIdentity(this IDbEngine engine)
        {
            //TODO: 后期支持 ORACLE 时再进行扩展。
            return engine.Execute("SELECT @@IDENTITY").ToScalar<long>();
        }

        #region Add

        /// <summary>
        /// 执行一个插入的命令，可以是匿名对象的部分成员（<paramref name="entity"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="entity">实体的实例对象，可以是匿名对象的部分成员（<paramref name="entity"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。</param>
        /// <param name="tableName">实体的实际表名称，可以为 null 值。</param>
        /// <returns>返回一个查询结果。</returns>
        public static DbResult<int> AddAnonymous<TEntity>(this IDbEngine engine, object entity, string tableName = null)
        {
            if(engine == null) throw new ArgumentNullException("engine");
            if(entity == null) throw new ArgumentNullException("entity");
            return ExecuteAED<TEntity>(engine, mapper => mapper.CreateInsertCommand(engine, entity, tableName));
        }

        /// <summary>
        /// 执行一个插入的命令。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="entity">实体的实例对象。</param>
        /// <param name="tableName">实体的实际表名称，可以为 null 值。</param>
        /// <returns>返回一个查询结果。</returns>
        public static DbResult<int> Add<TEntity>(this IDbEngine engine, TEntity entity, string tableName = null)
        {
            return AddAnonymous<TEntity>(engine, entity, tableName);
        }

        #endregion

        #region Modify

        /// <summary>
        /// 执行一个更新的命令，可以是匿名对象的部分成员（<paramref name="entity"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="entity">实体的实例对象，可以是匿名对象的部分成员（<paramref name="entity"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。</param>
        /// <param name="tableName">实体的实际表名称，可以为 null 值。</param>
        /// <returns>返回一个查询结果。</returns>
        [Obsolete("此方法已过时，请调用 ModifyAnonymous<TEntity> 方法。")]
        [System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)]
        public static DbResult<int> EditAnonymous<TEntity>(this IDbEngine engine, object entity, string tableName = null)
        {
            if(engine == null) throw new ArgumentNullException("engine");
            if(entity == null) throw new ArgumentNullException("entity");
            return ExecuteAED<TEntity>(engine, mapper => mapper.CreateUpdateCommand(engine, entity, tableName));
        }

        /// <summary>
        /// 执行一个更新的命令。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="entity">实体的实例对象。</param>
        /// <param name="tableName">实体的实际表名称，可以为 null 值。</param>
        /// <returns>返回一个查询结果。</returns>
        [Obsolete("此方法已过时，请调用 Modify<TEntity> 方法。")]
        public static DbResult<int> Edit<TEntity>(this IDbEngine engine, TEntity entity, string tableName = null)
        {
            return EditAnonymous<TEntity>(engine, entity, tableName);
        }

        /// <summary>
        /// 执行一个更新的命令，可以是匿名对象的部分成员（<paramref name="entity"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="entity">实体的实例对象，可以是匿名对象的部分成员（<paramref name="entity"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。</param>
        /// <param name="tableName">实体的实际表名称，可以为 null 值。</param>
        /// <returns>返回一个查询结果。</returns>
        public static DbResult<int> ModifyAnonymous<TEntity>(this IDbEngine engine, object entity, string tableName = null)
        {
            if(engine == null) throw new ArgumentNullException("engine");
            if(entity == null) throw new ArgumentNullException("entity");
            return ExecuteAED<TEntity>(engine, mapper => mapper.CreateUpdateCommand(engine, entity, tableName));
        }

        /// <summary>
        /// 执行一个更新的命令。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="entity">实体的实例对象。</param>
        /// <param name="tableName">实体的实际表名称，可以为 null 值。</param>
        /// <returns>返回一个查询结果。</returns>
        public static DbResult<int> Modify<TEntity>(this IDbEngine engine, TEntity entity, string tableName = null)
        {
            return ModifyAnonymous<TEntity>(engine, entity, tableName);
        }

        #endregion

        #region ModifyWhere

        /// <summary>
        /// 提供匹配条件，执行一个更新的命令，可以是匿名对象的部分成员（<paramref name="entity"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="entity">实体的实例对象，可以是匿名对象的部分成员（<paramref name="entity"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。</param>
        /// <param name="objectInstance">匿名参数集合实例。</param>
        public static DbResult<int> ModifyWhere<TEntity>(this IDbEngine engine, object entity, object objectInstance)
        {
            return ModifyWhere<TEntity>(engine, entity, new ExecuteParameterCollection(objectInstance));
        }

        /// <summary>
        /// 提供匹配条件，执行一个更新的命令，可以是匿名对象的部分成员（<paramref name="entity"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>  
        /// <param name="entity">实体的实例对象，可以是匿名对象的部分成员（<paramref name="entity"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。</param>
        /// <param name="ps">参数集合实例。</param>
        public static DbResult<int> ModifyWhere<TEntity>(this IDbEngine engine, object entity, ExecuteParameterCollection ps)
        {
            return ModifyWhere<TEntity>(engine, entity, CreateWhere(engine, ps), ps);
        }
        /// <summary>
        /// 提供匹配条件，执行一个更新的命令，可以是匿名对象的部分成员（<paramref name="entity"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="entity">实体的实例对象，可以是匿名对象的部分成员（<paramref name="entity"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。</param>
        /// <param name="where">条件表达式。</param>
        /// <param name="ps">参数集合实例。</param>
        public static DbResult<int> ModifyWhere<TEntity>(this IDbEngine engine, object entity, string where, ExecuteParameterCollection ps = null)
        {
            if(engine == null) throw new ArgumentNullException("engine");
            if(entity == null) throw new ArgumentNullException("entity");
            var mapper1 = EntityMapper.Instance<TEntity>.Mapper;
            var mapper2 = EntityMapper.Create(entity.GetType());
            var parSettings = engine.Owner.Injector.ParameterSettings;
            var setBuilder = new StringBuilder("UPDATE ")
                                .Append(parSettings.EscapeName(mapper1.Name))
                                .Append(" SET ");
            int index = 0;
            if(ps == null) ps = new ExecuteParameterCollection();
            foreach(var property2 in mapper2.Properties)
            {
                if(property2.IsIgnore || property2.Column.IsPrimaryKey) continue;
                if(!mapper1.ContainsProperty(property2.Name)) throw new NotSupportedException("表 {0} 没有属性 {1}。".Fmt(mapper1.Name, property2.Name));
                if(index > 0) setBuilder.Append(',');

                setBuilder.Append(parSettings.EscapeName(property2.Name))
                          .Append('=')
                          .Append(parSettings.PrefixWithText)
                          .Append("_value_")
                          .Append(index);
                ps.Add(parSettings.PrefixWithCollection + "_value_" + index, property2.GetValue(entity));
                index++;
            }
            if(where != null)
            {
                setBuilder.Append(" WHERE ").Append(where);
            }
            return engine.Execute(setBuilder.ToString(), ps).ToNonQuery();

        }

        #endregion

        #region Remove

        /// <summary>
        /// 执行一个删除的命令，在删除命令中 <paramref name="entityOrPKValues"/> 可以是主键的值（表只有一个主键），也可以是匿名对象的部分成员（<paramref name="entityOrPKValues"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="entityOrPKValues">实体的实例对象，在删除命令中 <paramref name="entityOrPKValues"/> 可以是主键的值（表只有一个主键，值允许是一个数组，表示删除多条记录），也可以是匿名对象的部分成员（<paramref name="entityOrPKValues"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。</param>
        /// <param name="tableName">实体的实际表名称，可以为 null 值。</param>
        /// <returns>返回一个查询结果。</returns>
        public static DbResult<int> RemoveAnonymous<TEntity>(this IDbEngine engine, object entityOrPKValues, string tableName = null)
        {
            if(engine == null) throw new ArgumentNullException("engine");
            if(entityOrPKValues == null) throw new ArgumentNullException("entityOrPKValues");
            return ExecuteAED<TEntity>(engine, mapper => mapper.CreateDeleteCommand(engine, entityOrPKValues, tableName));
        }

        /// <summary>
        /// 执行一个删除的命令。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="entity">实体的实例对象。</param>
        /// <param name="tableName">实体的实际表名称，可以为 null 值。</param>
        /// <returns>返回一个查询结果。</returns>
        public static DbResult<int> Remove<TEntity>(this IDbEngine engine, TEntity entity, string tableName = null)
        {
            return RemoveAnonymous<TEntity>(engine, entity, tableName);
        }


        #endregion

        /// <summary>
        /// 数据源表主键的默认字段名。
        /// </summary>
        public const string DefaultKeyName = "Id";

        #region Select

        /// <summary>
        /// 添加 SELECT 的字段。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="fields">字段的集合。</param>
        /// <returns>返回 <see cref="Aoite.Data.ISelect"/> 的实例。</returns>
        public static ISelect Select(this IDbEngine engine, params string[] fields)
        {
            if(engine == null) throw new ArgumentNullException("engine");
            return new SelectCommandBuilder(engine).Select(fields);
        }

        /// <summary>
        /// 添加 SELECT 的字段和 FORM 语句。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="fields">字段的集合。</param>
        /// <returns>返回 <see cref="Aoite.Data.ISelect"/> 的实例。</returns>
        public static ISelect Select<TEntity>(this IDbEngine engine, params string[] fields)
        {
            if(engine == null) throw new ArgumentNullException("engine");
            return new SelectCommandBuilder(engine).Select(fields).From(EntityMapper.Instance<TEntity>.Mapper.Name);
        }

        #endregion

        #region IBuilder

        /// <summary>
        /// 执行查询，并返回一个数据表结果。
        /// </summary>
        /// <param name="builder">生成器。</param>
        /// <returns>返回一个执行结果。</returns>
        public static TableResult ToTable(this IBuilder builder)
        {
            return builder.Execute().ToTable();
        }
        /// <summary>
        /// 执行查询，并返回一个数据集结果。
        /// </summary>
        /// <param name="builder">生成器。</param>
        /// <returns>返回一个执行结果。</returns>
        public static DataSetResult<DataSet> ToDataSet(this IBuilder builder)
        {
            return builder.Execute().ToDataSet();
        }
        /// <summary>
        /// 执行查询，并返回一个单值结果。
        /// </summary>
        /// <param name="builder">生成器。</param>
        /// <returns>返回一个执行结果。</returns>
        public static DbResult<object> ToScalar(this IBuilder builder)
        {
            return builder.Execute().ToScalar();
        }
        /// <summary>
        /// 执行查询，并返回一个单值结果。
        /// </summary>
        /// <typeparam name="TValue">值的数据类型。</typeparam>
        /// <param name="builder">生成器。</param>
        /// <returns>返回一个执行结果。</returns>
        public static DbResult<TValue> ToScalar<TValue>(this IBuilder builder)
        {
            return builder.Execute().ToScalar<TValue>();
        }

        /// <summary>
        /// 执行查询，并返回自定义只读实体的集合。
        /// </summary>
        /// <typeparam name="TEntity">实体的类型。</typeparam>
        /// <param name="builder">生成器。</param>
        /// <returns>返回一个执行结果。</returns>
        public static DbResult<List<TEntity>> ToEntities<TEntity>(this IBuilder builder)
        {
            return builder.Execute().ToEntities<TEntity>();
        }

        /// <summary>
        /// 执行分页查询，并返回自定义只读实体的集合。
        /// </summary>
        /// <typeparam name="TEntity">实体的类型。</typeparam>
        /// <param name="builder">生成器。</param>
        /// <param name="page">分页的实现。</param>
        /// <returns>返回一个执行结果。</returns>
        public static DbResult<GridData<TEntity>> ToEntities<TEntity>(this IBuilder builder, IPagination page)
        {
            return builder.Execute().ToEntities<TEntity>(page);
        }

        /// <summary>
        /// 执行分页查询，并返回自定义只读实体的集合。
        /// </summary>
        /// <typeparam name="TEntity">实体的类型。</typeparam>
        /// <param name="builder">生成器。</param>
        /// <param name="pageNumber">以 1 起始的页码。</param>
        /// <param name="pageSize">分页大小。</param>
        /// <returns>返回一个执行结果。</returns>
        public static DbResult<GridData<TEntity>> ToEntities<TEntity>(this IBuilder builder, int pageNumber, int pageSize = 10)
        {
            return builder.Execute().ToEntities<TEntity>(pageNumber, pageSize);
        }

        /// <summary>
        /// 执行查询，并返回自定义只读实体。
        /// </summary>
        /// <typeparam name="TEntity">实体的类型。</typeparam>
        /// <param name="builder">生成器。</param>
        /// <returns>返回一个执行结果。</returns>
        public static DbResult<TEntity> ToEntity<TEntity>(this IBuilder builder)
        {
            return builder.Execute().ToEntity<TEntity>();
        }


        /// <summary>
        /// 执行查询，并返回自定义只读实体的集合。
        /// </summary>
        /// <param name="builder">生成器。</param>
        /// <returns>返回一个执行结果。</returns>
        public static DbResult<List<dynamic>> ToEntities(this IBuilder builder)
        {
            return builder.Execute().ToEntities();
        }

        /// <summary>
        /// 执行分页查询，并返回自定义只读实体的集合。
        /// </summary>
        /// <param name="builder">生成器。</param>
        /// <param name="page">分页的实现。</param>
        /// <returns>返回一个执行结果。</returns>
        public static DbResult<GridData<dynamic>> ToEntities(this IBuilder builder, IPagination page)
        {
            return builder.Execute().ToEntities(page);
        }

        /// <summary>
        /// 执行分页查询，并返回自定义只读实体的集合。
        /// </summary>
        /// <param name="builder">生成器。</param>
        /// <param name="pageNumber">以 1 起始的页码。</param>
        /// <param name="pageSize">分页大小。</param>
        /// <returns>返回一个执行结果。</returns>
        public static DbResult<GridData<dynamic>> ToEntities(this IBuilder builder, int pageNumber, int pageSize = 10)
        {
            return builder.Execute().ToEntities(pageNumber, pageSize);
        }

        /// <summary>
        /// 执行查询，并返回自定义只读实体。
        /// </summary>
        /// <param name="builder">生成器。</param>
        /// <returns>返回一个执行结果。</returns>
        public static DbResult<dynamic> ToEntity(this IBuilder builder)
        {
            return builder.Execute().ToEntity();
        }

        #endregion

        #region FineOne

        /// <summary>
        /// 获取指定 Id 值的数据源对象。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="id">字段“Id”的值。</param>
        public static DbResult<TEntity> FindOne<TEntity>(this IDbEngine engine, object id)
        {
            return FindOne<TEntity, TEntity>(engine, id);
        }
        /// <summary>
        /// 获取指定 Id 键值的数据源对象。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="keyName">主键的列名。</param>
        /// <param name="keyValue">主键的列值。</param>
        public static DbResult<TEntity> FindOne<TEntity>(this IDbEngine engine, string keyName, object keyValue)
        {
            return FindOne<TEntity, TEntity>(engine, keyName, keyValue);
        }

        /// <summary>
        /// 获取指定 Id 值的数据源对象。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="id">字段“Id”的值。</param>
        public static DbResult<TView> FindOne<TEntity, TView>(this IDbEngine engine, object id)
        {
            return FindOne<TEntity, TView>(engine, null, id);
        }

        /// <summary>
        /// 获取指定 Id 键值的数据源对象。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="keyName">主键的列名。</param>
        /// <param name="keyValue">主键的列值。</param>
        public static DbResult<TView> FindOne<TEntity, TView>(this IDbEngine engine, string keyName, object keyValue)
        {
            if(engine == null) throw new ArgumentNullException("engine");
            if(keyValue == null) throw new ArgumentNullException("keyValue");

            var parSetting = engine.Owner.Injector.ParameterSettings;
            var m1 = EntityMapper.Instance<TEntity>.Mapper;
            if(string.IsNullOrEmpty(keyName))
            {
                var prop = m1.Properties.FirstOrDefault(p => p.Column.IsPrimaryKey);
                if(prop == null) keyName = DefaultKeyName;
                else keyName = prop.Name;
            }
            var fields = CreateFields<TEntity, TView>(engine);

            return engine.Execute("SELECT " + fields + " FROM " + parSetting.EscapeName(m1.Name)
                + " WHERE " + keyName + "=" + parSetting.PrefixWithText + "pk"
                , parSetting.PrefixWithCollection + "pk", keyValue).ToEntity<TView>();
        }

        #endregion

        #region FindOneWhere

        /// <summary>
        /// 提供匹配条件，获取一个对象。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="objectInstance">匿名参数集合实例。</param>
        public static DbResult<TEntity> FindOneWhere<TEntity>(this IDbEngine engine, object objectInstance)
        {
            return FindOneWhere<TEntity, TEntity>(engine, objectInstance);
        }
        /// <summary>
        /// 提供匹配条件，获取一个对象。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="ps">参数集合实例。</param>
        public static DbResult<TEntity> FindOneWhere<TEntity>(this IDbEngine engine, ExecuteParameterCollection ps = null)
        {
            return FindOneWhere<TEntity, TEntity>(engine, ps);
        }
        /// <summary>
        /// 提供匹配条件，获取一个对象。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="where">条件表达式。</param>
        /// <param name="ps">参数集合实例。</param>
        public static DbResult<TEntity> FindOneWhere<TEntity>(this IDbEngine engine, string where, ExecuteParameterCollection ps = null)
        {
            return FindOneWhere<TEntity, TEntity>(engine, where, ps);
        }

        /// <summary>
        /// 提供匹配条件，获取一个对象。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="objectInstance">匿名参数集合实例。</param>
        public static DbResult<TView> FindOneWhere<TEntity, TView>(this IDbEngine engine, object objectInstance)
        {
            return FindOneWhere<TEntity, TView>(engine, new ExecuteParameterCollection(objectInstance));
        }
        /// <summary>
        /// 提供匹配条件，获取一个对象。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="ps">参数集合实例。</param>
        public static DbResult<TView> FindOneWhere<TEntity, TView>(this IDbEngine engine, ExecuteParameterCollection ps = null)
        {
            return FindOneWhere<TEntity, TView>(engine, CreateWhere(engine, ps), ps);
        }
        /// <summary>
        /// 提供匹配条件，获取一个对象。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="where">条件表达式。</param>
        /// <param name="ps">参数集合实例。</param>
        public static DbResult<TView> FindOneWhere<TEntity, TView>(this IDbEngine engine, string where, ExecuteParameterCollection ps = null)
        {
            if(engine == null) throw new ArgumentNullException("engine");

            var parSetting = engine.Owner.Injector.ParameterSettings;
            var m1 = EntityMapper.Instance<TEntity>.Mapper;
            var fields = CreateFields<TEntity, TView>(engine);
            var commandText = "SELECT TOP 1 " + fields + " FROM " + parSetting.EscapeName(EntityMapper.Instance<TEntity>.Mapper.Name);
            if(!string.IsNullOrEmpty(where)) commandText += " WHERE " + where;
            return engine.Execute(commandText, ps).ToEntity<TView>();
        }

        #endregion

        #region FindAllWhere

        /// <summary>
        /// 提供匹配条件，获取对象的列表。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="objectInstance">匿名参数集合实例。</param>
        public static DbResult<List<TEntity>> FindAllWhere<TEntity>(this IDbEngine engine, object objectInstance)
        {
            return FindAllWhere<TEntity, TEntity>(engine, objectInstance);
        }
        /// <summary>
        /// 提供匹配条件，获取对象的列表。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="ps">参数集合实例。</param>
        public static DbResult<List<TEntity>> FindAllWhere<TEntity>(this IDbEngine engine, ExecuteParameterCollection ps = null)
        {
            return FindAllWhere<TEntity, TEntity>(engine, ps);
        }
        /// <summary>
        /// 提供匹配条件，获取对象的列表。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="where">条件表达式。</param>
        /// <param name="ps">参数集合实例。</param>
        public static DbResult<List<TEntity>> FindAllWhere<TEntity>(this IDbEngine engine, string where, ExecuteParameterCollection ps = null)
        {
            return FindAllWhere<TEntity, TEntity>(engine, where, ps);
        }

        /// <summary>
        /// 提供匹配条件，获取对象的列表。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="objectInstance">匿名参数集合实例。</param>
        public static DbResult<List<TView>> FindAllWhere<TEntity, TView>(this IDbEngine engine, object objectInstance)
        {
            return FindAllWhere<TEntity, TView>(engine, new ExecuteParameterCollection(objectInstance));
        }
        /// <summary>
        /// 提供匹配条件，获取对象的列表。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="ps">参数集合实例。</param>
        public static DbResult<List<TView>> FindAllWhere<TEntity, TView>(this IDbEngine engine, ExecuteParameterCollection ps = null)
        {
            return FindAllWhere<TEntity, TView>(engine, CreateWhere(engine, ps), ps);
        }
        /// <summary>
        /// 提供匹配条件，获取对象的列表。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="where">条件表达式。</param>
        /// <param name="ps">参数集合实例。</param>
        public static DbResult<List<TView>> FindAllWhere<TEntity, TView>(this IDbEngine engine, string where, ExecuteParameterCollection ps = null)
        {
            if(engine == null) throw new ArgumentNullException("engine");

            var parSetting = engine.Owner.Injector.ParameterSettings;
            var m1 = EntityMapper.Instance<TEntity>.Mapper;
            var fields = CreateFields<TEntity, TView>(engine);
            var commandText = "SELECT " + fields + " FROM " + parSetting.EscapeName(EntityMapper.Instance<TEntity>.Mapper.Name);
            if(!string.IsNullOrEmpty(where)) commandText += " WHERE " + where;
            return engine.Execute(commandText, ps).ToEntities<TView>();
        }

        #endregion

        #region FindAllPage

        /// <summary>
        /// 提供匹配条件，获取对象的列表。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="page">一个分页的实现。</param>
        /// <param name="objectInstance">匿名参数集合实例。</param>
        public static DbResult<GridData<TEntity>> FindAllPage<TEntity>(this IDbEngine engine, IPagination page, object objectInstance)
        {
            return FindAllPage<TEntity, TEntity>(engine, page, objectInstance);
        }
        /// <summary>
        /// 提供匹配条件，获取对象的列表。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="page">一个分页的实现。</param>
        /// <param name="ps">参数集合实例。</param>
        public static DbResult<GridData<TEntity>> FindAllPage<TEntity>(this IDbEngine engine, IPagination page, ExecuteParameterCollection ps = null)
        {
            return FindAllPage<TEntity, TEntity>(engine, page, ps);
        }
        /// <summary>
        /// 提供匹配条件，获取对象的列表。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="where">条件表达式。</param>
        /// <param name="page">一个分页的实现。</param>
        /// <param name="ps">参数集合实例。</param>
        public static DbResult<GridData<TEntity>> FindAllPage<TEntity>(this IDbEngine engine, IPagination page, string where, ExecuteParameterCollection ps = null)
        {
            return FindAllPage<TEntity, TEntity>(engine, page, where, ps);
        }

        /// <summary>
        /// 提供匹配条件，获取对象的列表。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="page">一个分页的实现。</param>
        /// <param name="objectInstance">匿名参数集合实例。</param>
        public static DbResult<GridData<TView>> FindAllPage<TEntity, TView>(this IDbEngine engine, IPagination page, object objectInstance)
        {
            return FindAllPage<TEntity, TView>(engine, page, new ExecuteParameterCollection(objectInstance));
        }
        /// <summary>
        /// 提供匹配条件，获取对象的列表。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="page">一个分页的实现。</param>
        /// <param name="ps">参数集合实例。</param>
        public static DbResult<GridData<TView>> FindAllPage<TEntity, TView>(this IDbEngine engine, IPagination page, ExecuteParameterCollection ps = null)
        {
            return FindAllPage<TEntity, TView>(engine, page, CreateWhere(engine, ps), ps);
        }
        /// <summary>
        /// 提供匹配条件，获取对象的列表。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="page">一个分页的实现。</param>
        /// <param name="where">条件表达式。</param>
        /// <param name="ps">参数集合实例。</param>
        public static DbResult<GridData<TView>> FindAllPage<TEntity, TView>(this IDbEngine engine, IPagination page, string where, ExecuteParameterCollection ps = null)
        {
            if(page == null) throw new ArgumentNullException("page");
            if(engine == null) throw new ArgumentNullException("engine");

            var parSetting = engine.Owner.Injector.ParameterSettings;
            var m1 = EntityMapper.Instance<TEntity>.Mapper;
            var fields = CreateFields<TEntity, TView>(engine);
            var commandText = "SELECT " + fields + " FROM " + parSetting.EscapeName(EntityMapper.Instance<TEntity>.Mapper.Name);
            if(!string.IsNullOrEmpty(where)) commandText += " WHERE " + where;
            return engine.Execute(commandText, ps).ToEntities<TView>(page);
        }

        #endregion

        #region Exists

        /// <summary>
        /// 判断指定的主键的列名的值是否已存在。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="keyValue">主键的列值。</param>
        public static Result<bool> Exists<TEntity>(this IDbEngine engine, object keyValue)
        {
            var prop = EntityMapper.Instance<TEntity>.Mapper.Properties.FirstOrDefault(p => p.Column.IsPrimaryKey);

            return Exists<TEntity>(engine, null, keyValue);
        }

        /// <summary>
        /// 判断指定的主键的列名的值是否已存在。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="keyName">主键的列名。</param>
        /// <param name="keyValue">主键的列值。</param>
        public static Result<bool> Exists<TEntity>(this IDbEngine engine, string keyName, object keyValue)
        {
            if(string.IsNullOrEmpty(keyName))
            {
                var prop = EntityMapper.Instance<TEntity>.Mapper.Properties.FirstOrDefault(p => p.Column.IsPrimaryKey);
                if(prop == null) keyName = DefaultKeyName;
                else keyName = prop.Name;
            }
            return ExistsWhere<TEntity>(engine, new ExecuteParameterCollection(keyName, keyValue));
        }

        #endregion

        #region ExistsWhere

        /// <summary>
        /// 判断指定的条件的数据是否已存在。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="objectInstance">匿名参数集合实例。</param>
        public static Result<bool> ExistsWhere<TEntity>(this IDbEngine engine, object objectInstance)
        {
            if(objectInstance == null) throw new ArgumentNullException("objectInstance");
            return ExistsWhere<TEntity>(engine, new ExecuteParameterCollection(objectInstance));
        }

        /// <summary>
        /// 判断指定的条件的数据是否已存在。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="ps">参数集合实例。</param>
        public static Result<bool> ExistsWhere<TEntity>(this IDbEngine engine, ExecuteParameterCollection ps)
        {
            if(ps == null || ps.Count == 0) throw new ArgumentNullException("objectInstance");
            return ExistsWhere<TEntity>(engine, CreateWhere(engine, ps), ps);
        }

        /// <summary>
        /// 判断指定的条件的数据是否已存在。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="where">条件表达式。</param>
        /// <param name="ps">参数集合实例。</param>
        public static Result<bool> ExistsWhere<TEntity>(this IDbEngine engine, string where, ExecuteParameterCollection ps = null)
        {
            if(engine == null) throw new ArgumentNullException("engine");

            var commandText = "SELECT 1 FROM "
                + engine.Owner.Injector.ParameterSettings.EscapeName(EntityMapper.Instance<TEntity>.Mapper.Name);
            if(!string.IsNullOrEmpty(where)) commandText += " WHERE " + where;
            var r = engine.Execute(commandText, ps).ToScalar();
            if(r.IsFailed) return r.Exception;
            return r.Value != null;
        }

        #endregion

        #region RowCount

        /// <summary>
        /// 获取数据表的总行数。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="objectInstance">匿名参数集合实例。</param>
        /// <returns>返回一个查询结果。</returns>
        public static DbResult<long> RowCount<TEntity>(this IDbEngine engine, object objectInstance)
        {
            if(objectInstance == null) throw new ArgumentNullException("objectInstance");
            return RowCount<TEntity>(engine, new ExecuteParameterCollection(objectInstance));
        }
        /// <summary>
        /// 获取数据表的总行数。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="ps">参数集合。</param>
        /// <returns>返回一个查询结果。</returns>
        public static DbResult<long> RowCount<TEntity>(this IDbEngine engine, ExecuteParameterCollection ps)
        {
            return RowCount<TEntity>(engine, CreateWhere(engine, ps));
        }
        /// <summary>
        /// 获取数据表的总行数。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="where">筛选条件，不含 WHERE 关键字。</param>
        /// <param name="ps">参数集合。</param>
        /// <returns>返回一个查询结果。</returns>
        public static DbResult<long> RowCount<TEntity>(this IDbEngine engine, string where = null, ExecuteParameterCollection ps = null)
        {
            if(engine == null) throw new ArgumentNullException("engine");

            var commandText = "SELECT COUNT(*) FROM " + engine.Owner.Injector.ParameterSettings.EscapeName(EntityMapper.Instance<TEntity>.Mapper.Name);
            if(!string.IsNullOrEmpty(where)) commandText += " WHERE " + where;
            return engine.Execute(commandText, ps).ToScalar<long>();
        }

        #endregion

        #endregion
    }
}
