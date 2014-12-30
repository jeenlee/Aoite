using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aoite.Data;

namespace System
{
    partial class Db
    {
        #region Execute

        /// <summary>
        /// 生成执行数据源查询与交互的执行器。
        /// </summary>
        /// <param name="command">执行查询的命令。</param>
        /// <returns>返回执行数据源查询与交互的执行器。</returns>
        public static IDbExecutor Execute(ExecuteCommand command)
        {
            return Db.Engine.Execute(command);
        }

        /// <summary>
        /// 生成执行数据源查询与交互的执行器。
        /// </summary>
        /// <param name="commandText">Transact-SQL 语句。第一个字符为“>”时，表示一个存储过程。</param>
        /// <returns>返回执行数据源查询与交互的执行器。</returns>
        public static IDbExecutor Execute(string commandText)
        {
            return Db.Engine.Execute(new ExecuteCommand(commandText));
        }

        /// <summary>
        /// 生成执行数据源查询与交互的执行器。
        /// </summary>
        /// <param name="commandText">Transact-SQL 语句。第一个字符为“>”时，表示一个存储过程。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns>返回执行数据源查询与交互的执行器。</returns>
        public static IDbExecutor Execute(string commandText, ExecuteParameterCollection parameters)
        {
            return Db.Engine.Execute(new ExecuteCommand(commandText, parameters));
        }

        /// <summary>
        /// 生成执行数据源查询与交互的执行器。
        /// </summary>
        /// <param name="commandText">Transact-SQL 语句。第一个字符为“>”时，表示一个存储过程。</param>
        /// <param name="parameters">匹配 Name/Value 的参数集合或 数组。</param>
        /// <returns>返回执行数据源查询与交互的执行器。</returns>
        public static IDbExecutor Execute(string commandText, params object[] parameters)
        {
            return Db.Engine.Execute(new ExecuteCommand(commandText, parameters));
        }

        /// <summary>
        /// 生成执行数据源查询与交互的执行器。
        /// </summary>
        /// <param name="commandText">Transact-SQL 语句。第一个字符为“>”时，表示一个存储过程。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns>返回执行数据源查询与交互的执行器。</returns>
        public static IDbExecutor Execute(string commandText, params ExecuteParameter[] parameters)
        {
            return Db.Engine.Execute(new ExecuteCommand(commandText, parameters));
        }

        /// <summary>
        /// 生成执行数据源查询与交互的执行器。
        /// </summary>
        /// <param name="commandText">Transact-SQL 语句。第一个字符为“>”时，表示一个存储过程。</param>
        /// <param name="objectInstance">任意类型的实例。</param>
        /// <returns>返回执行数据源查询与交互的执行器。</returns>
        public static IDbExecutor Execute(string commandText, object objectInstance)
        {
            return Db.Engine.Execute(new ExecuteCommand(commandText, new ExecuteParameterCollection(objectInstance)));
        }

        #endregion

        #region Adv

        #region Add

        /// <summary>
        /// 执行一个插入的命令，可以是匿名对象的部分成员（<paramref name="entity"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="entity">实体的实例对象，可以是匿名对象的部分成员（<paramref name="entity"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。</param>
        /// <param name="tableName">实体的实际表名称，可以为 null 值。</param>
        /// <returns>返回一个查询结果。</returns>
        public static DbResult<int> AddAnonymous<TEntity>(object entity, string tableName = null)
        {
            return Db.Engine.AddAnonymous<TEntity>(entity);
        }

        /// <summary>
        /// 执行一个插入的命令。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="entity">实体的实例对象。</param>
        /// <param name="tableName">实体的实际表名称，可以为 null 值。</param>
        /// <returns>返回一个查询结果。</returns>
        public static DbResult<int> Add<TEntity>(TEntity entity, string tableName = null)
        {
            return Db.Engine.Add<TEntity>(entity);
        }

        #endregion

        #region Edit

        /// <summary>
        /// 执行一个更新的命令，可以是匿名对象的部分成员（<paramref name="entity"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="entity">实体的实例对象，可以是匿名对象的部分成员（<paramref name="entity"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。</param>
        /// <param name="tableName">实体的实际表名称，可以为 null 值。</param>
        /// <returns>返回一个查询结果。</returns>
        [Obsolete("此方法已过时，请调用 ModifyAnonymous<TEntity> 方法。")]
        [System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)]
        public static DbResult<int> EditAnonymous<TEntity>(object entity, string tableName = null)
        {
            return Db.Engine.ModifyAnonymous<TEntity>(entity);
        }

        /// <summary>
        /// 执行一个更新的命令。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="entity">实体的实例对象。</param>
        /// <param name="tableName">实体的实际表名称，可以为 null 值。</param>
        /// <returns>返回一个查询结果。</returns>
        [Obsolete("此方法已过时，请调用 ModifyAnonymous<TEntity> 方法。")]
        [System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)]
        public static DbResult<int> Edit<TEntity>(TEntity entity, string tableName = null)
        {
            return Db.Engine.Modify<TEntity>(entity);
        }

        /// <summary>
        /// 执行一个更新的命令，可以是匿名对象的部分成员（<paramref name="entity"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="entity">实体的实例对象，可以是匿名对象的部分成员（<paramref name="entity"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。</param>
        /// <param name="tableName">实体的实际表名称，可以为 null 值。</param>
        /// <returns>返回一个查询结果。</returns>
        [Obsolete("此方法已过时，请调用 ModifyAnonymous<TEntity> 方法。")]
        [System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)]
        public static DbResult<int> ModifyAnonymous<TEntity>(object entity, string tableName = null)
        {
            return Db.Engine.ModifyAnonymous<TEntity>(entity);
        }

        /// <summary>
        /// 执行一个更新的命令。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="entity">实体的实例对象。</param>
        /// <param name="tableName">实体的实际表名称，可以为 null 值。</param>
        /// <returns>返回一个查询结果。</returns>
        [Obsolete("此方法已过时，请调用 Modify<TEntity> 方法。")]
        [System.ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)]
        public static DbResult<int> Modify<TEntity>(TEntity entity, string tableName = null)
        {
            return Db.Engine.Modify<TEntity>(entity);
        }
        #endregion

        #region Remove

        /// <summary>
        /// 执行一个删除的命令，在删除命令中 <paramref name="entityOrPKValues"/> 可以是主键的值（表只有一个主键），也可以是匿名对象的部分成员（<paramref name="entityOrPKValues"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="entityOrPKValues">实体的实例对象，在删除命令中 <paramref name="entityOrPKValues"/> 可以是主键的值（表只有一个主键，值允许是一个数组，表示删除多条记录），也可以是匿名对象的部分成员（<paramref name="entityOrPKValues"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。</param>
        /// <param name="tableName">实体的实际表名称，可以为 null 值。</param>
        /// <returns>返回一个查询结果。</returns>
        public static DbResult<int> RemoveAnonymous<TEntity>(object entityOrPKValues, string tableName = null)
        {
            return Db.Engine.RemoveAnonymous<TEntity>(entityOrPKValues);
        }

        /// <summary>
        /// 执行一个删除的命令。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="entity">实体的实例对象。</param>
        /// <param name="tableName">实体的实际表名称，可以为 null 值。</param>
        /// <returns>返回一个查询结果。</returns>
        public static DbResult<int> Remove<TEntity>(TEntity entity, string tableName = null)
        {
            return Db.Engine.Remove<TEntity>(entity);
        }

        #endregion

        #region Select

        /// <summary>
        /// 添加 SELECT 的字段。
        /// </summary>
        /// <param name="fields">字段的集合。</param>
        /// <returns>返回 <see cref="Aoite.Data.ISelect"/> 的实例。</returns>
        public static ISelect Select(params string[] fields)
        {
            return Engine.Select(fields);
        }

        /// <summary>
        /// 添加 SELECT 的字段和 FORM 语句。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="fields">字段的集合。</param>
        /// <returns>返回 <see cref="Aoite.Data.ISelect"/> 的实例。</returns>
        public static ISelect Select<TEntity>(params string[] fields)
        {
            return Engine.Select<TEntity>(fields);
        }

        /// <summary>
        /// 获取指定 Id 值的数据源对象。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="id">字段“Id”的值。</param>
        public static DbResult<TEntity> FindOne<TEntity>(object id)
        {
            return Engine.FindOne<TEntity>(id);
        }

        /// <summary>
        /// 获取指定 Id 键值的数据源对象。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="idKey">主键的列名。</param>
        /// <param name="idValue">主键的列值。</param>
        public static DbResult<TEntity> FindOne<TEntity>(string idKey, object idValue)
        {
            return Engine.FindOne<TEntity>(idKey, idValue);
        }

        /// <summary>
        /// 判断指定的主键的列名的值是否已存在。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="keyValue">主键的列值。</param>
        public static Result<bool> Exists<TEntity>(object keyValue)
        {
            return Engine.Exists<TEntity>(keyValue);
        }
        /// <summary>
        /// 判断指定的主键的列名的值是否已存在。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="idKey">主键的列名。</param>
        /// <param name="idValue">主键的列值。</param>
        public static Result<bool> Exists<TEntity>(string idKey, object idValue)
        {
            return Engine.Exists<TEntity>(idKey, idValue);
        }

        #endregion

        /// <summary>
        /// 获取数据表的总行数。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="where">筛选条件，不含 WHERE 关键字。</param>
        /// <returns>返回一个查询结果。</returns>
        public static DbResult<long> RowCount<TEntity>(string where = null)
        {
            return Db.Engine.RowCount<TEntity>(where);
        }
        #endregion
    }
}
