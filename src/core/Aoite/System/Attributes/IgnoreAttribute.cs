using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// 表示一个具有忽略性的标识。
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class IgnoreAttribute : Attribute
    {
        /// <summary>
        /// 忽略性标识的类型。
        /// </summary>
        public static readonly Type Type = typeof(IgnoreAttribute);

        /// <summary>
        /// 初始化一个 <see cref="System.IgnoreAttribute"/> 类的新实例。
        /// </summary>
        public IgnoreAttribute() { }
    }
}
