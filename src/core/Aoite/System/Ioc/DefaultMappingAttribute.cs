using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// 表示一个默认映射的目标数据类型的特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public class DefaultMappingAttribute : Attribute
    {
        private Type _ActualType;
        /// <summary>
        /// 获取默认映射的实际数据类型。
        /// </summary>
        public Type ActualType { get { return _ActualType; } }
        
        /// <summary>
        /// 初始化一个 <see cref="System.DefaultMappingAttribute"/> 类的新实例。
        /// </summary>
        /// <param name="actualType">默认映射的实际数据类型。</param>
        public DefaultMappingAttribute(Type actualType)
        {
            if(actualType == null) throw new ArgumentNullException("type");
            this._ActualType = actualType;
        }
    }
}
