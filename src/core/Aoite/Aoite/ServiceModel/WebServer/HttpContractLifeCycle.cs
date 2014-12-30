using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace Aoite.ServiceModel
{
    /// <summary>
    /// 表示一个基于 HTTP 的契约客户端生命周期。
    /// </summary>
    public class HttpContractLifeCycle : IContractLifeCycle
    {
        internal const string ZipFileHeaderName = "Aoite-Zip-File";
        internal const string ZipFileHeaderValue = "1";
        static HttpContractLifeCycle()
        {
            ServicePointManager.Expect100Continue = false;
            System.Net.WebRequest.DefaultWebProxy = null;
            ServicePointManager.DefaultConnectionLimit = 16;
        }

        private Uri _url;
        private ContractDomain _domain;
        private IContractClient _Client;
        /// <summary>
        /// 设置或获取所属的契约客户端。
        /// </summary>
        public IContractClient Client { get { return this._Client; } set { this._Client = value; } }
        /// <summary>
        /// 提供契约的域，初始化一个 <see cref="Aoite.ServiceModel.HttpContractLifeCycle"/> 类的新实例。
        /// </summary>
        /// <param name="domain">契约的域。</param>
        public HttpContractLifeCycle(ContractDomain domain)
        {
            this._domain = domain;
            this._url = new Uri(domain.Host);
        }

        private void PostMultipart(HttpWebRequest httpRequest, ContractRequest request, ContractFile[] files)
        {
            var ps = request.Parameters;
            var encoding = Serializer.Json.Encoding;
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            httpRequest.ContentType = "multipart/form-data; boundary=" + boundary;
            httpRequest.KeepAlive = true;
            boundary = "\r\n--" + boundary;
            using(var stream = httpRequest.GetRequestStream())
            {
                if(ps.Length > 0)
                {
                    string formTemplate = boundary + "\r\nContent-Disposition: form-data; name=\"{0}\";\r\n\r\n{1}\r\n";
                    var ns = request._method.ParameterNames;
                    var ts = request._method.ParameterTypes;

                    for(int i = 0; i < ns.Length; i++)
                    {
                        var realType = ts[i].GetNullableType();
                        string value;
                        switch(Type.GetTypeCode(realType))
                        {
                            case TypeCode.Object:
                                value = fastJSON.JSON.ToJSON(ps[i]);
                                break;
                            default:
                                value = Convert.ToString(ps[i]);
                                break;
                        }
                        stream.WriteBytes(encoding.GetBytes(formTemplate.Fmt(ns[i], value)));
                    }
                }
                string fileTemplate = boundary + "\r\nContent-Disposition: form-data; name=\"{0}\";filename=\"{1}\"\r\nContent-Type:application/octet-stream\r\n\r\n";

                foreach(var file in files)
                {
                    stream.WriteBytes(encoding.GetBytes(fileTemplate.Fmt(file.Name, file.FullName)));
                    stream.WriteBytes(file._FileBytes);
                }

                stream.WriteBytes(encoding.GetBytes("\r\n" + boundary + "--\r\n"));
            }
        }
        private void PostForm(HttpWebRequest httpRequest, ContractRequest request)
        {
            var ps = request.Parameters;
            var encoding = Serializer.Json.Encoding;
            httpRequest.ContentType = "application/x-www-form-urlencoded";

            if(ps.Length > 0)
            {
                var ns = request._method.ParameterNames;
                var ts = request._method.ParameterTypes;

                var form = HttpUtility.ParseQueryString(string.Empty);
                for(int i = 0; i < ns.Length; i++)
                {
                    var realType = ts[i].GetNullableType();
                    switch(Type.GetTypeCode(realType))
                    {
                        case TypeCode.Object:
                            form.Add(ns[i], fastJSON.JSON.ToJSON(ps[i]));
                            break;
                        default:
                            form.Add(ns[i], Convert.ToString(ps[i]));
                            break;
                    }
                }
                var bytes = encoding.GetBytes(form.ToString());
                httpRequest.ContentLength = bytes.Length;

                using(var stream = httpRequest.GetRequestStream())
                {
                    stream.WriteBytes(bytes);
                }
            }
            else httpRequest.ContentLength = 0;
        }

        private void ResponseFiles(HttpWebResponse httpResponse, ContractResponse response, Type returnType, string contentDisposition)
        {
            using(var responseStream = httpResponse.GetResponseStream())
            {
                if(httpResponse.Headers[ZipFileHeaderName] == ZipFileHeaderValue)
                {
                    using(var zipStream = new System.IO.MemoryStream((int)httpResponse.ContentLength))
                    {
                        responseStream.CopyTo(zipStream);
                        zipStream.Position = 0;
                        using(var zip = Aoite.Zip.ZipFile.Read(zipStream))
                        {
                            response._Files = new ContractFile[zip.Count];
                            for(int i = 0; i < zip.Count; i++)
                            {
                                var file = zip[i];
                                response._Files[i] = new ContractFile(file.FileName, (int)file.UncompressedSize, zip.ReadStream);
                            }
                        }
                    }
                }
                else response._Files = new ContractFile[] { new ContractFile(contentDisposition.Split('=')[1], (int)httpResponse.ContentLength, responseStream) };
            }

            if(returnType == Types.Result)
            {
                response._Values[0] = Result.Successfully;
            }
        }
        private void ResponseValue(HttpWebResponse httpResponse, ContractResponse response, Type returnType)
        {
            if(returnType != Types.Void)
            {
                string json;
                using(var responseStream = httpResponse.GetResponseStream())
                {
                    using(var reader = new System.IO.StreamReader(responseStream))
                    {
                        json = reader.ReadToEnd();
                    }
                }
                response._Values[0] = fastJSON.JSON.ToObject(json, returnType);
            }
        }

        private void OnResponse(HttpWebResponse httpResponse, ContractResponse response, Type returnType)
        {
            if(httpResponse.StatusCode == HttpStatusCode.OK)
            {
                response._Status = StatusCode.OK;
                var httpHeaders = httpResponse.Headers;
                foreach(var key in httpHeaders.AllKeys)
                {
                    if(key.StartsWith(AoiteWebApi.ContractHeaderNamePrefix))
                    {
                        response.Headers.TryAdd(key.Substring(AoiteWebApi.ContractHeaderNamePrefix.Length), httpHeaders[key]);
                    }
                }

                var sessionCookie = httpResponse.Cookies[this._domain.CookieName];
                if(sessionCookie != null) response._SessionId = sessionCookie.Value;

                var contentDisposition = httpResponse.GetResponseHeader("content-disposition");

                if(!string.IsNullOrEmpty(contentDisposition))
                {
                    this.ResponseFiles(httpResponse, response, returnType, contentDisposition);
                }
                else
                {
                    this.ResponseValue(httpResponse, response, returnType);
                }
            }
            else
            {
                switch(httpResponse.StatusCode)
                {
                    case HttpStatusCode.BadRequest:
                        response._Status = StatusCode.BadRequest;
                        break;
                    case HttpStatusCode.Unauthorized:
                        response._Status = StatusCode.Unauthorized;
                        break;
                    case HttpStatusCode.NotFound:
                        response._Status = StatusCode.NotFound;
                        break;
                    default:
                        response._Status = StatusCode.InternalServerError;
                        break;
                }
                using(var responseStream = httpResponse.GetResponseStream())
                {
                    using(var reader = new System.IO.StreamReader(responseStream))
                    {
                        response._Message = reader.ReadToEnd();
                    }
                }
            }
        }

        /// <summary>
        /// 获取指定请求的响应。
        /// </summary>
        /// <param name="request">契约的请求。</param>
        /// <returns>返回一个请求的响应。</returns>
        public ContractResponse GetResponse(ContractRequest request)
        {
            var ps = request.Parameters;
            var encoding = Serializer.Json.Encoding;
            var response = new ContractResponse()
            {
                _Id = request.Id,
                _Values = new object[ps.Length + 1]
            };

            try
            {
                var url = new Uri(this._url, request.ContractName + "/" + request.MethodName);
                var httpRequest = HttpWebRequest.Create(url) as HttpWebRequest;
                httpRequest.Method = "POST";

                if(request.SessionId != null)
                    httpRequest.Headers.Add("Cookie", new Cookie(this._domain.CookieName, request.SessionId).ToString());

                if(request.HasHeaders)
                {
                    foreach(var item in request.Headers)
                    {
                        httpRequest.Headers.Add(AoiteWebApi.ContractHeaderNamePrefix + item.Key, Convert.ToString(item.Value));
                    }
                }

                var files = request.Files;
                if(files != null && files.Length > 0) this.PostMultipart(httpRequest, request, files);
                else this.PostForm(httpRequest, request);

                var returnType = request._method.ReturnType;
                using(var httpResponse = httpRequest.GetResponse() as HttpWebResponse)
                {
                    this.OnResponse(httpResponse, response, returnType);
                }

            }
            catch(Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex);
#if DEBUG
                throw;
#else
                response._Status = StatusCode.InternalServerError;
                response._Message = ex.Message;
#endif
            }
            return response;
        }

        /// <summary>
        /// 打开契约客户端的连接。
        /// </summary>
        /// <returns>返回一个结果。</returns>
        public Result Open()
        {
            return Result.Successfully;
        }

        /// <summary>
        /// 关闭契约客户端的连接。
        /// </summary>
        public void Close() { }

        void IDisposable.Dispose() { }
    }
}
