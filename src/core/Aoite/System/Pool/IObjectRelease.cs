using System.Collections.Concurrent;
using System.Threading;

namespace System
{
    /// <summary>
    /// 定义一个可重复使用的对象。
    /// </summary>
    public interface IObjectRelease
    {
        /// <summary>
        /// 对象在释放时发生。
        /// </summary>
        void Release();
    }
}
