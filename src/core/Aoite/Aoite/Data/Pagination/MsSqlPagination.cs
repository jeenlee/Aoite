using System.Data.Common;
using System.Text.RegularExpressions;

namespace Aoite.Data
{
    /// <summary>
    /// 基于 MSSQL 的分页。
    /// </summary>
    public class MsSqlPagination : SqlPagination
    {
        /// <summary>
        /// 分页实例。
        /// </summary>
        public static readonly MsSqlPagination Instance = new MsSqlPagination();

        /// <summary>
        /// 初始化一个 <see cref="Aoite.Data.MsSqlPagination"/> 类的新实例。
        /// </summary>
        public MsSqlPagination() { }

        /// <summary>
        /// 获取分页的字符串格式项。
        /// </summary>
        protected override string PageFormat
        {
            get
            {
                return @"SELECT * FROM (SELECT ROW_NUMBER() OVER({4}) AS {1},* FROM ({0}) ____t1____) ____t2____ WHERE {1}>{2} AND {1}<={3}";
            }
        }

        /// <summary>
        /// 获取统计的字符串格式项。
        /// </summary>
        protected override string TotalFormat
        {
            get { return @"SELECT COUNT(*) FROM ({0}) ____t____"; }
        }

        internal static readonly Regex OrderByRegex = new Regex(@"\s*order\s+by\s+[^\s,\)\(]+(?:\s+(?:asc|desc))?(?:\s*,\s*[^\s,\)\(]+(?:\s+(?:asc|desc))?)*", RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// 获取最后一个匹配的 Order By 结果。
        /// </summary>
        /// <param name="commandText">原查询字符串。</param>
        /// <returns>返回 Order By 结果。</returns>
        protected static Match GetOrderByMatch(string commandText)
        {
            var match = OrderByRegex.Match(commandText);
            while(match.Success)
            {
                if((match.Index + match.Length) == commandText.Length) return match;
                match = match.NextMatch();
            }
            return match;
        }

        /// <summary>
        /// 创建指定查询字符串的统计总行数查询字符串。
        /// </summary>
        /// <param name="commandText">原查询字符串。</param>
        public override string CreateTotalCountCommand(string commandText)
        {
            var match = GetOrderByMatch(commandText);
            return string.Format(TotalFormat, match.Success ? commandText.Remove(match.Index) : commandText);
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
            var end = pageNumber * pageSize;
            var match = GetOrderByMatch(command.CommandText);
            var orderBy = "ORDER BY getdate()";
            if(match.Success)
            {
                command.CommandText = command.CommandText.Remove(match.Index);
                orderBy = match.Value.Trim();
            }

            command.CommandText = string.Format(PageFormat
                , command.CommandText
                , this.RowNumberName
                , start
                , end
                , orderBy);
        }
    }
}
