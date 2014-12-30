namespace System
{
    /// <summary>
    /// 表示一个忽略性的标识。
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class IgnoreAttribute : Attribute
    {
        /// <summary>
        /// 忽略性标识的类型。
        /// </summary>
        public static readonly Type Type = typeof(IgnoreAttribute);
    }
}
