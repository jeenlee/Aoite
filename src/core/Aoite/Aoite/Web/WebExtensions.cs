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
        private static string RootPath = GA.MapUrl("~/");

        /// <summary>
        /// 新建或更新客户端的 Cookie。
        /// </summary>
        /// <param name="request">当前 HTTP 请求。</param>
        /// <param name="name">新 Cookie 的名称。</param>
        /// <param name="value">新 Cookie 的值。如果值为 null 值，则表示移除该项。</param>
        /// <param name="path">要与当前 Cookie 一起传输的虚拟路径。</param>
        /// <param name="httpOnly">指定 Cookie 是否可通过客户端脚本访问。</param>
        public static void Cookie(this HttpRequest request, string name, string value, string path = null, bool httpOnly = false)
        {
            Cookie(request, name, value, DateTime.Now.AddDays(3), path ?? RootPath, httpOnly);
        }

        /// <summary>
        /// 新建或更新客户端的 Cookie。
        /// </summary>
        /// <param name="request">当前 HTTP 请求。</param>
        /// <param name="name">新 Cookie 的名称。</param>
        /// <param name="value">新 Cookie 的值。如果值为 null 值，则表示移除该项。</param>
        /// <param name="expires">此 Cookie 的过期日期和时间。</param>
        /// <param name="path">要与当前 Cookie 一起传输的虚拟路径。</param>
        /// <param name="httpOnly">指定 Cookie 是否可通过客户端脚本访问。</param>
        public static void Cookie(this HttpRequest request, string name, string value, DateTime expires, string path = null, bool httpOnly = false)
        {
            var response = request.RequestContext.HttpContext.Response;

            var cookie = response.Cookies[name];
            cookie.Value = value;
            cookie.Expires = value == null ? DateTime.Now.AddYears(-1) : expires;
            cookie.HttpOnly = httpOnly;
            if(path != null) cookie.Path = path;
        }

        /// <summary>
        /// 指定名称，获取客户端的 Cookie 值。
        /// </summary>
        /// <param name="request">当前 HTTP 请求。</param>
        /// <param name="name">Cookie 的名称。</param>
        /// <returns>返回 Cookie 的值。默认值为 <see cref="System.String.Empty"/>。</returns>
        public static string Cookie(this HttpRequest request, string name)
        {
            var cookie = request.Cookies[name];
            if(cookie == null || cookie.Value == null) return string.Empty;
            return cookie.Value;
        }

        /// <summary>
        /// 指定名称，移除客户端的 Cookie 值。
        /// </summary>
        /// <param name="request">当前 HTTP 请求。</param>
        /// <param name="name">Cookie 的名称。</param>
        /// <returns>返回 Cookie 的值。默认值为 <see cref="System.String.Empty"/>。</returns>
        public static string CookieRemove(this HttpRequest request, string name)
        {
            var value = Cookie(request, name);
            Cookie(request, name, null, DateTime.Now);
            return value;
        }

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
