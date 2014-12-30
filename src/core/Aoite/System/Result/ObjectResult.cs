namespace System
{
    /// <summary>
    /// 表示包含一个 <see cref="System.Object"/> 类型返回值的结果。
    /// </summary>
    [Serializable]
    public class ObjectResult : Result<object>
    {
        /// <summary>
        /// 初始化一个 <see cref="System.ObjectResult"/> 类的新实例。
        /// </summary>
        public ObjectResult() { }

        /// <summary>
        /// 指定结果的返回值，初始化一个 <see cref="System.ObjectResult"/> 类的新实例。
        /// </summary>
        /// <param name="value">结果的返回值。</param>
        public ObjectResult(object value) : base(value) { }

        /// <summary>
        /// 指定结果的返回值和引发的异常，初始化一个 <see cref="System.ObjectResult"/> 类的新实例。
        /// </summary>
        /// <param name="value">结果的返回值。只有当 <paramref name="exception"/> 为 null 时，此参数才会有效。</param>
        /// <param name="exception">引发异常的 <see cref="System.Exception"/>。</param>
        public ObjectResult(object value, Exception exception) : base(value, exception) { }

        /// <summary>
        /// 指定引发的异常，初始化一个 <see cref="System.ObjectResult"/> 类的新实例。
        /// </summary>
        /// <param name="exception">引发异常的 <see cref="System.Exception"/>。</param>
        public ObjectResult(Exception exception) : base(exception) { }

        /// <summary>
        /// <see cref="System.ObjectResult"/> 和 <see cref="System.Exception"/> 的隐式转换。
        /// </summary>
        /// <param name="exception">引发异常的 <see cref="System.Exception"/>。</param>
        /// <returns>表示一个异常的结果。</returns>
        public static implicit operator ObjectResult(Exception exception)
        {
            return new ObjectResult(exception);
        }
    }
}