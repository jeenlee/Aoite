using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Redis.Commands
{
    /// <summary>
    /// 表示一个 Redis 的命令。
    /// </summary>
    public abstract class RedisCommand
    {
        private readonly string _command;
        private readonly object[] _args;
        /// <summary>
        /// 获取命令。
        /// </summary>
        public string Command { get { return _command; } }
        /// <summary>
        /// 获取参数。
        /// </summary>
        public object[] Arguments { get { return _args; } }
        
        internal RedisCommand(string command, params object[] args)
        {
            if(string.IsNullOrEmpty(command)) throw new ArgumentNullException("command");
            if(args == null) throw new ArgumentNullException("args");
            _command = command;
            _args = args;
        }

        internal abstract void SetCallback<TValue>(Action<TValue> callback);
        internal abstract void RunCallback(RedisExecutor executor);
    }
}
