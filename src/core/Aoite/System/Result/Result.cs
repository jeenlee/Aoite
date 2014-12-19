using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// 表示一个结果。
    /// </summary>
    [Serializable]
    public class Result : IResult
    {
        internal const string SuccessedString = "执行成功！";
        internal const string NullValueString = "[null]";
        /// <summary>
        /// 表示成功、且无法修改的结果。
        /// </summary>
        public readonly static Result Successfully = new SuccessfullyResult();

        internal string _Message;
        [NonSerialized, Ignore]
        internal Exception _Exception;
        internal int _Status = ResultStatus.Succeed;

        /// <summary>
        /// 获取或设置执行结果描述错误的信息。
        /// </summary>
        public virtual string Message { get { return this._Message; } set { this.ToFailded(value); } }
        /// <summary>
        /// 获取或设置执行时发生的错误。结果状态 <see cref="System.ResultStatus.Succeed"/> 时，该值为 null 值。
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public virtual Exception Exception
        {
            get
            {
                if(this._Exception == null && this.IsFailed)
                    this._Exception = new ResultException(this._Message, this._Status);
                return this._Exception;
            }
            set { this.ToFailded(value); }
        }

        /// <summary>
        /// 获取一个值，表示执行结果是否为失败。
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public virtual bool IsFailed { get { return this._Status != ResultStatus.Succeed; } }
        /// <summary>
        /// 获取一个值，表示执行结果是否为成功。
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public virtual bool IsSucceed { get { return this._Status == ResultStatus.Succeed; } }
        /// <summary>
        /// 获取执行的状态码。
        /// </summary>
        public virtual int Status { get { return this._Status; } }

        /// <summary>
        /// 初始化一个 <see cref="System.Result"/> 类的新实例。
        /// </summary>
        public Result() { }

        /// <summary>
        /// 指定引发的异常和状态码，初始化一个 <see cref="System.Result"/> 类的新实例。
        /// </summary>
        /// <param name="exception">引发异常的 <see cref="System.Exception"/>。如果为 null 值，将不会更改返回结果的状态。</param>
        /// <param name="status">结果的状态码。</param>
        public Result(Exception exception, int status = ResultStatus.Failed)
        {
            this.ToFailded(exception, status);
        }

        /// <summary>
        /// 指定描述错误的信息和状态码，初始化一个 <see cref="System.Result"/> 类的新实例。
        /// </summary>
        /// <param name="mssage">描述错误的信息。如果为 null 值，将不会更改返回结果的状态。</param>
        /// <param name="status">结果的状态码。</param>
        public Result(string mssage, int status = ResultStatus.Failed)
        {
            this.ToFailded(mssage, status);
        }

        /// <summary>
        /// 返回以字符串形式描述的结果。
        /// </summary>
        /// <returns>如果这是一个成功的操作结果，将返回“执行成功！”，否则返回异常的描述信息。</returns>
        public override string ToString()
        {
            if(this.IsSucceed) return Result.SuccessedString;
            return this._Message;
        }

        internal virtual object GetValue() { return null; }
        internal virtual void SetValue(object value) { }
        internal virtual Type GetValueType() { return null; }

        #region implicit operator

        /// <summary>
        /// <see cref="System.Result"/> 和 <see cref="System.Exception"/> 的隐式转换。
        /// </summary>
        /// <param name="exception">引发异常的 <see cref="System.Exception"/>。</param>
        /// <returns>表示一个异常的结果。</returns>
        public static implicit operator Result(Exception exception)
        {
            if(exception == null) return Result.Successfully;
            return new Result(exception);
        }

        /// <summary>
        /// <see cref="System.Result"/> 和 <see cref="System.String"/> 的隐式转换。
        /// </summary>
        /// <param name="message">描述异常结果的信息。</param>
        /// <returns>表示一个异常的结果。</returns>
        public static implicit operator Result(string message)
        {
            if(message == null) return Result.Successfully;
            return new Result(message);
        }

        /// <summary>
        /// <see cref="System.String"/> 和 <see cref="System.Result"/> 的隐式转换。
        /// </summary>
        /// <param name="result">返回结果。</param>
        /// <returns>返回字符串形式的结果。如果该结果为 null 值，则返回 null 值。</returns>
        public static implicit operator string(Result result)
        {
            if(result == null) return null;
            return result.ToString();
        }

        #endregion
    }
}
