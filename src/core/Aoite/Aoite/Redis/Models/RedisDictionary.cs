using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Aoite.Redis
{
    /// <summary>
    /// 表示 Redis 键和值的集合。
    /// </summary>
    public class RedisDictionary : Dictionary<string, BinaryValue>
    {
        /// <summary>
        /// 初始化 <see cref="Aoite.Redis.RedisDictionary"/> 类的新实例，该实例为空且具有默认的初始容量，并使用键类型的默认相等比较器。
        /// </summary>
        public RedisDictionary() : base() { }
        /// <summary>
        /// 初始化 <see cref="Aoite.Redis.RedisDictionary"/> 类的新实例，该实例为空且具有指定的初始容量，并为键类型使用默认的相等比较器。
        /// </summary>
        /// <param name="capacity"><see cref="Aoite.Redis.RedisDictionary"/> 可包含的初始元素数。</param>
        public RedisDictionary(int capacity) : base(capacity) { }
        /// <summary>
        /// 初始化 <see cref="Aoite.Redis.RedisDictionary"/> 类的新实例，该实例包含从指定的 System.Collections.Generic.IDictionary&lt;System.String, System.BinaryValue 中复制的元素并为键类型使用默认的相等比较器。
        /// </summary>
        /// <param name="dictionary">System.Collections.Generic.IDictionary&lt;System.String, System.BinaryValue，它的元素被复制到新的 <see cref="Aoite.Redis.RedisDictionary"/></param>
        public RedisDictionary(IDictionary<string, BinaryValue> dictionary) : base(dictionary) { }
        /// <summary>
        /// 用序列化数据初始化 <see cref="Aoite.Redis.RedisDictionary"/> 类的新实例。
        /// </summary>
        /// <param name="info">一个 <see cref="System.Runtime.Serialization.SerializationInfo"/> 对象，它包含序列化 <see cref="Aoite.Redis.RedisDictionary"/> 所需的信息。</param>
        /// <param name="context"><see cref="System.Runtime.Serialization.StreamingContext"/> 结构，该结构包含与 <see cref="Aoite.Redis.RedisDictionary"/> 相关联的序列化流的源和目标。</param>
        protected RedisDictionary(SerializationInfo info, StreamingContext context) : base(info, context) { }

        /// <summary>
        /// 初始化 <see cref="Aoite.Redis.RedisDictionary"/> 类的新实例，该实例包含从指定的 <see cref="System.Collections.IDictionary"/> 中复制的元素并为键类型使用默认的相等比较器。
        /// </summary>
        /// <param name="dictionary"><see cref="System.Collections.IDictionary"/>，它的元素被复制到新的 <see cref="Aoite.Redis.RedisDictionary"/></param>
        public RedisDictionary(System.Collections.IDictionary dictionary)
            : base(dictionary.Count)
        {
            foreach(System.Collections.DictionaryEntry item in dictionary)
            {
                this.Add(Convert.ToString(item.Key), BinaryValue.Create(item.Value));
            }
        }

        /// <summary>
        /// 初始化 <see cref="Aoite.Redis.RedisDictionary"/> 类的新实例，该实例包含从指定的 <see cref="Aoite.Redis.RedisFieldItem"/> 数组中复制的元素并为键类型使用默认的相等比较器。
        /// </summary>
        /// <param name="items"><see cref="Aoite.Redis.RedisFieldItem"/> 数组，它的元素被复制到新的 <see cref="Aoite.Redis.RedisDictionary"/></param>
        public RedisDictionary(params RedisFieldItem[] items)
            : base(items.Length)
        {
            foreach(var item in items)
            {
                this[item.Field] = item.Value;
            }
        }
    }
}
