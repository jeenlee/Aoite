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
        public ResultException(int status) : this("错误 " + status, status) { }

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

        /// <summary>
        /// 将当前错误转换为 <see cref="System.Result"/> 类的新实例。
        /// </summary>
        /// <returns>返回一个 <see cref="System.Result"/> 类的新实例。</returns>
        public Result ToResult()
        {
            return new Result(this, this.Status);
        }

        /// <summary>
        /// 将当前错误转换为 <see cref="System.Result&lt;TValue&gt;"/> 类的新实例。
        /// </summary>
        /// <typeparam name="TValue">返回值的数据类型。</typeparam>
        /// <returns>返回一个 <see cref="System.Result&lt;TValue&gt;"/> 类的新实例。</returns>
        public Result<TValue> ToResult<TValue>()
        {
            return new Result<TValue>(this, this.Status);
        }

        /// <summary>
        /// 将当前错误转换为 <typeparamref name="TResult" /> 类的新实例。
        /// </summary>
        /// <typeparam name="TResult">结果的数据类型。</typeparam>
        /// <returns>返回一个 <typeparamref name="TResult" /> 类的新实例。</returns>
        public TResult ToCustomResult<TResult>() where TResult : Result
        {
            return (TResult)this.ToCustomResult(typeof(TResult));
        }

        /// <summary>
        /// 将当前错误转换为 <paramref name="resultType"/> 类的新实例。
        /// </summary>
        /// <param name="resultType">结果的数据类型。</param>
        /// <returns>返回一个 <paramref name="resultType"/> 类的新实例。</returns>
        public Result ToCustomResult(Type resultType)
        {
            var result = Activator.CreateInstance(resultType) as Result;
            result.ToFailded(this, this.Status);
            return result;
        }
    }
}
