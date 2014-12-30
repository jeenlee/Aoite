using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Aoite.ServiceModel
{
    /// <summary>
    /// 表示一个契约的客户端。
    /// </summary>
    public abstract class ContractClient : ObjectDisposableBase, IContractClient
    {
        private ContractDomain _Domain;
        ContractDomain IContractClient.Domain { get { return this._Domain; } }

        private ContractInfo _Contract;
        IContractInfo IContractClient.Contract { get { return this._Contract; } }
        private IContractLifeCycle _LifeCycle;
        IContractLifeCycle IContractClient.LifeCycle { get { return this._LifeCycle; } }

        private bool _KeepAlive;
        bool IContractClient.KeepAlive { get { return this._KeepAlive; } }

        /// <summary>
        /// 提供契约域、契约的信息和持久化标识，初始化一个 <see cref="Aoite.ServiceModel.ContractClient"/> 类的新实例。
        /// </summary>
        /// <param name="domain">契约的域。</param>
        /// <param name="contract">契约的信息。</param>
        /// <param name="keepAlive">指示是否为一个持久连接的契约客户端。</param>
        public ContractClient(ContractDomain domain, ContractInfo contract, bool keepAlive)
        {
            if(domain == null) throw new ArgumentNullException("domain");
            this._Domain = domain;
            this._Contract = contract;
            if(this._KeepAlive = keepAlive) this._LifeCycle = domain.CreateLifeCycle(this);
        }

        /// <summary>
        /// 调用指定方法标识索引和参数数组的服务端方法。
        /// </summary>
        /// <param name="identity">方法的标识索引。</param>
        /// <param name="parameters">方法的参数集合。</param>
        /// <returns>一组方法调用后的返回值。索引 (0) 表示方法的返回值，其他表示 "ref" 或 "out" 的返回值。</returns>
        protected virtual object[] CallMethod(int identity, params object[] parameters)
        {
            return this._Domain.CallMethod(this, identity, parameters);
        }

        /// <summary>
        /// 执行与释放或重置托管资源相关的应用程序定义的任务。
        /// </summary>
        protected override void DisposeManaged()
        {
            if(this._LifeCycle != null)
            {
                _Domain.ReleaseLifeCycle(this._LifeCycle);
                this._LifeCycle = null;
            }
        }

        private List<ContractFile> _lastFiles = new List<ContractFile>();

        void IContractClient.AddFile(string fileName, int fileSize, Stream content)
        {
            this._lastFiles.Add(new ContractFile(fileName, fileSize, content));
        }

        void IContractClient.AddFile(string fileName)
        {
            if(string.IsNullOrEmpty(fileName)) throw new ArgumentNullException("fileName");
            using(var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                this._lastFiles.Add(new ContractFile(fileStream.Name, (int)fileStream.Length, fileStream));
            }
        }

        void IContractClient.AddFile(FileStream fileStream)
        {
            if(fileStream == null) throw new ArgumentNullException("fileStream");
            this._lastFiles.Add(new ContractFile(fileStream.Name, (int)fileStream.Length, fileStream));
        }

        void IContractClient.AddFile(ContractFile file)
        {
            if(file == null) throw new ArgumentNullException("file");
            this._lastFiles.Add(file);
        }
        ContractFile[] IContractClient.GetFiles(bool peekTime)
        {
            if(this._lastFiles.Count == 0) return null;

            var files = this._lastFiles.ToArray();
            if(!peekTime) this._lastFiles = new List<ContractFile>();
            return files;
        }
    }
}
