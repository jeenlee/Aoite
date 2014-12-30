using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// 表示两个对象的比较的结果。
    /// </summary>
    public class CompareResult
    {
        /// <summary>
        /// 设置或获取对象的名称。
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 设置或获取第一个对象的值。
        /// </summary>
        public object Value1 { get; set; }
        /// <summary>
        /// 设置或获取第二个对象的值。
        /// </summary>
        public object Value2 { get; set; }

        /// <summary>
        /// 返回比较结果的描述。
        /// </summary>
        /// <returns>返回一个字符串。</returns>
        public override string ToString()
        {
            return "{0} 的 {1} 和 {2} 不匹配。".Fmt(Name, Value1 ?? "<NULL>", Value2 ?? "<NULL>");
        }
    }
}
