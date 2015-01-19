using Aoite.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace System
{
    /// <summary>
    /// 提供对延迟初始化的支持，并提供重新延迟初始化的支持。
    /// </summary>
    /// <typeparam name="T">指定正在延迟初始化的对象的类型。</typeparam>
    public class Mean<T>
    {
        private Func<Lazy<T>> _lazyCreater;
        private Lazy<T> _lazy;
        /// <summary>
        /// 初始化 <see cref="System.Mean&lt;T&gt;"/> 类的新实例。发生延迟初始化时，使用目标类型的默认构造函数。
        /// </summary>
        public Mean()
        {
            this._lazyCreater = () => new Lazy<T>();
            this.Reset();
        }
        /// <summary>
        /// 初始化 <see cref="System.Mean&lt;T&gt;"/> 类的新实例。发生延迟初始化时，使用目标类型的默认构造函数和指定的初始化模式。
        /// </summary>
        /// <param name="isThreadSafe">true 表示此示例可由多个线程同时使用；false 表示此实例一次只能由一个线程使用。</param>
        public Mean(bool isThreadSafe)
        {
            this._lazyCreater = () => new Lazy<T>(isThreadSafe);
            this.Reset();
        }
        /// <summary>
        /// 初始化 <see cref="System.Mean&lt;T&gt;"/> 类的新实例。发生延迟初始化时，使用指定的初始化函数。
        /// </summary>
        /// <param name="valueFactory">在需要时被调用以产生延迟初始化值的委托。</param>
        public Mean(Func<T> valueFactory)
        {
            this._lazyCreater = () => new Lazy<T>(valueFactory);
            this.Reset();
        }
        /// <summary>
        /// 初始化 <see cref="System.Mean&lt;T&gt;"/> 类的新实例，其中使用 T 的默认构造函数和指定的线程安全性模式。
        /// </summary>
        /// <param name="mode">线程安全性模式。</param>
        public Mean(LazyThreadSafetyMode mode)
        {
            this._lazyCreater = () => new Lazy<T>(mode);
            this.Reset();
        }
        /// <summary>
        /// 初始化 <see cref="System.Mean&lt;T&gt;"/> 类的新实例。发生延迟初始化时，使用指定的初始化函数和初始化模式。
        /// </summary>
        /// <param name="valueFactory">在需要时被调用以产生延迟初始化值的委托。</param>
        /// <param name="isThreadSafe">true 表示此示例可由多个线程同时使用；false 表示此实例一次只能由一个线程使用。</param>
        public Mean(Func<T> valueFactory, bool isThreadSafe)
        {
            this._lazyCreater = () => new Lazy<T>(valueFactory, isThreadSafe);
            this.Reset();
        }
        /// <summary>
        /// 初始化 <see cref="System.Mean&lt;T&gt;"/> 类的新实例，其中使用指定的初始化函数和线程安全性模式。
        /// </summary>
        /// <param name="valueFactory">在需要时被调用以产生延迟初始化值的委托。</param>
        /// <param name="mode">线程安全性模式。</param>
        public Mean(Func<T> valueFactory, LazyThreadSafetyMode mode)
        {
            this._lazyCreater = () => new Lazy<T>(valueFactory, mode);
            this.Reset();
        }

        /// <summary>
        /// 获取一个值，该值指示是否已为此 <see cref="System.Mean&lt;T&gt;"/> 实例创建一个值。
        /// </summary>
        public bool IsValueCreated { get { return this._lazy.IsValueCreated; } }

        /// <summary>
        /// 获取当前 <see cref="System.Mean&lt;T&gt;"/> 实例的延迟初始化值。
        /// </summary>
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        public T Value { get { return this._lazy.Value; } }

        /// <summary>
        /// 释放上一次初始化的值，并重置延迟初始化的值。
        /// </summary>
        public void Reset()
        {
            lock(this)
            {
                if(this._lazy != null && this._lazy.IsValueCreated && this._lazy.Value is IDisposable)
                {
                    (this._lazy.Value as IDisposable).Dispose();
                }
                this._lazy = this._lazyCreater();
            }
        }
    }
}
