using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// 表示 <see cref="System.Result" /> 的状态。
    /// </summary>
    public class ResultStatus
    {
        /// <summary>
        /// 成功的结果。
        /// </summary>
        public const int Succeed = 0;
        /// <summary>
        /// 失败的结果。
        /// </summary>
        public const int Failed = -1;
    }
}
