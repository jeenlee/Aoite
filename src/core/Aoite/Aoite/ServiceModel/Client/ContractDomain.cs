using Aoite.Serialization;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Aoite.ServiceModel
{
    /// <summary>
    /// 表示一个契约的域。
    /// </summary>
    public class ContractDomain : HeadersBase, Aoite.Net.IHostPort
    {
        private static readonly Dictionary<string, ContractDomain> Domains = new Dictionary<string, ContractDomain>(StringComparer.CurrentCultureIgnoreCase);
        /// <summary>
        /// 获取指定名称的契约域。
        /// </summary>
        /// <param name="name">契约域的名称。</param>
        /// <returns></returns>
        public static ContractDomain GetDomain(string name)
        {
            return Domains.TryGetValue(name);
        }

        /// <summary>
        /// 获取所有已注册的契约域信息。
        /// </summary>
        public readonly static ConcurrentDictionary<Type, ContractInfo> AllContracts = new ConcurrentDictionary<Type, ContractInfo>();

        private string _CookieName = HttpSessionProvider.DefaultCookieName;
        /// <summary>
        /// 获取或设置契约域的会话 Cookie 的名称。
        /// </summary>
        public string CookieName
        {
            get { return _CookieName; }
            set
            {
                if(string.IsNullOrEmpty(value)) throw new ArgumentNullException("value"); _CookieName = value;
            }
        }

        private string _Name;
        /// <summary>
        /// 设置或获取契约域的名称。
        /// </summary>
        public string Name
        {
            get { return this._Name; }
            set
            {
                Domains.Remove(this._Name);
                this._Name = value;
                Domains[value] = this;
            }
        }

        /// <summary>
        /// 获取或设置服务地址。
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// 获取或设置服务端口。
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 获取契约域的命名空间表达式。可以是一个完整的命名空间，也可以是“*”起始，或者以“*”结尾。符号“*”只能出现一次。通过“|”可以同时包含多个命名空间。
        /// </summary>
        public string NamespaceExpression { get; set; }

        private int _MaxBufferSize = 2048;
        /// <summary>
        /// 设置或获取最大缓冲区大小。默认为 2048 字节。
        /// </summary>
        public int MaxBufferSize { get { return this._MaxBufferSize; } set { this._MaxBufferSize = value; } }

#if DEBUG
        private int _ResponseTimeout = -1;
#else
        private int _ResponseTimeout = 5000;
#endif
        /// <summary>
        /// 设置或获取响应的超时时间。默认为 5000 毫秒。若设置为 -1 表示无限等待。
        /// </summary>
        public int ResponseTimeout { get { return this._ResponseTimeout; } set { this._ResponseTimeout = value; } }

        /// <summary>
        /// 获取或设置契约域的证书。
        /// </summary>
        public ContractCertificate Certificate { get; set; }

        private Dictionary<string, object> _ExtendData = new Dictionary<string, object>(StringComparer.CurrentCultureIgnoreCase);
        /// <summary>
        /// 设置或获取扩展的用户数据集合。
        /// </summary>
        public Dictionary<string, object> ExtendData
        {
            get { return this._ExtendData; }
            set { this._ExtendData = new Dictionary<string, object>(value, StringComparer.CurrentCultureIgnoreCase); }
        }

        private Serializer _Serializer = new QuicklySerializer();
        /// <summary>
        /// 获取或设置契约的序列化器。
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

        private IClientSessionProvider _SessionProvider;
        /// <summary>
        /// 获取或设置契约域的会话提供。
        /// </summary>
        public IClientSessionProvider SessionProvider
        {
            get
            {
                if(this._SessionProvider == null)
                {
                    lock(this)
                    {
                        if(this._SessionProvider == null)
                        {
                            if(GA.IsWebRuntime)
                            {
                                this._SessionProvider = new HttpSessionProvider(this._ExtendData);
                            }
                            else
                            {
                                this._SessionProvider = new SingleSessionProvider();
                            }
                        }
                    }
                }
                return this._SessionProvider;
            }
            set { this._SessionProvider = value; }
        }

        /// <summary>
        /// 初始化一个 <see cref="Aoite.ServiceModel.ContractDomain"/> 类的新实例。
        /// </summary>
        public ContractDomain()
        {
            this._Name = string.Empty;
            Domains[this._Name] = this;
            IsWebHost = new Lazy<bool>(() =>
            {
                Uri uri;
                return Uri.TryCreate(this.Host, UriKind.Absolute, out uri);
            });
            //LifeCyclesPool = new ObjectPool<IContractLifeCycle>(this.CreateNewLifeCycle);

        }

        /// <summary>
        /// 创建一个契约域的 <see cref="System.IMapFilter"/> 类的新实例。
        /// </summary>
        /// <returns>返回一个 <see cref="System.IMapFilter"/> 类的新实例。</returns>
        public IMapFilter CreateMapFilter()
        {
            if(string.IsNullOrEmpty(this.NamespaceExpression)) throw new ArgumentNullException("NamespaceExpression");
            return new DefaultMapFilter(this.NamespaceExpression);
        }

        /// <summary>
        /// 添加或获取一个指定契约类型的契约信息。
        /// </summary>
        /// <param name="contractType">契约的数据类型。</param>
        /// <returns>返回一个契约的信息。</returns>
        public virtual ContractInfo AddOrGet(Type contractType)
        {
            ContractInfo contract;
            if(!AllContracts.TryGetValue(contractType, out contract))
            {
                lock(contractType)
                {
                    if(!AllContracts.TryGetValue(contractType, out contract))
                    {
                        contract = ContractInfo.GetContractInfo(contractType);
                        AllContracts.TryAdd(contractType, contract);
                    }
                }
            }
            return contract;
        }

        /// <summary>
        /// 创建指定契约类型的代理实例。
        /// </summary>
        /// <param name="contractType">契约的数据类型。</param>
        /// <param name="keepAlive">指示是否为一个持久连接的契约客户端。</param>
        /// <returns>返回一个代理实例。</returns>
        public virtual object Create(Type contractType, bool keepAlive = false)
        {
            return this.AddOrGet(contractType).CreateProxyObject(this, keepAlive);
        }

        /// <summary>
        /// 创建指定契约类型的代理实例。
        /// </summary>
        /// <typeparam name="TContract">契约的数据类型。</typeparam>
        /// <param name="keepAlive">指示是否为一个持久连接的契约客户端。</param>
        /// <returns>返回一个代理实例。</returns>
        public virtual TContract Create<TContract>(bool keepAlive = false)
        {
            return (TContract)Create(typeof(TContract), keepAlive);
        }

        /// <summary>
        /// 提供契约的客户端，创建一个新的契约生命周期。
        /// </summary>
        /// <param name="client">正在调用契约的客户端。</param>
        /// <returns>返回一个契约生命周期的实现。</returns>
        public IContractLifeCycle CreateLifeCycle(IContractClient client)
        {
            var lifeCycle = this.CreateNewLifeCycle();//this.LifeCyclesPool.Acquire();//
            lifeCycle.Client = client;
            lifeCycle.Open().ThrowIfFailded();
            return lifeCycle;
        }
        private Lazy<bool> IsWebHost;

        /// <summary>
        /// 提供契约的客户端，创建一个新的契约生命周期。
        /// </summary>
        /// <returns>返回一个契约生命周期的实现。</returns>
        protected virtual IContractLifeCycle CreateNewLifeCycle()
        {
            if(IsWebHost.Value) return new HttpContractLifeCycle(this);
            return new ContractLifeCycle(this);
        }

        /// <summary>
        /// 创建一个契约请求。
        /// </summary>
        /// <param name="client">正在调用契约的客户端。</param>
        /// <param name="method">契约的方法。</param>
        /// <param name="parameters">请求的参数列表。</param>
        /// <returns>返回一个新的契约请求。</returns>
        protected virtual ContractRequest CreateRequest(IContractClient client, ContractMethod method, params object[] parameters)
        {
            return new ContractRequest(this, client.Contract, method, parameters, client.GetFiles(false));
        }

        internal void ReleaseLifeCycle(IContractLifeCycle liftCycle)
        {
            liftCycle.Close();
            liftCycle.Client = null;
            //LifeCyclesPool.Release(liftCycle);
        }
        private ContractResponse GetResponse(IContractClient client, ContractRequest request)
        {
            lock(client)
            {
                IContractLifeCycle liftCycle = client.KeepAlive ? client.LifeCycle : this.CreateLifeCycle(client);

                var response = liftCycle.GetResponse(request);

                if(!client.KeepAlive)
                {
                    ReleaseLifeCycle(liftCycle);
                    liftCycle = null;
                }

                return response;
            }
        }

        /// <summary>
        /// 调用指定方法标识索引和参数数组的服务端方法。
        /// </summary>
        /// <param name="client">正在调用契约的客户端。</param>
        /// <param name="identity">方法的标识索引。</param>
        /// <param name="parameters">方法的参数集合。</param>
        /// <returns>一组方法调用后的返回值。索引 (0) 表示方法的返回值，其他表示 "ref" 或 "out" 的返回值。</returns>
        public virtual object[] CallMethod(IContractClient client, int identity, params object[] parameters)
        {
            var contract = client.Contract;
            var method = contract.Methods[identity];
            var domain = client.Domain;

            try
            {
                var request = this.CreateRequest(client, method, parameters);
                var response = this.GetResponse(client, request);
                if(response == null) throw new System.Runtime.Serialization.SerializationException("无效的契约请求或响应。");

                if(response.Status != StatusCode.OK)
                    throw new ResponseException(response.Status, response.Message);

                if(!string.IsNullOrEmpty(response.SessionId)) domain.SessionProvider.SessionId = response.SessionId;

                if(response.HasHeaders)
                {
                    var headers = domain.Headers;
                    lock(headers)
                    {
                        foreach(var item in response.Headers)
                        {
                            if(item.Value == null)
                            {
                                object o;
                                headers.TryRemove(item.Key, out o);
                            }
                            else headers[item.Key] = item.Value;
                        }
                    }
                }
                var files = response._Files;
                if(files != null && files.Length > 0)
                {
                    foreach(var file in files)
                    {
                        client.AddFile(file);

                    }
                }
                return response.Values;
            }
            catch(Exception ex)
            {
                var returnType = method.ReturnType;
                if(returnType != Types.Result
                    && !returnType.IsSubclassOf(Types.Result)) throw;

                try
                {
                    var r = Activator.CreateInstance(returnType) as Result;
                    if(ex is System.ComponentModel.DataAnnotations.ValidationException) r.ToFailded(ex, 1);
                    else if(ex is ResponseException) r.ToFailded(ex, (int)(ex as ResponseException).Status);
                    else r.ToFailded(ex);

                    var result = new object[method.ParameterTypes.Length + 1];
                    result[0] = r;
                    return result;
                }
                catch(Exception) { }

                throw;
            }
        }
    }
}
