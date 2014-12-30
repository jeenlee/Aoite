using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.ServiceModel
{
    /// <summary>
    /// 表示提供应用程序单一性会话的支持。
    /// </summary>
    public class SingleSessionProvider : IClientSessionProvider
    {
        /// <summary>
        /// 获取或设置契约域的会话标识。
        /// </summary>
        public string SessionId { get; set; }
        /// <summary>
        /// 初始化一个 <see cref="Aoite.ServiceModel.SingleSessionProvider"/> 类的新实例。
        /// </summary>
        public SingleSessionProvider() { }
    }

    /// <summary>
    /// 表示提供应用程序多线程会话的支持。
    /// </summary>
    public class MutilationSessionProvider : IClientSessionProvider
    {
        private System.Threading.ThreadLocal<string> _SessionId = new System.Threading.ThreadLocal<string>();
        /// <summary>
        /// 获取或设置契约域的会话标识。
        /// </summary>
        public string SessionId { get { return this._SessionId.Value; } set { this._SessionId.Value = value; } }
        
        /// <summary>
        /// 初始化一个 <see cref="Aoite.ServiceModel.MutilationSessionProvider"/> 类的新实例。
        /// </summary>
        public MutilationSessionProvider() { }
    }
}
