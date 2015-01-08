using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// 表示 <see cref="System.Result"/> 的扩展。
    /// </summary>
    public static class ResultExtensions
    {
        /// <summary>
        /// 如果结果出现异常则抛出错误，否则不做任何处理。
        /// </summary>
        /// <typeparam name="TResult">结果的数据类型。</typeparam>
        /// <param name="result"><see cref="System.Result"/> 的派生类实例。</param>
        public static TResult ThrowIfFailded<TResult>(this TResult result)
            where TResult : Result
        {
            if(result == null) throw new ArgumentNullException("result");

            if(result.IsFailed)
            {
                GA.OnGlobalError(result, result.Exception);
                throw result.Exception;
            }
            return result;
        }

        /// <summary>
        /// 指定错误信息和状态码，将当前结果切换到失败状态。
        /// </summary>
        /// <typeparam name="TResult">结果的数据类型。</typeparam>
        /// <param name="result"><see cref="System.Result"/> 的派生类实例。</param>
        /// <param name="exception">引发异常的 <see cref="System.Exception"/>。如果为 null 值，将不会更改返回结果的状态。</param>
        /// <param name="status">结果的状态码。</param>
        public static TResult ToFailded<TResult>(this TResult result, Exception exception, int status = ResultStatus.Failed)
           where TResult : Result
        {
            if(result == null) throw new ArgumentNullException("result");

            if(exception != null)
            {
                result._Message = exception.Message;
                result._Exception = exception;
                result._Status = status;
            }
            return result;
        }

        /// <summary>
        /// 指定错误信息和状态码，将当前结果切换到失败状态。
        /// </summary>
        /// <typeparam name="TResult">结果的数据类型。</typeparam>
        /// <param name="result"><see cref="System.Result"/> 的派生类实例。</param>
        /// <param name="message">描述错误的信息。如果为 null 值，将不会更改返回结果的状态。</param>
        /// <param name="status">结果的状态码。</param>
        public static TResult ToFailded<TResult>(this TResult result, string message, int status = ResultStatus.Failed)
            where TResult : Result
        {
            if(result == null) throw new ArgumentNullException("result");

            if(message != null)
            {
                result._Message = message;
                result._Status = status;
            }
            return result;
        }

        /// <summary>
        /// 将当前结果切换到成功状态，并且清除结果的错误信息。
        /// </summary>
        /// <typeparam name="TResult">结果的数据类型。</typeparam>
        /// <param name="result"><see cref="System.Result"/> 的派生类实例。</param>
        public static TResult ToSuccessed<TResult>(this TResult result)
            where TResult : Result
        {
            if(result == null) throw new ArgumentNullException("result");

            result._Status = ResultStatus.Succeed;
            result._Message = null;
            result._Exception = null;
            return result;
        }

        /// <summary>
        /// 将当前结果切换到成功的状态，并且清除结果的错误信息。
        /// </summary>
        /// <typeparam name="TResult">结果的数据类型。</typeparam>
        /// <typeparam name="TValue">结果值的数据类型。</typeparam>
        /// <param name="result"><see cref="System.Result"/> 的派生类实例。</param>
        /// <param name="value">结果返回的值。</param>
        public static TResult ToSuccessed<TResult, TValue>(this TResult result, TValue value)
            where TResult : Result<TValue>
        {
            if(result == null) throw new ArgumentNullException("result");

            result._Value = value;
            return ToSuccessed(result);
        }

    }
}
