using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Redis.Commands
{
    /// <summary>
    /// 表示一个 Redis 的命令。
    /// </summary>
    /// <typeparam name="T">命令返回值的数据类型。</typeparam>
    public abstract class RedisCommand<T> : RedisCommand
    {
        internal RedisCommand(string command, params object[] args) : base(command, args) { }

        internal abstract T Parse(RedisExecutor executor);

        Action<T> _callback;
        internal override void SetCallback<TValue>(Action<TValue> callback)
        {
            _callback = v => callback((TValue)((object)v));
        }

        internal override void RunCallback(RedisExecutor executor)
        {
            var value = this.Parse(executor);
            var callback = this._callback;
            if(callback != null)
            {
                callback(value);
            }
        }
    }
}
