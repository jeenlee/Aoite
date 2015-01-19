using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Redis.Commands
{
    class RedisItem<T> : RedisCommand<T> where T : IRedisItem
    {
        private bool _checkType;
        public RedisItem(bool checkType, string command, params object[] args)
            : base(command, args)
        {
            this._checkType = checkType;
        }

        internal override T Parse(RedisExecutor executor)
        {
            if(this._checkType)
            {
                executor.AssertType(RedisReplyType.MultiBulk);
                executor.AssertSize(2);
            }
            T item = (T)Activator.CreateInstance(typeof(T), true);
            item.Parse(executor);
            return item;
        }
    }
}
