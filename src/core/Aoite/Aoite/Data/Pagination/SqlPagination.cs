using System.Data.Common;

namespace Aoite.Data
{
    /// <summary>
    /// 基于 SQL 的分页。
    /// </summary>
    public abstract class SqlPagination : PaginationBase
    {
        private string _RowNumberName = DefaultRowNumberName;
        /// <summary>
        /// 获取或设置页码字段的列名。
        /// </summary>
        public string RowNumberName
        {
            get
            {
                return this._RowNumberName;
            }
            set
            {
                this._RowNumberName = value;
            }
        }

        /// <summary>
        /// 获取分页的字符串格式项。
        /// </summary>
        protected abstract string PageFormat { get; }
        /// <summary>
        /// 获取统计的字符串格式项。
        /// </summary>
        protected abstract string TotalFormat { get; }

        /// <summary>
        /// 对指定的 <see cref="System.Data.Common.DbCommand"/> 进行分页处理。
        /// </summary>
        /// <param name="pageNumber">从 1 开始的页码。</param>
        /// <param name="pageSize">页的大小。</param>
        /// <param name="command">数据源查询命令</param>
        public override void ProcessCommand(int pageNumber, int pageSize, DbCommand command)
        {
            var start = (pageNumber - 1) * pageSize;
            var end = pageNumber * pageSize;
            command.CommandText = string.Format(PageFormat
                , command.CommandText
                , this._RowNumberName
                , start
                , end);
        }

        /// <summary>
        /// 创建指定查询字符串的统计总行数查询字符串。
        /// </summary>
        /// <param name="commandText">原查询字符串。</param>
        public override string CreateTotalCountCommand(string commandText)
        {
            return string.Format(TotalFormat, commandText);
        }
    }
}
