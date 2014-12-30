using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.ServiceModel
{
    /// <summary>
    /// 定义契约是否允许匿名访问的特性。
    /// </summary>
    [AttributeUsageAttribute(AttributeTargets.Interface | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AllowAnonymousAttribute : Attribute
    {
        internal readonly static Type Type = typeof(AllowAnonymousAttribute);

        /// <summary>
        /// 初始化一个 <see cref="Aoite.ServiceModel.AllowAnonymousAttribute"/> 类的新实例。
        /// </summary>
        public AllowAnonymousAttribute() { }
    }
}
