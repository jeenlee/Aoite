using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlServerCe;
using System.Linq;
using System.Text;

namespace Aoite.Data
{
    /// <summary>
    /// 基于 Microsoft SQL Server Compact 查询与交互的注入器。
    /// </summary>
    public class SqlCeDbInjector : DbInjectorBase
    {
        /// <summary>
        /// 获取注入器的实例。
        /// </summary>
        public readonly static SqlCeDbInjector Instance = new SqlCeDbInjector();

        /// <summary>
        /// 获取数据源的提供程序。
        /// </summary>
        public override DbProviderFactory Factory { get { return SqlCeProviderFactory.Instance; } }

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
            var p = parameter as SqlCeParameter;
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
        public override PaginationBase CreatePagination(IDbEngine engine)
        {
            return MsSqlCePagination.Instance;
        }
    }

    /// <summary>
    /// 基于 Microsoft SQL Server Compact 查询与交互的操作引擎。
    /// </summary>
    public class MsSqlCeEngine : DbEngine
    {
        /// <summary>
        /// 初始化一个 <see cref="Aoite.Data.MsSqlCeEngine"/> 类的新实例。
        /// </summary>
        /// <param name="connectionString">连接字符串。</param>
        public MsSqlCeEngine(string connectionString)
            : base(connectionString, SqlCeDbInjector.Instance) { }

        /// <summary>
        /// 提供数据源和密码，初始化一个 <see cref="Aoite.Data.MsSqlCeEngine"/> 类的新实例。
        /// </summary>
        /// <param name="datasource">SQL Server Compact 数据源的文件路径和名称。</param>
        /// <param name="password">数据源密码，最多包含 40 个字符。</param>
        public MsSqlCeEngine(string datasource, string password)
            : this(string.Format("Persist Security Info=False;Data Source='{0}';password='{1}'", datasource, password)) { }

        /// <summary>
        /// 创建新数据源。
        /// </summary>
        public void CreateDatabase()
        {
            using(SqlCeEngine engine = new SqlCeEngine(this.ConnectionString))
            {
                engine.CreateDatabase();
            }
        }
    }
}
