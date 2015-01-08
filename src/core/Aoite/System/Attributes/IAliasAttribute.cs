namespace System
{
    /// <summary>
    /// 表示一个具有别名的特性。
    /// </summary>
    public interface IAliasAttribute
    {
        /// <summary>
        /// 获取或设置别名。
        /// </summary>
        string Name { get; set; }
    }
}
