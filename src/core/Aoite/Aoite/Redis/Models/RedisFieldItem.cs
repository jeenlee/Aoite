using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Redis
{
    /// <summary>
    /// 表示一个 Reids 的域值项。
    /// </summary>
    public class RedisFieldItem : IRedisItem
    {
        private string _Field;
        /// <summary>
        /// 获取或设置项的域名。
        /// </summary>
        public string Field { get { return this._Field; } set { this._Field = value; } }

        private BinaryValue _Value;
        /// <summary>
        /// 获取或设置项的域值。
        /// </summary>
        public BinaryValue Value { get { return this._Value; } set { this._Value = value; } }
        
        /// <summary>
        /// 初始化一个 <see cref="Aoite.Redis.RedisFieldItem"/> 类的新实例。
        /// </summary>
        public RedisFieldItem() { }
        
        /// <summary>
        /// 初始化一个 <see cref="Aoite.Redis.RedisFieldItem"/> 类的新实例。
        /// </summary>
        /// <param name="field">域名。</param>
        /// <param name="value">域值。</param>
        public RedisFieldItem(string field, BinaryValue value)
        {
            this._Field = field;
            this._Value = value;
        }

        void IRedisItem.Parse(RedisExecutor executor)
        {
            this._Field = executor.ReadBulkString();
            this._Value = executor.ReadBulk();
        }
    }
}
