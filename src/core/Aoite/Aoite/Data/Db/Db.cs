using Aoite.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// 有关于数据的上下文。
    /// </summary>
    public static partial class Db
    {
        private readonly static object SyncObjectM = new object();
        private readonly static object SyncObjectE = new object();

        private static DbEngineManager _Manager;
        /// <summary>
        /// 设置或获取当前运行环境的数据源查询与交互引擎集合管理器。
        /// </summary>
        public static DbEngineManager Manager
        {
            get
            {
                if(_Manager == null)
                {
                    lock(SyncObjectM)
                    {
                        if(_Manager == null) _Manager = new DbEngineManager();
                    }
                }
                return _Manager;
            }
            set
            {
                //if(value == null) throw new ArgumentNullException("value");
                lock(SyncObjectM)
                {
                    _Manager = value;
                }
            }
        }

        const string DEFAULT_CONNECTION_NAME = "DB:";
        const string READONLY_CONNECTION_NAME = "DB_READONLY:";
        internal const string NameWithDefualtEngine = "Default";
        internal const string NameWithReadonlyEngine = "Readonly";

        private static bool _isInitialized = false;
        private static void InitializationEngines()
        {
            if(_isInitialized) return;
            lock(SyncObjectE)
            {
                if(_isInitialized) return;

                if(_Engine == null)
                {
                    foreach(ConnectionStringSettings conn in ConfigurationManager.ConnectionStrings)
                    {
                        if(!string.IsNullOrEmpty(conn.Name))
                        {
                            string connectionString = conn.ConnectionString;
                            string name = conn.Name;
                            int dbType;
                            if(name.StartsWith(DEFAULT_CONNECTION_NAME, StringComparison.OrdinalIgnoreCase))
                            {
                                name = name.Remove(0, DEFAULT_CONNECTION_NAME.Length);
                                dbType = 0;
                            }
                            else if(name.StartsWith(READONLY_CONNECTION_NAME, StringComparison.OrdinalIgnoreCase))
                            {
                                name = name.Remove(0, READONLY_CONNECTION_NAME.Length);
                                dbType = 2;
                            }
                            else continue;

                            var engine = DbEngine.Create(conn.ProviderName, connectionString);
                            if(engine == null) throw new SettingsPropertyWrongTypeException("无效的数据源提供程序。发生在“" + conn.Name + "”的 providerName 属性值为“" + conn.ProviderName + "”。");

                            engine._Name = name;
                            if(_Engine == null)
                            {
                                _Engine = engine;
                            }
                            if(dbType == 2)
                            {
                                engine.IsReadonly = true;
                                _Readonly = engine;
                            }
                            Manager.Add(engine.Name, engine);
                        }
                    }
                }

                _isInitialized = true;
            }
        }


        private static DbEngine _Engine;
        /// <summary>
        /// 获取当前运行环境的数据源查询与交互引擎的实例。
        /// </summary>
        public static DbEngine Engine
        {
            get
            {
                InitializationEngines();
                return _Engine ?? Manager[NameWithDefualtEngine];
            }
        }

        private static DbEngine _Readonly;
        /// <summary>
        /// 获取当前运行环境的只读数据源查询与交互引擎的实例。
        /// </summary>
        public static DbEngine Readonly
        {
            get
            {
                InitializationEngines();
                return _Readonly ?? Manager[NameWithReadonlyEngine];
            }
        }

        /// <summary>
        /// 获取一个值，指示当前上下文在线程中是否已创建。
        /// </summary>
        public static bool IsThreadContext
        {
            get
            {
                if(Engine == null) throw new ArgumentNullException("Db.Engine");
                return Engine.IsThreadContext;
            }
        }

        /// <summary>
        /// 创建并返回一个 <see cref="Aoite.Data.DbContext"/>。返回当前线程上下文包含的 <see cref="Aoite.Data.DbContext"/> 或创建一个新的  <see cref="Aoite.Data.DbContext"/>。
        /// <para>当释放一个 <see cref="Aoite.Data.DbContext"/> 后，将会重新创建。</para>
        /// </summary>
        public static DbContext Context
        {
            get
            {
                if(Engine == null) throw new ArgumentNullException("Db.Engine");
                return Engine.Context;
            }
        }

        /// <summary>
        /// 创建并返回一个事务性 <see cref="Aoite.Data.DbContext"/>。返回当前线程上下文包含的 <see cref="Aoite.Data.DbContext"/> 或创建一个新的  <see cref="Aoite.Data.DbContext"/>。
        /// <para>当释放一个 <see cref="Aoite.Data.DbContext"/> 后，将会重新创建。</para>
        /// </summary>
        public static DbContext ContextTransaction
        {
            get
            {
                if(Engine == null) throw new ArgumentNullException("Db.Engine");
                return Engine.ContextTransaction;
            }
        }

        /// <summary>
        /// 设置当前运行环境的数据源查询与交互引擎的实例。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        public static void SetEngine(DbEngine engine)
        {
            bool callMonitorExit = true;
            Threading.Monitor.Enter(SyncObjectE);
            try
            {
                _Engine = engine;
                Manager.Add(NameWithDefualtEngine, engine);
                if(_Readonly == null)
                {
                    callMonitorExit = false;
                    Threading.Monitor.Exit(SyncObjectE);
                    SetReadOnly(engine);
                }
            }
            finally
            {
                if(callMonitorExit) Threading.Monitor.Exit(SyncObjectE);
            }
        }

        /// <summary>
        /// 设置当前运行环境的数据源查询与交互引擎（只读数据引擎）。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        public static void SetReadOnly(DbEngine engine)
        {
            lock(SyncObjectE)
            {
                _Readonly = engine;
                Manager.Add(NameWithReadonlyEngine, engine);
            }
        }
    }

    //class DbUnitTest : DbEngine
    //{
    //    public readonly static DbUnitTest Instance = new DbUnitTest();
    //    private DbUnitTest():base("",) { }

    //    public override Data.Common.DbProviderFactory Factory
    //    {
    //        get { throw new NotImplementedException(); }
    //    }

    //    public override string DescribeParameter(Data.Common.DbParameter parameter)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

}
