using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Aoite.Serialization
{
    using X2 = System.Xml.Serialization.XmlSerializer;

    /// <summary>
    /// 表示 Xml 的序列化器。
    /// </summary>
    public class XmlSerializer : Serializer
    {
        /// <summary>
        /// 初始化一个 <see cref="Aoite.Serialization.XmlSerializer"/> 类的新实例。
        /// </summary>
        public XmlSerializer() { }
        /// <summary>
        /// 初始化一个 <see cref="Aoite.Serialization.XmlSerializer"/> 类的新实例。
        /// </summary>
        /// <param name="encoding">字符编码。</param>
        public XmlSerializer(Encoding encoding) : base(encoding) { }

        /// <summary>
        /// 获取序列化器。
        /// </summary>
        /// <returns>返回一个序列化器。</returns>
        protected virtual X2 GetSerializer(Type type)
        {
            return new X2(type);
        }

        /// <summary>
        /// 读取对象。
        /// </summary>
        /// <typeparam name="TData">可序列化对象的类型。</typeparam>
        /// <param name="stream">序列化的流。</param>
        /// <returns>返回序列化对象。</returns>
        protected override TData Reading<TData>(Stream stream)
        {
            return (TData)this.GetSerializer(typeof(TData)).Deserialize(stream);
        }

        /// <summary>
        /// 读取对象。
        /// </summary>
        /// <param name="stream">序列化的流。</param>
        /// <returns>返回序列化对象。</returns>
        protected override object Reading(Stream stream)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 写入可序列化的对象。
        /// </summary>
        /// <typeparam name="TData">可序列化对象的类型。</typeparam>
        /// <param name="stream">可序列化的流。</param>
        /// <param name="data">可序列化的对象。</param>
        protected override void Writing<TData>(Stream stream, TData data)
        {
            this.GetSerializer(typeof(TData)).Serialize(stream, data);
        }
    }
}
