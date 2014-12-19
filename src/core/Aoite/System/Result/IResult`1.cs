using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// 定义包含一个返回值的结果。
    /// </summary>
    /// <typeparam name="TValue">返回值的数据类型。</typeparam>
    public interface IResult<TValue> : IResult
    {
        /// <summary>
        /// 获取或设置结果的返回值。
        /// </summary>
        TValue Value { get; set; }
    }
}
