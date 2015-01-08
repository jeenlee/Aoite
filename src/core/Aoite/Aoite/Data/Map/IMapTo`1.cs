using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Data
{
    /// <summary>
    /// 定义一个填充器的映射目标。
    /// </summary>
    /// <typeparam name="TTo">映射目标的数据类型。</typeparam>
    public interface IMapTo<TTo>
    {
        /// <summary>
        /// 设置映射目标的值。
        /// </summary>
        /// <param name="to">目标的值。</param>
        void To(TTo to);
    }
}
