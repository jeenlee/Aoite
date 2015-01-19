using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Redis
{
    /// <summary>
    /// 表示使用 Redis 期间发生的错误。
    /// </summary>
    public class RedisException : Exception
    {
        internal RedisException(string message) : base(message) { }
    }

    /// <summary>
    /// 表示使用 Redis 期间发生的通讯错误。
    /// </summary>
    public class RedisIOException : RedisException
    {
        internal RedisIOException(string message) : base(message) { }
    }

    /// <summary>
    /// 表示使用 Redis 期间发生的协议错误。
    /// </summary>
    public class RedisProtocolException : RedisException
    {
        internal RedisProtocolException(string message) : base(message) { }
    }

    /// <summary>
    /// 表示使用 Redis 期间发生的回复错误。
    /// </summary>
    public class RedisReplyException : RedisException
    {
        internal RedisReplyException(string message) : base(message) { }
    }
}
