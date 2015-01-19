using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// 表示依赖注入后期映射的特性，如果后期依赖元素在非 <see cref="System.AttributeTargets.Parameter"/>，而又考虑忽略，可以采用 <see cref="System.IgnoreAttribute"/> 对其进行忽略后期绑定。
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Interface | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class LastMappingAttribute : Attribute
    {
        internal static readonly Type Type = typeof(LastMappingAttribute);
    }
}
