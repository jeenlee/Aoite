using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Data;
using System.Data.Common;

namespace Aoite.Data
{
    /// <summary>
    /// 表示一个单元测试预期的数据源引擎。
    /// </summary>
    public class ExpectedDbEngine : DbEngine
    {
        /// <summary>
        /// 初始化一个 <see cref="Aoite.Data.ExpectedDbEngine"/> 类的新实例。
        /// </summary>
        public ExpectedDbEngine() : base(string.Empty, ExpectedDbInjector.Instance) { }

        List<object> _results = new List<object>();
        int _resultsIndex = 0;
        /// <summary>
        /// 批量增加一系列的预期结果。
        /// </summary>
        /// <param name="results">一系列的预期结果。</param>
        /// <returns>返回当前实例。</returns>
        public ExpectedDbEngine TestRange(params object[] results)
        {
            this._results.AddRange(results);
            return this;
        }

        /// <summary>
        /// 定义预期一个结果。
        /// </summary>
        /// <typeparam name="T">返回值的数据类型。</typeparam>
        /// <param name="callback">回调方法。</param>
        /// <returns>返回当前实例。</returns>
        public ExpectedDbEngine Test<T>(Func<DbCommand, T> callback)
        {
            this._results.Add(new Func<DbCommand, object>(db => callback(db)));
            return this;
        }

        /// <summary>
        /// 定义预期一个结果。
        /// </summary>
        /// <param name="result">预期结果。</param>
        /// <returns>返回当前实例。</returns>
        public ExpectedDbEngine Test(object result)
        {
            this._results.Add(result);
            return this;
        }

        /// <summary>
        /// 弹出一个最早预期的结果。
        /// </summary>
        /// <typeparam name="T">结果的数据类型。</typeparam>
        /// <param name="command">数据源命令。</param>
        /// <returns>返回一个结果。</returns>
        public T Pop<T>(DbCommand command)
        {
            object o = this._results[this._resultsIndex++];
            if(o is Func<DbCommand, object>) o = ((Func<DbCommand, object>)o)(command);
            try
            {
                return (T)o;
            }
            catch(Exception ex)
            {
                throw new InvalidCastException("无法将预期类型 {0} 转换为实际类型 {1}。".Fmt(typeof(T).Name, o == null ? "<NULL>" : o.GetType().Name), ex);
            }
        }
        /// <summary>
        /// 重置预期的排序（重新开始）。
        /// </summary>
        /// <returns>返回当前实例。</returns>
        public ExpectedDbEngine Retest()
        {
            this._resultsIndex = 0;
            return this;
        }
        /// <summary>
        /// 清空加入预期队列的结果。
        /// </summary>
        /// <returns>返回当前实例。</returns>
        public ExpectedDbEngine Clear()
        {
            this._results.Clear();
            return this;
        }
    }
 
 
    class ExpectedDbInjector : SqlDbInjector
    {
        new public static readonly ExpectedDbInjector Instance = new ExpectedDbInjector();

        public override IDbExecutor CreateExecutor(IDbEngine engine, ExecuteCommand command)
        {
            return new ExpectedDbExecutor(engine, command);
        }
    }

    class ExpectedDbExecutor : DefaultDbExecutor
    {
        public ExpectedDbExecutor(IDbEngine engine, ExecuteCommand command) : base(engine, command) { }

        private T Dequeue<T>(System.Data.Common.DbCommand command) { return (this.Engine.Owner as ExpectedDbEngine).Pop<T>(command); }

        protected override void Open() { }
        protected override void Close() { }

        public override TableResult ToTable()
        {
            return this.Execute<TableResult, AoiteTable>(ExecuteType.Table, this.Dequeue<AoiteTable>);
        }

        public override TableResult ToTable(int pageNumber, int pageSize = 10)
        {
            return this.ToTable();
        }

        public override DbResult<TEntity> ToEntity<TEntity>()
        {
            return this.Execute<DbResult<TEntity>, TEntity>(ExecuteType.Reader, this.Dequeue<TEntity>);
        }

        public override DbResult<List<TEntity>> ToEntities<TEntity>()
        {
            return this.Execute<DbResult<List<TEntity>>, List<TEntity>>(ExecuteType.Reader, this.Dequeue<List<TEntity>>);
        }

        public override DbResult<GridData<TEntity>> ToEntities<TEntity>(int pageNumber, int pageSize = 10)
        {
            return this.Execute<DbResult<GridData<TEntity>>, GridData<TEntity>>(ExecuteType.Reader, this.Dequeue<GridData<TEntity>>);
        }

        public override DbResult<int> ToNonQuery()
        {
            return this.Execute<DbResult<int>, int>(ExecuteType.NoQuery, this.Dequeue<int>);
        }

        public override DbResult<TValue> ToReader<TValue>(ExecuteReaderHandler<TValue> callback)
        {
            return this.Execute<DbResult<TValue>, TValue>(ExecuteType.Reader, dbCommand => callback(this.Dequeue<DataTable>(dbCommand).CreateDataReader()));
        }

        public override DbResult ToReader(ExecuteReaderHandler callback)
        {
            return this.Execute<DbResult, VoidValue>(ExecuteType.Reader, dbCommand =>
            {
                callback(this.Dequeue<DataTable>(dbCommand).CreateDataReader());
                return default(VoidValue);
            });
        }

        public override DbResult<TValue> ToScalar<TValue>()
        {
            return this.Execute<DbResult<TValue>, TValue>(ExecuteType.Scalar, this.Dequeue<TValue>);
        }

        protected override TDataSetResult ToDataSet<TDataSetResult, TDataSet>()
        {
            return this.Execute<TDataSetResult, TDataSet>(ExecuteType.DataSet, this.Dequeue<TDataSet>);
        }
    }

    /// <summary>
    /// 表示一个内存数据库。
    /// </summary>
    public class MemoryDatabase : ObjectDisposableBase
    {
        /// <summary>
        /// 初始化一个 <see cref="Aoite.Data.MemoryDatabase"/> 类的新实例。
        /// </summary>
        public MemoryDatabase() { }

        ConcurrentDictionary<Type, IMemoryTable> _tables = new ConcurrentDictionary<Type, IMemoryTable>();

        /// <summary>
        /// 获取指定实体类型的内存数据表。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        /// <returns>返回一个内存数据表。</returns>
        public IMemoryTable<TEntity> GetTable<TEntity>()
        {
            this.ThrowWhenDisposed();
            return _tables.GetOrAdd(typeof(TEntity), k => new MemoryTable<TEntity>(this)) as IMemoryTable<TEntity>;
        }


        /// <summary>
        /// 执行与释放或重置托管资源相关的应用程序定义的任务。
        /// </summary>
        protected override void DisposeManaged()
        {
            this._tables.Clear();
            this._tables = null;
        }
    }

    /// <summary>
    /// 表示一张内存的数据表。
    /// </summary>
    public interface IMemoryTable { }
    /// <summary>
    /// 表示一张强类型的内存的数据表。
    /// </summary>
    /// <typeparam name="TEntity">实体的数据类型。</typeparam>
    public interface IMemoryTable<TEntity> : IMemoryTable, IEnumerable<TEntity>
    {
        /// <summary>
        /// 获取表名。
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 获取所属的数据库。
        /// </summary>
        MemoryDatabase Database { get; }
        /// <summary>
        /// 获取总行数。
        /// </summary>
        int Count { get; }
        /// <summary>
        /// 查找指定主键的一条数据。
        /// </summary>
        /// <param name="key">主键。</param>
        /// <returns>返回一条数据。</returns>
        TEntity FindOne(object key);
        /// <summary>
        /// 插入数据。
        /// </summary>
        /// <param name="entity">实体。</param>
        /// <returns>返回受影响的行数。</returns>
        int Insert(TEntity entity);
        /// <summary>
        /// 更新数据。
        /// </summary>
        /// <param name="entity">实体。</param>
        /// <returns>返回受影响的行数。</returns>
        int Update(object entity);
        /// <summary>
        /// 删除数据。
        /// </summary>
        /// <param name="key">主键。</param>
        /// <returns>返回受影响的行数。</returns>
        int Delete(object key);
        /// <summary>
        /// 查找指定主键的一条数据。
        /// </summary>
        /// <param name="command">数据源命令。</param>
        /// <returns>返回一条数据。</returns>
        TEntity FindOne(DbCommand command);
        /// <summary>
        /// 插入数据。
        /// </summary>
        /// <param name="command">数据源命令。</param>
        /// <returns>返回受影响的行数。</returns>
        int Insert(DbCommand command);
        /// <summary>
        /// 更新数据。
        /// </summary>
        /// <param name="command">数据源命令。</param>
        /// <returns>返回受影响的行数。</returns>
        int Update(DbCommand command);
        /// <summary>
        /// 判断指定列的值是否已存在。
        /// </summary>
        /// <param name="name">列名。</param>
        /// <param name="value">列值。</param>
        /// <returns>存在返回 true，否则返回 false。</returns>
        bool Exists(string name, object value);
        /// <summary>
        /// 判断指定列的值是否已存在。
        /// </summary>
        /// <param name="command">数据源命令。</param>
        /// <returns>存在返回 true，否则返回 false。</returns>
        bool Exists(DbCommand command);
    }
    internal class MemoryTable<TEntity> : IMemoryTable<TEntity>
    {
        ConcurrentDictionary<string, TEntity> _table = new ConcurrentDictionary<string, TEntity>();

        public string Name { get { return EntityMapper.Instance<EntityMapper>.Mapper.Name; } }

        public MemoryTable(MemoryDatabase database)
        {
            this._Database = database;
        }

        private MemoryDatabase _Database;
        public MemoryDatabase Database { get { return _Database; } }


        public int Count { get { return this._table.Count; } }
        private string GetKey(object entity)
        {
            if(entity == null) throw new ArgumentNullException("entity");
            object key;
            if(entity is Dictionary<string, object>)
            {
                var dict = entity as Dictionary<string, object>;
                key = (from p in EntityMapper.Instance<TEntity>.Mapper.Properties
                       where p.Column.IsPrimaryKey
                       select dict[p.Name]).FirstOrDefault();
            }
            else
            {
                key = (from p in EntityMapper.Instance<TEntity>.Mapper.Properties
                       where p.Column.IsPrimaryKey
                       select p.GetValue(entity)).FirstOrDefault();
            }
            if(key == null) throw new ArgumentNullException("entity", "类型 {0} 不存在主键".Fmt(typeof(TEntity).Name));
            return string.Intern(Convert.ToString(key));
        }

        public bool Exists(string name, object value)
        {
            var p = EntityMapper.Instance<TEntity>.Mapper[name];
            if(p == null) throw new NotSupportedException("{0} 找不到属性".Fmt(name));
            foreach(var item in _table.Values)
            {
                if(object.Equals(p.GetValue(item), value)) return true;
            }
            return false;
        }
        public bool Exists(DbCommand command)
        {
            var p = command.Parameters[0];
            if(p == null) throw new ArgumentNullException("command.Parameters");

            return Exists(p.ParameterName.RemoveStart(), p.Value);
        }

        public int Insert(DbCommand command)
        {
            var mp = EntityMapper.Instance<TEntity>.Mapper;
            TEntity entity = Activator.CreateInstance<TEntity>();
            foreach(var p in mp.Properties)
            {
                var pName = "@" + p.Name;
                if(command.Parameters.Contains(pName))
                    p.SetValue(entity, command.Parameters[pName].Value);
            }
            return this.Insert(entity);
        }

        public int Update(DbCommand command)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>(StringComparer.CurrentCultureIgnoreCase);
            foreach(DbParameter p in command.Parameters)
            {
                dict.Add(p.ParameterName.RemoveStart(), p.Value);
            }
            return this.Update(dict);
        }

        public int Insert(TEntity entity)
        {
            var key = GetKey(entity);

            if(!this._table.TryAdd(key, entity.CopyTo<TEntity>()))
                throw new NotSupportedException("主键已存在，插入失败。");
            return 1;
        }

        public int Update(object entity)
        {
            var key = GetKey(entity);

            lock(key)
            {
                TEntity oldEntity;
                if(this._table.TryGetValue(key, out oldEntity))
                {
                    var mp1 = EntityMapper.Instance<TEntity>.Mapper;
                    if(entity is Dictionary<string, object>)
                    {
                        foreach(var item in entity as Dictionary<string, object>)
                        {
                            var p1 = mp1[item.Key];
                            if(p1 == null) throw new NotSupportedException("字段 {0} 不存在，更新失败。".Fmt(item.Key));
                            p1.SetValue(oldEntity, item.Value);
                        }
                        return 1;
                    }

                    var mp2 = EntityMapper.Create(entity.GetType());
                    foreach(var p2 in mp2.Properties)
                    {
                        var p1 = mp1[p2.Name];
                        if(p1 == null) throw new NotSupportedException("字段 {0} 不存在，更新失败。".Fmt(p2.Name));
                        p1.SetValue(oldEntity, p2.GetValue(entity));
                    }
                    return 1;
                }
            }
            return 0;
        }

        public int Delete(object key)
        {
            if(key == null) throw new ArgumentNullException("key");

            TEntity entity;
            if(!this._table.TryRemove(Convert.ToString(key), out entity)) return 0;
            return 1;
        }

        public TEntity FindOne(DbCommand command)
        {
            return FindOne(command.Parameters[0].Value);
        }
        public TEntity FindOne(object key)
        {
            if(key == null) throw new ArgumentNullException("key");
            var entity = this._table.TryGetValue(Convert.ToString(key));
            return entity;
        }

        IEnumerator<TEntity> IEnumerable<TEntity>.GetEnumerator()
        {
            return this._table.Values.ToList().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this._table.Values.ToList().GetEnumerator();
        }


    }
}
