using System;
using System.Collections.Generic;
using System.Text;

namespace Aoite.Data
{
    /// <summary>
    /// 基于页码的分页实现。
    /// </summary>
    public class Pagination : PgParameters
    {
        /// <summary>
        /// 初始化一个 <see cref="Aoite.Data.Pagination"/> 类的新实例。
        /// </summary>
        public Pagination() : base() { }
        /// <summary>
        /// 初始化一个 <see cref="Aoite.Data.Pagination"/> 类的新实例。
        /// </summary>
        /// <param name="pageNumber">以 1 起始的页码。</param>
        public Pagination(int pageNumber) : base(pageNumber) { }
        /// <summary>
        /// 提供分页数据，初始化一个 <see cref="Aoite.Data.Pagination"/> 类的新实例。
        /// </summary>
        /// <param name="pageNumber">以 1 起始的页码。</param>
        /// <param name="pageSize">分页大小。</param>
        public Pagination(int pageNumber, int pageSize) : base(pageNumber, pageSize) { }
    }
}
