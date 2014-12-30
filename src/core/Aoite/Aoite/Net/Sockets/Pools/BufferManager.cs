using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Aoite.Net
{
    /// <summary>
    /// 表示一个防止内存碎片的缓冲区管理。
    /// </summary>
    internal sealed class BufferManager : IDisposable
    {
        private readonly int BufferSize;

        private System.ServiceModel.Channels.BufferManager _InnerBufferManager;

        /// <summary>
        /// 初始化一个 <see cref="Aoite.Net.BufferManager"/> 类的新实例。
        /// </summary>
        /// <param name="maxBufferPoolSize">缓冲池的最大大小。</param>
        /// <param name="maxBufferSize">单独缓冲区的最大大小。</param>
        public BufferManager(int maxBufferPoolSize, int maxBufferSize)
        {
            this._InnerBufferManager = System.ServiceModel.Channels.BufferManager.CreateBufferManager(maxBufferPoolSize, maxBufferSize);
            this.BufferSize = maxBufferSize;
        }

        /// <summary>
        /// 分配指定 <see cref="System.Net.Sockets.SocketAsyncEventArgs"/> 的缓冲区。
        /// </summary>
        /// <param name="args">异步套接字操作。</param>
        public void AssignBuffer(SocketAsyncEventArgs args)
        {
            args.SetBuffer(this._InnerBufferManager.TakeBuffer(this.BufferSize), 0, this.BufferSize);
        }

        /// <summary>
        /// 释放指定 <see cref="System.Net.Sockets.SocketAsyncEventArgs"/> 的缓冲区。
        /// </summary>
        /// <param name="args">异步套接字操作。</param>
        public void FreeBuffer(SocketAsyncEventArgs args)
        {
            this._InnerBufferManager.ReturnBuffer(args.Buffer);
        }

        private bool _IsDisposed;

        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        public void Dispose()
        {
            if(this._IsDisposed) return;
            this._IsDisposed = true;
            this._InnerBufferManager.Clear();
            this._InnerBufferManager = null;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 析构函数。
        /// </summary>
        ~BufferManager()
        {
            this.Dispose();
        }
    }
}
