using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Redis
{
    /// <summary>
    /// 表示一个 Reids 的成员权重项。
    /// </summary>
    public class RedisScoreItem : IRedisItem
    {
        private double _Score;
        /// <summary>
        /// 获取或设置项的权重。
        /// </summary>
        public double Score { get { return this._Score; } set { this._Score = value; } }

        private BinaryValue _Member;
        /// <summary>
        /// 获取或设置项的成员。
        /// </summary>
        public BinaryValue Member { get { return this._Member; } set { this._Member = value; } }

        /// <summary>
        /// 初始化一个 <see cref="Aoite.Redis.RedisScoreItem"/> 类的新实例。
        /// </summary>
        public RedisScoreItem() { }

        /// <summary>
        /// 初始化一个 <see cref="Aoite.Redis.RedisScoreItem"/> 类的新实例。
        /// </summary>
        /// <param name="score">权重值。</param>
        /// <param name="member">成员。</param>
        public RedisScoreItem(double score, BinaryValue member)
        {
            this._Score = score;
            this._Member = member;
        }

        void IRedisItem.Parse(RedisExecutor executor)
        {
            this._Member = executor.ReadBulk();
            this._Score = Aoite.Redis.Commands.RedisFloat.FromString(executor.ReadBulkString());
        }
    }
}
