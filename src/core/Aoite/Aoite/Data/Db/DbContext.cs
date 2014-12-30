using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Aoite.Data
{
    /// <summary>
    /// 数据源查询与交互引擎的上下文对象，一个对象创建一个数据源连接。
    /// </summary>
    public class DbContext : ObjectDisposableBase, IDbEngine
    {
        private DbTransaction _Transaction;
        private readonly DbEngine _Engine;
        private DbConnection _Connection;

        /// <summary>
        /// 获取连接上下文。
        /// </summary>
        public DbConnection ContextConnection { get { return this._Connection; } }

        /// <summary>
        /// 获取事务上下文。
        /// </summary>
        public DbTransaction ContextTransaction { get { return this._Transaction; } }

        /// <summary>
        /// 获取一个值，指示当前上下文是否事务中。
        /// </summary>
        public bool IsTransaction { get { return this._Transaction != null; } }

        DbConnection IDbEngine.CreateConnection() { return this._Connection; }
        DbTransaction IDbEngine.CreateTransaction() { return this._Transaction; }

        private readonly bool IsUnitTestTime = false;

        internal DbContext(DbEngine engine)
        {
            if(engine == null) throw new ArgumentNullException("engine");
            this._Engine = engine;
            //if(!(this.IsUnitTestTime = (engine == DbUnitTest.Instance)))
            this._Connection = engine.CreateConnection();
        }

        /// <summary>
        /// 为打开的连接更改当前数据源。
        /// </summary>
        /// <param name="databaseName">为要使用的连接指定数据源名称。</param>
        public void ChangeDatabase(string databaseName)
        {
            this.Open();
            this._Connection.ChangeDatabase(databaseName);
        }

        /// <summary>
        /// 获取当前上下文的所属引擎。
        /// </summary>
        public DbEngine Owner
        {
            get
            {
                return this._Engine;
            }
        }

        private void BulkCopyMSSQL(BulkCopyArguments args)
        {
            System.Data.SqlClient.SqlBulkCopy bulkCopy = new System.Data.SqlClient.SqlBulkCopy(this._Connection as System.Data.SqlClient.SqlConnection);
            bulkCopy.BatchSize = args.BatchSize;
            bulkCopy.BulkCopyTimeout = args.BulkCopyTimeout;
            bulkCopy.DestinationTableName = args.DestinationTableName;
            bulkCopy.NotifyAfter = args.NotifyAfter;

            if(args.RowsCopied != null)
            {
                bulkCopy.SqlRowsCopied += (ss, ee) =>
                {
                    ee.Abort = args.RowsCopied(ee.RowsCopied);
                };
            }
            bulkCopy.WriteToServer(args.Table, args.RowState);
        }

        private void BulkCopyOracle(BulkCopyArguments args)
        {
            //DDTek.Oracle.OracleBulkCopy bulkCopy = new DDTek.Oracle.OracleBulkCopy(this._Connection as DDTek.Oracle.OracleConnection);
            //bulkCopy.BatchSize = args.BatchSize;
            //bulkCopy.BulkCopyTimeout = args.BulkCopyTimeout;
            //bulkCopy.DestinationTableName = args.DestinationTableName;
            //bulkCopy.NotifyAfter = args.NotifyAfter;
            //if(args.RowsCopied != null)
            //{
            //    bulkCopy.OracleRowsCopied += (ss, ee) =>
            //    {
            //        ee.Abort = args.RowsCopied(ee.RowsCopied);
            //    };
            //}
            //bulkCopy.WriteToServer(args.Table, args.RowState);
        }

        /// <summary>
        /// 批量数据导入。
        /// </summary>
        /// <param name="args">参数。</param>
        public void BulkCopy(BulkCopyArguments args)
        {
            switch(this._Engine.Injector.Provider)
            {
                case DbEngineProvider.MicrosoftSqlServer:
                    this.BulkCopyMSSQL(args);
                    break;
                //case DbEngineProvider.Oracle:
                //    this.BulkCopyOracle(args);
                //    break;
                default:
                    throw new NotSupportedException("当前数据提供程序不支持批量加载功能。");
            }
        }

        /// <summary>
        /// 获取一个值，该值指示当前上下文的连接是否已关闭。
        /// </summary>
        public bool IsClosed
        {
            get
            {
                if(this.IsUnitTestTime) return false;
                return this._Connection == null || this._Connection.State == ConnectionState.Closed;
            }
        }

        /// <summary>
        /// 打开连接。在执行查询时，若数据源尚未打开则自动打开数据源。
        /// </summary>
        public DbContext Open()
        {
            if(!this.IsUnitTestTime && this._Connection.State == ConnectionState.Closed) this._Connection.Open();
            return this;
        }

        /// <summary>
        /// 启动数据源事务，并打开数据源连接。
        /// </summary>
        public DbContext OpenTransaction()
        {
            return this.OpenTransaction(IsolationLevel.Unspecified);
        }

        /// <summary>
        /// 指定事务的隔离级别，并打开数据源连接（如果没有打开）。
        /// </summary>
        /// <param name="isolationLevel">指定事务的隔离级别。。</param>
        public DbContext OpenTransaction(IsolationLevel isolationLevel)
        {
            if(!this.IsUnitTestTime)
            {
                this.Open();
                if(this._Transaction != null) throw new NotSupportedException("已有一个新的事务，在无法确定对旧的事务是否提交或回滚的情况下，系统抛出了异常。");
                this._Transaction = this._Connection.BeginTransaction(isolationLevel);
            }
            return this;
        }

        private Result TransactionHandler(Action action)
        {
            if(!this.IsUnitTestTime)
            {
                try
                {
                    action();
                }
                catch(Exception ex)
                {
                    return ex;
                }
                finally
                {
                    this._Transaction.Dispose();
                    this._Transaction = null;
                }
            }
            return Result.Successfully;
        }

        /// <summary>
        /// 提交数据源事务。
        /// </summary>
        /// <returns>返回操作的结果。</returns>
        public Result Commit()
        {
            if(this._Transaction == null) throw new NotSupportedException("并非以事务的方式打开连接。");
            return TransactionHandler(this._Transaction.Commit);
        }

        /// <summary>
        /// 从挂起状态回滚事务。
        /// </summary>
        /// <returns>返回操作的结果。</returns>
        public Result Rollback()
        {
            if(this._Transaction == null) throw new NotSupportedException("并非以事务的方式打开连接。");
            return TransactionHandler(this._Transaction.Rollback);
        }

        /// <summary>
        /// 关闭并释放数据源连接。
        /// </summary>
        public DbContext Close()
        {
            this.Dispose();
            return this;
        }

        /// <summary>
        /// 执行与释放或重置托管资源相关的应用程序定义的任务。
        /// </summary>
        protected override void DisposeManaged()
        {
            if(this.IsUnitTestTime) return;
            if(this._Connection != null) lock(this) if(this._Connection != null)
                    {
                        if(this._Connection.State != ConnectionState.Closed)
                            using(this._Connection)
                            {
                                if(this._Transaction != null) this._Transaction.Dispose();
                                this._Connection.TryClose();
                            }

                        this._Connection = null;
                        this._Transaction = null;
                        this._Engine.ResetContext();
                    }
        }

    }
}
