using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Aoite.Data
{
    /// <summary>
    /// 表示 <see cref="Aoite.Data.IDbExecutor.ToReader(Aoite.Data.ExecuteReaderHandler)"/> 的委托。
    /// </summary>
    /// <param name="reader">数据读取器。</param>
    public delegate void ExecuteReaderHandler(DbDataReader reader);

    /// <summary>
    /// 表示 <see cref="Aoite.Data.IDbExecutor.ToReader&lt;TResultValue&gt;(Aoite.Data.ExecuteReaderHandler&lt;TResultValue&gt;)"/> 的委托。
    /// </summary>
    /// <typeparam name="TResultValue">返回值的类型。</typeparam>
    /// <param name="reader">数据读取器。</param>
    /// <returns>返回操作结果的值。</returns>
    public delegate TResultValue ExecuteReaderHandler<TResultValue>(DbDataReader reader);
}
