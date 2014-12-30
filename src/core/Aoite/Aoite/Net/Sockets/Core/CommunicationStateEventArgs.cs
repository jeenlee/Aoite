using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Net
{
    /// <summary>
    /// 通讯状态发生更改后发生的事件委托。
    /// </summary>
    /// <param name="sender">事件对象。</param>
    /// <param name="e">事件参数。</param>
    public delegate void CommunicationStateEventHandler(object sender, CommunicationStateEventArgs e);

    /// <summary>
    /// 通讯状态发生更改后发生的事件参数。
    /// </summary>
    public class CommunicationStateEventArgs : EventArgs
    {
        private CommunicationState _State;
        /// <summary>
        /// 获取一个值，表示通讯更改后的状态。
        /// </summary>
        public CommunicationState State
        {
            get
            {
                return this._State;
            }
        }

        /// <summary>
        /// 初始化一个 <see cref="Aoite.Net.CommunicationStateEventArgs"/> 类的新实例。
        /// </summary>
        /// <param name="state">通讯更改后的状态。</param>
        public CommunicationStateEventArgs(CommunicationState state)
        {
            this._State = state;
        }

        /// <summary>
        /// 返回通讯状态的字符串表示形式。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this._State.ToString();
        }
    }

    /// <summary>
    /// 通讯状态发生更改后发生的事件参数（异常状态时）。
    /// </summary>
    public class CommunicationFailedStateEventArgs : CommunicationStateEventArgs
    {
        private Exception _Exception;
        /// <summary>
        /// 获取一个值，表示通讯过程中抛出的异常。
        /// </summary>
        public Exception Exception
        {
            get
            {
                return this._Exception;
            }
        }

        /// <summary>
        /// 初始化一个 <see cref="Aoite.Net.CommunicationFailedStateEventArgs"/> 类的新实例。
        /// </summary>
        /// <param name="exception">通讯的异常。</param>
        public CommunicationFailedStateEventArgs(Exception exception)
            : base(CommunicationState.Failed)
        {
            this._Exception = exception;
        }

        /// <summary>
        /// 获取以字符串形式表示的通讯异常。
        /// </summary>
        public override string ToString()
        {
            return this._Exception.ToString();
        }
    }
}
