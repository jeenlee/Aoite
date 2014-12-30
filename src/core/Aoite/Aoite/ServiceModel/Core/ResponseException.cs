using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Aoite.ServiceModel
{
    /// <summary>
    /// 表示服务响应的错误。
    /// </summary>
    [Serializable]
    public class ResponseException : Exception
    {
        private StatusCode _Status;
        /// <summary>
        /// 获取响应的状态码。
        /// </summary>
        public StatusCode Status { get { return this._Status; } }

        /// <summary>
        /// 使用状态码和错误描述信息初始化一个 <see cref="Aoite.ServiceModel.ResponseException"/> 类的新实例。
        /// </summary>
        /// <param name="status">响应的状态码。</param>
        /// <param name="message">描述错误的消息。</param>
        public ResponseException(StatusCode status, string message)
            : base(message)
        {
            this._Status = status;
        }

        /// <summary>
        /// 用序列化数据初始化 <see cref="Aoite.ServiceModel.ResponseException"/> 类的新实例。
        /// </summary>
        /// <param name="info">有关所引发异常的序列化的对象数据。</param>
        /// <param name="context">有关源或目标的上下文信息。</param>
        protected ResponseException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this._Status = (StatusCode)info.GetInt32("$Status");
        }

        /// <summary>
        /// 用关于异常的信息设置 <see cref="System.Runtime.Serialization.SerializationInfo"/>。
        /// </summary>
        /// <param name="info">有关所引发异常的序列化的对象数据。</param>
        /// <param name="context">有关源或目标的上下文信息。</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("$Status", (int)this._Status);
        }
    }
}
