using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Aoite.Data
{

    /// <summary>
    /// 基于 Microsoft SQL Server 查询与交互的注入器。
    /// </summary>
    public class SqlDbInjector : DbInjectorBase
    {
        /// <summary>
        /// 获取注入器的实例。
        /// </summary>
        public readonly static SqlDbInjector Instance = new SqlDbInjector();
        /// <summary>
        /// 获取数据源的提供程序。
        /// </summary>
        public override DbProviderFactory Factory { get { return SqlClientFactory.Instance; } }

        /// <summary>
        /// 获取一个值，表示当前数据操作的数据提供程序的类型。
        /// </summary>
        public override DbEngineProvider Provider { get { return DbEngineProvider.MicrosoftSqlServer; } }

        /// <summary>
        /// 描述指定的 <see cref="System.Data.Common.DbParameter"/>。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <param name="parameter">一个 <see cref="System.Data.Common.DbParameter"/> 的实例。</param>
        /// <returns>返回参数描述的 <see cref="System.String"/> 值。</returns>
        public override string DescribeParameter(IDbEngine engine, DbParameter parameter)
        {
            var p = parameter as SqlParameter;
            string desc = p.ParameterName;

            switch(p.Direction)
            {
                case ParameterDirection.InputOutput:
                    desc += " IN OUT";
                    break;
                case ParameterDirection.Output:
                    desc += " OUT";
                    break;
            }
            desc += " " + p.SqlDbType;
            switch(p.SqlDbType)
            {
                case SqlDbType.Char:
                case SqlDbType.NChar:
                case SqlDbType.NVarChar:
                case SqlDbType.Decimal:
                case SqlDbType.Money:
                case SqlDbType.Float:
                case SqlDbType.VarChar:
                case SqlDbType.Binary:
                case SqlDbType.VarBinary:
                    desc += "(" + p.Size + ")";
                    break;
            }
            return desc;
        }

        /// <summary>
        /// 创建一个分页组件。
        /// </summary>
        /// <param name="engine">数据源查询与交互引擎的实例。</param>
        /// <returns>返回一个分页组件。</returns>
        public override PaginationBase CreatePagination(IDbEngine engine) { return MsSqlPagination.Instance; }

        internal override void OnPropertyInsertMapper(IDbEngine engine
             , PropertyMapper property
             , ref int index
             , StringBuilder fieldsBuilder
             , StringBuilder valueBuilder
             , ExecuteParameterCollection ps
             , DefaultParameterSettings parSettings
             , object entity)
        {
            if(property.Column.IsPrimaryKey
                && object.Equals(property.GetValue(entity), property.TypeDefaultValue))
                return;
            base.OnPropertyInsertMapper(engine, property, ref index, fieldsBuilder, valueBuilder, ps, parSettings, entity);
        }
    }

    /// <summary>
    /// 基于 Microsoft SQL Server 查询与交互的操作引擎。
    /// </summary>
    public class MsSqlEngine : DbEngine
    {
        const string IntegratedSecurityFormat = "Data Source={0};Initial Catalog={1};Integrated Security=True;Connect Timeout={2};";
        const string UserPasswordFormat = "Data Source={0};Initial Catalog={1};User ID={2};Password={3};Connect Timeout={4};";

        /// <summary>
        /// 初始化一个 <see cref="Aoite.Data.MsSqlEngine"/> 类的新实例。
        /// </summary>
        /// <param name="connectionString">连接字符串。</param>
        public MsSqlEngine(string connectionString) : base(connectionString, SqlDbInjector.Instance) { }

        /// <summary>
        /// 初始化一个 <see cref="Aoite.Data.MsSqlEngine"/> 类的新实例。
        /// </summary>
        /// <param name="dataSource">数据源。</param>
        /// <param name="initialCatalog">数据源。</param>
        public MsSqlEngine(string dataSource, string initialCatalog) : this(dataSource, initialCatalog, 15) { }

        /// <summary>
        /// 初始化一个 <see cref="Aoite.Data.MsSqlEngine"/> 类的新实例。
        /// </summary>
        /// <param name="dataSource">数据源。</param>
        /// <param name="initialCatalog">数据源。</param>
        /// <param name="connectTimeout">指示连接超时时限。</param>
        public MsSqlEngine(string dataSource, string initialCatalog, int connectTimeout)
            : this(string.Format(IntegratedSecurityFormat, dataSource, initialCatalog, connectTimeout)) { }

        /// <summary>
        /// 初始化一个 <see cref="Aoite.Data.MsSqlEngine"/> 类的新实例。
        /// </summary>
        /// <param name="dataSource">数据源。</param>
        /// <param name="initialCatalog">数据源。</param>
        /// <param name="userId">登录账户。</param>
        /// <param name="passwrod">登录密码。</param>
        public MsSqlEngine(string dataSource, string initialCatalog, string userId, string passwrod)
            : this(dataSource, initialCatalog, userId, passwrod, 15) { }

        /// <summary>
        /// 初始化一个 <see cref="Aoite.Data.MsSqlEngine"/> 类的新实例。
        /// </summary>
        /// <param name="dataSource">数据源。</param>
        /// <param name="initialCatalog">数据源。</param>
        /// <param name="userId">登录账户。</param>
        /// <param name="passwrod">登录密码。</param>
        /// <param name="connectTimeout">指示连接超时时限。</param>
        public MsSqlEngine(string dataSource, string initialCatalog, string userId, string passwrod, int connectTimeout)
            : this(string.Format(UserPasswordFormat, dataSource, initialCatalog, userId, passwrod, connectTimeout)) { }

        #region Methods

        /// <summary>
        /// 将当前的数据源备份到指定路径。
        /// <para>说明：该备份模式为 完整备份。</para>
        /// </summary>
        /// <param name="filename">备份文件路径。</param>
        public DbResult<int> BackupFile(string filename)
        {
            return this.BackupFile(this.CreateConnection().Database, filename);
        }

        /// <summary>
        /// 将给定的数据源备份到指定路径。
        /// <para>说明：该备份模式为 完整备份。</para>
        /// </summary>
        /// <param name="dbname">需要备份的数据源。</param>
        /// <param name="filename">备份文件路径。</param>
        public DbResult<int> BackupFile(string dbname, string filename)
        {
            return this.Execute("BACKUP DATABASE @database TO DISK = @filename", "@database", dbname, "@filename", filename)
                       .ToNonQuery();
        }

        /// <summary>
        /// 立即将给定的数据源切换至脱机模式。
        /// <para>说明：脱机后该数据源将无法访问。</para>
        /// </summary>
        /// <param name="dbname">将要切换至脱机模式的数据源。</param>
        public DbResult<int> OffineMode(string dbname)
        {
            // 也可以使用 sp_dboption 存储过程
            return this.Execute("ALTER DATABASE " + dbname + " SET OFFLINE WITH ROLLBACK IMMEDIATE;")
                       .ToNonQuery();
        }

        /// <summary>
        /// 将给定的备份文件替换到当前数据源。
        /// <para>说明：执行此方法前，必须将当前连接字符串的默认为 master 数据源，并且至少执行 <see cref="Aoite.Data.MsSqlEngine.OffineMode"/> 方法或 <see cref="Aoite.Data.MsSqlEngine.SingeUserMode"/> 方法。</para>
        /// </summary>
        /// <param name="filename">备份文件路径。</param>
        public DbResult<int> RestoreReplaceFile(string filename)
        {
            return this.RestoreReplaceFile(this.CreateConnection().Database, filename);
        }

        /// <summary>
        /// 将给定的备份文件替换到给定的数据源。
        /// <para>说明：执行此方法前，必须将当前连接字符串的默认为 master 数据源。并执行 <see cref="Aoite.Data.MsSqlEngine.OffineMode"/> 方法将数据源脱机。</para>
        /// </summary>
        /// <param name="dbname">需要恢复的数据源。</param>
        /// <param name="filename">备份文件路径。</param>
        public DbResult<int> RestoreReplaceFile(string dbname, string filename)
        {
            //RESTORE DATABASE 后面无法跟参数。
            return this.Execute("RESTORE DATABASE " + dbname + " FROM DISK=@filename WITH REPLACE;", "@filename", filename)
                       .ToNonQuery();
        }

        /// <summary>
        /// 立即将给定的数据源切换至单用户模式。
        /// <para>说明：单用户模式后，数据源将拒绝其他任何用户访问数据源。</para>
        /// </summary>
        /// <param name="dbname">将要切换至单用户模式的数据源。</param>
        public DbResult<int> SingeUserMode(string dbname)
        {
            return this.Execute("ALTER DATABASE " + dbname + " SET SINGLE_USER WITH ROLLBACK IMMEDIATE;")
                       .ToNonQuery();
        }

        #endregion Methods
    }
}
