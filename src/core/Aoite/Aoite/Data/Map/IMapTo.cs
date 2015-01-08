using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Data
{
    /// <summary>
    /// 定义一个填充器的映射目标。
    /// </summary>
    public interface IMapTo
    {
        /// <summary>
        /// 设置映射目标的值。
        /// </summary>
        /// <typeparam name="TEntity">映射目标的数据类型。</typeparam>
        /// <param name="to">目标的值。</param>
        /// <returns>返回映射目标的值</returns>
        TEntity To<TEntity>(TEntity to);
    }
}
