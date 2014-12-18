using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// 表示一个 <see cref="System.Result"/> 的异常。
    /// </summary>
    public class ResultException : Exception
    {
        /// <summary>
        /// 获取或设置执行的状态码。
        /// </summary>
        public int Status { get { return base.HResult; } }

        /// <summary>
        /// 初始化一个 <see cref="System.ResultException"/> 类的新实例。
        /// </summary>
        /// <param name="status">结果的状态码。</param>
        public ResultException(int status) : this("错误 " + status + "。", status) { }

        /// <summary>
        /// 初始化一个 <see cref="System.ResultException"/> 类的新实例。
        /// </summary>
        /// <param name="message">描述错误的信息。</param>
        /// <param name="status">结果的状态码。</param>
        public ResultException(string message, int status = ResultStatus.Failed)
            : base(message)
        {
            if(status == ResultStatus.Succeed) throw new ArgumentOutOfRangeException("status", "不能用 " + ResultStatus.Succeed + " 表示一个错误的状态，因为 " + ResultStatus.Succeed + " 被认定为成功的状态。");
            base.HResult = status;
        }
    }
}
