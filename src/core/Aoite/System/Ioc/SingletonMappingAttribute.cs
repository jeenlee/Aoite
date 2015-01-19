namespace System
{
    /// <summary>
    /// 表示依赖注入采用单例模式的特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class SingletonMappingAttribute : Attribute
    {
        internal static readonly Type Type = typeof(SingletonMappingAttribute);
    }
}
