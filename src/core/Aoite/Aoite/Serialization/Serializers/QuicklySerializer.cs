using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Aoite.Serialization.BinarySuite;

namespace Aoite.Serialization
{
    /// <summary>
    /// 表示一个快速的二进制序列化器。
    /// </summary>
    public class QuicklySerializer: Serializer
    {
        /// <summary>
        /// 初始化一个 <see cref="Aoite.Serialization.QuicklySerializer"/> 类的新实例。
        /// </summary>
        public QuicklySerializer() : this(Encoding.UTF8) { }

        /// <summary>
        /// 初始化一个 <see cref="Aoite.Serialization.QuicklySerializer"/> 类的新实例。
        /// </summary>
        /// <param name="encoding">字符编码。</param>
        public QuicklySerializer(Encoding encoding) : base(encoding) { }

        /// <summary>
        /// 读取对象。
        /// </summary>
        /// <param name="stream">序列化的流。</param>
        /// <returns>返回序列化对象。</returns>
        protected override object Reading(Stream stream)
        {
            return new ObjectReader(stream, this.Encoding).Deserialize();
        }

        /// <summary>
        /// 写入可序列化的对象。
        /// </summary>
        /// <typeparam name="TData">可序列化对象的类型。</typeparam>
        /// <param name="stream">可序列化的流。</param>
        /// <param name="data">可序列化的对象。</param>
        protected override void Writing<TData>(Stream stream, TData data)
        {
            new ObjectWriter(stream, this.Encoding).Serialize(data);
        }
    }
}
