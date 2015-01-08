using System.Data.Common;

namespace Aoite.Data
{
    /// <summary>
    /// 基于 SQLITE 的分页。
    /// </summary>
    public class SQLitePagination : SqlPagination
    {
        /// <summary>
        /// 分页实例。
        /// </summary>
        public static readonly SQLitePagination Instance = new SQLitePagination();
        /// <summary>
        /// 初始化一个 <see cref="Aoite.Data.SQLitePagination"/> 类的新实例。
        /// </summary>
        public SQLitePagination() { }

        /// <summary>
        /// 获取分页的字符串格式项。
        /// </summary>
        protected override string PageFormat
        {
            get
            {
                return @"SELECT * FROM ({0}) table_tmp LIMIT {1} OFFSET {2}";
            }
        }

        /// <summary>
        /// 获取统计的字符串格式项。
        /// </summary>
        protected override string TotalFormat
        {
            get { return @"SELECT COUNT(*) FROM ({0}) ____t____"; }
        }

        /// <summary>
        /// 对指定的 <see cref="System.Data.Common.DbCommand"/> 进行分页处理。
        /// </summary>
        /// <param name="pageNumber">从 1 开始的页码。</param>
        /// <param name="pageSize">页的大小。</param>
        /// <param name="command">数据源查询命令</param>
        public override void ProcessCommand(int pageNumber, int pageSize, DbCommand command)
        {
            var start = (pageNumber - 1) * pageSize;
            command.CommandText = string.Format(PageFormat
                , command.CommandText
                , pageSize
                , start);
        }
    }
}
