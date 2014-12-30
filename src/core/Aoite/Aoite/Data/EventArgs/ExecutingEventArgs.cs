using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Data
{
    /// <summary>
    /// 数据源查询与交互执行前发生的事件委托。
    /// </summary>
    /// <param name="sender">事件对象。</param>
    /// <param name="e">事件参数。</param>
    public delegate void ExecutingEventHandler(object sender, ExecutingEventArgs e);

    /// <summary>
    /// 数据源查询与交互执行前发生的事件参数。
    /// </summary>
    public class ExecutingEventArgs : EventArgs
    {
        internal ExecutingEventArgs(ExecuteCommand command)
        {
            this._Command = command;
        }

        private ExecuteCommand _Command;
        /// <summary>
        /// 获取一个值，表示执行命令。
        /// </summary>
        public ExecuteCommand Command { get { return this._Command; } }

        internal Aoite.Data.Profilers.ProfilerItem ProfilerItem;

        private ExecuteType _ExecuteType;
        /// <summary>
        /// 获取一个值，表示执行查询的操作类型。
        /// </summary>
        public ExecuteType ExecuteType { get { return this._ExecuteType; } internal set { this._ExecuteType = value; } }
    }
}
