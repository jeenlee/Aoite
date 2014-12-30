using Aoite.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.ServiceModel
{
    /// <summary>
    /// 表示一个契约服务器的基类。
    /// </summary>
    public abstract class ContractServerBase : IContractServer
    {
        /// <summary>
        /// 服务器抛出已捕获的错误时发生。
        /// </summary>
        public event ExceptionEventHandler Error;
        /// <summary>
        /// 契约方法调用前发生。
        /// </summary>
        public event EventHandler Calling;
        /// <summary>
        /// 契约方法调用后发生。
        /// </summary>
        public event EventHandler Called;
        /// <summary>
        /// 契约实例创建后发生。
        /// </summary>
        public event EventHandler InstanceCreated;

        private IClientSessionProvider _SessionProvider;
        /// <summary>
        /// 获取或设置契约域的会话提供。
        /// </summary>
        internal IClientSessionProvider SessionProvider
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
                                this._SessionProvider = new HttpSessionProvider(this.GetConfiguration().ExtendData);
                            }
                            else
                            {
                                this._SessionProvider = new MutilationSessionProvider();
                            }
                        }
                    }
                }
                return this._SessionProvider;
            }
            set { this._SessionProvider = value; }
        }
        ServerConfigurationBase IContractServer.Configuration { get { return this.GetConfiguration(); } }

        private IIocContainer _Container = new IocContainer();
        /// <summary>
        /// 获取服务容器。
        /// </summary>
        public virtual IIocContainer Container { get { return _Container; } }

        private Lazy<ICacheProvider> _CacheProvider;
        /// <summary>
        /// 获取缓存的提供程序。
        /// </summary>
        public ICacheProvider CacheProvider { get { return this._CacheProvider.Value; } }

        /// <summary>
        /// 初始化一个 <see cref="Aoite.ServiceModel.ContractServerBase"/> 类的新实例。
        /// </summary>
        public ContractServerBase()
        {
            this._CacheProvider = new Lazy<ICacheProvider>(this.CreateCacheProvider);
        }

        /// <summary>
        /// 初始化服务器。
        /// </summary>
        protected void Initiailze()
        {
            var config = this.GetConfiguration();
            if(!string.IsNullOrEmpty(config.NamespaceExpression)) this.Container.AutoMap(new DefaultMapFilter(config.NamespaceExpression), config.Register);

            foreach(var service in config)
            {
                ObjectFactory.Global.AddService(service.Type, lmp => Locals.LocalClient.CreateLocalClient(this, service), true);
            }

            ObjectFactory.Global.MapResolve += Global_MapResolve;
        }

        void Global_MapResolve(object sender, MapResolveEventArgs e)
        {
            var serviceName = e.ExpectType.GetServiceName();
            var service = this.GetConfiguration().GetService(serviceName);
            if(service != null)
            {
                e.Callback = lmp => Locals.LocalClient.CreateLocalClient(this, service);
                e.SingletonMode = true;
            }
        }


        /// <summary>
        /// 创建缓存的提供程序。
        /// </summary>
        /// <returns>返回缓存的提供程序。</returns>
        protected virtual ICacheProvider CreateCacheProvider()
        {
            return this.CreateCacheProvider(this.GetConfiguration().CacheProviderType);
        }

        /// <summary>
        /// 获取服务器的配置信息。
        /// </summary>
        /// <returns>返回服务器的配置信息。</returns>
        protected abstract ServerConfigurationBase GetConfiguration();

        /// <summary>
        /// 服务器抛出已捕获的错误时发生。
        /// </summary>
        /// <param name="exception">已捕获的错误描述。</param>
        protected virtual void OnError(Exception exception)
        {
            System.Diagnostics.Trace.WriteLine(exception);
            var handler = this.Error;
            if(handler != null) handler(this, new ExceptionEventArgs(exception));
        }

        /// <summary>
        /// 契约方法调用前发生。
        /// </summary>
        /// <param name="context">契约的上下文信息。</param>
        protected virtual void OnCalling(ContractContext context)
        {
            var handler = this.Calling;
            if(handler != null) handler(this, EventArgs.Empty);
        }

        /// <summary>
        /// 契约方法调用后发生。
        /// </summary>
        /// <param name="context">契约的上下文信息。</param>
        protected virtual void OnCalled(ContractContext context)
        {
            var handler = this.Called;
            if(handler != null) handler(this, EventArgs.Empty);
        }

        /// <summary>
        /// 契约实例创建后发生。
        /// </summary>
        protected virtual void OnInstanceCreated(object instance)
        {
            var handler = this.InstanceCreated;
            if(handler != null) handler(instance, EventArgs.Empty);
        }

        #region CallMethod

        /// <summary>
        /// 处理契约请求。
        /// </summary>
        /// <param name="request">契约请求。</param>
        /// <param name="clientEndPoint">客户端地址。</param>
        /// <returns>返回契约上下文。</returns>
        protected virtual ContractContext ProcessRequest(ContractRequest request, string clientEndPoint)
        {
            if(request == null) throw new ArgumentNullException("request");

            var context = new ContractContext(this, request, clientEndPoint);
            ContractContext.Current = context;

            if(!this.ValidtionContract(context)) return context;

            return this.Call(context);
        }

        internal ContractContext Call(ContractContext context)
        {
            if(!this.HasResponse(this.OnCalling, context))
                this.OnCall(context);
            this.OnCalled(context);
            context.Source.Response._Id = context.Source.Request.Id;
            return context;
        }

        private bool ValidtionContract(ContractContext context)
        {
            if(context == null) throw new ArgumentNullException("context");

            var request = context.Source.Request;

            if(string.IsNullOrEmpty(request.ContractName) || string.IsNullOrEmpty(request.MethodName))
            {
                context.CreateResponse(StatusCode.BadRequest, "请求无效。");
                return false;
            }

            var service = this.GetConfiguration().GetService(request);
            if(service == null)
            {
                context.CreateResponse(StatusCode.NotFound, "找不到契约。");
                return false;
            }

            var method = service.FindMethod(request);
            if(method == null)
            {
                context.CreateResponse(StatusCode.NotFound, "找不到方法。");
                return false;
            }
            context._Service = service;
            context._Method = method;
            return true;
        }

        /// <summary>
        /// 契约方法调用时发生。
        /// </summary>
        /// <param name="context">契约的上下文信息。</param>
        protected virtual void OnCall(ContractContext context)
        {
            //- Calling
            //- ==>     A                               B                       C                       D
            //- ==> ContractServer.OnCalling -> Instance.OnCalling -> TypeFilters.OnCalling -> MethodFilters.OnCalling
            //- Called（Calling 反序）

            var request = context.Source.Request;
            var service = context.Source.Service;
            var method = context.Source.Method;
            var ps = request.Parameters;
            var byRefs = method.ParameterByRefs;
            var config = this.GetConfiguration();

            object returnValue;
            object[] lastMappingValues = null;
            if(service.LastMappingValues != null) lastMappingValues = service.LastMappingValues(service.RealType ?? service.Type);
            else if(config.LastMappingValues != null) lastMappingValues = config.LastMappingValues(service);

            object instance;
            try
            {
                instance = service.GetServiceInstance(this, lastMappingValues ?? new object[0]);
                if(instance == null) throw new ArgumentNullException("instance");
            }
            catch(Exception ex)
            {
                this.OnError(ex);
                this.CreateReponseOnInternalServerError(context, ex, "创建契约实例失败。");
                return;
            }

            this.OnInstanceCreated(instance);

            var si = instance as IServiceHandler;
            context._Instance = instance;
            var isServiceInstance = si != null;
            var hasServiceFilters = service.Filters.Length > 0;
            var hasMethodFilters = method.Filters.Length > 0;

            if(isServiceInstance && this.HasResponse(si.Calling, context)) goto END_CALLED; //- B
            if(hasServiceFilters)
                foreach(var filter in service.Filters) if(this.HasResponse(filter.Calling, context)) goto END_CALLED; //- C
            if(hasMethodFilters)
                foreach(var filter in method.Filters) if(this.HasResponse(filter.Calling, context)) goto END_CALLED; //- B

            if(!config.IsPublicServices && !method.AllowAnonymous && !service.AllowAnonymous && !context.IsAuthenticated)
            {
                context.CreateResponse(StatusCode.Unauthorized, "请求未经授权。");
                goto END_CALLED;
            }

            try
            {
                method.Validation(ps);
                returnValue = method.CallMethod(instance, ps);
            }
            catch(System.ComponentModel.DataAnnotations.ValidationException ex)
            {
                context.CreateResponse(StatusCode.BadRequest, ex.ValidationResult.ErrorMessage);
                goto END_CALLED;
            }
            catch(Exception ex)
            {
                this.OnError(ex);
                if(ex is ResultException)
                {
                    var returnType = method.ReturnType;
                    if(returnType == Types.Result || returnType.IsSubclassOf(Types.Result))
                    {
                        var resultEx = ex as ResultException;
                        try
                        {
                            var r = Activator.CreateInstance(returnType) as Result;
                            r.ToFailded(resultEx, resultEx.Status);
                            returnValue = r;
                            goto RE_LABEL;
                        }
                        catch(Exception ex2)
                        {
                            this.OnError(ex2);
                            ex2 = ex;
                        }
                    }

                }
                this.CreateReponseOnInternalServerError(context, ex);
                goto END_CALLED;
            }
        RE_LABEL:
            var values = new object[ps.Length + 1];
            values[0] = returnValue;
            for(int i = 0; i < ps.Length; i++) values[i + 1] = byRefs[i] ? ps[i] : null;

            context.CreateResponse(values);
        END_CALLED:
            if(hasMethodFilters) foreach(var filter in method.Filters) filter.Called(context); //- D
            if(hasServiceFilters) foreach(var filter in service.Filters) filter.Called(context); //- C
            if(isServiceInstance) si.Called(context); //- B
        }

        internal bool HasResponse(Action<ContractContext> action, ContractContext context)
        {
            action(context);
            return context.Source.Response != null;
        }
        internal string GetErrorMessage(Exception ex, string defaultError = "服务发生了内部错误。")
        {
            var config = this.GetConfiguration();
            var error = config.DefaultInternalErrors;
            if(string.IsNullOrEmpty(error))
            {
                error = config.ReturnInternalErrors
                   ? ex.ToString()
                    : defaultError;
            }
            return error;
        }
        internal void CreateReponseOnInternalServerError(ContractContext context, Exception ex, string defaultError = "服务发生了内部错误。")
        {
            context.CreateResponse(StatusCode.InternalServerError, this.GetErrorMessage(ex, defaultError));
        }

        #endregion
    }
}
