using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Net
{
    /// <summary>
    /// 表示一个可靠的异步套接字的用户数据集合。
    /// </summary>
    public class AsyncDataCollection : Dictionary<string, object>
    {
        /// <summary>
        /// 初始化一个 <see cref="Aoite.Net.AsyncDataCollection"/> 类的新实例。
        /// </summary>
        public AsyncDataCollection() : base(StringComparer.CurrentCultureIgnoreCase) { }
    }
}
