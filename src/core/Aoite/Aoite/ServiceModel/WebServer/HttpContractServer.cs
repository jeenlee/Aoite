using Aoite.ServiceModel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;

namespace System.Web
{
    /// <summary>
    /// 表示一个 HTTP 契约服务器。
    /// </summary>
    public class HttpContractServer : ContractServerBase, IContractServer
    {
        private HttpContractConfiguration _Configuration;
        /// <summary>
        /// 获取服务器的配置信息。
        /// </summary>
        public HttpContractConfiguration Configuration { get { return this._Configuration; } }

        /// <summary>
        /// HTTP 契约服务器开始执行请求发生。
        /// </summary>
        public event EventHandler BeginHttpRequest;
        /// <summary>
        /// HTTP 契约服务器结束执行请求发生。
        /// </summary>
        public event EventHandler EndHttpRequest;

        /// <summary>
        /// 提供配置信息，初始化一个 <see cref="System.Web.HttpContractServer"/> 类的新实例。
        /// </summary>
        /// <param name="configuration">服务器的配置信息。</param>
        public HttpContractServer(HttpContractConfiguration configuration)
        {
            if(configuration == null) throw new ArgumentNullException("configuration");

            this._Configuration = configuration;

            this.Initiailze();
        }

        /// <summary>
        /// 获取服务器的配置信息。
        /// </summary>
        /// <returns>返回服务器的配置信息。</returns>
        protected override ServerConfigurationBase GetConfiguration()
        {
            return this._Configuration;
        }

        private void ToApiMetadata(HttpApplication application, HttpContractConfiguration config)
        {
            var httpContext = application.Context;
            StringBuilder builder = new StringBuilder();
            var name = config.GlobalServiceName.ToCamelCase();
            builder.AppendLine("(function () {")
                .AppendLine("    if (!console) console = { log: function (m) { } };")
                .Append("    var ").Append(name).Append(" = window.").Append(name).Append(" || (window.").Append(name).AppendLine(" = {});")
                //.AppendLine("    jQuery.support.cors = true; // IE9 以下必须采用此方法。")
                .Append("    ").Append(name).AppendLine(".$settings = {")
                .Append("        baseUrl : \"/").Append(config.SegmentName).AppendLine("/\",")
                .Append("        loginUrl : \"\",")
                .AppendLine(@"        getDefaultResult : function () { return { status: -1, message: ""系统发生了未知异常。"", value: undefined } },
        error: function (status, message) { },
        success: function (data) { },
        remote: function (url, ps) {
            var settings = this.$settings || this;
            var result;
            $.ajax({
                url: settings.baseUrl + url,
                data: JSON.stringify(ps),
                success: function (data, textStatus, jqXHR) {
                    if (settings.success) settings.success(data);
                    result = data;
                },
                contentType: ""application/json; charset=UTF-8"",
                dataType: ""json"",
                type: ""POST"",
                cache: false,
                async: false,
                error: function (jqXHR, status, error) {
                    if (settings.error && settings.error(jqXHR.status, jqXHR.responseText)===false) return;
                    if (jqXHR.status == 401) {
                        top.document.location.href = settings.loginUrl;
                        return;
                    }
                    if(console) {
                    console.log(""错误："" + jqXHR.status);
                    console.log(jqXHR.responseText);
                    }
                    result = settings.getDefaultResult();
                }
            });
            return result;
        }
    }
    ").Append(name).Append(@".remote = aoite.$settings.remote;
})();

(function () {");

            foreach(var contract in config)
            {
                contract.ToJsonMetadata(builder, name);
            }
            builder.AppendLine("})();");

            var httpResponse = httpContext.Response;
            httpResponse.ContentType = "text/javascript";
            httpResponse.Write(builder.ToString());
            application.StopAndReturn();
        }

        internal Func<ConcurrentDictionary<string, object>> CreateHeadersFactory(NameValueCollection headers)
        {
            return () =>
            {
                var cd = new ConcurrentDictionary<string, object>();
                foreach(var key in headers.AllKeys)
                {
                    if(key.StartsWith(AoiteWebApi.ContractHeaderNamePrefix))
                    {
                        cd.TryAdd(key.Substring(AoiteWebApi.ContractHeaderNamePrefix.Length), headers[key]);
                    }
                }
                return cd;
            };
        }
        internal ContractCertificate CreateContractCertificate(NameValueCollection headers)
        {
            return new ContractCertificate()
            {
                AppKey = headers.Get(AoiteWebApi.ContractCertificateAppKeyName),
                AppSecret = headers.Get(AoiteWebApi.ContractCertificateAppSecretName)
            };
        }

        private object CreateContext(HttpApplication application, HttpContractConfiguration config)
        {
            var httpContext = application.Context;
            var httpRequest = httpContext.Request;
            var segments = httpRequest.Url.Segments;
            if(segments.Length < 2 || segments[1].ToLower() != (config.SegmentName + "/").ToLower()) return null;
            if(segments.Length == 3)
            {
                var m = segments[2].ToLower();
                if(m == "metadata" || m == "metadata.js")
                {
                    this.ToApiMetadata(application, config);
                    return null;
                }
            }
            if(segments.Length != 4) return ContractContext.NewResponse(StatusCode.BadRequest, "请求无效。");

            var contractName = segments[2];
            if(contractName.Last() == '/') contractName = contractName.RemoveEnd();
            var contractService = config.GetService(contractName);
            if(contractService == null) return ContractContext.NewResponse(StatusCode.NotFound, "找不到契约。");

            var methodName = segments[3];
            if(methodName.Last() == '/') methodName = methodName.RemoveEnd();
            var contractMethods = contractService.FindMethods(methodName);
            if(contractMethods == null) return ContractContext.NewResponse(StatusCode.NotFound, "找不到方法。");
            if(contractMethods.Count > 1) return ContractContext.NewResponse(StatusCode.BadRequest, "发现重名方法，请联系服务端管理员整改。");

            var method = contractMethods[0];
            object[] parameters;

            try
            {
                parameters = this.CreateParameters(httpRequest, method);
            }
            catch(Exception ex)
            {
                this.OnError(ex);
                return ContractContext.NewResponse(StatusCode.BadRequest, "参数无效。");
            }

            var sessionId = httpRequest.Cookie(config.CookieName);
            if(string.IsNullOrEmpty(sessionId)) sessionId = httpRequest.QueryString.Get(config.CookieName);
            // URL Session 的支持

            var headers = httpRequest.Headers;
            var files = this.CreateFiles(httpRequest);

            var cRequest = new ContractRequest(this.CreateHeadersFactory(headers),
                this.CreateContractCertificate(headers)
                , sessionId, contractName, methodName, parameters, files);

            ContractContext context = new ContractContext(this, cRequest, httpRequest.GetClientAddress());
            context._Method = method;
            context._Service = contractService;
            return context;
        }

        internal ContractResponse Process(HttpApplication application)
        {
            this.OnBeginHttpRequest(application);
            try
            {
                var httpContext = application.Context;
                var config = this._Configuration;

                var value = this.CreateContext(application, config);

                if(value == null) return null;

                if(value is ContractResponse) return value as ContractResponse;
                var context = value as ContractContext;
                AoiteWebApi.Context = context;

                if(!this.HasResponse(this.OnCalling, context)) this.OnCall(context);
                this.OnCalled(context);
                var sessionId = context.Source.Response.SessionId;
                if(!string.IsNullOrEmpty(sessionId))
                    httpContext.Request.Cookie(config.CookieName
                        , sessionId
                        , DateTime.Now.AddSeconds(this._Configuration.SessionExpiration)
                        , null
                        , true);
                return context.Source.Response;
            }
            catch(Exception ex)
            {
                this.OnError(ex);
                return ContractContext.NewResponse(StatusCode.InternalServerError, this.GetErrorMessage(ex));
            }
            finally
            {
                this.OnEndHttpRequest(application);
            }
        }

        private void AddParameters(Dictionary<string, string> httpPatamters, NameValueCollection collection)
        {
            for(int i = 0; i < collection.Count; i++)
            {
                httpPatamters[collection.GetKey(i)] = collection[i];
            }
        }

        private object ChangeType(object value, Type realType)
        {
            if(realType.IsInstanceOfType(value)) return value;

            if(realType.IsPrimitive
                || realType == Types.Decimal
                || realType == Types.TimeSpan
                || realType == Types.Uri
                || realType == Types.Guid)
                return value.CastTo(realType);

            if(realType.IsEnum)
                return realType.ToEnumValue(value);

            if(value is string)
            {
                return fastJSON.JSON.ToObject(Convert.ToString(value), realType);
            }
            else if(value is Dictionary<string, object>)
            {
                return fastJSON.JSON.ToObject(value as Dictionary<string, object>, realType);
            }
            else if(realType.IsArray && value is List<object>)
            {
                return fastJSON.JSON.ToObject(value as List<object>, realType);
            }
            throw new NotSupportedException("无法将类型 {0} 转换为 {1}。".Fmt(value.GetType().FullName, realType.FullName));
        }

        private object[] CreateParametersWithJson(ContractMethod method, string json)
        {
            var jsonPatamters = fastJSON.JSON.ToObject<Dictionary<string, object>>(json);
            var pLength = method.ParameterTypes.Length;

            var parameters = new object[pLength];
            for(int i = 0; i < pLength; i++)
            {
                var pType = method.ParameterTypes[i];
                Type realType;
                bool allowNull;
                if(pType.IsValueType)
                {
                    realType = pType.GetNullableType();
                    allowNull = pType != realType;
                }
                else
                {
                    realType = pType;
                    allowNull = pType.IsClass;
                }

                var pName = method.ParameterNames[i];
                object value;
                if(jsonPatamters.TryGetValue(pName, out value) && value != null)
                {
                    parameters[i] = ChangeType(value, realType);
                }
                else parameters[i] = allowNull ? null : pType.GetDefaultValue();

            }
            return parameters;
        }

        private object[] CreateParameters(HttpRequest httpRequest, ContractMethod method)
        {
            var httpPatamters = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
            if(httpRequest.HttpMethod == "GET")
            {
                this.AddParameters(httpPatamters, httpRequest.QueryString);
            }
            else
            {
                if(httpRequest.ContentType.iContains("json"))
                {
                    using(Stream receiveStream = httpRequest.InputStream)
                    {
                        Encoding encoding = httpRequest.ContentEncoding ?? Encoding.UTF8;
                        using(StreamReader readStream = new StreamReader(receiveStream, encoding))
                        {
                            var json = readStream.ReadToEnd();
                            return this.CreateParametersWithJson(method, json);
                        }
                    }
                }
                else this.AddParameters(httpPatamters, httpRequest.Form);
            }

            var pLength = method.ParameterTypes.Length;
            var parameters = new object[pLength];
            for(int i = 0; i < pLength; i++)
            {
                var pType = method.ParameterTypes[i];
                Type realType;
                bool allowNull;
                if(pType.IsValueType)
                {
                    realType = pType.GetNullableType();
                    allowNull = pType != realType;
                }
                else
                {
                    realType = pType;
                    allowNull = pType.IsClass;
                }

                var pName = method.ParameterNames[i];
                string value;
                if(httpPatamters.TryGetValue(pName, out value) && value.Length > 0)
                {
                    parameters[i] = ChangeType(value, realType);
                }
                else parameters[i] = allowNull ? null : pType.GetDefaultValue();

            }
            return parameters;
        }

        private ContractFile[] CreateFiles(HttpRequest request)
        {
            var httpFiles = request.Files;
            var fileCount = httpFiles.Count;
            if(fileCount == 0) return null;
            List<ContractFile> cFiles = new List<ContractFile>();

            for(int i = 0; i < fileCount; i++)
            {
                var httpFile = httpFiles[i];
                if(httpFile.ContentLength == 0) continue;
                cFiles.Add(new ContractFile(httpFile.FileName, httpFile.ContentLength, httpFile.InputStream, false));
            }
            return cFiles.ToArray();
        }

        /// <summary>
        /// HTTP 契约服务器开始执行请求发生的方法。
        /// </summary>
        /// <param name="application">当前的 <see cref="System.Web.HttpApplication"/> 的实例。</param>
        protected virtual void OnBeginHttpRequest(HttpApplication application)
        {
            var ev = this.BeginHttpRequest;
            if(ev != null) ev(application, EventArgs.Empty);
        }

        /// <summary>
        /// HTTP 契约服务器结束执行请求发生的方法。
        /// </summary>
        /// <param name="application">当前的 <see cref="System.Web.HttpApplication"/> 的实例。</param>
        protected virtual void OnEndHttpRequest(HttpApplication application)
        {
            var ev = this.EndHttpRequest;
            if(ev != null) ev(application, EventArgs.Empty);
        }
    }
}