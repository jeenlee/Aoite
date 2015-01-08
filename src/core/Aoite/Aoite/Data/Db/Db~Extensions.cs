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
            return Engine.Execute(command);
        }

        /// <summary>
        /// 生成执行数据源查询与交互的执行器。
        /// </summary>
        /// <param name="commandText">Transact-SQL 语句。第一个字符为“>”时，表示一个存储过程。</param>
        /// <returns>返回执行数据源查询与交互的执行器。</returns>
        public static IDbExecutor Execute(string commandText)
        {
            return Engine.Execute(commandText);
        }

        /// <summary>
        /// 生成执行数据源查询与交互的执行器。
        /// </summary>
        /// <param name="commandText">Transact-SQL 语句。第一个字符为“>”时，表示一个存储过程。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns>返回执行数据源查询与交互的执行器。</returns>
        public static IDbExecutor Execute(string commandText, ExecuteParameterCollection parameters)
        {
            return Engine.Execute(commandText, parameters);
        }

        /// <summary>
        /// 生成执行数据源查询与交互的执行器。
        /// </summary>
        /// <param name="commandText">Transact-SQL 语句。第一个字符为“>”时，表示一个存储过程。</param>
        /// <param name="parameters">匹配 Name/Value 的参数集合或 数组。</param>
        /// <returns>返回执行数据源查询与交互的执行器。</returns>
        public static IDbExecutor Execute(string commandText, params object[] parameters)
        {
            return Engine.Execute(commandText, parameters);
        }

        /// <summary>
        /// 生成执行数据源查询与交互的执行器。
        /// </summary>
        /// <param name="commandText">Transact-SQL 语句。第一个字符为“>”时，表示一个存储过程。</param>
        /// <param name="parameters">参数集合。</param>
        /// <returns>返回执行数据源查询与交互的执行器。</returns>
        public static IDbExecutor Execute(string commandText, params ExecuteParameter[] parameters)
        {
            return Engine.Execute(commandText, parameters);
        }

        /// <summary>
        /// 生成执行数据源查询与交互的执行器。
        /// </summary>
        /// <param name="commandText">Transact-SQL 语句。第一个字符为“>”时，表示一个存储过程。</param>
        /// <param name="objectInstance">任意类型的实例。</param>
        /// <returns>返回执行数据源查询与交互的执行器。</returns>
        public static IDbExecutor Execute(string commandText, object objectInstance)
        {
            return Engine.Execute(commandText, objectInstance);
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
            return Engine.AddAnonymous<TEntity>(entity, tableName);
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
            return Engine.Add<TEntity>(entity, tableName);
        }

        #endregion

        #region Modify

        /// <summary>
        /// 执行一个更新的命令，可以是匿名对象的部分成员（<paramref name="entity"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="entity">实体的实例对象，可以是匿名对象的部分成员（<paramref name="entity"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。</param>
        /// <param name="tableName">实体的实际表名称，可以为 null 值。</param>
        /// <returns>返回一个查询结果。</returns>
        public static DbResult<int> ModifyAnonymous<TEntity>(object entity, string tableName = null)
        {
            return Engine.ModifyAnonymous<TEntity>(entity, tableName);
        }

        /// <summary>
        /// 执行一个更新的命令。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="entity">实体的实例对象。</param>
        /// <param name="tableName">实体的实际表名称，可以为 null 值。</param>
        /// <returns>返回一个查询结果。</returns>
        public static DbResult<int> Modify<TEntity>(TEntity entity, string tableName = null)
        {
            return Engine.Modify<TEntity>(entity, tableName);
        }

        #endregion

        #region ModifyWhere

        /// <summary>
        /// 提供匹配条件，执行一个更新的命令，可以是匿名对象的部分成员（<paramref name="entity"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="entity">实体的实例对象，可以是匿名对象的部分成员（<paramref name="entity"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。</param>
        /// <param name="objectInstance">匿名参数集合实例。</param>
        public static DbResult<int> ModifyWhere<TEntity>(object entity, object objectInstance)
        {
            return Engine.ModifyWhere<TEntity>(entity, objectInstance);
        }

        /// <summary>
        /// 提供匹配条件，执行一个更新的命令，可以是匿名对象的部分成员（<paramref name="entity"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>  
        /// <param name="entity">实体的实例对象，可以是匿名对象的部分成员（<paramref name="entity"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。</param>
        /// <param name="ps">参数集合实例。</param>
        public static DbResult<int> ModifyWhere<TEntity>(object entity, ExecuteParameterCollection ps)
        {
            return Engine.ModifyWhere<TEntity>(entity, ps);
        }
        /// <summary>
        /// 提供匹配条件，执行一个更新的命令，可以是匿名对象的部分成员（<paramref name="entity"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="entity">实体的实例对象，可以是匿名对象的部分成员（<paramref name="entity"/> 属性成员和 <typeparamref name="TEntity"/> 属性成员必须一致）。</param>
        /// <param name="where">条件表达式。</param>
        /// <param name="ps">参数集合实例。</param>
        public static DbResult<int> ModifyWhere<TEntity>(object entity, string where, ExecuteParameterCollection ps = null)
        {
            return Engine.ModifyWhere<TEntity>(entity, where, ps);
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
            return Engine.RemoveAnonymous<TEntity>(entityOrPKValues, tableName);
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
            return Engine.Remove<TEntity>(entity, tableName);
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

        #endregion

        #region FineOne

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
        /// <param name="keyName">主键的列名。</param>
        /// <param name="keyValue">主键的列值。</param>
        public static DbResult<TEntity> FindOne<TEntity>(string keyName, object keyValue)
        {
            return Engine.FindOne<TEntity>(keyName, keyValue);
        }

        /// <summary>
        /// 获取指定 Id 值的数据源对象。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="id">字段“Id”的值。</param>
        public static DbResult<TView> FindOne<TEntity, TView>(object id)
        {
            return Engine.FindOne<TEntity, TView>(id);
        }
        /// <summary>
        /// 获取指定 Id 键值的数据源对象。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="keyName">主键的列名。</param>
        /// <param name="keyValue">主键的列值。</param>
        public static DbResult<TView> FindOne<TEntity, TView>(string keyName, object keyValue)
        {
            return Engine.FindOne<TEntity, TView>(keyName, keyValue);
        }

        #endregion

        #region FindOneWhere

        /// <summary>
        /// 提供匹配条件，获取一个对象。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="objectInstance">匿名参数集合实例。</param>
        public static DbResult<TEntity> FindOneWhere<TEntity>(object objectInstance)
        {
            return Engine.FindOneWhere<TEntity>(objectInstance);
        }
        /// <summary>
        /// 提供匹配条件，获取一个对象。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="ps">参数集合实例。</param>
        public static DbResult<TEntity> FindOneWhere<TEntity>(ExecuteParameterCollection ps = null)
        {
            return Engine.FindOneWhere<TEntity>(ps);
        }
        /// <summary>
        /// 提供匹配条件，获取一个对象。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="where">条件表达式。</param>
        /// <param name="ps">参数集合实例。</param>
        public static DbResult<TEntity> FindOneWhere<TEntity>(string where, ExecuteParameterCollection ps = null)
        {
            return Engine.FindOneWhere<TEntity>(where, ps);
        }

        /// <summary>
        /// 提供匹配条件，获取一个对象。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="objectInstance">匿名参数集合实例。</param>
        public static DbResult<TView> FindOneWhere<TEntity, TView>(object objectInstance)
        {
            return Engine.FindOneWhere<TEntity, TView>(objectInstance);
        }
        /// <summary>
        /// 提供匹配条件，获取一个对象。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="ps">参数集合实例。</param>
        public static DbResult<TView> FindOneWhere<TEntity, TView>(ExecuteParameterCollection ps = null)
        {
            return Engine.FindOneWhere<TEntity, TView>(ps);
        }
        /// <summary>
        /// 提供匹配条件，获取一个对象。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="where">条件表达式。</param>
        /// <param name="ps">参数集合实例。</param>
        public static DbResult<TView> FindOneWhere<TEntity, TView>(string where, ExecuteParameterCollection ps = null)
        {
            return Engine.FindOneWhere<TEntity, TView>(where, ps);
        }

        #endregion

        #region FindAllWhere

        /// <summary>
        /// 提供匹配条件，获取对象的列表。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="objectInstance">匿名参数集合实例。</param>
        public static DbResult<List<TEntity>> FindAllWhere<TEntity>(object objectInstance)
        {
            return Engine.FindAllWhere<TEntity>(objectInstance);
        }
        /// <summary>
        /// 提供匹配条件，获取对象的列表。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="ps">参数集合实例。</param>
        public static DbResult<List<TEntity>> FindAllWhere<TEntity>(ExecuteParameterCollection ps = null)
        {
            return Engine.FindAllWhere<TEntity>(ps);
        }
        /// <summary>
        /// 提供匹配条件，获取对象的列表。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="where">条件表达式。</param>
        /// <param name="ps">参数集合实例。</param>
        public static DbResult<List<TEntity>> FindAllWhere<TEntity>(string where, ExecuteParameterCollection ps = null)
        {
            return Engine.FindAllWhere<TEntity>(where, ps);
        }

        /// <summary>
        /// 提供匹配条件，获取对象的列表。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="objectInstance">匿名参数集合实例。</param>
        public static DbResult<List<TView>> FindAllWhere<TEntity, TView>(object objectInstance)
        {
            return Engine.FindAllWhere<TEntity, TView>(objectInstance);
        }

        /// <summary>
        /// 提供匹配条件，获取对象的列表。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="ps">参数集合实例。</param>
        public static DbResult<List<TView>> FindAllWhere<TEntity, TView>(ExecuteParameterCollection ps = null)
        {
            return Engine.FindAllWhere<TEntity, TView>(ps);
        }
        /// <summary>
        /// 提供匹配条件，获取对象的列表。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="where">条件表达式。</param>
        /// <param name="ps">参数集合实例。</param>
        public static DbResult<List<TView>> FindAllWhere<TEntity, TView>(string where, ExecuteParameterCollection ps = null)
        {
            return Engine.FindAllWhere<TEntity, TView>(where, ps);
        }

        #endregion

        #region FindAllPage

        /// <summary>
        /// 提供匹配条件，获取对象的列表。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="page">一个分页的实现。</param>
        /// <param name="objectInstance">匿名参数集合实例。</param>
        public static DbResult<GridData<TEntity>> FindAllPage<TEntity>(IPagination page, object objectInstance)
        {
            return Engine.FindAllPage<TEntity>(page, objectInstance);
        }
        /// <summary>
        /// 提供匹配条件，获取对象的列表。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="page">一个分页的实现。</param>
        /// <param name="ps">参数集合实例。</param>
        public static DbResult<GridData<TEntity>> FindAllPage<TEntity>(IPagination page, ExecuteParameterCollection ps = null)
        {
            return Engine.FindAllPage<TEntity>(page, ps);
        }
        /// <summary>
        /// 提供匹配条件，获取对象的列表。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="where">条件表达式。</param>
        /// <param name="page">一个分页的实现。</param>
        /// <param name="ps">参数集合实例。</param>
        public static DbResult<GridData<TEntity>> FindAllPage<TEntity>(IPagination page, string where, ExecuteParameterCollection ps = null)
        {
            return Engine.FindAllPage<TEntity>(page, where, ps);
        }

        /// <summary>
        /// 提供匹配条件，获取对象的列表。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="page">一个分页的实现。</param>
        /// <param name="objectInstance">匿名参数集合实例。</param>
        public static DbResult<GridData<TView>> FindAllPage<TEntity, TView>(IPagination page, object objectInstance)
        {
            return Engine.FindAllPage<TEntity, TView>(page, objectInstance);
        }
        /// <summary>
        /// 提供匹配条件，获取对象的列表。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="page">一个分页的实现。</param>
        /// <param name="ps">参数集合实例。</param>
        public static DbResult<GridData<TView>> FindAllPage<TEntity, TView>(IPagination page, ExecuteParameterCollection ps = null)
        {
            return Engine.FindAllPage<TEntity, TView>(page, ps);
        }
        /// <summary>
        /// 提供匹配条件，获取对象的列表。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <param name="page">一个分页的实现。</param>
        /// <param name="where">条件表达式。</param>
        /// <param name="ps">参数集合实例。</param>
        public static DbResult<GridData<TView>> FindAllPage<TEntity, TView>(IPagination page, string where, ExecuteParameterCollection ps = null)
        {
            return Engine.FindAllPage<TEntity, TView>(page, where, ps);
        }

        #endregion

        #region Exists

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
        /// <param name="keyName">主键的列名。</param>
        /// <param name="keyValue">主键的列值。</param>
        public static Result<bool> Exists<TEntity>(string keyName, object keyValue)
        {
            return Engine.Exists<TEntity>(keyName, keyValue);
        }

        #endregion

        #region ExistsWhere

        /// <summary>
        /// 判断指定的条件的数据是否已存在。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="objectInstance">匿名参数集合实例。</param>
        public static Result<bool> ExistsWhere<TEntity>(object objectInstance)
        {
            return Engine.ExistsWhere<TEntity>(objectInstance);
        }

        /// <summary>
        /// 判断指定的条件的数据是否已存在。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="ps">参数集合实例。</param>
        public static Result<bool> ExistsWhere<TEntity>(ExecuteParameterCollection ps)
        {
            return Engine.ExistsWhere<TEntity>(ps);
        }

        /// <summary>
        /// 判断指定的条件的数据是否已存在。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="where">条件表达式。</param>
        /// <param name="ps">参数集合实例。</param>
        public static Result<bool> ExistsWhere<TEntity>(string where, ExecuteParameterCollection ps = null)
        {
            return Engine.ExistsWhere<TEntity>(where, ps);
        }

        #endregion

        #region RowCount

        /// <summary>
        /// 获取数据表的总行数。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="objectInstance">匿名参数集合实例。</param>
        /// <returns>返回一个查询结果。</returns>
        public static DbResult<long> RowCount<TEntity>(object objectInstance)
        {
            return Engine.RowCount<TEntity>(objectInstance);
        }
        /// <summary>
        /// 获取数据表的总行数。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="ps">参数集合。</param>
        /// <returns>返回一个查询结果。</returns>
        public static DbResult<long> RowCount<TEntity>(ExecuteParameterCollection ps)
        {
            return Engine.RowCount<TEntity>(ps);
        }
        /// <summary>
        /// 获取数据表的总行数。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <param name="where">筛选条件，不含 WHERE 关键字。</param>
        /// <param name="ps">参数集合。</param>
        /// <returns>返回一个查询结果。</returns>
        public static DbResult<long> RowCount<TEntity>(string where = null, ExecuteParameterCollection ps = null)
        {
            return Engine.RowCount<TEntity>(where, ps);
        }

        #endregion

        #region Other

        /// <summary>
        /// 创建指定视图类型的字段列表。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <typeparam name="TView">视图的数据类型。</typeparam>
        /// <returns>返回一个字段的列表。</returns>
        public static string CreateFields<TEntity, TView>()
        {
            return Engine.CreateFields<TEntity, TView>();
        }

        /// <summary>
        /// 创建一个条件查询语句。
        /// </summary>
        /// <param name="ps">参数集合实例。</param>
        /// <param name="binary">二元运算符。</param>
        /// <returns>返回一个条件查询语句。</returns>
        public static string CreateWhere(ExecuteParameterCollection ps, string binary = "AND")
        {
            return Engine.CreateWhere(ps, binary);
        }

        /// <summary>
        /// 获取最后递增序列值。
        /// </summary>
        /// <returns>返回一个结果。</returns>
        public static Result<long> GetLastIdentity<TEntity>()
        {
            return Engine.GetLastIdentity<TEntity>();
        }

        #endregion

        #endregion
    }
}
