using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// 表示包含一个返回值的结果。
    /// </summary>
    /// <typeparam name="TValue">返回值的数据类型。</typeparam>
    [Serializable]
    public class Result<TValue> : Result, IResult<TValue>
    {

        /// <summary>
        /// 表示一个操作结果返回值的类型。
        /// </summary>
        public static readonly Type ValueType = typeof(TValue);

        /// <summary>
        /// 获取或设置结果的返回值。
        /// </summary>
        protected internal TValue _Value;
        /// <summary>
        /// 获取或设置结果的返回值。
        /// </summary>
        public virtual TValue Value { get { return this._Value; } set { this.ToSuccessed(value); } }

        /// <summary>
        /// 获取一个值，表示结果的返回值。若当前结果包含错误，将会抛出异常。
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        [Ignore]
        public TValue UnsafeValue
        {
            get
            {
                if(this.IsFailed)
                {
                    GA.OnGlobalError(this, this.Exception);
                    throw this.Exception;
                }
                return this._Value;
            }
        }

        /// <summary>
        /// 初始化一个 <see cref="System.Result&lt;TValue&gt;"/> 类的新实例。
        /// </summary>
        public Result() { }

        /// <summary>
        /// 指定结果的返回值，初始化一个 <see cref="System.Result&lt;TValue&gt;"/> 类的新实例。
        /// </summary>
        /// <param name="value">结果的返回值。</param>
        public Result(TValue value) { this._Value = value; }

        /// <summary>
        /// 指定引发的异常和状态码，初始化一个 <see cref="System.Result&lt;TValue&gt;"/> 类的新实例。
        /// </summary>
        /// <param name="exception">引发异常的 <see cref="System.Exception"/>。</param>
        /// <param name="status">结果的状态码。</param>
        public Result(Exception exception, int status = ResultStatus.Failed) : base(exception, status) { }

        /// <summary>
        /// 指定描述错误的信息和状态码，初始化一个 <see cref="System.Result&lt;TValue&gt;"/> 类的新实例。
        /// </summary>
        /// <param name="message">描述错误的信息。</param>
        /// <param name="status">结果的状态码。</param>
        public Result(string message, int status = ResultStatus.Failed) : base(message, status) { }

        /// <summary>
        /// 返回以字符串形式描述的结果。
        /// </summary>
        /// <returns>如果这是一个成功的操作结果，将返回字符串形式的值，否则返回异常的描述信息。</returns>
        public override string ToString()
        {
            if(this.IsSucceed)
            {
                if(this._Value == null) return NullValueString;
                return this._Value.ToString();
            }
            return base.ToString();
        }

        /// <summary>
        /// <see cref="System.Result&lt;TValue&gt;"/> 和 <see cref="System.Exception"/> 的隐式转换。
        /// </summary>
        /// <param name="exception">引发异常的 <see cref="System.Exception"/>。</param>
        /// <returns>表示一个异常的结果。</returns>
        public static implicit operator Result<TValue>(Exception exception)
        {
            return new Result<TValue>(exception);
        }

        /// <summary>
        /// <see cref="System.Result&lt;TValue&gt;"/> 和 <see cref="System.String"/> 的隐式转换。
        /// </summary>
        /// <param name="message">描述错误的信息。</param>
        /// <returns>表示一个异常的结果。</returns>
        public static implicit operator Result<TValue>(string message)
        {
            return new Result<TValue>(message);
        }

        /// <summary>
        /// <see cref="System.Result&lt;TValue&gt;"/> 和 <typeparamref name="TValue"/> 的隐式转换。
        /// </summary>
        /// <param name="value">结果的返回值。</param>
        /// <returns>表示包含返回值的结果。</returns>
        public static implicit operator Result<TValue>(TValue value)
        {
            return new Result<TValue>(value);
        }

        internal override object GetValue()
        {
            return this._Value;
        }
        internal override void SetValue(object value)
        {
            this._Value = (TValue)value;
        }
        internal override Type GetValueType()
        {
            return ValueType;
        }
    }
}
