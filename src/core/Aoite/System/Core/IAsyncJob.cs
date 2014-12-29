using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// 表示一个异步任务的委托。
    /// </summary>
    /// <param name="job">异步任务。</param>
    public delegate void AsyncJobHandler(IAsyncJob job);

    /// <summary>
    /// 表示一个异步任务的标识。
    /// </summary>
    public interface IAsyncJob
    {
        /// <summary>
        /// 异步任务抛出错误时发生。
        /// </summary>
        event ExceptionEventHandler Error;
        /// <summary>
        /// 获取异步任务的唯一编号。
        /// </summary>
        Guid Id { get; }
        /// <summary>
        /// 获取或设置自定义数据。
        /// </summary>
        object State { get; set; }
        /// <summary>
        /// 获取关联的异步操作。
        /// </summary>
        System.Threading.Tasks.Task Task { get; }
        /// <summary>
        /// 获取此任务是否已完成。
        /// </summary>
        bool IsCompleted { get; }
        /// <summary>
        /// 立即取消异步任务。
        /// </summary>
        void Cancel();
        /// <summary>
        /// 强制终止当前任务。
        /// </summary>
        void Abort();
        /// <summary>
        /// 等待完成执行过程。
        /// </summary>
        /// <param name="millisecondsTimeout">等待的毫秒数，或为 <see cref="System.Threading.Timeout.Infinite"/>(-1)，表示无限期等待。</param>
        /// <returns>如果在分配的时间内完成执行，则为 true；否则为 false。</returns>
        bool Wait(int millisecondsTimeout = System.Threading.Timeout.Infinite);

        /// <summary>
        /// 延迟时间执行任务。
        /// </summary>
        /// <param name="millisecondsDelay">等待延迟的毫秒数。</param>
        void Delay(int millisecondsDelay);
    }
}
