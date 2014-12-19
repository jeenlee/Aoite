using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// 定义一个结果。
    /// </summary>
    public interface IResult
    {
        /// <summary>
        /// 获取或设置执行结果描述错误的信息。
        /// </summary>
        string Message { get; set; }

        /// <summary>
        /// 获取或设置执行时发生的错误。
        /// </summary>
        Exception Exception { get; set; }
        /// <summary>
        /// 获取一个值，表示执行结果是否为失败。
        /// </summary>
        bool IsFailed { get; }

        /// <summary>
        /// 获取一个值，表示执行结果是否为成功。
        /// </summary>
        bool IsSucceed { get; }

        /// <summary>
        /// 获取执行结果的状态码。
        /// </summary>
        int Status { get; }
    }
}
