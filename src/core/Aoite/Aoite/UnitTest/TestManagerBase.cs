using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Aoite.Data
{
    /// <summary>
    /// 表示数据源测试管理器。
    /// </summary>
    public abstract class TestManagerBase : ObjectDisposableBase
    {
        static TestManagerBase()
        {
            Scripts.ParseFolder(GA.FullPath("_scripts"));
            Scripts.ParseFolder(GA.FullPath("_sql"));
        }


        static SqlScriptsManager Scripts = new SqlScriptsManager();

        private DbEngine _Engine;
        /// <summary>
        /// 获取数据源引擎。
        /// </summary>
        public DbEngine Engine { get { return this._Engine; } protected set { this._Engine = value; } }


        private DbEngineManager _Manager;
        /// <summary>
        /// 获取数据源引擎管理器。
        /// </summary>
        public DbEngineManager Manager { get { return this._Manager; } protected set { this._Manager = value; } }

        /// <summary>
        /// 获取或设置契约的测试用例。
        /// </summary>
        public Aoite.ServiceModel.ContractTester Contract { get; set; }

        /// <summary>
        /// 创建指定契约类型的代理实例。
        /// </summary>
        /// <typeparam name="TContract">契约的数据类型。</typeparam>
        /// <param name="keepAlive">指示是否为一个持久连接的契约客户端。</param>
        /// <returns>返回一个代理实例。</returns>
        public TContract Create<TContract>(bool keepAlive = false)
        {
            return this.Contract.Create<TContract>(keepAlive);
        }

        /// <summary>
        /// 执行指定键名的脚本。
        /// </summary>
        /// <typeparam name="TKey">键的数据类型。</typeparam>
        /// <param name="key">脚本的键名。</param>
        /// <returns>返回当前实例。</returns>
        public TestManagerBase Execute<TKey>(TKey key)
        {
            this.Engine
                .Execute(Scripts[Convert.ToString(key)])
                .ToNonQuery()
                .ThrowIfFailded();
            return this;
        }

        /// <summary>
        /// 批量执行指定键名的脚本。
        /// </summary>
        /// <param name="keys">脚本的键名列表。</param>
        /// <returns>返回当前实例。</returns>
        public TestManagerBase Execute(params string[] keys)
        {
            foreach(var key in keys)
            {
                this.Execute<string>(key);
            }
            return this;
        }

        /// <summary>
        /// 获取所有的脚本键名。
        /// </summary>
        public IEnumerable<string> ScriptKeys
        {
            get { return Scripts.Keys; }
        }

        /// <summary>
        /// 获取指定键名的脚本。
        /// </summary>
        /// <typeparam name="TKey">键的数据类型。</typeparam>
        /// <param name="key">脚本的键名。</param>
        /// <returns>返回脚本。</returns>
        public string GetScript<TKey>(TKey key)
        {
            return Scripts.GetScript(key);
        }

        /// <summary>
        /// 执行与释放或重置托管资源相关的应用程序定义的任务。
        /// </summary>
        protected override void DisposeManaged()
        {
            this.Contract.TryDispose();
        }
    }
}
