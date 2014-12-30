using System.Collections.Concurrent;
using System.Threading;

namespace System
{
    /// <summary>
    /// 定义一个可重复使用的对象。
    /// </summary>
    public interface IObjectRelease
    {
        /// <summary>
        /// 对象在释放时发生。
        /// </summary>
        void Release();
    }
    /// <summary>
    /// 表示安全线程的可复用对象池。
    /// </summary>
    /// <typeparam name="T">对象的数据类型。</typeparam>
    public class ObjectPool<T> : ObjectDisposableBase
    {
        private ConcurrentQueue<T> _Queue;
        private Func<T> _objectFactory;
        private Semaphore _semaphore;

        private int _MaxObjectCount;
        /// <summary>
        /// 设置或获取一个值，指示最大的对象数。默认为 0，表示不做限制。
        /// </summary>
        public int MaxObjectCount
        {
            get { return this._MaxObjectCount; }
            set
            {
                if(this._MaxObjectCount == value) return;
                lock(this)
                {
                    if(value < 1) this._semaphore = null;
                    else this._semaphore = new Semaphore(value / 10, value);
                    this._MaxObjectCount = value;
                }
            }
        }

        /// <summary>
        /// 获取当前内部池。
        /// </summary>
        protected ConcurrentQueue<T> InnerPool { get { return this._Queue; } }

        /// <summary>
        /// 获取对象池的元素数。
        /// </summary>
        public int Count { get { return this._Queue.Count; } }

        /// <summary>
        /// 初始化一个 <see cref="System.ObjectPool&lt;T&gt;"/> 类的新实例。
        /// </summary>
        public ObjectPool() : this(null) { }

        /// <summary>
        /// 提供在需要时被调用以产生延迟初始化值的委托，初始化一个 <see cref="System.ObjectPool&lt;T&gt;"/> 类的新实例。
        /// </summary>
        /// <param name="objectFactory">在需要时被调用以产生延迟初始化值的委托。</param>
        public ObjectPool(Func<T> objectFactory)
        {
            this._objectFactory = objectFactory;
            this._Queue = new ConcurrentQueue<T>();
        }

        /// <summary>
        /// 创建对象时发生。 
        /// </summary>
        /// <returns>返回一个新的对象。</returns>
        protected virtual T OnCreateObject()
        {
            return this._objectFactory == null ? (T)Activator.CreateInstance(typeof(T), true) : this._objectFactory();
        }

        /// <summary>
        /// 获取一个对象池的对象。
        /// </summary>
        /// <returns>返回一个已释放或新的对象。</returns>
        public virtual T Acquire()
        {
            this.ThrowWhenDisposed();
            if(this._semaphore != null)
                lock(this) if(this._semaphore != null) this._semaphore.WaitOne();
            T item;
            return this._Queue.TryDequeue(out item) ? item : this.OnCreateObject();
        }

        /// <summary>
        /// 释放一个对象，并将其放入对象池中。
        /// </summary>
        /// <param name="obj">对象池。</param>
        public virtual void Release(T obj)
        {
            this.ThrowWhenDisposed();
            var ro = obj as IObjectRelease;
            if(ro != null) ro.Release();
            this._Queue.Enqueue(obj);
            if(this._semaphore != null)
                lock(this) if(this._semaphore != null) this._semaphore.Release();
        }

        /// <summary>
        /// 获取一个对象池的对象，并执行 <paramref name="callback"/>，并将对象放回池中。
        /// </summary>
        /// <param name="callback">回调函数。</param>
        public virtual void AcquireRelease(Action<T> callback)
        {
            this.ThrowWhenDisposed();
            var t = this.Acquire();
            try
            {
                callback(t);
            }
            finally
            {
                this.Release(t);
            }
        }

        /// <summary>
        /// 获取一个对象池的对象，并执行 <paramref name="callback"/>，并将对象放回池中。
        /// </summary>
        /// <typeparam name="TResult">返回的数据类型。</typeparam>
        /// <param name="callback">回调函数。</param>
        /// <returns>返回回调函数的返回值。</returns>
        public virtual TResult AcquireRelease<TResult>(Func<T, TResult> callback)
        {
            this.ThrowWhenDisposed();
            var t = this.Acquire();
            try
            {
                return callback(t);
            }
            finally
            {
                this.Release(t);
            }
        }

        /// <summary>
        /// 执行与释放或重置托管资源相关的应用程序定义的任务。
        /// </summary>
        /// <param name="item">当前对象。</param>
        protected virtual void DisposeItem(T item) { }

        /// <summary>
        /// 执行与释放或重置托管资源相关的应用程序定义的任务。
        /// </summary>
        protected override void DisposeManaged()
        {
            T item;
            while(this._Queue.TryDequeue(out item))
            {
                DisposeItem(item);
            }
            this._Queue = null;
            this._objectFactory = null;
            if(this._semaphore != null)
            {
                this._semaphore.Dispose();
                this._semaphore = null;
            }
        }
    }
}
