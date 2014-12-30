using Aoite.ServiceModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace System.Web
{
    /// <summary>
    /// 定义 Aoite WEB API 应用程序中的所有应用程序对象共有的方法、属性和事件。此类是用户在 Global.asax 文件中所定义的应用程序的基类。
    /// </summary>
    public class AoiteApiHttpApplication : HttpApplication
    {
        /// <summary>
        /// 初始化 <see cref="System.Web.AoiteApiHttpApplication"/> 类的新实例。
        /// </summary>
        /// <param name="confPath">服务的配置文件。</param>
        public AoiteApiHttpApplication(string confPath = "~/server.conf")
        {
            AoiteWebApi.CreateServer(confPath);
            this.BeginRequest += WebApiHttpApplication_BeginRequest;
        }

        void WebApiHttpApplication_BeginRequest(object sender, EventArgs e)
        {
            var response = AoiteWebApi.Server.Process(this);
            if(response == null) return;
            var httpContext = this.Context;
            Action delFileAction = null;
            var httpResponse = httpContext.Response;
            foreach(var item in response.Headers)
            {
                if(item.Value == null) continue;
                httpResponse.Headers.Add(AoiteWebApi.ContractHeaderNamePrefix + item.Key, Convert.ToString(item.Value));
            }
            //- 跨域支持
            //httpResponse.AddHeader("Access-Control-Allow-Methods", "OPTIONS,POST,GET");
            //httpResponse.AddHeader("Access-Control-Allow-Headers", "x-requested-with,content-type");
            //httpResponse.AddHeader("Access-Control-Allow-Origin", "*");
            if(response.Status != StatusCode.OK)
            {
                httpResponse.StatusCode = (int)response.Status;
                httpResponse.Write(response.Message);
            }
            else
            {
                var files = response.Files;
                if(files != null && files.Length > 0)
                {
                    ResponseWithFiles(httpContext, httpResponse, response, files, ref delFileAction);
                }
                else
                {
                    var values = response.Values;
                    if(values != null && values.Length > 0)
                    {
                        var json = fastJSON.JSON.ToJSON(values[0]);
                        httpResponse.ContentType = "application/json";
                        httpResponse.Charset = Serializer.Json.Encoding.WebName;
                        httpResponse.Write(json);
                    }
                }
            }
            this.StopAndReturn();
            if(delFileAction != null) delFileAction();
        }

        static void ResponseWithFiles(HttpContext httpContext
           , HttpResponse httpResponse
           , ContractResponse response
           , ContractFile[] files, ref Action delFileAction)
        {
            var path = files[0].Name;
            if(files.Length > 1)
            {
                var tmpsFolder = httpContext.Server.MapPath("~/aoite_tmps");
                GA.IO.CreateDirectory(tmpsFolder);
                httpResponse.Headers.Add(HttpContractLifeCycle.ZipFileHeaderName, HttpContractLifeCycle.ZipFileHeaderValue);
                path = Path.Combine(tmpsFolder, Guid.NewGuid().ToString() + ".zip");
                delFileAction = () =>
                {
                    int testCount = 0;
                    while(true)
                    {
                        try
                        {
                            File.Delete(path);
                            break;
                        }
                        catch(Exception)
                        {
                            if(testCount++ > 3) throw;
                        }
                    }
                };
                using(Aoite.Zip.ZipFile zip = new Aoite.Zip.ZipFile(path))
                {
                    foreach(var file in files)
                    {
                        zip.AddFile(file.Name);
                    }
                    zip.Save();
                }
                httpResponse.ContentType = "application/x-zip-compressed";
            }
            else httpResponse.ContentType = "application/" + Path.GetExtension(path).Substring(1);

            var fileLength = new FileInfo(path).Length;
            httpResponse.AddHeader("content-disposition", String.Format("attachment;filename={0}", httpContext.Server.UrlEncode(Path.GetFileName(path))));
            httpResponse.AddHeader("content-length", fileLength.ToString());
            httpResponse.TransmitFile(path);
        }
    }
}
