using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.ComponentModel.DataAnnotations
{
    /// <summary>
    /// 指定需要数据字段值，并且不能为默认值。
    /// </summary>  
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class NonDefaultAttribute : RequiredAttribute
    {
        /// <summary>
        /// 初始化一个 <see cref="System.ComponentModel.DataAnnotations.NonDefaultAttribute"/> 类的新实例。
        /// </summary>
        public NonDefaultAttribute() { }

        /// <summary>
        /// 获取或设置默认值。
        /// </summary>
        public object DefaultValue { get; set; }

        /// <summary>
        /// 检查必填数据字段的值是否不为空或默认值。
        /// </summary>
        /// <param name="value">要验证的数据字段值。</param>
        /// <returns>如果验证成功，则为 true；否则为 false。</returns>
        public override bool IsValid(object value)
        {
            return base.IsValid(value) && !object.Equals(value, DefaultValue ?? value.GetType().GetDefaultValue());
        }
    }
}
