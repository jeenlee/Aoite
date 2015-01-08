namespace System
{
    /// <summary>
    /// 表示一个或多个用于唯一标识实体的的特性。
    /// </summary>
    public interface IKeyAttribute
    {
        /// <summary>
        /// 获取或设置一个值，指示是否为唯一标识。
        /// </summary>
        bool IsKey { get; set; }
    }
}
