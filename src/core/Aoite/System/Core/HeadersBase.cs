using System.Collections.Concurrent;

namespace System
{
    using CDictionary = ConcurrentDictionary<string, object>;
    using LazyDictionary = Lazy<ConcurrentDictionary<string, object>>;

    /// <summary>
    /// 包含头部信息的契约相关基类。
    /// </summary>
    public abstract class HeadersBase
    {
        private LazyDictionary _Headers;
        /// <summary>
        /// 获取或设置契约的头部信息。
        /// </summary>
        public CDictionary Headers { get { return this._Headers.Value; } }

        /// <summary>
        /// 获取一个值，该值指示当前是否包含契约的头部信息。
        /// </summary>
        public bool HasHeaders { get { return this._Headers.IsValueCreated; } }

        /// <summary>
        /// 初始化一个 <see cref="System.HeadersBase"/> 类的新实例。
        /// </summary>
        public HeadersBase()
            : this(() => new CDictionary(StringComparer.CurrentCultureIgnoreCase)) { }

        /// <summary>
        /// 提供在需要时被调用以产生延迟初始化值的委托，初始化一个 <see cref="System.HeadersBase"/> 类的新实例。
        /// </summary>
        /// <param name="headersFactory">在需要时被调用以产生延迟初始化值的委托。</param>
        public HeadersBase(Func<CDictionary> headersFactory)
        {
            this._Headers = new LazyDictionary(headersFactory);
        }
    }
}
