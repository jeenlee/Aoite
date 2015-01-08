using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Data
{
    /// <summary>
    /// 表示一个给定返回值数据类型的查询结果的接口。
    /// </summary>
    /// <typeparam name="TValue">返回值的类型。</typeparam>
    public interface IDbResult<TValue> : IDbResult, IResult<TValue> { }
}
