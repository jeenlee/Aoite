using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Redis
{
    /// <summary>
    /// 表示 Redis 键和乘法因子的集合。
    /// </summary>
    public class RedisWeightDictionary : Dictionary<string, double>
    {
        /// <summary>
        /// 初始化 <see cref="Aoite.Redis.RedisWeightDictionary"/> 类的新实例，该实例为空且具有默认的初始容量，并使用键类型的默认相等比较器。
        /// </summary>
        public RedisWeightDictionary() : base() { }
        /// <summary>
        /// 初始化 <see cref="Aoite.Redis.RedisWeightDictionary"/> 类的新实例，该实例为空且具有指定的初始容量，并为键类型使用默认的相等比较器。
        /// </summary>
        /// <param name="capacity"><see cref="Aoite.Redis.RedisWeightDictionary"/> 可包含的初始元素数。</param>
        public RedisWeightDictionary(int capacity) : base(capacity) { }
        /// <summary>
        /// 初始化 <see cref="Aoite.Redis.RedisWeightDictionary"/> 类的新实例，该实例包含从指定的 System.Collections.Generic.IDictionary&lt;System.String, System.Double&gt; 中复制的元素并为键类型使用默认的相等比较器。
        /// </summary>
        /// <param name="dictionary">System.Collections.Generic.IDictionary&lt;System.String, System.Double&gt;，它的元素被复制到新的 <see cref="Aoite.Redis.RedisWeightDictionary"/></param>
        public RedisWeightDictionary(IDictionary<string, double> dictionary) : base(dictionary) { }

        /// <summary>
        /// 使用默认的乘法因子（1），初始化 <see cref="Aoite.Redis.RedisWeightDictionary"/> 类的新实例，该实例包含从指定的 <paramref name="keys"/> 中复制的元素并为键类型使用默认的相等比较器。
        /// </summary>
        /// <param name="keys">键名的数组。</param>
        public RedisWeightDictionary(params string[] keys)
            : base(keys.Length)
        {
            foreach(var item in keys) this[item] = 1;
        }

        /// <summary>
        /// 初始化 <see cref="Aoite.Redis.RedisWeightDictionary"/> 类的新实例，该实例包含从指定的 <paramref name="keys"/> 和  <paramref name="weights"/> 中复制的元素。
        /// </summary>
        /// <param name="keys">键名的数组。</param>
        /// <param name="weights">乘法因子的数组。</param>
        public RedisWeightDictionary(string[] keys, double[] weights)
            : base(keys.Length)
        {
            if(keys.Length != weights.Length) throw new ArgumentException("键的数组必须和乘法因子数组的长度一致。");
            for(int i = 0; i < keys.Length; i++)
            {
                this[keys[i]] = weights[i];
            }
        }
    }
}
