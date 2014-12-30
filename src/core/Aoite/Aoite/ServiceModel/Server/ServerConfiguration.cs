using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Aoite.ServiceModel
{
    /// <summary>
    /// 表示一个基础的契约服务器配置。
    /// </summary>
    public abstract class ServerConfigurationBase : IEnumerable<ContractService>
    {
        private Dictionary<string, ContractService> _Contracts = new Dictionary<string, ContractService>(StringComparer.CurrentCultureIgnoreCase);

        /// <summary>
        /// 设置或获取后期映射的参数值数组的参数。
        /// </summary>
        public Func<ContractService, object[]> LastMappingValues { get; set; }

        internal const string DefaultUserSessionName = "$Logion.User$";
        private string _UserSessionName = DefaultUserSessionName;
        /// <summary>
        /// 设置或获取用户会话的关键字。默认为“$Logion.User$”，该值不能设置为空。
        /// </summary>
        public string UserSessionName
        {
            get { return this._UserSessionName; }
            set
            {
                if(string.IsNullOrEmpty(value)) throw new ArgumentNullException("value");
                this._UserSessionName = value;
            }
        }

        /// <summary>
        /// 获取或设置一个值，指示是否为公共服务。若为公共服务，所有的服务均无需检验是否合法。
        /// </summary>
        public bool IsPublicServices { get; set; }

        /// <summary>
        /// 获取或设置缓存提供程序的类型。
        /// </summary>
        public string CacheProviderType { get; set; }

        private int _SessionExpiration = 1200;
        /// <summary>
        /// 获取或设置会话的最大超时时间（秒）。默认为 1200 秒。
        /// </summary>
        public int SessionExpiration { get { return this._SessionExpiration; } set { this._SessionExpiration = value < 1 ? 60 : value; } }

        private int _DefaultLockTimeout = 60;
        /// <summary>
        /// 获取或设置默认的全局锁超时时间（秒）。默认为 60 秒。
        /// </summary>
        public int DefaultLockTimeout { get { return this._DefaultLockTimeout; } set { this._DefaultLockTimeout = value; } }

        /// <summary>
        /// 获取服务的命名空间表达式。可以是一个完整的命名空间，也可以是“*”起始，或者以“*”结尾。符号“*”只能出现一次。通过“|”可以同时包含多个命名空间。
        /// </summary>
        public string NamespaceExpression { get; set; }

        /// <summary>
        /// 获取或设置一个值，表示服务发生内部错误是否返回异常的详细描述。默认为 false，表示不返回详细异常。如果有指定 <seealso cref="DefaultInternalErrors"/>，该值将会无效。
        /// </summary>
        public bool ReturnInternalErrors { get; set; }

        /// <summary>
        /// 获取或设置默认的服务服务发生内部返回异常的详细描述。如果设定该值，服务端发生内部错误时将会返回此信息。
        /// </summary>
        public string DefaultInternalErrors { get; set; }
        private Dictionary<string, object> _ExtendData = new Dictionary<string, object>(StringComparer.CurrentCultureIgnoreCase);
        /// <summary>
        /// 设置或获取扩展的用户数据集合。
        /// </summary>
        public Dictionary<string, object> ExtendData
        {
            get { return this._ExtendData; }
            set { this._ExtendData = new Dictionary<string, object>(value, StringComparer.CurrentCultureIgnoreCase); }
        }

        internal int GetLockTimeout(TimeSpan? timeout)
        {
            int milliseconds = timeout.HasValue
                ? (int)timeout.Value.TotalMilliseconds
                : (this._DefaultLockTimeout * 1000);

            return milliseconds < 5000 ? 5000 : milliseconds;
        }


        /// <summary>
        /// 获取注册契约的元素数。
        /// </summary>
        public int Count { get { return this._Contracts.Count; } }

        /// <summary>
        /// 注册一个契约。
        /// </summary>
        /// <typeparam name="TContract">契约的数据类型。</typeparam>
        /// <typeparam name="TReal">契约的真实服务类型。</typeparam>
        /// <param name="lastMappingValues">后期映射的参数值数组的参数委托。</param>
        public void Register<TContract, TReal>(Func<Type, object[]> lastMappingValues = null)
        {
            Register(typeof(TContract), typeof(TReal), lastMappingValues);
        }
        /// <summary>
        /// 注册一个契约。
        /// </summary>
        /// <typeparam name="TContract">契约的数据类型。</typeparam>
        /// <param name="realType">契约的真实服务类型。</param>
        /// <param name="lastMappingValues">后期映射的参数值数组的参数委托。</param>
        public void Register<TContract>(Type realType = null, Func<Type, object[]> lastMappingValues = null)
        {
            Register(typeof(TContract), realType, lastMappingValues);
        }
        /// <summary>
        /// 注册一个契约。
        /// </summary>
        /// <param name="type">契约的数据类型。</param>
        public virtual void Register(Type type)
        {
            this.Register(type, null, null);
        }
        /// <summary>
        /// 注册一个契约。
        /// </summary>
        /// <param name="type">契约的数据类型。</param>
        /// <param name="realType">契约的真实服务类型。</param>
        /// <param name="lastMappingValues">后期映射的参数值数组的参数委托。</param>
        public virtual void Register(Type type, Type realType, Func<Type, object[]> lastMappingValues = null)
        {
            var contract = ContractService.GetContractService(type);
            contract.RealType = realType;
            contract.LastMappingValues = lastMappingValues;
            this._Contracts[contract.Name] = contract;
        }

        /// <summary>
        /// 提供契约的请求，获取契约的服务实例。
        /// </summary>
        /// <param name="request">一个契约的请求。</param>
        /// <returns>返回一个契约的服务实例。</returns>
        public virtual ContractService GetService(ContractRequest request)
        {
            return this.GetService(request.ContractName);
        }
        /// <summary>
        /// 提供契约的名称，获取契约的服务实例。
        /// </summary>
        /// <param name="contractName">一个契约的名称。</param>
        /// <returns>返回一个契约的服务实例。</returns>
        public virtual ContractService GetService(string contractName)
        {
            ContractService contract;
            this._Contracts.TryGetValue(contractName, out contract);
            return contract;
        }

        IEnumerator<ContractService> IEnumerable<ContractService>.GetEnumerator()
        {
            return this._Contracts.Values.GetEnumerator();
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this._Contracts.Values.GetEnumerator();
        }
    }

    /// <summary>
    /// 表示一个契约服务端的配置信息。
    /// </summary>
    public class ServerConfiguration : ServerConfigurationBase, Aoite.Net.IHostPort
    {
        #region <<服务配置>>
        /// <summary>
        /// 获取或设置主机地址。
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 获取或设置主机端口。
        /// </summary>
        public int Port { get; set; }

        private Serializer _Serializer = new Aoite.Serialization.QuicklySerializer();
        /// <summary>
        /// 获取或设置序列化器。
        /// </summary>
        public Serializer Serializer
        {
            get { return this._Serializer; }
            set
            {
                if(value == null) throw new ArgumentNullException("value");
                this._Serializer = value;
            }
        }

        #endregion

        #region <<通讯属性>>

        private int _MaxBufferSize = 2048;
        /// <summary>
        /// 设置或获取最大缓冲区大小。默认为 2048 字节。
        /// </summary>
        public int MaxBufferSize { get { return this._MaxBufferSize; } set { this._MaxBufferSize = value; } }

        private int _MaxConnectionCount = 10240;
        /// <summary>
        /// 设置或获取允许最大的连接数。默认为 10240 个连接数。
        /// </summary>
        public int MaxConnectionCount { get { return this._MaxConnectionCount; } set { this._MaxConnectionCount = value; } }

        private int _ListenBacklog = 1024;
        /// <summary>
        /// 设置或获取一个值，表示挂起连接队列的最大长度。默认为 1024 并发数。
        /// </summary>
        public int ListenBacklog { get { return this._ListenBacklog; } set { this._ListenBacklog = value; } }

        #endregion

        /// <summary>
        /// 初始化一个默认的 <see cref="Aoite.ServiceModel.ServerConfiguration"/> 类的新实例。
        /// </summary>
        public ServerConfiguration() { }
    }
}
