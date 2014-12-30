using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Aoite.Serialization
{
    /// <summary>
    /// 表示二进制的序列化器。
    /// </summary>
    public class BinarySerializer : Serializer
    {
        /// <summary>
        /// 初始化一个 <see cref="Aoite.Serialization.BinarySerializer"/> 类的新实例。
        /// </summary>
        public BinarySerializer() { }

        /// <summary>
        /// 初始化一个 <see cref="Aoite.Serialization.BinarySerializer"/> 类的新实例。
        /// </summary>
        /// <param name="encoding">字符编码。</param>
        public BinarySerializer(Encoding encoding) : base(encoding) { }

        /// <summary> 
        /// 创建序列化器。
        /// </summary>
        public event Func<BinaryFormatter> CreateSerializer;

        /// <summary>
        /// 获取序列化器。
        /// </summary>
        /// <returns>返回一个序列化器。</returns>
        protected virtual BinaryFormatter GetSerializer()
        {
            if(CreateSerializer == null)
            {
                var b = new BinaryFormatter();
                b.AssemblyFormat = FormatterAssemblyStyle.Simple;

                return b;
            }
            return this.CreateSerializer();
        }

        /// <summary>
        /// 读取对象。
        /// </summary>
        /// <param name="stream">序列化的流。</param>
        /// <returns>返回序列化对象。</returns>
        protected override object Reading(Stream stream)
        {
            return this.GetSerializer().Deserialize(stream);
        }

        /// <summary>
        /// 写入可序列化的对象。
        /// </summary>
        /// <typeparam name="TData">可序列化对象的类型。</typeparam>
        /// <param name="stream">可序列化的流。</param>
        /// <param name="data">可序列化的对象。</param>
        protected override void Writing<TData>(Stream stream, TData data)
        {
            this.GetSerializer().Serialize(stream, data);
        }
    }
}