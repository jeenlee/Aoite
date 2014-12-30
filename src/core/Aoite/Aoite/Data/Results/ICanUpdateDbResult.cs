using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Data
{
    /// <summary>
    /// 表示一个可更新的查询结果。
    /// </summary>
    public interface ICanUpdateDbResult : IDbResult
    {
        /// <summary>
        /// 获取一个值，表示值是否已被修改。
        /// </summary>
        bool IsChanged { get; }

        /// <summary>
        /// 提交当前返回值，更新数据源对应的记录。
        /// <para>如果在更新过程中发生了异常，将会通知当前查询结果的 <see cref="System.IResult.Exception"/> 属性。</para>
        /// </summary>  
        /// <returns>返回更新的行数。如果返回一个 0 则表示当前返回值并改动，如果返回一个小于 0 的整数，则表示更新发生了异常。</returns>
        int Update();
        /// <summary>
        /// 提交当前返回值，更新数据源对应的记录。
        /// <para>如果在更新过程中发生了异常，将会通知当前查询结果的 <see cref="System.IResult.Exception"/> 属性。</para>
        /// </summary>  
        /// <param name="continueUpdateOnError">指示在行更新过程中遇到错误时是否生成异常。如果要继续更新而不生成异常，则为 true，否则为 false。</param>
        /// <returns>返回更新的行数。如果返回一个 0 则表示当前返回值并改动，如果返回一个小于 0 的整数，则表示更新发生了异常。</returns>
        int Update(bool continueUpdateOnError);
    }
}
