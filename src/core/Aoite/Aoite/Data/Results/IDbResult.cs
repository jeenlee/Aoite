using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Aoite.Data
{
    /// <summary>
    /// 表示一个查询结果的接口。
    /// </summary>
    public interface IDbResult : IResult, IDisposable
    {
        /// <summary>
        /// 获取一个值，表示查询操作的 <see cref="System.Data.Common.DbCommand"/> 对象。
        /// </summary>
        DbCommand Command { get; }

        /// <summary>
        /// 获取一个值，表示查询结果是否已释放资源。
        /// </summary>
        bool IsDisposed { get; }

        /// <summary>
        /// 获取一个值，表示数据源的操作引擎。
        /// </summary>
        IDbEngine Engine { get; }

        /// <summary>
        /// 指定参数的名称，获取一个参数的值。
        /// </summary>
        /// <param name="parameterName">参数的名称。</param>
        /// <returns>返回具有指定名称的参数值。</returns>
        object this[string parameterName] { get; }

        /// <summary>
        /// 指定参数的索引，获取一个参数的值。
        /// </summary>
        /// <param name="parameterIndex">参数的从零开始的索引。</param>
        /// <returns>返回索引处的参数值。</returns>
        object this[int parameterIndex] { get; }

        /// <summary>
        /// 获取一个值，表示查询操作的返回值。
        /// </summary>
        object Value { get; }
    }
}
