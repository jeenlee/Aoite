using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.ServiceModel
{
    /// <summary>
    /// 表示一个契约的响应。
    /// </summary>
    public class ContractResponse : HeadersBase
    {
        internal string _Id;
        /// <summary>
        /// 获取请求的唯一标识。
        /// </summary>
        public string Id { get { return this._Id; } }
        internal object[] _Values;
        /// <summary>
        /// 获取响应的返回值。
        /// </summary>
        public object[] Values { get { return this._Values; } }

        internal StatusCode _Status;
        /// <summary>
        /// 获取响应的状态码。
        /// </summary>
        public StatusCode Status { get { return this._Status; } }

        internal string _Message;
        /// <summary>
        /// 表示响应的消息。
        /// </summary>
        public string Message { get { return this._Message; } }

        internal string _SessionId;
        /// <summary>
        /// 获取响应的会话标识。如果标识不为空，则会替换当前契约域的会话标识。
        /// </summary>
        public string SessionId
        {
            get { return this._SessionId; }
            internal set { this._SessionId = value; }
        }

        internal ContractFile[] _Files;
        /// <summary>
        /// 获取响应的文件列表。
        /// </summary>
        public ContractFile[] Files { get { return this._Files; } }
        internal ContractResponse() { }
    }
}
