namespace System
{
    /// <summary>
    /// 表示映射解析的事件参数。
    /// </summary>
    public class MapResolveEventArgs : EventArgs
    {
        private Type _ExpectType;
        /// <summary>
        /// 获取预期的类型。
        /// </summary>
        public Type ExpectType { get { return this._ExpectType; } }
        /// <summary>
        /// 获取或设置一个值，表示映射是否采用单例模式。
        /// </summary>
        public bool SingletonMode { get; set; }
        /// <summary>
        /// 实例的获取方式回调方法。
        /// </summary>
        public InstanceCreatorCallback Callback { get; set; }

        internal MapResolveEventArgs(Type expectType)
        {
            this._ExpectType = expectType;
        }
    }
}
