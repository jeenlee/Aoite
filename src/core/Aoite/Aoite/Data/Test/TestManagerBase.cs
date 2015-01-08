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
        /// <summary>
        /// 初始化一个 <see cref="Aoite.Data.TestManagerBase"/> 类的新实例。
        /// </summary>
        public TestManagerBase()
        {
            Scripts = new SqlScriptsManager();
            Scripts.ParseFolder(GA.FullPath("_scripts"));
            Scripts.ParseFolder(GA.FullPath("_sql"));
        }

        /// <summary>
        /// 获取脚本管理工具。
        /// </summary>
        public SqlScriptsManager Scripts { get; private set; }

        /// <summary>
        /// 获取数据源引擎。
        /// </summary>
        public DbEngine Engine { get; protected set; }

        /// <summary>
        /// 获取数据源引擎管理器。
        /// </summary>
        public DbEngineManager Manager { get; protected set; }

        /// <summary>
        /// 执行指定键名的脚本。
        /// </summary>
        /// <typeparam name="TKey">键的数据类型。</typeparam>
        /// <param name="key">脚本的键名。</param>
        /// <returns>返回当前实例。</returns>
        public TestManagerBase Execute<TKey>(TKey key)
        {
            this.ThrowWhenDisposed();

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
            this.ThrowWhenDisposed();

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
            get
            {
                this.ThrowWhenDisposed();
                return Scripts.Keys;
            }
        }

        /// <summary>
        /// 获取指定键名的脚本。
        /// </summary>
        /// <typeparam name="TKey">键的数据类型。</typeparam>
        /// <param name="key">脚本的键名。</param>
        /// <returns>返回脚本。</returns>
        public string GetScript<TKey>(TKey key)
        {
            this.ThrowWhenDisposed();
            return Scripts.GetScript(key);
        }

        /// <summary>
        /// 执行与释放或重置托管资源相关的应用程序定义的任务。
        /// </summary>
        protected override void DisposeManaged()
        {
            this.Scripts.Clear();
            this.Scripts = null;
            this.Engine = null;
            this.Manager = null;
        }
    }
}
