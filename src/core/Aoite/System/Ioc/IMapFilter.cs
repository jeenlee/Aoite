using System.Collections.Generic;
using System.Linq;

namespace System
{
    /// <summary>
    /// 定义依赖注入与控制反转的映射筛选器。
    /// </summary>
    public interface IMapFilter
    {
        /// <summary>
        /// 获取筛选器的命名空间规则列表。
        /// </summary>
        NamesapceRule[] Rules { get; }
        /// <summary>
        /// 获取或设置筛选器的实际类型的完全限定名的格式项，例如“{0}.Default{1}”，索引 0 表示 - 预期定义接口 - 的命名空间，索引 1 表示 - 预期定义接口 - 的名称（已去 I）。
        /// </summary>
        string ActualTypeFullNameFormat { get; set; }
        /// <summary>
        /// 判断指定命名空间是否匹配规则。
        /// </summary>
        /// <param name="namespace">一个命名空间。</param>
        /// <returns>如果匹配返回 true，否则返回 false。</returns>
        bool NamespaceIsMatch(string @namespace);
        /// <summary>
        /// 判断指定 <paramref name="expectType"/> 和 <paramref name="actualType"/> 是否开启单例模式。
        /// </summary>
        /// <param name="expectType">预期定义的类型。</param>
        /// <param name="actualType">实际映射的类型。</param>
        /// <returns>如果启用单例模式则返回 true，否则返回 false。</returns>
        bool IsSingletonMode(Type expectType, Type actualType);
        /// <summary>
        /// 判断指定 <paramref name="type"/> 是否是一个预期定义的类型。
        /// </summary>
        /// <param name="type">类型。</param>
        /// <returns>如果 <paramref name="type"/> 是一个预期的类型返回 true，否则返回 false。</returns>
        bool IsExpectType(Type type);
        /// <summary>
        /// 根据指定的 <paramref name="expectType"/> ，查找在 <paramref name="allTypes"/> 集合中对应的实际定义的类型。
        /// </summary>
        /// <param name="allTypes">当前应用程序所有已加载的类型。</param>
        /// <param name="expectType">预期定义的类型。</param>
        /// <returns>如果找到返回一个 <see cref="System.Type"/> 的实例，否则返回 null 值。</returns>
        Type FindActualType(IEnumerable<IGrouping<string, Type>> allTypes, Type expectType);
    }
}
