using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace System.Web
{
    /// <summary>
    /// 基于 Web 的扩展方法。
    /// </summary>
    public static class WebExtensions
    {
        /// <summary>
        /// 获取请求客户端地址。
        /// </summary>
        /// <param name="request">当前 HTTP 请求。</param>
        /// <returns>返回一个客户端地址。</returns>
        public static string GetClientAddress(this HttpRequest request)
        {
            string ipAddress = request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if(!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if(addresses.Length != 0)
                {
                    return addresses[0];
                }
            }
            ipAddress = request.ServerVariables["REMOTE_ADDR"];
            if(string.IsNullOrEmpty(ipAddress)) ipAddress = request.UserHostAddress;
            return ipAddress;
        }
    }
}
