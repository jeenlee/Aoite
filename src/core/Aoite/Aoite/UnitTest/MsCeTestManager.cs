using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Aoite.Data
{
    /// <summary>
    /// 基于 Miscsoft SQL Server Compact 的数据源单元测试管理器。
    /// </summary>
    public class MsCeTestManager : TestManagerBase
    {
        private string _DatabasePath;
        /// <summary>
        /// 获取数据源路径。
        /// </summary>
        public string DatabasePath { get { return this._DatabasePath; } }

        /// <summary>
        /// 随机的数据源路径，初始化一个 <see cref="Aoite.Data.MsCeTestManager"/> 类的新实例。
        /// </summary>
        public MsCeTestManager() : this(Path.Combine(GA.FullPath("_databases"), Guid.NewGuid().ToString() + ".sdf")) { }

        /// <summary>
        /// 随机的数据源路径，初始化一个 <see cref="Aoite.Data.MsCeTestManager"/> 类的新实例。
        /// </summary>
        /// <param name="databasePath">数据源路径。</param>
        public MsCeTestManager(string databasePath)
        {
            if(string.IsNullOrEmpty(databasePath)) throw new ArgumentNullException("databasePath");
            this._DatabasePath = databasePath;
            GA.IO.CreateDirectory(Path.GetDirectoryName(databasePath));
            var engine = new MsSqlCeEngine(this._DatabasePath, null);
            engine.CreateDatabase();
            this.Engine = engine;
            this.Engine.Executing += _Engine_Executing;

            this.Manager = new DbEngineManager();
            this.Manager.Add(Db.NameWithDefualtEngine, this.Engine);
            this.Manager.Add(Db.NameWithReadonlyEngine, this.Engine);
        }

        void _Engine_Executing(object sender, ExecutingEventArgs e)
        {
            if(e.ExecuteType != ExecuteType.NoQuery) return;
            var command = e.Command.RuntimeObject as System.Data.Common.DbCommand;
            command.CommandText = ReplaceVarchar(command.CommandText);
        }

        static readonly Regex RegexReplaceVarchar = new Regex(@"\bvarchar\b", RegexOptions.Multiline | RegexOptions.Compiled);

        /// <summary>
        /// 由于 Miscsoft SQL Server Compact 不支持 varchar 数据类型，此方法将所有 varchar 转换为 nvarchar。
        /// </summary>
        /// <param name="sql">SQL 脚本。</param>
        /// <returns>返回一个新的 SQL 脚本。</returns>
        public string ReplaceVarchar(string sql)
        {
            return RegexReplaceVarchar.Replace(sql, "nvarchar");
        }

        /// <summary>
        /// 执行与释放或重置托管资源相关的应用程序定义的任务。
        /// </summary>
        protected override void DisposeManaged()
        {
            this.Engine = null;
            this.Manager = null;
            while(true)
            {
                try
                {
                    File.Delete(this._DatabasePath);
                    break;
                }
                catch(Exception)
                {
                    //GC.Collect();
                    //GC.WaitForPendingFinalizers();
                    System.Threading.Thread.Sleep(99);
                }
            }
            base.DisposeManaged();
        }
    }
}
