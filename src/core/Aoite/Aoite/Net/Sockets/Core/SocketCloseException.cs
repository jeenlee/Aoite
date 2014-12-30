using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Net
{
    /// <summary>
    /// 表示关闭套接字所引发的异常。
    /// </summary>
    public class SocketCloseException : Exception
    {
        /// <summary>
        /// 表示远程套接字关闭的错误。
        /// </summary>
        public static readonly SocketCloseException Instance = new SocketCloseException();

        /// <summary>
        /// 初始化一个 <see cref="Aoite.Net.SocketCloseException"/> 类的新实例。
        /// </summary>
        private SocketCloseException() : base() { }
    }
}
