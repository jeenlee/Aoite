namespace System
{
    /// <summary>
    /// 表示异常信息的事件方法。
    /// </summary>
    /// <param name="sender">对象。</param>
    /// <param name="e">参数。</param>
    public delegate void ExceptionEventHandler(object sender, ExceptionEventArgs e);

    /// <summary>
    /// 表示异常信息的事件参数。
    /// </summary>
    public class ExceptionEventArgs : EventArgs
    {
        private Exception _Exception;
        /// <summary>
        /// 获取一个值，表示抛出的错误。
        /// </summary>
        public virtual Exception Exception { get { return this._Exception; } set { this._Exception = value; } }

        /// <summary>
        /// 初始化一个 <see cref="System.ExceptionEventArgs "/> 类的新实例。
        /// </summary>
        public ExceptionEventArgs() { }

        /// <summary>
        /// 提供一个错误，初始化一个 <see cref="System.ExceptionEventArgs "/> 类的新实例。
        /// </summary>
        /// <param name="exception">一个错误。</param>
        public ExceptionEventArgs(Exception exception)
        {
            this._Exception = exception;
        }

        /// <summary>
        /// 提供错误的描述，初始化一个 <see cref="System.ExceptionEventArgs "/> 类的新实例。
        /// </summary>
        /// <param name="message">错误的描述。</param>
        public ExceptionEventArgs(string message) : this(new Exception(message)) { }

        /// <summary>
        /// 创建并返回当前异常的字符串表示形式。
        /// </summary>
        /// <returns>当前异常的字符串表示形式。</returns>
        public override string ToString()
        {
            if(this.Exception == null)
                return base.ToString();
            return this.Exception.ToString();
        }
    }
}