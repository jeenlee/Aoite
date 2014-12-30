using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Aoite.ServiceModel
{
    /// <summary>
    /// 表示提供 HTTP 会话的支持。
    /// </summary>
    public class HttpSessionProvider : IClientSessionProvider
    {
        /// <summary>
        /// 获取契约域的会话 Cookie 的名称。
        /// </summary>
        public const string DefaultCookieName = "AOITE_SESSION_ID";

        private string _CookieName = DefaultCookieName;
        /// <summary>
        /// 获取或设置契约域的会话 Cookie 的名称。
        /// </summary>
        public string CookieName { get { return this._CookieName; } set { this._CookieName = value; } }

        private TimeSpan _CookieExpires = TimeSpan.FromMinutes(30);
        /// <summary>
        /// 获取或设置此 Cookie 的过期日期和时间。
        /// </summary>
        public TimeSpan CookieExpires { get { return this._CookieExpires; } set { this._CookieExpires = value; } }

        private string _CookiePath = "/";
        /// <summary>
        /// 获取或设置要与当前 Cookie 一起传输的虚拟路径。
        /// </summary>
        public string CookiePath { get { return this._CookiePath; } set { this._CookiePath = value; } }

        private string GetSession(HttpCookieCollection cookies)
        {
            return cookies.AllKeys.IndexOf(this._CookieName) > -1
                ? cookies[this._CookieName].Value
                : null;
        }

        /// <summary>
        /// 获取或设置契约域的会话标识。
        /// </summary>
        public virtual string SessionId
        {
            get
            {
                var context = HttpContext.Current;
                return GetSession(context.Request.Cookies)
                    ?? GetSession(context.Response.Cookies);
            }
            set
            {
                var cookie = HttpContext.Current.Response.Cookies[this._CookieName];
                if(value == null)
                {
                    cookie.Expires = DateTime.Now.AddHours(-1);
                }
                else
                {
                    cookie.Value = value;
                    if(this._CookieExpires != TimeSpan.Zero) cookie.Expires = DateTime.Now.AddMinutes(this._CookieExpires.TotalMinutes);
                }
                cookie.HttpOnly = true;
                cookie.Path = this._CookiePath;
            }
        }

        /// <summary>
        /// 初始化 <see cref="Aoite.ServiceModel.HttpSessionProvider"/> 类的新实例。
        /// </summary>
        /// <param name="extendData">扩展配置。</param>
        public HttpSessionProvider(Dictionary<string, object> extendData)
        {
            var o_CookieExpires = extendData.TryGetValue("CookieExpires");
            var o_CookieName = extendData.TryGetValue("CookieName");
            var o_CookiePath = extendData.TryGetValue("CookiePath");
            if(o_CookieExpires != null)
                this._CookieExpires = TimeSpan.FromSeconds(Convert.ToInt32(o_CookieExpires));
            if(o_CookieName != null)
                this._CookieName = Convert.ToString(o_CookieName);
            if(o_CookiePath != null)
                this._CookiePath = Convert.ToString(o_CookiePath);
        }
    }
}
