using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// 表示一个释放分配的资源的基类。
    /// </summary>
    public abstract class ObjectDisposableBase : IObjectDisposable
    {
        private bool _IsDisposed;
        /// <summary>
        /// 指示当前对象是否已被释放。
        /// </summary>
        public bool IsDisposed { get { return this._IsDisposed; } }

        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        public virtual void Dispose()
        {
            if(!this._IsDisposed)
            {
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        /// <param name="disposing">为 true 则释放托管资源和非托管资源；为 false 则仅释放非托管资源。</param>
        protected virtual void Dispose(bool disposing)
        {
            this._IsDisposed = true;
            if(disposing) this.DisposeManaged();
            this.DisposeUnmanaged();
        }

        /// <summary>
        /// 执行与释放或重置托管资源相关的应用程序定义的任务。
        /// </summary>
        protected virtual void DisposeManaged() { }
        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        protected virtual void DisposeUnmanaged() { }

        /// <summary>
        /// 对已释放的对象执行操作时所引发的异常。
        /// </summary>
        protected virtual void ThrowWhenDisposed()
        {
            if(this._IsDisposed) throw new ObjectDisposedException(this.GetType().FullName);
        }

        /// <summary>
        /// 析构函数。
        /// </summary>
        ~ObjectDisposableBase()
        {
            if(!this._IsDisposed)
            {
                this.Dispose(false);
            }
        }
    }
}
