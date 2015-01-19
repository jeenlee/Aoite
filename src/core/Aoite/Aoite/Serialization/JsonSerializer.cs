using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace Aoite.Serialization
{
    /// <summary>
    /// 表示一个基于 JSON 的序列化器。
    /// </summary>
    public class JsonSerializer : Serializer
    {
        private JavaScriptSerializer _serializer = new JavaScriptSerializer();

        /// <summary>
        /// 初始化一个 <see cref="Aoite.Serialization.JsonSerializer"/> 类的新实例。
        /// </summary>
        public JsonSerializer() : this(new JavaScriptSerializer()) { }
        /// <summary>
        /// 提供一个 <see cref="System.Web.Script.Serialization.JavaScriptSerializer"/>，初始化一个 <see cref="Aoite.Serialization.JsonSerializer"/> 类的新实例。
        /// </summary>
        /// <param name="javaScriptSerializer"></param>
        public JsonSerializer(JavaScriptSerializer javaScriptSerializer)
        {
            _serializer = javaScriptSerializer;
        }

        /// <summary>
        /// 读取对象。
        /// </summary>
        /// <param name="stream">序列化的流。</param>
        /// <returns>返回序列化对象。</returns>
        protected override object Reading(Stream stream)
        {
            using(var stream2 = new StreamReader(stream, this.Encoding))
            {
                var json = stream2.ReadToEnd();
                return _serializer.DeserializeObject(json);
            }
        }

        /// <summary>
        /// 读取对象。
        /// </summary>
        /// <typeparam name="TData">可序列化对象的类型。</typeparam>
        /// <param name="stream">序列化的流。</param>
        /// <returns>返回序列化对象。</returns>
        protected override TData Reading<TData>(Stream stream)
        {
            using(var stream2 = new StreamReader(stream, this.Encoding))
            {
                var json = stream2.ReadToEnd();
                return _serializer.Deserialize<TData>(json);
            }
        }

        /// <summary>
        /// 写入可序列化的对象。
        /// </summary>
        /// <typeparam name="TData">可序列化对象的类型。</typeparam>
        /// <param name="stream">可序列化的流。</param>
        /// <param name="data">可序列化的对象。</param>
        protected override void Writing<TData>(Stream stream, TData data)
        {
            var bytes = this.Encoding.GetBytes(_serializer.Serialize(data));
            stream.Write(bytes, 0, bytes.Length);
        }


        /// <summary>
        /// 快速反序列化 JSON 字符串。
        /// </summary>
        /// <param name="json">JSON 字符串。</param>
        /// <returns>返回一个对象。</returns>
        public object FastRead(string json)
        {
            return _serializer.DeserializeObject(json);
        }
        /// <summary>
        /// 快速反序列化 JSON 字符串。
        /// </summary>
        /// <typeparam name="TData">可序列化对象的类型。</typeparam>
        /// <param name="json">JSON 字符串。</param>
        /// <returns>返回一个对象。</returns>
        public TData FastRead<TData>(string json)
        {
            return _serializer.Deserialize<TData>(json);
        }
        /// <summary>
        /// 快速反序列化 JSON 字符串。
        /// </summary>
        /// <param name="json">JSON 字符串。</param>
        /// <param name="type">可序列化对象的类型。</param>
        /// <returns>返回一个对象。</returns>
        public object FastRead(string json, Type type)
        {
            return _serializer.Deserialize(json, type);
        }

        /// <summary>
        /// 快速序列化 JSON 对象。
        /// </summary>
        /// <typeparam name="TData">可序列化对象的类型。</typeparam>
        /// <param name="data">可序列化的对象。</param>
        /// <returns>返回 JSON 字符串。</returns>
        public string FastWrite<TData>(TData data)
        {
            return _serializer.Serialize(data);
        }

    }
}
