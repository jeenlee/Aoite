using System;
using System.Collections.Generic;
using System.Text;

namespace Aoite.Data
{
    internal abstract class CommandBuilderBase : IBuilder
    {
        internal IDbEngine _engine;

        public abstract ExecuteParameterCollection Parameters { get; }
        public abstract string CommandText { get; }

        internal CommandBuilderBase(IDbEngine engine)
        {
            this._engine = engine;
        }

        public IDbExecutor Execute()
        {
            return this._engine.Execute(this.End());
        }
        public abstract ExecuteCommand End();


        public abstract ISelect OrderBy(params string[] fields);

        public abstract ISelect GroupBy(params string[] fields);



        IDbEngine IDbExecutor.Engine
        {
            get { return this._engine; }
        }

        ExecuteCommand IDbExecutor.Command
        {
            get { return this.End(); }
        }

        DataSetResult<System.Data.DataSet> IDbExecutor.ToDataSet()
        {
            return this.Execute().ToDataSet();
        }

        DataSetResult<TDataSet> IDbExecutor.ToDataSet<TDataSet>()
        {
            return this.Execute().ToDataSet<TDataSet>();
        }

        DbResult<List<TEntity>> IDbExecutor.ToEntities<TEntity>()
        {
            return this.Execute().ToEntities<TEntity>();
        }

        DbResult<GridData<TEntity>> IDbExecutor.ToEntities<TEntity>(IPagination page)
        {
            return this.Execute().ToEntities<TEntity>(page);
        }

        DbResult<GridData<TEntity>> IDbExecutor.ToEntities<TEntity>(int pageNumber, int pageSize)
        {
            return this.Execute().ToEntities<TEntity>(pageNumber, pageSize);
        }

        DbResult<TEntity> IDbExecutor.ToEntity<TEntity>()
        {
            return this.Execute().ToEntity<TEntity>();
        }

        DbResult<List<dynamic>> IDbExecutor.ToEntities()
        {
            return this.Execute().ToEntities();
        }

        DbResult<GridData<dynamic>> IDbExecutor.ToEntities(IPagination page)
        {
            return this.Execute().ToEntities(page);
        }

        DbResult<GridData<dynamic>> IDbExecutor.ToEntities(int pageNumber, int pageSize)
        {
            return this.Execute().ToEntities(pageNumber, pageSize);
        }

        DbResult<dynamic> IDbExecutor.ToEntity()
        {
            return this.Execute().ToEntity();
        }

        DbResult<int> IDbExecutor.ToNonQuery()
        {
            return this.Execute().ToNonQuery();
        }

        DbResult IDbExecutor.ToReader(ExecuteReaderHandler callback)
        {
            return this.Execute().ToReader(callback);
        }

        DbResult<TValue> IDbExecutor.ToReader<TValue>(ExecuteReaderHandler<TValue> callback)
        {
            return this.Execute().ToReader<TValue>(callback);
        }

        DbResult<object> IDbExecutor.ToScalar()
        {
            return this.Execute().ToScalar();
        }

        DbResult<TValue> IDbExecutor.ToScalar<TValue>()
        {
            return this.Execute().ToScalar<TValue>();
        }

        TableResult IDbExecutor.ToTable()
        {
            return this.Execute().ToTable();
        }

        TableResult IDbExecutor.ToTable(IPagination page)
        {
            return this.Execute().ToTable(page);
        }

        TableResult IDbExecutor.ToTable(int pageNumber, int pageSize)
        {
            return this.Execute().ToTable(pageNumber, pageSize);
        }
    }

}
