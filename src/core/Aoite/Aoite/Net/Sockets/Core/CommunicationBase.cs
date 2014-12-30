using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Aoite.Net
{

    /// <summary>
    /// 定义一个通讯。
    /// </summary>
    public interface ICommunication : IDisposable
    {
        /// <summary>
        /// 获取或设置与此客户端关联的用户或应用程序对象。
        /// </summary>
        object Tag { get; set; }
        /// <summary>
        /// 获取客户端自定义数据的集合。
        /// </summary>
        AsyncDataCollection Data { get; }
        /// <summary>
        /// 使用 <paramref name="key"/> 获取或设置自定义数据的集合的值。
        /// </summary>
        /// <param name="key">自定义数据的键。</param>
        /// <returns>获取一个已存在的自定义数据的值，或一个 null 值。</returns>
        object this[string key] { get; set; }
        /// <summary>
        /// 获取一个值，表示通讯的状态。
        /// </summary>
        CommunicationState State { get; }
        /// <summary>
        /// 通讯状态发生更改后发生。
        /// </summary>
        event CommunicationStateEventHandler StateChanged;
        /// <summary>
        /// 打开通讯连接。
        /// </summary>
        /// <returns>返回打开通讯的结果。</returns>
        Result Open();

        /// <summary>
        /// 关闭通讯连接时发生。
        /// </summary>
        void Close();

    }
    /// <summary>
    /// 表示一个通讯的基类。
    /// </summary>
    public abstract class CommunicationBase : ObjectDisposableBase, ICommunication
    {
        /// <summary>
        /// 获取或设置与此客户端关联的用户或应用程序对象。
        /// </summary>
        public object Tag { get; set; }

        private Lazy<AsyncDataCollection> _LazyData = new Lazy<AsyncDataCollection>();
        /// <summary>
        /// 获取客户端自定义数据的集合。
        /// </summary>
        public AsyncDataCollection Data { get { return this._LazyData.Value; } }

        /// <summary>
        /// 使用 <paramref name="key"/> 获取或设置自定义数据的集合的值。
        /// </summary>
        /// <param name="key">自定义数据的键。</param>
        /// <returns>获取一个已存在的自定义数据的值，或一个 null 值。</returns>
        public object this[string key] { get { return this.Data.TryGetValue(key); } set { this.Data[key] = value; } }


        private long _IsRunning = 0L;
        /// <summary>
        /// 获取一个值，该值指示通讯是否正在运行中。
        /// </summary>
        public virtual bool IsRunning { get { return Interlocked.Read(ref this._IsRunning) == 1L; } }

        private long _State = 0;

        /// <summary>
        /// 获取一个值，表示通讯的状态。
        /// </summary>
        public CommunicationState State { get { return (CommunicationState)Interlocked.Read(ref this._State); } }

        /// <summary>
        /// 通讯状态发生更改后发生。
        /// </summary>
        public event CommunicationStateEventHandler StateChanged;

        /// <summary>
        /// 初始化一个 <see cref="Aoite.Net.CommunicationBase"/> 类的新实例。
        /// </summary>
        public CommunicationBase() { }

        /// <summary>
        /// 通讯尚未启动时抛出异常。
        /// </summary>
        protected virtual void ThrowWhenUnopened()
        {
            if(!this.IsRunning) throw new NotSupportedException("通讯尚未启动。");
        }

        #region Communication

        /// <summary>
        /// 打开通讯连接。
        /// </summary>
        /// <returns>返回打开通讯的结果。</returns>
        public Result Open()
        {
            this.ThrowWhenDisposed();
            if(!this.IsRunning)
            {
                Interlocked.Exchange(ref this._IsRunning, 1);
                this.SwitchState(CommunicationState.Opening);
                try
                {
                    this.OnOpen();
                    this.SwitchState(CommunicationState.Opened);
                }
                catch(Exception ex)
                {
                    this.SwitchState(ex);
                    this.Close();
                    return ex;
                }
            }
            return Result.Successfully;
        }

        /// <summary>
        /// 打开通讯连接时发生。
        /// </summary>
        protected abstract void OnOpen();

        /// <summary>
        /// 关闭通讯连接时发生。
        /// </summary>
        public void Close()
        {
            this.ThrowWhenDisposed();
            if(!this.IsRunning) return;
            Interlocked.Exchange(ref this._IsRunning, 0);

            this.SwitchState(CommunicationState.Closing);
            try
            {
                this.OnClose();
            }
            finally
            {
                this.SwitchState(CommunicationState.Closed);
            }
        }

        /// <summary>
        /// 关闭通讯时发生。
        /// </summary>
        protected abstract void OnClose();

        #endregion

        #region SwitchState

        /// <summary>
        /// 切换通讯状态。
        /// </summary>
        /// <param name="state">通讯的状态。</param>
        protected void SwitchState(CommunicationState state)
        {
            this.OnSwitchState(state, null);
        }

        /// <summary>
        /// 切换到异常的通讯状态，通讯会先关闭连接，并触发异常状态事件。
        /// </summary>
        /// <param name="exception">通讯中抛出的异常。</param>
        protected void SwitchState(Exception exception)
        {
            this.OnSwitchState(CommunicationState.Failed, exception);
        }

        /// <summary>
        /// 切换通讯状态。
        /// </summary>
        /// <param name="state">通讯的状态。</param>
        /// <param name="exception">通讯中抛出的异常。</param>
        protected internal virtual void OnSwitchState(CommunicationState state, Exception exception)
        {
            Interlocked.Exchange(ref this._State, (long)state);
            if(this.StateChanged != null)
            {
                if(state == CommunicationState.Failed)
                {
                    this.StateChanged(this, new CommunicationFailedStateEventArgs(exception));
                    this.Close();
                }
                else this.StateChanged(this, new CommunicationStateEventArgs(state));
            }
        }

        #endregion
    }
}
