using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// 表示一个表格数据源。
    /// </summary>
    public abstract class GridData
    {
        /// <summary>
        /// 初始化一个 <see cref="System.GridData&lt;TModel&gt;"/> 类的新实例。
        /// </summary>
        public GridData() { }
        /// <summary>
        /// 获取或设置行的总数。
        /// </summary>
        public long Total { get; set; }
        /// <summary>
        /// 获取行的数据。
        /// </summary>
        /// <returns>行的数据。</returns>
        public abstract Array GetRows();
    }

    /// <summary>
    /// 表示一个表格数据源。
    /// </summary>
    /// <typeparam name="TModel">数据源的行数据类型。</typeparam>
    public class GridData<TModel> : GridData
    {
        /// <summary>
        /// 初始化一个 <see cref="System.GridData&lt;TModel&gt;"/> 类的新实例。
        /// </summary>
        public GridData() { }

        /// <summary>
        /// 获取指定索引的数据。
        /// </summary>
        /// <param name="index">数据的索引。</param>
        /// <returns>返回一个数据。</returns>
        public TModel this[int index]
        {
            get
            {
                var rowCount = this.Rows == null ? 0 : this.Rows.Length;
                if(rowCount == 0) throw new NotSupportedException("没有数据。");
                if(index < 0 || index >= rowCount) throw new ArgumentOutOfRangeException("index");
                return this.Rows[index];
            }
        }

        /// <summary>
        /// 获取或设置行的数据。
        /// </summary>
        public TModel[] Rows { get; set; }

        /// <summary>
        /// 获取行的数据。
        /// </summary>
        /// <returns>行的数据。</returns>
        public override Array GetRows()
        {
            return this.Rows;
        }
    }
}
