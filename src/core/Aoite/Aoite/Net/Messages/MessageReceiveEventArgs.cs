using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Net
{
    /// <summary>
    /// 接收消息后发生的事件委托。
    /// </summary>
    /// <param name="sender">事件对象。</param>
    /// <param name="e">事件参数。</param>
    public delegate void MessageReceiveEventHandler(object sender, MessageReceiveEventArgs e);

    /// <summary>
    /// 接收消息后发生的事件参数。
    /// </summary>
    public class MessageReceiveEventArgs : SaeaEventArgs
    {
        internal readonly MessageTranslater Translater;
        private byte[] _MessageBuffer;
        /// <summary>
        /// 获取消息的缓冲区。
        /// </summary>
        public byte[] MessageBuffer { get { return this._MessageBuffer; } internal set { this._MessageBuffer = value; } }

        internal MessageReceiveEventArgs(IMessageSocket messageSocket)
        {
            this.Translater = new MessageTranslater(messageSocket);
        }

        internal override void Release()
        {
            base.Release();
            this._MessageBuffer = null;
            this.Translater.Release();
        }
    }

}
