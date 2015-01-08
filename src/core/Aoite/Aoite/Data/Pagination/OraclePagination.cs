using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace Aoite.Data
{/*
    /// <summary>
    /// 基于 ORACLE 的分页。
    /// </summary>
    public class OraclePagination : SqlPagination
    {
        /// <summary>
        /// 分页实例。
        /// </summary>
        public static readonly OraclePagination Instance = new OraclePagination();
        
        /// <summary>
        /// 初始化一个 <see cref="Aoite.Data.OraclePagination"/> 类的新实例。
        /// </summary>
        public OraclePagination() { }

        /// <summary>
        /// 获取分页的字符串格式项。
        /// </summary>
        protected override string PageFormat
        {
            get
            {
                return @"SELECT * FROM
     (
     SELECT  rownum {1},a.*
     FROM
          (
         {0}
          ) ____a____
     WHERE rownum <= {3}
     ) ____b____
WHERE {1} > {2}";
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
            base.ProcessCommand(pageNumber, pageSize, command);
            command.CommandText = OracleDbInjector.HandingCommandText(command.CommandText);
        }
    }
  */
}
