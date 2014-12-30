namespace System
{
    /// <summary>
    /// 表示一个命名空间的规则模式。
    /// </summary>
    public enum NamesapceRuleMode
    {
        /// <summary>
        /// 确定在使用指定的比较选项进行比较时此命名空间是否与指定的表达式完全匹配。
        /// </summary>
        Equals,
        /// <summary>
        /// 确定在使用指定的比较选项进行比较时此命名空间的开头是否与指定的表达式匹配。
        /// </summary>
        StartsWith,
        /// <summary>
        /// 确定在使用指定的比较选项进行比较时此命名空间的结尾是否与指定的表达式匹配。
        /// </summary>
        EndsWith
    }
}
