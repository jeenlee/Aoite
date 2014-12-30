using System;

namespace Aoite.Data
{
    /// <summary>
    /// 表示强制中断了正在执行操作的错误。
    /// </summary>
    public class ExecuteAbortException : Exception
    {
        /// <summary>
        /// 初始化一个 <see cref="Aoite.Data.ExecuteAbortException"/> 类的新实例。
        /// </summary>
        private ExecuteAbortException() : base("强制中断了正在执行的操作。") { }

        /// <summary>
        /// 强制中断了正在执行操作的错误。
        /// </summary>
        public static readonly ExecuteAbortException Instance = new ExecuteAbortException();
    }
}