using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using CDictionary = System.Collections.Concurrent.ConcurrentDictionary<string, object>;

namespace Aoite.ServiceModel
{
    /// <summary>
    /// 表示一个契约的请求。
    /// </summary>
    public class ContractRequest : HeadersBase
    {
        static long index = 1000;
        private string _Id = System.Threading.Interlocked.Increment(ref index).ToStringOrEmpty();
        /// <summary>
        /// 获取请求的唯一标识。
        /// </summary>
        public string Id { get { return this._Id; } }

        private string _SessionId;
        /// <summary>
        /// 获取契约域的会话标识。
        /// </summary>
        public string SessionId { get { return this._SessionId; } internal set { this._SessionId = value; } }

        private string _ContractName;
        /// <summary>
        /// 获取请求的契约名称。
        /// </summary>
        public string ContractName { get { return this._ContractName; } }

        private string _MethodName;
        /// <summary>
        /// 获取请求的方法名称。
        /// </summary>
        public string MethodName { get { return this._MethodName; } }

        private object[] _Parameters;
        /// <summary>
        /// 获取请求的参数列表。
        /// </summary>
        public object[] Parameters { get { return this._Parameters; } }

        private ContractFile[] _Files;
        /// <summary>
        /// 获取请求的文件列表。
        /// </summary>
        public ContractFile[] Files { get { return this._Files; } }

        private ContractCertificate _Certificate;
        /// <summary>
        /// 获取请求的契约证书。
        /// </summary>
        public ContractCertificate Certificate { get { return this._Certificate; } }

        [Ignore]
        internal readonly IContractInfo _contract;
        [Ignore]
        internal readonly ContractMethod _method;

        internal ContractRequest(ContractContext current, ContractService service, ContractMethod method, object[] parameters, ContractFile[] files)
        {
            this._ContractName = service.Name;
            this._MethodName = method.Name;
            this._Parameters = parameters;
            this._Files = files;
            this._contract = service;
            if(current == null) return;
            var request = current.Source.Request;

            this._SessionId = request._SessionId;
            this._Certificate = request._Certificate;
        }
        internal ContractRequest(ContractDomain domain, IContractInfo contract, ContractMethod method, object[] parameters, ContractFile[] files)
            : this(() => domain.Headers, domain.Certificate, domain.SessionProvider.SessionId, contract.Name, method.Name, parameters, files)
        {
            method.Validation(parameters);
            this._contract = contract;
            this._method = method;
        }

        internal ContractRequest(Func<CDictionary> headersFactory, ContractCertificate certificate, string sessionId, string contractName, string methodName, object[] parameters, ContractFile[] files)
            : base(headersFactory)
        {
            this._ContractName = contractName;
            this._MethodName = methodName;
            this._Parameters = parameters;
            this._Files = files;

            this._SessionId = sessionId;

            this._Certificate = certificate;
        }
    }
}
