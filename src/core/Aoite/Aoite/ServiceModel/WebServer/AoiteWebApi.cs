using Aoite.ServiceModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace System.Web
{
    /// <summary>
    /// 一个 AOITE Web API 应用程序。
    /// </summary>
    public static class AoiteWebApi
    {
        internal const string ContractHeaderNamePrefix = "Contract-";
        internal const string ContractCertificateAppKeyName = "ContractCertificate-AppKey";
        internal const string ContractCertificateAppSecretName = "ContractCertificate-AppSecret";
        internal const string ContractContextItemName = "$ContractContext$";

        private static HttpContractServer _Server;
        /// <summary>
        /// 获取相关的服务器。
        /// </summary>
        public static HttpContractServer Server { get { return _Server; } }
        /// <summary>
        /// 获取当前线程中的上下文契约。
        /// </summary>
        public static ContractContext Context
        {
            get
            {
                var httpContext = HttpContext.Current;
                var context = httpContext.Items[AoiteWebApi.ContractContextItemName] as ContractContext;
                if(context != null) return context;
                return null;
                //lock(httpContext)
                //{
                //    if(_Server != null) Context = context = CreateEmptyRequestContext(httpContext);
                //}
                //return context;
            }
            internal set
            {
                HttpContext.Current.Items[AoiteWebApi.ContractContextItemName] = value;
            }
        }
        internal static ContractContext CreateEmptyRequestContext(HttpContext httpContext)
        {
            var httpRequest = httpContext.Request;
            var headers = httpRequest.Headers;
            var request = new ContractRequest(_Server.CreateHeadersFactory(headers)
                , _Server.CreateContractCertificate(headers)
                , httpRequest.Cookie(_Server.Configuration.CookieName)
                , null, null, null, null);
            return new ContractContext(_Server, request, httpRequest.GetClientAddress());
        }
        internal static void CreateServer(string confPath = "~/server.conf")
        {
            if(_Server != null) return;
            var config = JsonConf.LoadFromFile<HttpContractConfiguration>(Hosting.HostingEnvironment.MapPath(confPath));
            _Server = new HttpContractServer(config);
        }

        internal static void StopAndReturn(this HttpApplication application)
        {
            var httpResponse = application.Response;
            httpResponse.Flush();
            httpResponse.SuppressContent = true;
            httpResponse.TrySkipIisCustomErrors = true;
            application.CompleteRequest();
        }
    }
}
