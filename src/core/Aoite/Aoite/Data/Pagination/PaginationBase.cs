using System.Data.Common;

namespace Aoite.Data
{
    /// <summary>
    /// 分页基类。
    /// </summary>
    public abstract class PaginationBase
    {
        /// <summary>
        /// 获取默认页码字段的列名。
        /// </summary>
        public const string DefaultRowNumberName = "_RN_";

        /// <summary>
        /// 对指定的 <see cref="System.Data.Common.DbCommand"/> 进行分页处理。
        /// </summary>
        /// <param name="pageNumber">从 1 开始的页码。</param>
        /// <param name="pageSize">页的大小。</param>
        /// <param name="command">数据源查询命令</param>
        public abstract void ProcessCommand(int pageNumber, int pageSize, DbCommand command);

        /// <summary>
        /// 创建指定查询字符串的统计总行数查询字符串。
        /// </summary>
        /// <param name="commandText">原查询字符串。</param>
        public abstract string CreateTotalCountCommand(string commandText);
    }
}