using Aoite.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Data
{
    /// <summary>
    /// 基于 Miscsoft SQL Server 的数据源单元测试管理器。
    /// </summary>
    public class MsSqlTestManager : TestManagerBase
    {
        private MsSqlEngine m_masterEngine;

        /// <summary>
        /// 获取数据源的名称。
        /// </summary>
        public string DatabaseName { get; private set; }
        /// <summary>
        /// 获取可读写的用户名称。
        /// </summary>
        public string DbOwnerUser { get; private set; }
        /// <summary>
        /// 获取只读的用户名称。
        /// </summary>
        public string ReadonlyUser { get; private set; }

        /// <summary>
        /// 提供数据库地址，初始化一个 <see cref="Aoite.Data.MsSqlTestManager"/> 类的新实例。
        /// </summary>
        /// <param name="server">数据库地址。</param>
        public MsSqlTestManager(string server = "localhost")
            : this("Data Source={0};Initial Catalog=master;Integrated Security=True;".Fmt(server), server) { }

        /// <summary>
        /// 提供数据源连接字符串，初始化一个 <see cref="Aoite.Data.MsSqlTestManager"/> 类的新实例。
        /// </summary>
        /// <param name="connectionString">数据源连接字符串。</param>
        /// <param name="server">服务器地址。</param>
        public MsSqlTestManager(string connectionString, string server)
        {
            if(string.IsNullOrEmpty(connectionString)) throw new ArgumentNullException("connectionString");
            if(string.IsNullOrEmpty(server)) throw new ArgumentNullException("server");

            m_masterEngine = new MsSqlEngine(connectionString);
            var dbDire = GA.FullPath("_databases");
            GA.IO.CreateDirectory(dbDire);
            this.DatabaseName = Guid.NewGuid().ToString();
            this.DbOwnerUser = Guid.NewGuid().ToString();
            this.ReadonlyUser = Guid.NewGuid().ToString();
            var sql = @"CREATE DATABASE [{0}]
 ON  PRIMARY ( NAME = N'{0}', FILENAME = N'{1}\{0}.mdf' , SIZE = 5120KB , FILEGROWTH = 1024KB )
 LOG ON ( NAME = N'{0}_log', FILENAME = N'{1}\{0}_log.ldf' , SIZE = 2048KB , FILEGROWTH = 10%)";
            using(var context = m_masterEngine.Context)
            {
                context.Open();
                context
                    .Execute(sql.Fmt(this.DatabaseName, dbDire))
                    .ToNonQuery()
                    .ThrowIfFailded();
                CreateUser(this.DatabaseName, this.DbOwnerUser, false, context);
                CreateUser(this.DatabaseName, this.ReadonlyUser, true, context);
            }

            this.Manager = new DbEngineManager();
            this.Manager.Add(Db.NameWithDefualtEngine, this.Engine = new MsSqlEngine(server, this.DatabaseName, this.DbOwnerUser, "123456"));
            this.Manager.Add(Db.NameWithReadonlyEngine, new MsSqlEngine(server, this.DatabaseName, this.ReadonlyUser, "123456"));
        }

        private void CreateUser(string database, string loginId, bool isReadonly, DbContext context)
        {
            context.ContextConnection.ChangeDatabase("master");
            context.Execute("CREATE LOGIN [{1}] WITH PASSWORD=N'123456', DEFAULT_DATABASE=[{0}], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF"
                .Fmt(database, loginId))
                .ToNonQuery()
                .ThrowIfFailded();

            context.ContextConnection.ChangeDatabase(database);
            context.Execute("CREATE USER [{0}] FOR LOGIN [{0}]".Fmt(loginId))
                .ToNonQuery()
                .ThrowIfFailded();
            if(isReadonly)
            {
                context.Execute("EXEC sp_addrolemember N'db_datareader', N'{0}'".Fmt(loginId))
                    .ToNonQuery()
                    .ThrowIfFailded();
            }
            else
            {
                context.Execute("EXEC sp_addrolemember N'db_owner', N'{0}'".Fmt(loginId))
                    .ToNonQuery()
                    .ThrowIfFailded();
            }
            context.ContextConnection.ChangeDatabase("master");
        }

        /// <summary>
        /// 执行与释放或重置托管资源相关的应用程序定义的任务。
        /// </summary>
        protected override void DisposeManaged()
        {
            using(var context = this.m_masterEngine.Context)
            {
                context.Open();
                context.ContextConnection.ChangeDatabase("master");
                context.Execute("ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE".Fmt(this.DatabaseName)).ToNonQuery().ThrowIfFailded();
                context.Execute("DROP DATABASE [{0}]".Fmt(this.DatabaseName)).ToNonQuery().ThrowIfFailded();
                context.Execute("DROP LOGIN [{0}]".Fmt(this.DbOwnerUser)).ToNonQuery().ThrowIfFailded();
                context.Execute("DROP LOGIN [{0}]".Fmt(this.ReadonlyUser)).ToNonQuery().ThrowIfFailded();
            }
            this.m_masterEngine = null;

            base.DisposeManaged();
        }
    }
}
