using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Aoite.Data
{
    /// <summary>
    /// 表示一个数据源查询与交互引擎。
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{ConnectionString}")]
    public partial class DbEngine : IDbEngine
    {
        /// <summary>
        /// 指示在实体填充时，是否忽略没有找到的属性。默认为 true。当该值为 false 时，则找不到属性将会引发异常。
        /// </summary>
        public static bool IgnoreUnfoundEntityProperty = true;

        private string _ConnectionString;
        /// <summary>
        /// 获取或设置一个值，表示当前数据源的连接字符串。
        /// </summary>
        public string ConnectionString { get { return this._ConnectionString; } set { this._ConnectionString = value; } }

        private IDbInjector _Injector;
        /// <summary>
        /// 获取当前数据源查询与交互的注入器。
        /// </summary>
        public IDbInjector Injector { get { return _Injector; } }

        /// <summary>
        /// 设置或获取一个值，指示当前数据库是否为只读数据库。
        /// </summary>
        public bool IsReadonly { get; set; }

        private readonly System.Threading.ThreadLocal<DbContext> _threadLocalContent = new System.Threading.ThreadLocal<DbContext>();
        internal void ResetContext()
        {
            this._threadLocalContent.Value = null;
        }
        /// <summary>
        /// 获取一个值，指示当前上下文在线程中是否已创建。
        /// </summary>
        public bool IsThreadContext { get { return this._threadLocalContent.Value != null; } }

        /// <summary>
        /// 创建并返回一个 <see cref="Aoite.Data.DbContext"/>。返回当前线程上下文包含的 <see cref="Aoite.Data.DbContext"/> 或创建一个新的  <see cref="Aoite.Data.DbContext"/>。
        /// <para>当释放一个 <see cref="Aoite.Data.DbContext"/> 后，下一次调用获取将会重新创建上下文。</para>
        /// </summary>
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        public virtual DbContext Context
        {
            get
            {
                if(this._threadLocalContent.Value == null) this._threadLocalContent.Value = new DbContext(this);
                return this._threadLocalContent.Value;
            }
        }

        /// <summary>
        /// 创建并返回一个事务性 <see cref="Aoite.Data.DbContext"/>。返回当前线程上下文包含的 <see cref="Aoite.Data.DbContext"/> 或创建一个新的  <see cref="Aoite.Data.DbContext"/>。
        /// <para>当释放一个 <see cref="Aoite.Data.DbContext"/> 后，下一次调用获取将会重新创建上下文。</para>
        /// </summary>
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        public virtual DbContext ContextTransaction { get { return this.Context.OpenTransaction(); } }

        /// <summary>
        /// 提供数据源连接字符串和数据源查询与交互的注入器，初始化一个 <see cref="Aoite.Data.DbEngine"/> 类的新实例。
        /// </summary>
        /// <param name="connectionString">数据源连接字符串。</param>
        /// <param name="injector">数据源查询与交互的注入器。</param>
        public DbEngine(string connectionString, IDbInjector injector)
        {
            if(injector == null) throw new ArgumentNullException("injector");

            this._Injector = injector;
            this._ConnectionString = connectionString;
        }

        /// <summary>
        /// 测试数据源的连接。
        /// </summary>
        public virtual Result TestConnection()
        {
            DbConnection conn = null;
            try
            {
                using(conn = this._Injector.CreateConnection(this)) conn.Open();
            }
            catch(Exception ex)
            {
                return ex;
            }
            finally
            {
                conn.TryClose();
            }
            return Result.Successfully;
        }

        #region IDbEngine Members

        DbEngine IDbEngine.Owner { get { return this; } }

        /// <summary>
        /// 创建并返回一个到数据源的连接。
        /// </summary>
        /// <returns>返回一个到数据源的连接。</returns>
        public DbConnection CreateConnection() { return this._Injector.CreateConnection(this); }
        DbTransaction IDbEngine.CreateTransaction() { return null; }

        #endregion

        #region Manager

        /// <summary>
        /// 引擎的管理器。
        /// </summary>
        protected internal DbEngineManager _Manager;

        /// <summary>
        /// 引擎的唯一标识符。
        /// </summary>
        protected internal string _Name = Guid.NewGuid().ToString();

        /// <summary>
        /// 获取引擎的管理器。
        /// </summary>
        public DbEngineManager Manager { get { return this._Manager; } }

        /// <summary>
        /// 获取引擎的唯一标识符。
        /// </summary>
        public virtual string Name { get { return this._Name; } }

        #endregion

        #region Executing & Executed
        /// <summary>
        /// 在引擎执行命令时发生。
        /// </summary>
        public event ExecutingEventHandler Executing;
        /// <summary>
        /// 在引擎执行命令后发生。
        /// </summary>
        public event ExecutedEventHandler Executed;

        internal void InternalOnExecuting(IDbEngine engine, ExecuteType type, ExecuteCommand command)
        {
            this.OnExecuting(engine, type, command);
        }
        /// <summary>
        /// 表示 <see cref="Aoite.Data.DbEngine.Executing"/> 事件的处理方法。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="type">执行的类型。</param>
        /// <param name="command">执行的命令。</param>
        protected virtual void OnExecuting(IDbEngine engine, ExecuteType type, ExecuteCommand command)
        {
            if(this._Manager != null) this._Manager.InternalOnExecuting(engine, type, command);
            var handler = this.Executing;
            if(handler != null) handler(engine, command.GetEventArgs(type, null));
        }

        internal void InternalOnExecuted(IDbEngine engine, ExecuteType type, ExecuteCommand command, IDbResult result)
        {
            this.OnExecuted(engine, type, command, result);
        }
        /// <summary>
        /// 表示 <see cref="Aoite.Data.DbEngine.Executed"/> 事件的处理方法。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="type">执行的类型。</param>
        /// <param name="result">操作的返回值。</param>
        /// <param name="command">执行的命令。</param>
        protected virtual void OnExecuted(IDbEngine engine, ExecuteType type, ExecuteCommand command, IDbResult result)
        {
            if(this._Manager != null) this._Manager.InternalOnExecuted(engine, type, command, result);
            var handler = this.Executed;
            if(handler != null) handler(engine, command.GetEventArgs(type, result));
        }

        #endregion

        #region Static

        /// <summary>
        /// 获取数据源查询与交互引擎提供程序的字符串表示。
        /// </summary>
        /// <param name="provider">提供程序。</param>
        public static string GetProviderString(DbEngineProvider provider)
        {
            switch(provider)
            {
                case DbEngineProvider.MicrosoftSqlServer:
                    return Provider_Microsoft_SQL_Server;
                case DbEngineProvider.MicrosoftSqlServerCompact:
                    return Provider_Microsoft_SQL_Server_Compact;
                //case DbEngineProvider.MicrosoftOleDb2003:
                //    return Provider_Microsoft_OleDb2003;
                //case DbEngineProvider.MicrosoftOleDb2007:
                //    return Provider_Microsoft_OleDb2007;
                //case DbEngineProvider.SQLite:
                //    return Provider_SQLite;
                //case DbEngineProvider.Oracle:
                //    return Provider_Oracle;
                //case DbEngineProvider.MySql:
                //    return Provider_MySql;
            }
            return null;
        }

        /// <summary>
        /// 根据指定的提供程序创建一个数据源查询与交互引擎的实例。
        /// </summary>
        /// <param name="provider">提供程序。</param>
        /// <param name="connectionString">连接字符串。</param>
        public static DbEngine Create(DbEngineProvider provider, string connectionString)
        {
            switch(provider)
            {
                case DbEngineProvider.MicrosoftSqlServer:
                    return new MsSqlEngine(connectionString);
                case DbEngineProvider.MicrosoftSqlServerCompact:
                    return new MsSqlCeEngine(connectionString);
                //case DbEngineProvider.MicrosoftOleDb2003:
                //case DbEngineProvider.MicrosoftOleDb2007:
                //    return new OleDbEngine(provider, connectionString);
                //case DbEngineProvider.SQLite:
                //    return new SQLiteEngine(connectionString);
                //case DbEngineProvider.Oracle:
                //    return new OracleEngine(connectionString);
                //case DbEngineProvider.MySql:
                //    return new MySqlEngine(connectionString);
            }
            return null;
        }

        private const string Provider_Microsoft_SQL_Server_Simple = "sql";
        private const string Provider_Microsoft_SQL_Server = "mssql";
        private const string Provider_Microsoft_SQL_Server_Compact_Simple1 = "sqlce";
        private const string Provider_Microsoft_SQL_Server_Compact_Simple2 = "ce";
        private const string Provider_Microsoft_SQL_Server_Compact = "mssqlce";
        private const string Provider_Microsoft_OleDb2003 = "oledb2003";
        private const string Provider_Microsoft_OleDb2007 = "oledb2007";
        private const string Provider_SQLite = "sqlite";
        private const string Provider_Oracle = "oracle";
        private const string Provider_MySql = "mysql";

        /// <summary>
        /// 根据指定的提供程序创建一个数据源查询与交互引擎的实例。
        /// </summary>
        /// <param name="provider">提供程序。</param>
        /// <param name="connectionString">连接字符串。</param>
        public static DbEngine Create(string provider, string connectionString)
        {
            if(provider != null) provider = provider.ToLower();
            switch(provider)
            {
                case Provider_Microsoft_SQL_Server_Simple:
                case Provider_Microsoft_SQL_Server:
                    return new MsSqlEngine(connectionString);
                case Provider_Microsoft_SQL_Server_Compact_Simple1:
                case Provider_Microsoft_SQL_Server_Compact_Simple2:
                case Provider_Microsoft_SQL_Server_Compact:
                    return new MsSqlCeEngine(connectionString);
                //case Provider_Microsoft_OleDb2003:
                //    return new OleDbEngine(DbEngineProvider.MicrosoftOleDb2003, connectionString);
                //case Provider_Microsoft_OleDb2007:
                //    return new OleDbEngine(DbEngineProvider.MicrosoftOleDb2007, connectionString);
                //case Provider_SQLite:
                //    return new SQLiteEngine(connectionString);
                //case Provider_Oracle:
                //    return new OracleEngine(connectionString);
                //case Provider_MySql:
                //    return new MySqlEngine(connectionString);
            }
            return null;
        }

        #endregion
    }
}
