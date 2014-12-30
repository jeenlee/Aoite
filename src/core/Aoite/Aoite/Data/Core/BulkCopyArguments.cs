using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Aoite.Data
{
    /// <summary>
    /// 数据批量加载功能参数。
    /// </summary>
    public class BulkCopyArguments
    {
        /// <summary>
        /// 初始化一个 <see cref="Aoite.Data.BulkCopyArguments"/> 类的新实例。
        /// </summary>
        /// <param name="destinationTableName">服务器上目标表的名称。</param>
        /// <param name="table">一个 <see cref="System.Data.DataTable"/>，它的行将被复制到目标表中。</param>
        public BulkCopyArguments(string destinationTableName, DataTable table) : this(destinationTableName, table, (DataRowState)0) { }
        /// <summary>
        /// 初始化一个 <see cref="Aoite.Data.BulkCopyArguments"/> 类的新实例。
        /// </summary>
        /// <param name="destinationTableName">服务器上目标表的名称。</param>
        /// <param name="table">一个 <see cref="System.Data.DataTable"/>，它的行将被复制到目标表中。</param>
        /// <param name="rowState"><see cref="System.Data.DataRowState"/> 枚举中的一个值。只有与行状态匹配的行才会被复制到目标表中。</param>
        public BulkCopyArguments(string destinationTableName, DataTable table, DataRowState rowState)
        {
            if(string.IsNullOrEmpty(destinationTableName)) throw new ArgumentNullException("destinationTableName");
            if(table == null) throw new ArgumentNullException("table");

            this._DestinationTableName = destinationTableName;
            this._Table = table;
            this._RowState = rowState;
        }

        private int _BatchSize;
        /// <summary>
        /// 获取或设置每一批次中的行数。在每一批次结束时，将该批次中的行发送到服务器。
        /// <para>整数值；或者如果未设置任何值，则为零。</para>
        /// </summary>
        public int BatchSize
        {
            get
            {
                return this._BatchSize;
            }
            set
            {
                this._BatchSize = value;
            }
        }

        private int _BulkCopyTimeout;
        /// <summary>
        /// 获取或设置超时之前操作完成所允许的秒数。
        /// <para>整数值。</para>
        /// </summary>
        public int BulkCopyTimeout
        {
            get
            {
                return this._BulkCopyTimeout;
            }
            set
            {
                this._BulkCopyTimeout = value;
            }
        }

        private string _DestinationTableName;
        /// <summary>
        /// 获取服务器上目标表的名称。
        /// <para>字符串值；或者如果未提供任何值，则为 null。</para>
        /// </summary>
        public string DestinationTableName
        {
            get
            {
                return this._DestinationTableName;
            }
        }

        private int _NotifyAfter;
        /// <summary>
        /// 获取或设置定义在生成通知事件之前要处理的行数。
        /// <para>整数值，或者如果未设置该属性，则为零。</para>
        /// </summary>
        public int NotifyAfter
        {
            get
            {
                return this._NotifyAfter;
            }
            set
            {
                this._NotifyAfter = value;
            }
        }

        private RowsCopiedDelegate _RowsCopied;
        /// <summary>
        /// 获取或设置在每次处理完 <see cref="Aoite.Data.BulkCopyArguments.NotifyAfter"/> 属性指定的行数时发生。
        /// </summary>
        public RowsCopiedDelegate RowsCopied
        {
            get
            {
                return this._RowsCopied;
            }
            set
            {
                this._RowsCopied = value;
            }
        }

        private DataTable _Table;
        /// <summary>
        /// 获取一个 <see cref="System.Data.DataTable"/>，它的行将被复制到目标表中。
        /// </summary>
        public DataTable Table
        {
            get
            {
                return this._Table;
            }
        }

        private DataRowState _RowState;
        /// <summary>
        /// 获取 <see cref="System.Data.DataRowState"/> 枚举中的一个值。只有与行状态匹配的行才会被复制到目标表中。
        /// </summary>
        public DataRowState RowState
        {
            get
            {
                return this._RowState;
            }
        }
    }

    /// <summary>
    /// 批量操作时的委托。
    /// </summary>
    /// <param name="rowsCopied">当前批量复制操作期间复制的行数。</param>
    /// <returns>返回一个值，指示是否应中止批量复制操作的值。</returns>
    public delegate bool RowsCopiedDelegate(long rowsCopied);
}
