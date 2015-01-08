using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// 提供分页接口的实现。
    /// </summary>
    public interface IPagination
    {
        /// <summary>
        /// 获取或设置以 1 起始的页码。
        /// </summary>
        int PageNumber { get; set; }
        /// <summary>
        /// 获取或设置分页大小。默认为 10。
        /// </summary>
        int PageSize { get; set; }
    }

    /// <summary>
    /// 表示一个具有分页的参数集。
    /// </summary>
    public class PgParameters : IPagination
    {
        private int _PageNumber;
        /// <summary>
        /// 获取或设置以 1 起始的页码。
        /// </summary>
        public int PageNumber
        {
            get
            {
                return this._PageNumber;
            }
            set
            {
                if(value < 1) value = 1;
                this._PageNumber = value;
            }
        }

        private int _PageSize;
        /// <summary>
        /// 获取或设置分页大小。默认为 10。
        /// </summary>
        public int PageSize
        {
            get
            {
                return this._PageSize;
            }
            set
            {
                if(value < 1) value = 10;
                this._PageSize = value;
            }
        }

        /// <summary>
        /// 初始化一个 <see cref="System.PgParameters"/> 类的新实例。
        /// </summary>
        public PgParameters() : this(1) { }
        /// <summary>
        /// 初始化一个 <see cref="System.PgParameters"/> 类的新实例。
        /// </summary>
        /// <param name="pageNumber">以 1 起始的页码。</param>
        public PgParameters(int pageNumber) : this(pageNumber, 10) { }
        /// <summary>
        /// 初始化一个 <see cref="System.PgParameters"/> 类的新实例。
        /// </summary>
        /// <param name="pageNumber">以 1 起始的页码。</param>
        /// <param name="pageSize">分页大小。</param>
        public PgParameters(int pageNumber, int pageSize)
        {
            this.PageNumber = pageNumber;
            this.PageSize = pageSize;
        }
    }
}

