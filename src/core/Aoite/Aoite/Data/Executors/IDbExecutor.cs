using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Aoite.Data
{
    /// <summary>
    /// 定义一个数据源查询与交互的执行器。
    /// </summary>
    public interface IDbExecutor
    {
        /// <summary>
        /// 获取数据源查询与交互引擎的实例。
        /// </summary>
        IDbEngine Engine { get; }
        /// <summary>
        /// 获取执行的命令。
        /// </summary>
        ExecuteCommand Command { get; }

        /// <summary>
        /// 执行查询命令，并返回数据集结果。
        /// </summary>
        /// <returns>返回一个执行结果。</returns>
        DataSetResult<DataSet> ToDataSet();
        /// <summary>
        /// 执行查询命令，并返回自定义的数据集结果。
        /// </summary>
        /// <typeparam name="TDataSet">自定义的数据集类型。</typeparam>
        /// <returns>返回一个执行结果。</returns>
        DataSetResult<TDataSet> ToDataSet<TDataSet>() where TDataSet : DataSet, new();
        /// <summary>
        /// 执行查询命令，并返回自定义实体的集合结果。
        /// </summary>
        /// <typeparam name="TEntity">实体的类型。</typeparam>
        /// <returns>返回一个执行结果。</returns>
        DbResult<List<TEntity>> ToEntities<TEntity>();
        /// <summary>
        /// 执行分页查询命令，并返回自定义实体的集合结果。
        /// </summary>
        /// <typeparam name="TEntity">实体的类型。</typeparam>
        /// <param name="page">一个分页的实现。</param>
        /// <returns>返回一个执行结果。</returns>
        DbResult<GridData<TEntity>> ToEntities<TEntity>(IPagination page);
        /// <summary>
        /// 执行分页查询命令，并返回自定义实体的集合结果。
        /// </summary>
        /// <typeparam name="TEntity">实体的类型。</typeparam>
        /// <param name="pageNumber">以 1 起始的页码。</param>
        /// <param name="pageSize">分页大小。</param>
        /// <returns>返回一个执行结果。</returns>
        DbResult<GridData<TEntity>> ToEntities<TEntity>(int pageNumber, int pageSize = 10);
        /// <summary>
        /// 执行查询命令，并返回自定义的实体结果。
        /// </summary>
        /// <typeparam name="TEntity">实体的类型。</typeparam>
        /// <returns>返回一个执行结果。</returns>
        DbResult<TEntity> ToEntity<TEntity>();

        /// <summary>
        /// 执行查询命令，并返回自定义实体的集合结果。
        /// </summary>
        /// <returns>返回一个执行结果。</returns>
        DbResult<List<dynamic>> ToEntities();
        /// <summary>
        /// 执行分页查询命令，并返回自定义实体的集合结果。
        /// </summary>
        /// <param name="page">一个分页的实现。</param>
        /// <returns>返回一个执行结果。</returns>
        DbResult<GridData<dynamic>> ToEntities(IPagination page);
        /// <summary>
        /// 执行分页查询命令，并返回自定义实体的集合结果。
        /// </summary>
        /// <param name="pageNumber">以 1 起始的页码。</param>
        /// <param name="pageSize">分页大小。</param>
        /// <returns>返回一个执行结果。</returns>
        DbResult<GridData<dynamic>> ToEntities(int pageNumber, int pageSize = 10);
        /// <summary>
        /// 执行查询命令，并返回自定义的实体结果。
        /// </summary>
        /// <returns>返回一个执行结果。</returns>
        DbResult<dynamic> ToEntity();

        /// <summary>
        /// 执行查询命令，并返回受影响的行数结果。
        /// </summary>
        /// <returns>返回一个执行结果。</returns>
        DbResult<int> ToNonQuery();
        /// <summary>
        /// 执行查询命令，并构建一个读取器结果。
        /// </summary>
        /// <param name="callback">给定的读取器委托。</param>
        /// <returns>返回一个执行结果。</returns>
        DbResult ToReader(ExecuteReaderHandler callback);
        /// <summary>
        /// 执行查询命令，并构建一个读取器结果。
        /// </summary>
        /// <typeparam name="TValue">返回值的类型。</typeparam>
        /// <param name="callback">给定的读取器委托。</param>
        /// <returns>返回一个执行结果。</returns>
        DbResult<TValue> ToReader<TValue>(ExecuteReaderHandler<TValue> callback);
        /// <summary>
        /// 执行查询命令，并返回查询结果集中第一行的第一列。忽略额外的列或行。
        /// </summary>
        /// <returns>返回一个执行结果。</returns>
        DbResult<object> ToScalar();
        /// <summary>
        /// 指定值的数据类型，执行查询命令，并返回查询结果集中第一行的第一列。忽略额外的列或行。
        /// </summary>
        /// <typeparam name="TValue">值的数据类型。</typeparam>
        /// <returns>返回一个执行结果。</returns>
        DbResult<TValue> ToScalar<TValue>();
        /// <summary>
        /// 执行查询命令，并返回数据表结果。
        /// </summary>
        /// <returns>返回一个执行结果。</returns>
        TableResult ToTable();
        /// <summary>
        /// 执行分页查询命令，并返回数据表结果。
        /// </summary>
        /// <param name="page">一个分页的实现。</param>
        /// <returns>返回一个执行结果。</returns>
        TableResult ToTable(IPagination page);
        /// <summary>
        /// 执行分页查询命令，并返回数据表结果。
        /// </summary>
        /// <param name="pageNumber">以 1 起始的页码。</param>
        /// <param name="pageSize">分页大小。</param>
        /// <returns>返回一个执行结果。</returns>
        TableResult ToTable(int pageNumber, int pageSize = 10);
    }
}
