using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Cache
{
    static class CacheOptions
    {
        /// <summary>
        /// 单位，秒。
        /// </summary>
        public const string SessionExpiration = "SessionExpiration";
        /// <summary>
        /// 单位，毫秒。
        /// </summary>
        public const string MinTimeout = "MinTimeout";
        internal static int GetLockTimeout(TimeSpan? timeout, int minTimeout)
        {
            int milliseconds = timeout.HasValue
                ? (int)timeout.Value.TotalMilliseconds
                : minTimeout;

            return milliseconds < minTimeout ? minTimeout : milliseconds;
        }
    }
}
