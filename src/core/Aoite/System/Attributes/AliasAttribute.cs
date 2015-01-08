namespace System
{
    /// <summary>
    /// 表示一个具有别名的特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class AliasAttribute : Attribute, IAliasAttribute
    {
        private string _Name;
        /// <summary>
        /// 获取或设置别名。
        /// </summary>
        public string Name { get { return this._Name; } set { this._Name = value; } }
        /// <summary>
        /// 初始化一个 <see cref="System.AliasAttribute"/> 类的新实例。
        /// </summary>
        public AliasAttribute() { }
        /// <summary>
        /// 提供别名，初始化一个 <see cref="System.AliasAttribute"/> 类的新实例。
        /// </summary>
        /// <param name="name">别名。</param>
        public AliasAttribute(string name)
        {
            this._Name = name;
        }
    }
}