using Aoite.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Web
{
    /// <summary>
    /// 表示一个 HTTP 契约服务端的配置信息。
    /// </summary>
    public class HttpContractConfiguration : ServerConfigurationBase
    {
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

        private string _SegmentName = "api";
        /// <summary>
        /// 获取或设置首个路径段的名称。默认为 api。
        /// </summary>
        public string SegmentName { get { return this._SegmentName; } set { this._SegmentName = value == null ? "api" : value.ToLower(); } }

        private string _GlobalServiceName = "aoite";
        /// <summary>
        /// 获取或设置全局的服务名称。这用于客户端脚本生成时使用。默认为“aoite”。
        /// </summary>
        public string GlobalServiceName { get { return this._GlobalServiceName; } set { this._GlobalServiceName = value; } }


        /// <summary>
        /// 初始化一个默认的 <see cref="System.Web.HttpContractConfiguration"/> 类的新实例。
        /// </summary>
        public HttpContractConfiguration() { }
    }
}