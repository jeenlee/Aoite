using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// 表示 Redis 生存时间单位。
    /// </summary>
    public enum RedisExpireTimeUnit
    {
        /// <summary>
        /// 表示以秒为单位的生存时间。
        /// </summary>
        EX,
        /// <summary>
        /// 表示以毫秒为单位的生存时间。
        /// </summary>
        PX
    }

    /// <summary>
    /// 表示 Redis 键的行为。
    /// </summary>
    public enum RedisKeyBehavior
    {
        /// <summary>
        /// 表示采用默认行为。
        /// </summary>
        None,
        /// <summary>
        /// 表示只在键不存在时，才对键进行操作。
        /// </summary>
        NX,
        /// <summary>
        /// 只在键已经存在时，才对键进行操作。
        /// </summary>
        XX
    }

    /// <summary>
    /// 表示 Reids 的插入位置。
    /// </summary>
    public enum RedisInsertPosition
    {
        /// <summary>
        /// 表示在指定的元素前插入。
        /// </summary>
        Before,
        /// <summary>
        /// 表示在指定的元素后插入。
        /// </summary>
        After,
    }

    /// <summary>
    /// 表示 Redis 的聚合方式。
    /// </summary>
    public enum RedisAggregate
    {
        /// <summary>
        /// 表示“和”聚合。
        /// </summary>
        Sum,
        /// <summary>
        /// 表示“最小值”聚合。
        /// </summary>
        Min,
        /// <summary>
        /// 表示“最大值”聚合。
        /// </summary>
        Max,
    }

}
