
using System;
using System.Collections.Generic;
using System.Data;

namespace Aoite.Data
{
    /// <summary>
    /// 数据源查询与交互完成后的事件委托。
    /// </summary>
    /// <param name="sender">事件对象。</param>
    /// <param name="e">事件参数。</param>
    public delegate void ExecutedEventHandler(object sender, ExecutedEventArgs e);

    /// <summary>
    /// 表示数据源查询与交互完成后的事件参数。
    /// </summary>
    public class ExecutedEventArgs : ExecutingEventArgs
    {
        internal ExecutedEventArgs(ExecuteCommand command) : base(command) { }

        private IDbResult _Result;
        /// <summary>
        /// 获取一个值，表示数据源的操作结果。
        /// </summary>
        public IDbResult Result { get { return this._Result; } internal set { this._Result = value; } }
    }
}
