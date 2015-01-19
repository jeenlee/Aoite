using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Redis
{
    /// <summary>
    /// 表示一个 Reids 的键值项。
    /// </summary>
    public class RedisKeyItem : IRedisItem
    {
        private string _Key;
        /// <summary>
        /// 获取或设置项的键名。
        /// </summary>
        public string Key { get { return this._Key; } set { this._Key = value; } }

        private BinaryValue _Value;
        /// <summary>
        /// 获取或设置项的键值。
        /// </summary>
        public BinaryValue Value { get { return this._Value; } set { this._Value = value; } }

        /// <summary>
        /// 初始化一个 <see cref="Aoite.Redis.RedisKeyItem"/> 类的新实例。
        /// </summary>
        public RedisKeyItem() { }
        
        /// <summary>
        /// 初始化一个 <see cref="Aoite.Redis.RedisKeyItem"/> 类的新实例。
        /// </summary>
        /// <param name="key">键名。</param>
        /// <param name="value">键值。</param>
        public RedisKeyItem(string key, BinaryValue value)
        {
            this._Key = key;
            this._Value = value;
        }

        void IRedisItem.Parse(RedisExecutor executor)
        {
            this._Key = executor.ReadBulkString();
            this._Value = executor.ReadBulk();
        }
    }
}
