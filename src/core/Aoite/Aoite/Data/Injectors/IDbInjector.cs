using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Aoite.Data
{
    /// <summary>
    /// 定义一个数据源查询与交互的注入器。
    /// </summary>
    public interface IDbInjector
    {
        /// <summary>
        /// 获取用于创建提供程序对数据源类的实现的实例。
        /// </summary>
        DbProviderFactory Factory { get; }
        /// <summary>
        /// 获取一个值，表示当前数据操作的数据提供程序的类型。
        /// </summary>
        DbEngineProvider Provider { get; }
        /// <summary>
        /// 获取参数的配置。
        /// </summary>
        DefaultParameterSettings ParameterSettings { get; }

        /// <summary>
        /// 创建并返回一个到数据源的连接。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <returns>返回一个到数据源的连接。</returns>
        DbConnection CreateConnection(IDbEngine engine);
        /// <summary>
        /// 描述指定的 <see cref="System.Data.Common.DbParameter"/>。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="parameter">一个 <see cref="System.Data.Common.DbParameter"/> 的实例。</param>
        /// <returns>返回参数描述的 <see cref="System.String"/> 值。</returns>
        string DescribeParameter(IDbEngine engine, DbParameter parameter);
        /// <summary>
        /// 创建并返回一个与给定数据源关联的命令对象。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="command">查询命令。</param>
        /// <param name="conn">指定数据源。</param>
        /// <returns>返回一个命令对象。</returns>
        DbCommand CreateDbCommand(IDbEngine engine, ExecuteCommand command, DbConnection conn = null);

        /// <summary>
        /// 指定类型映射器创建一个获取最后递增序列值的命令。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="mapper">类型映射器。</param>
        /// <returns>返回一个查询命令。</returns>
        ExecuteCommand CreateLastIdentityCommand(IDbEngine engine, TypeMapper mapper);
        /// <summary>
        /// 指定实体创建一个插入的命令。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="mapper">类型映射器。</param>
        /// <param name="entity">实体的实例对象。</param>
        /// <param name="tableName">实体的实际表名称，可以为 null 值。</param>
        /// <returns>返回一个查询命令。</returns>
        ExecuteCommand CreateInsertCommand(IDbEngine engine, TypeMapper mapper, object entity, string tableName = null);
        /// <summary>
        /// 指定实体创建一个更新的命令。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="mapper">类型映射器。</param>
        /// <param name="entity">实体的实例对象。</param>
        /// <param name="tableName">实体的实际表名称，可以为 null 值。</param>
        /// <returns>返回一个查询命令。</returns>
        ExecuteCommand CreateUpdateCommand(IDbEngine engine, TypeMapper mapper, object entity, string tableName = null);
        /// <summary>
        /// 指定实体创建一个删除的命令。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="mapper">类型映射器。</param>
        /// <param name="entityOrPKValue">实体的实例对象（引用类型）或一个主键的值（值类型）。</param>
        /// <param name="tableName">实体的实际表名称，可以为 null 值。</param>
        /// <returns>返回一个查询命令。</returns>
        ExecuteCommand CreateDeleteCommand(IDbEngine engine, TypeMapper mapper, object entityOrPKValue, string tableName = null);
        /// <summary>
        /// 创建一个数据源查询与交互的执行器。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="command">执行查询的命令。</param>
        /// <returns>返回一个执行器。</returns>
        IDbExecutor CreateExecutor(IDbEngine engine, ExecuteCommand command);

        /// <summary>
        /// 创建一个分页组件。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <returns>返回一个分页组件。</returns>
        PaginationBase CreatePagination(IDbEngine engine);
    }
}
