using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// 定义一种对象释放分配的资源的方法。
    /// </summary>
    public interface IObjectDisposable : IDisposable
    {
        /// <summary>
        /// 指示当前对象是否已被释放。
        /// </summary>
        bool IsDisposed { get; }
    }
}
