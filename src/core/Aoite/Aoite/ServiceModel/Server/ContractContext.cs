using Aoite.Cache;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Aoite.ServiceModel
{
    /// <summary>
    /// 获取契约的上下文信息。
    /// </summary>
    public class ContractContext : ObjectDisposableBase
    {

        [ThreadStatic]
        private static ContractContext _Current;
        /// <summary>
        /// 获取当前线程中的上下文契约。
        /// </summary>
        public static ContractContext Current
        {
            get
            {
                if(GA.IsWebRuntime) return System.Web.AoiteWebApi.Context;
                return _Current;
            }
            internal set
            {
                if(GA.IsWebRuntime)
                {
                    System.Web.AoiteWebApi.Context = value;
                }
                else _Current = value;
            }
        }

        /// <summary>
        /// 在单元测试的运行环境创建一个新的契约上下文。非单元测试运行环境，此方法将会抛出异常。
        /// </summary>
        /// <param name="user">当前已授权的登录用户。</param>
        public static void NewUnitTestContext(object user = null)
        {
            if(!GA.IsUnitTestRuntime) throw new PlatformNotSupportedException();
            _Current = new ContractContextUnitTest() { User = user };
            _Current._Session = new Lazy<ICacheGroup>(() => new MemoryCacheProvider());
        }

        /// <summary>
        /// 表示一个契约的来源。
        /// </summary>
        public class ContextSource
        {
            ContractContext _context;
            internal ContextSource(ContractContext context)
            {
                this._context = context;
            }
            /// <summary>
            /// 获取当前契约实例。
            /// </summary>
            public object Instance { get { return this._context._Instance; } }

            /// <summary>
            /// 获取当前契约服务器。
            /// </summary>
            public IContractServer Server { get { return this._context._Server; } }
            /// <summary>
            /// 获取契约的请求。
            /// </summary>
            public ContractRequest Request { get { return this._context._Request; } }
            /// <summary>
            /// 获取契约的服务提供程序。
            /// </summary>
            public ContractService Service { get { return this._context._Service; } }

            /// <summary>
            /// 获取契约的服务方法。
            /// </summary>
            public virtual ContractMethod Method { get { return this._context._Method; } }
            /// <summary>
            /// 获取契约的响应。
            /// </summary>
            public virtual ContractResponse Response { get { return this._context._Response; } }
        }

        private IContractServer _Server;
        private ContractRequest _Request;
        internal ContractService _Service;
        internal ContractMethod _Method;
        private ContractResponse _Response;
        internal object _Instance;
        internal bool _IsLocalClient;
        /// <summary>
        /// 获取一个值，指示当前上下文是否为本地客户端。
        /// </summary>
        public bool IsLocalClient { get { return this._IsLocalClient; } }
        /// <summary>
        /// 获取本地客户端归属的实际上下文。
        /// </summary>
        public virtual ContractContext OwnerContext { get { return null; } }

        private ContextSource _Source;
        /// <summary>
        /// 获取契约的来源信息。
        /// </summary>
        public virtual ContextSource Source { get { return this._Source; } }

        /// <summary>
        /// 获取缓存的提供程序。
        /// </summary>
        public virtual ICacheProvider CacheProvider
        {
            get { return this._Server.CacheProvider; }
        }

        private Lazy<ICacheGroup> _Session;
        /// <summary>
        /// 获取当前契约的会话。
        /// </summary>
        public virtual ICacheGroup Session { get { return this._Session.Value; } }

        private Lazy<HybridDictionary> _ItemsLazy = new Lazy<HybridDictionary>();
        /// <summary>
        /// 获取或设置上下文的临时数据。
        /// </summary>
        public HybridDictionary Items
        {
            get
            {
                return this._ItemsLazy.Value;
            }
        }

        /// <summary>
        /// 获取或设置当前已授权的登录用户。
        /// </summary>
        public virtual dynamic User
        {
            get { return this.Session[this._Source.Server.Configuration.UserSessionName]; }
            set { this.Session[this._Source.Server.Configuration.UserSessionName] = value; }
        }

        private string _ClientEndPoint;
        /// <summary>
        /// 获取的客户端文本形式的地址。
        /// </summary>
        public virtual string ClientEndPoint { get { return this._ClientEndPoint; } }

        private bool _IsAuthenticated;
        /// <summary>
        /// 获取或设置一个值，该值指示是否验证了用户。
        /// </summary>
        public virtual bool IsAuthenticated { get { return this._IsAuthenticated; } set { this._IsAuthenticated = value; } }

        internal string _ResponseSessionId;
        private List<ContractFile> _lastFiles = new List<ContractFile>();

        internal ContractContext() { }
        internal ContractContext(IContractServer server, ContractRequest request)
        {
            this._Server = server;
            this._Request = request;
            this._Source = new ContextSource(this);
        }

        internal ContractContext(IContractServer server, ContractRequest request, string clientEndPoint)
            : this(server, request, clientEndPoint, true) { }

        internal ContractContext(IContractServer server, ContractRequest request, string clientEndPoint, bool initSession)
            : this(server, request)
        {
            this._ClientEndPoint = clientEndPoint;
            if(initSession) this.InitSession();
        }
        private void InitSession()
        {
            this._Session = new Lazy<ICacheGroup>(() =>
            {
                if(string.IsNullOrEmpty(this._Request.SessionId))
                {
                    this._Request.SessionId = Guid.NewGuid().ToString("N");
                    if(this._Response == null) this._ResponseSessionId = this._Request.SessionId;
                    else this._Response._SessionId = this._Request.SessionId;
                }
                return this._Server.CacheProvider.CreatetCacheGroup(this._Request.SessionId);
            });
        }

        /// <summary>
        /// 添加一个文件到当前响应。
        /// </summary>
        /// <param name="fileName">文件的完全限定名。</param>
        /// <param name="fileSize">文件的大小（以字节为单位）。</param>
        /// <param name="content">一个 <see cref="System.IO.Stream"/> 对象，该对象指向一个准备下载的内容。</param>
        public void AddFile(string fileName, int fileSize, Stream content)
        {
            if(GA.IsWebRuntime) throw new NotSupportedException();
            this.AddFile(new ContractFile(fileName, fileSize, content));
        }

        /// <summary>
        /// 添加一个文件到当前响应。
        /// </summary>
        /// <param name="fileName">文件的完全限定名。</param>
        public void AddFile(string fileName)
        {
            if(string.IsNullOrEmpty(fileName)) throw new ArgumentNullException("fileName");
            if(GA.IsWebRuntime)
            {
                if(this._Method.ReturnType != Types.Void || this._Method.ReturnType != Types.Result)
                    throw new NotSupportedException("添加文件的返回值必须是一个 void 或 System.Result。");
                this._lastFiles.Add(new ContractFile(fileName));
            }
            else
            {
                using(var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    this.AddFile(new ContractFile(fileStream.Name, (int)fileStream.Length, fileStream));
                }
            }
        }

        /// <summary>
        /// 添加一个文件到当前响应。
        /// </summary>
        /// <param name="fileStream">文件流。</param>
        public void AddFile(FileStream fileStream)
        {
            if(GA.IsWebRuntime) throw new NotSupportedException();
            if(fileStream == null) throw new ArgumentNullException("fileStream");
            this.AddFile(new ContractFile(fileStream.Name, (int)fileStream.Length, fileStream));
        }

        /// <summary>
        /// 添加一个文件到当前响应。
        /// </summary>
        /// <param name="file">文件。</param>
        public virtual void AddFile(ContractFile file)
        {
            if(GA.IsWebRuntime) throw new NotSupportedException();
            if(file == null) throw new ArgumentNullException("file");
            this._lastFiles.Add(file);
        }

        /// <summary>
        /// 获取所有正在等待下载文件列表。
        /// </summary>
        /// <param name="peekTime">指示是否为查看模式，如果为 true 则下一次访问此方法仍然可以获取数据，否则下一次访问之前如果没有添加新文件将会返回一个 null 值。</param>
        /// <returns>返回所有正在等待下载文件列表，如果没有等待下载的文件，将返回一个 null 值。</returns>
        public virtual ContractFile[] GetFiles(bool peekTime)
        {
            if(this._lastFiles.Count == 0) return null;

            var files = this._lastFiles.ToArray();
            if(peekTime) this._lastFiles = new List<ContractFile>();
            return files;
        }

        /// <summary>
        /// 创建一个成功的响应。
        /// </summary>
        /// <param name="values">值的集合。</param>
        public void CreateResponse(object[] values)
        {
            this._Response = new ContractResponse();
            this._Response._Status = StatusCode.OK;
            this._Response._Values = values;
            this._Response._Files = this.GetFiles(false);
            this._Response._SessionId = this._ResponseSessionId;
        }

        /// <summary>
        /// 创建一个失败的响应。
        /// </summary>
        /// <param name="status">响应的状态码。</param>
        /// <param name="message">响应的消息。</param>
        public void CreateResponse(StatusCode status, string message)
        {
            this._Response = NewResponse(status, message);
            this._Response._Files = this.GetFiles(false);
            this._Response._SessionId = this._ResponseSessionId;
        }

        /// <summary>
        /// 创建一个失败的响应。
        /// </summary>
        /// <param name="status">响应的状态码。</param>
        /// <param name="message">响应的消息。</param>
        public static ContractResponse NewResponse(StatusCode status, string message)
        {
            var response = new ContractResponse();
            response._Status = status;
            response._Message = message;
            return response;
        }
    }

    /// <summary>
    /// 表示一个执行命令模型的用户工厂，该工厂基于服务契约架构。
    /// </summary>
    public class ContractUserFactory : Aoite.CommandModel.IUserFactory
    {
        /// <summary>
        /// 获取唯一实例。
        /// </summary>
        public static readonly Aoite.CommandModel.IUserFactory Instance = new ContractUserFactory();
        private ContractUserFactory() { }

        /// <summary>
        /// 获取执行命令模型的用户。
        /// </summary>
        /// <param name="container">服务容器。</param>
        dynamic CommandModel.IUserFactory.GetUser(IIocContainer container)
        {
            return container.GetService<ContractContext>().User;
        }
    }
    class ContractContextUnitTest : ContractContext
    {
        public ContractContextUnitTest() { }

        public override ContractContext.ContextSource Source { get { throw new NotSupportedException("单元测试运行环境不支持此属性。"); } }

        public override ICacheProvider CacheProvider { get { return base.Session as ICacheProvider; } }

        private dynamic _User;
        public override dynamic User
        {
            get
            {
                return this._User;
            }
            set
            {
                this._User = value;
            }
        }
    }

    class LocalClientContractContext : ContractContext
    {
        ContractContext _owner;

        public override bool IsAuthenticated
        {
            get
            {
                return this._owner.IsAuthenticated;
            }
            set
            {
                this._owner.IsAuthenticated = value;
            }
        }

        public override ContractContext OwnerContext { get { return this._owner; } }
        public override ICacheProvider CacheProvider { get { return this._owner.CacheProvider; } }
        public override ICacheGroup Session { get { return this._owner.Session; } }

        public override dynamic User { get { return this._owner.User; } set { this._owner.User = value; } }

        internal LocalClientContractContext(ContractContext owner, IContractServer server, ContractRequest request, string clientEndPoint)
            : base(server, request, clientEndPoint, false)
        {
            if(owner == null) throw new ArgumentNullException("owner");
            this._owner = owner;
        }
    }
}
