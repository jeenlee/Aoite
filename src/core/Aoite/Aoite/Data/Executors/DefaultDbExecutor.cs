using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Aoite.Data
{
    /// <summary>
    /// 表示一个默认的数据源查询与交互的执行器。
    /// </summary>
    public class DefaultDbExecutor : IDbExecutor
    {
        private IDbInjector _engineInjector;
        private DbEngine _owner;
        private DbConnection _connection;
        private DbTransaction _transaction;

        /// <summary>
        /// 初始化一个 <see cref="Aoite.Data.DefaultDbExecutor"/> 类的新实例。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="command">执行的命令。</param>
        public DefaultDbExecutor(IDbEngine engine, ExecuteCommand command)
        {
            if(engine == null) throw new ArgumentNullException("engine");
            this._Engine = engine;
            this._Command = command;
            this._owner = engine.Owner;
            this._connection = engine.CreateConnection();
            this._transaction = engine.CreateTransaction();
            this._engineInjector = this._owner.Injector;
        }

        private IDbEngine _Engine;
        /// <summary>
        /// 获取数据源查询与交互引擎的实例。
        /// </summary>
        public virtual IDbEngine Engine { get { return this._Engine; } }

        private ExecuteCommand _Command;
        /// <summary>
        /// 获取执行的命令。
        /// </summary>
        public virtual ExecuteCommand Command { get { return this._Command; } }

        /// <summary>
        /// 提供一个 <see cref="System.Data.Common.DbCommand"/> 的实例，创建一个关联的数据适配器。
        /// </summary>
        /// <param name="command">一个 <see cref="System.Data.Common.DbCommand"/> 的实例。</param>
        /// <returns>返回一个关联的数据适配器。</returns>
        protected virtual DbDataAdapter CreateDataAdapter(DbCommand command)
        {
            var adapter = this._engineInjector.Factory.CreateDataAdapter();
            adapter.SelectCommand = command;
            return adapter;
        }

        /// <summary>
        /// 命令执行前发生。
        /// </summary>
        /// <param name="type">执行的类型。</param>
        protected virtual void OnExecuting(ExecuteType type)
        {
            this._owner.InternalOnExecuting(this._Engine, type, this._Command);
        }

        /// <summary>
        /// 命令执行前发生。
        /// </summary>
        /// <param name="type">执行的类型。</param>
        /// <param name="result">操作的返回值。</param>
        protected virtual void OnExecuted(ExecuteType type, IDbResult result)
        {
            this._owner.InternalOnExecuted(this._Engine, type, this._Command, result);
        }

        /// <summary>
        /// 创建一个关联执行器的 <see cref="System.Data.Common.DbCommand"/> 的实例。
        /// </summary>
        /// <returns>返回一个关联执行器的 <see cref="System.Data.Common.DbCommand"/> 的实例。</returns>
        protected virtual DbCommand CreateDbCommand()
        {
            var dbCommand = this._engineInjector.CreateDbCommand(this._Engine, this._Command, this._connection);
            if(this._transaction != null) dbCommand.Transaction = this._transaction;
            return dbCommand;
        }

        /// <summary>
        /// 打开数据源的连接。
        /// </summary>
        protected virtual void Open()
        {
            this._connection.TryOpen();
        }
        /// <summary>
        /// 关闭数据源的连接。
        /// </summary>
        protected virtual void Close()
        {
            this._connection.TryClose();
        }
        /// <summary>
        /// 执行数据源的查询与交互。
        /// </summary>
        /// <typeparam name="TDbResult">返回结果的数据类型。</typeparam>
        /// <typeparam name="TDbValue">返回值的数据类型。</typeparam>
        /// <param name="type">执行的类型。</param>
        /// <param name="callback">执行时的回调函数。</param>
        /// <returns>返回一个执行结果。</returns>
        protected virtual TDbResult Execute<TDbResult, TDbValue>(ExecuteType type, Func<DbCommand, TDbValue> callback)
            where TDbResult : DbResult<TDbValue>, new()
        {

            var dbCommand = this.CreateDbCommand();
            this.OnExecuting(type);
            TDbResult result = new TDbResult();
            try
            {
                this.Open();
                var value = callback(dbCommand);
                result.Initialization(this._Engine, dbCommand, value, null);
            }
            catch(Exception ex)
            {
                result.Initialization(this._Engine, dbCommand, default(TDbValue), this._Command.Aborted ? ExecuteAbortException.Instance : ex);
            }
            if(this._Engine == this._owner) this.Close();

            this.OnExecuted(type, result);
            return result;
        }

        /// <summary>
        /// 执行查询命令，并返回数据集结果。
        /// </summary>
        /// <typeparam name="TDataSetResult">数据集返回结果的类型。</typeparam>
        /// <typeparam name="TDataSet">数据集的类型。</typeparam>
        /// <returns>返回一个执行结果。</returns>
        protected virtual TDataSetResult ToDataSet<TDataSetResult, TDataSet>()
            where TDataSetResult : DataSetResult<TDataSet>, new()
            where TDataSet : DataSet, new()
        {
            return this.Execute<TDataSetResult, TDataSet>(ExecuteType.DataSet
                , dbCommand => dbCommand.ExecuteDataSet<TDataSet>(this.CreateDataAdapter(dbCommand)));
        }

        private long GetTotalCount(PaginationBase page)
        {
            var exeCommand = this._Command.Clone() as ExecuteCommand;
            exeCommand.CommandText = page.CreateTotalCountCommand(exeCommand.CommandText);
            var dbCommand = this._engineInjector.CreateDbCommand(this._Engine, exeCommand, this._connection);
            if(this._transaction != null) dbCommand.Transaction = this._transaction;
            return dbCommand.ExecuteScalar<long>();
        }

        /// <summary>
        /// 执行查询命令，并返回数据表结果。
        /// </summary>
        /// <returns>返回一个执行结果。</returns>
        public virtual TableResult ToTable()
        {
            return this.Execute<TableResult, AoiteTable>(ExecuteType.Table, dbCommand => dbCommand.ExecuteTable(this.CreateDataAdapter(dbCommand)));
        }

        /// <summary>
        /// 执行分页查询命令，并返回数据表结果。
        /// </summary>
        /// <param name="pageNumber">以 1 起始的页码。</param>
        /// <param name="pageSize">分页大小。</param>
        /// <returns>返回一个执行结果。</returns>
        public virtual TableResult ToTable(int pageNumber, int pageSize = 10)
        {
            if(pageNumber < 1) pageNumber = 1;
            if(pageSize < 1) pageSize = 1;

            return this.Execute<TableResult, AoiteTable>(ExecuteType.Table, dbCommand =>
            {
                var page = this._owner.Injector.CreatePagination(this._Engine);
                page.ProcessCommand(pageNumber, pageSize, dbCommand);
                var value = dbCommand.ExecuteTable(this.CreateDataAdapter(dbCommand));
                value.TotalRowCount = this.GetTotalCount(page);
                return value;
            });
        }

        /// <summary>
        /// 执行分页查询命令，并返回数据表结果。
        /// </summary>
        /// <param name="page">一个分页的实现。</param>
        /// <returns>返回一个执行结果。</returns>
        public TableResult ToTable(IPagination page)
        {
            return this.ToTable(page.PageNumber, page.PageSize);
        }

        #region Dynamic

        /// <summary>
        /// 执行查询命令，并返回自定义的实体结果。
        /// </summary>
        /// <returns>返回一个执行结果。</returns>
        public virtual DbResult<dynamic> ToEntity()
        {
            return this.Execute<DbResult<dynamic>, dynamic>(ExecuteType.Reader, dbCommand =>
            {
                using(var reader = dbCommand.ExecuteReader())
                {
                    if(!reader.Read()) return null;
                    return new DynamicEntityValue(reader);
                }
            });
        }
        /// <summary>
        /// 执行查询命令，并返回自定义实体的集合结果。
        /// </summary>
        /// <returns>返回一个执行结果。</returns>
        public virtual DbResult<List<dynamic>> ToEntities()
        {
            return this.Execute<DbResult<List<dynamic>>, List<dynamic>>(ExecuteType.Reader, DbExtensions.ExecuteEntities);
        }
        /// <summary>
        /// 执行分页查询命令，并返回自定义实体的集合结果。
        /// </summary>
        /// <param name="pageNumber">以 1 起始的页码。</param>
        /// <param name="pageSize">分页大小。</param>
        /// <returns>返回一个执行结果。</returns>
        public virtual DbResult<GridData<dynamic>> ToEntities(int pageNumber, int pageSize = 10)
        {
            return this.Execute<DbResult<GridData<dynamic>>, GridData<dynamic>>(ExecuteType.Reader, dbCommand =>
            {
                var page = this._owner.Injector.CreatePagination(this._Engine);
                page.ProcessCommand(pageNumber, pageSize, dbCommand);
                var entities = dbCommand.ExecuteEntities();
                var value = new GridData<dynamic>()
                {
                    Rows = entities.ToArray(),
                    Total = this.GetTotalCount(page)
                };
                return value;
            });
        }
        /// <summary>
        /// 执行分页查询命令，并返回自定义实体的集合结果。
        /// </summary>
        /// <param name="page">一个分页的实现。</param>
        /// <returns>返回一个执行结果。</returns>
        public DbResult<GridData<dynamic>> ToEntities(IPagination page)
        {
            return this.ToEntities(page.PageNumber, page.PageSize);
        }

        #endregion

        /// <summary>
        /// 执行查询命令，并返回自定义的实体结果。
        /// </summary>
        /// <typeparam name="TEntity">实体的类型。</typeparam>
        /// <returns>返回一个执行结果。</returns>
        public virtual DbResult<TEntity> ToEntity<TEntity>()
        {
            var mapper = TypeMapper.Instance<TEntity>.Mapper;
            return this.Execute<DbResult<TEntity>, TEntity>(ExecuteType.Reader, dbCommand =>
            {
                using(var reader = dbCommand.ExecuteReader())
                {
                    if(!reader.Read()) return default(TEntity);
                    var value = (TEntity)Activator.CreateInstance(typeof(TEntity), true);
                    return mapper.From(reader).To(value);
                }
            });
        }

        /// <summary>
        /// 执行查询命令，并返回自定义实体的集合结果。
        /// </summary>
        /// <typeparam name="TEntity">实体的类型。</typeparam>
        /// <returns>返回一个执行结果。</returns>
        public virtual DbResult<List<TEntity>> ToEntities<TEntity>()
        {
            return this.Execute<DbResult<List<TEntity>>, List<TEntity>>(ExecuteType.Reader, DbExtensions.ExecuteEntities<TEntity>);
        }
        /// <summary>
        /// 执行分页查询命令，并返回自定义实体的集合结果。
        /// </summary>
        /// <typeparam name="TEntity">实体的类型。</typeparam>
        /// <param name="pageNumber">以 1 起始的页码。</param>
        /// <param name="pageSize">分页大小。</param>
        /// <returns>返回一个执行结果。</returns>
        public virtual DbResult<GridData<TEntity>> ToEntities<TEntity>(int pageNumber, int pageSize = 10)
        {
            return this.Execute<DbResult<GridData<TEntity>>, GridData<TEntity>>(ExecuteType.Reader, dbCommand =>
            {
                var page = this._owner.Injector.CreatePagination(this._Engine);
                page.ProcessCommand(pageNumber, pageSize, dbCommand);
                var entities = dbCommand.ExecuteEntities<TEntity>();
                var value = new GridData<TEntity>()
                {
                    Rows = entities.ToArray(),
                    Total = this.GetTotalCount(page)
                };
                return value;
            });
        }
        /// <summary>
        /// 执行分页查询命令，并返回自定义实体的集合结果。
        /// </summary>
        /// <typeparam name="TEntity">实体的类型。</typeparam>
        /// <param name="page">一个分页的实现。</param>
        /// <returns>返回一个执行结果。</returns>
        public DbResult<GridData<TEntity>> ToEntities<TEntity>(IPagination page)
        {
            return this.ToEntities<TEntity>(page.PageNumber, page.PageSize);
        }
        /// <summary>
        /// 执行查询命令，并返回受影响的行数结果。
        /// </summary>
        /// <returns>返回一个执行结果。</returns>
        public virtual DbResult<int> ToNonQuery()
        {
            if(this._owner.IsReadonly) throw new ReadOnlyException("数据库为只读状态，不允许进行 ToNonQuery 操作。");
            return this.Execute<DbResult<int>, int>(ExecuteType.NoQuery, dbCommand => dbCommand.ExecuteNonQuery());
        }

        /// <summary>
        /// 执行查询命令，并构建一个读取器结果。
        /// </summary>
        /// <param name="callback">给定的读取器委托。</param>
        /// <returns>返回一个执行结果。</returns>
        public virtual DbResult ToReader(ExecuteReaderHandler callback)
        {
            return this.Execute<DbResult, VoidValue>(ExecuteType.Reader, dbCommand =>
            {
                using(var reader = dbCommand.ExecuteReader())
                {
                    callback(reader);
                }
                return default(VoidValue);
            });

        }

        /// <summary>
        /// 执行查询命令，并构建一个读取器结果。
        /// </summary>
        /// <typeparam name="TValue">返回值的类型。</typeparam>
        /// <param name="callback">给定的读取器委托。</param>
        /// <returns>返回一个执行结果。</returns>
        public virtual DbResult<TValue> ToReader<TValue>(ExecuteReaderHandler<TValue> callback)
        {
            return this.Execute<DbResult<TValue>, TValue>(ExecuteType.Reader, dbCommand =>
            {
                using(var reader = dbCommand.ExecuteReader())
                {
                    return callback(reader);
                }
            });
        }
        /// <summary>
        /// 执行查询命令，并返回查询结果集中第一行的第一列。忽略额外的列或行。
        /// </summary>
        /// <returns>返回一个执行结果。</returns>
        public DbResult<object> ToScalar() { return this.ToScalar<object>(); }
        /// <summary>
        /// 指定值的数据类型，执行查询命令，并返回查询结果集中第一行的第一列。忽略额外的列或行。
        /// </summary>
        /// <typeparam name="TValue">值的数据类型。</typeparam>
        /// <returns>返回一个执行结果。</returns>
        public virtual DbResult<TValue> ToScalar<TValue>()
        {
            return this.Execute<DbResult<TValue>, TValue>(ExecuteType.Scalar, DbExtensions.ExecuteScalar<TValue>);
        }

        /// <summary>
        /// 执行查询命令，并返回数据集结果。
        /// </summary>
        /// <returns>返回一个执行结果。</returns>
        public DataSetResult<DataSet> ToDataSet()
        {
            return this.ToDataSet<DataSet>();
        }

        /// <summary>
        /// 执行查询命令，并返回自定义的数据集结果。
        /// </summary>
        /// <typeparam name="TDataSet">自定义的数据集类型。</typeparam>
        /// <returns>返回一个执行结果。</returns>
        public virtual DataSetResult<TDataSet> ToDataSet<TDataSet>()
            where TDataSet : DataSet, new()
        {
            return this.Execute<DataSetResult<TDataSet>, TDataSet>(ExecuteType.DataSet
                , dbCommand => dbCommand.ExecuteDataSet<TDataSet>(this.CreateDataAdapter(dbCommand)));
        }


    }
}
