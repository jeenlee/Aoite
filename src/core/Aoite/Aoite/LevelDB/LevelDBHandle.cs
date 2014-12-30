using System;
using System.Collections.Generic;
using System.Text;

namespace Aoite.LevelDB
{
    /// <summary>
    /// 表示一个 LevelDB 的句柄基类。
    /// </summary>
    public abstract class LevelDBHandle : ObjectDisposableBase
    {
        internal IntPtr _handle;
        /// <summary>
        /// 初始化一个 <see cref="Aoite.LevelDB.LevelDBHandle"/> 类的新实例。
        /// </summary>
        public LevelDBHandle() { }

        internal abstract void DestroyUnmanaged();

        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        protected override void DisposeUnmanaged()
        {
            if(_handle != IntPtr.Zero)
            {
                this.DestroyUnmanaged();
                _handle = IntPtr.Zero;
            }

            base.DisposeUnmanaged();
        }

    }
}
