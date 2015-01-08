using System.Threading;
using System.Threading.Tasks;

namespace System
{
    /// <summary>
    /// 表示异步操作的一系列扩展。
    /// </summary>
    public static class Ajob
    {
        class AsyncJob : ObjectDisposableBase, IAsyncJob
        {
            private Task _Task;
            private object _State;
            private TimeSpan _Interval;
            private AsyncJobHandler _Job;

            public TaskStatus Status
            {
                get
                {
                    this.ThrowWhenDisposed();
                    return _Task == null ? TaskStatus.Created : _Task.Status;
                }
            }

            public object State
            {
                get
                {
                    this.ThrowWhenDisposed();
                    return _State;
                }
                set
                {
                    this.ThrowWhenDisposed();
                    _State = value;
                }
            }

            public Task Task
            {
                get
                {
                    this.ThrowWhenDisposed();
                    return this._Task;
                }
            }

            public AsyncJob(AsyncJobHandler job, TimeSpan timeSpan, object state)
            {
                if(job == null) throw new ArgumentNullException("job");
                _Interval = timeSpan;
                _Job = job;
                this._State = state;
            }

            public AsyncJob Start(Action<object> action, TaskCreationOptions options)
            {
                _Task = Task.Factory.StartNew(action, this, options);
                return this;
            }

            public bool Wait(int millisecondsTimeout = Timeout.Infinite)
            {
                this.ThrowWhenDisposed();
                return _Task.Wait(millisecondsTimeout);
            }

            public void Cancel()
            {
                this.ThrowWhenDisposed();
                this.Dispose();
            }

            public bool WaitForNextTask()
            {
                return Thread.CurrentThread.Join(_Interval);
            }

            public void RunTask()
            {
                _Job(this);
            }

            protected override void DisposeManaged()
            {
                this._Task = null;
                this._Job = null;
                this._State = null;
                base.DisposeManaged();
            }

            public void Delay(int millisecondsDelay)
            {
                this.ThrowWhenDisposed();
                Thread.CurrentThread.Join(millisecondsDelay);
            }

            public void OnError(Exception exception)
            {
                var ev = GlobalError;
                if(ev != null) ev(this, new ExceptionEventArgs(exception));
            }
        }

        private static void OnceInvoke(object state)
        {
            AsyncJob tme = state as AsyncJob;
            if(tme.IsDisposed || tme.WaitForNextTask() || tme.IsDisposed) return;
            try
            {
                tme.RunTask();
            }
            catch(Exception ex)
            {
                tme.OnError(ex);
                throw;
            }
        }
        private static void LoopInvoke(object state)
        {
            AsyncJob tme = state as AsyncJob;
            while(true)
            {
                if(tme.IsDisposed || tme.WaitForNextTask()) return;
                try
                {
                    tme.RunTask();
                }
                catch(Exception ex)
                {
                    tme.OnError(ex);
                    throw;
                }
            }
        }

        /// <summary>
        /// 当异步任务发生异常时发生。
        /// </summary>
        public static event ExceptionEventHandler GlobalError;

        /// <summary>
        /// 指定 <paramref name="milliseconds"/> 毫秒后执行一次 <paramref name="job"/>。
        /// </summary>
        /// <param name="job">异步任务。</param>
        /// <param name="milliseconds">间隔的毫秒数，为 0 时表示立即执行。</param>
        /// <param name="state">自定义数据。</param>
        /// <returns>返回异步任务的标识</returns>
        public static IAsyncJob Once(AsyncJobHandler job, double milliseconds = 0, object state = null)
        {
            return Once(job, TimeSpan.FromMilliseconds(milliseconds), state);
        }

        /// <summary>
        /// 指定间隔时间执行一次 <paramref name="job"/>。
        /// </summary>
        /// <param name="interval">间隔的时间。</param>
        /// <param name="job">异步任务。</param>
        /// <param name="state">自定义数据。</param>
        /// <returns>返回异步任务的标识</returns>
        public static IAsyncJob Once(AsyncJobHandler job, TimeSpan interval, object state = null)
        {
            return new AsyncJob(job, interval, state).Start(OnceInvoke, TaskCreationOptions.PreferFairness);
        }

        /// <summary>
        /// 指定间隔 <paramref name="milliseconds"/> 毫秒后重复执行 <paramref name="job"/>。
        /// </summary>
        /// <param name="milliseconds">间隔的毫秒数，为 0 时表示立即执行。</param>
        /// <param name="job">异步任务。</param>
        /// <param name="state">自定义数据。</param>
        /// <returns>返回异步任务的标识</returns>
        public static IAsyncJob Loop(AsyncJobHandler job, double milliseconds = 0, object state = null)
        {
            return Loop(job, TimeSpan.FromMilliseconds(milliseconds), state);
        }

        /// <summary>
        /// 指定间隔时间重复执行 <paramref name="job"/>。
        /// </summary>
        /// <param name="interval">间隔的时间。</param>
        /// <param name="job">异步任务。</param>
        /// <param name="state">自定义数据。</param>
        /// <returns>返回异步任务的标识</returns>
        public static IAsyncJob Loop(AsyncJobHandler job, TimeSpan interval, object state = null)
        {
            return new AsyncJob(job, interval, state).Start(LoopInvoke, TaskCreationOptions.LongRunning);
        }
    }
}
