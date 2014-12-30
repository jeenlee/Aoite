using System;
using System.Collections.Generic;
using System.Text;

namespace Aoite.Data
{
    /// <summary>
    /// 基于偏移量的分页实现。
    /// </summary>
    public class LimitPagination : IPagination
    {
        #region IPagination 成员

        int IPagination.PageNumber
        {
            get { return (this.Start / this.Limit) + 1; }
            set { this.Start = (value - 1) * this.Limit; }
        }

        int IPagination.PageSize
        {
            get { return this.Limit; }
            set { this.Limit = value; }
        }

        #endregion

        private int _Start;
        /// <summary>
        /// 获取或设置以 0 起始的索引。
        /// </summary>
        public int Start
        {
            get
            {
                return this._Start;
            }
            set
            {
                if(value < 0) value = 0;
                this._Start = value;
            }
        }

        private int _Limit;
        /// <summary>
        /// 获取或设置分页偏移量。默认为 10。
        /// </summary>
        public int Limit
        {
            get
            {
                return this._Limit;
            }
            set
            {
                if(value < 1) value = 10;
                this._Limit = value;
            }
        }

        /// <summary>
        /// 初始化一个 <see cref="Aoite.Data.LimitPagination"/> 类的新实例。
        /// </summary>
        public LimitPagination() : this(0) { }
        /// <summary>
        /// 初始化一个 <see cref="Aoite.Data.LimitPagination"/> 类的新实例。
        /// </summary>
        /// <param name="start">以 0 起始的索引。</param>
        public LimitPagination(int start) : this(start, 10) { }
        /// <summary>
        /// 初始化一个 <see cref="Aoite.Data.LimitPagination"/> 类的新实例。
        /// </summary>
        /// <param name="start">以 0 起始的索引。</param>
        /// <param name="limit">分页偏移量。</param>
        public LimitPagination(int start, int limit)
        {
            this.Start = start;
            this.Limit = limit;
        }
    }

}
