using Aoite.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace System
{
    /// <summary>
    /// 表示一个序列化器基类。
    /// </summary>
    public abstract class Serializer //: IObjectSerializer
    {
        /// <summary>
        /// 获取默认的快速二进制序列化器。
        /// </summary>
        public readonly static QuicklySerializer Quickly = new QuicklySerializer();
        /// <summary>
        /// 获取默认的二进制序列化器。
        /// </summary>
        public readonly static BinarySerializer Binary = new BinarySerializer();
        /// <summary>
        /// 获取默认的 Xml 序列化器。
        /// </summary>
        public readonly static XmlSerializer Xml = new XmlSerializer();
        /// <summary>
        /// 获取默认的 Json 序列化器。
        /// </summary>
        public readonly static JsonSerializer Json = new JsonSerializer() { };

        private Encoding _Encoding;

        /// <summary>
        /// 初始化一个 <see cref="System.Serializer"/> 类的新实例。
        /// </summary>
        public Serializer() { }

        /// <summary>
        /// 初始化一个 <see cref="System.Serializer"/> 类的新实例。
        /// </summary>
        /// <param name="encoding">字符编码。</param>
        public Serializer(Encoding encoding)
        {
            this._Encoding = encoding;
        }

        /// <summary>
        /// 表示读取后发生，其中 sender 参数是一个 <see cref="System.Result"/> 返回值。
        /// </summary>
        public event EventHandler ReadFinish;

        /// <summary>
        /// 表示写入后发生，其中 sender 参数是一个 <see cref="System.Result"/> 返回值。
        /// </summary>
        public event EventHandler WriteFinish;

        /// <summary>
        /// 获取或设置一个值，表示字符编码。
        /// </summary>
        public virtual Encoding Encoding
        {
            get
            {
                if(this._Encoding == null) return Encoding.UTF8;
                return this._Encoding;
            }
            set
            {
                this._Encoding = value;
            }
        }

        /// <summary>
        /// 读取对象。
        /// </summary>
        /// <typeparam name="TData">可序列化对象的类型。</typeparam>
        /// <param name="path">序列化的路径。</param>
        /// <returns>返回序列化对象的结果。</returns>
        public Result<TData> Read<TData>(string path)
        {
            if(string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");
            using(var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return this.Read<TData>(stream);
            }
        }
        /// <summary>
        /// 读取对象。
        /// </summary>
        /// <param name="path">序列化的路径。</param>
        /// <returns>返回序列化对象的结果。</returns>
        public Result<object> Read(string path)
        {
            if(string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");
            using(var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return this.Read(stream);
            }
        }

        /// <summary>
        /// 读取对象。
        /// </summary>
        /// <typeparam name="TData">可序列化对象的类型。</typeparam>
        /// <param name="stream">序列化的流。</param>
        /// <returns>返回序列化对象的结果。</returns>
        public Result<TData> Read<TData>(Stream stream)
        {
            Result<TData> result = new Result<TData>();
            try
            {
                result.Value = (TData)this.Reading<TData>(stream);
            }
            catch(Exception ex)
            {
                result.ToFailded(new System.IO.IOException("对象 {0} 反序列化失败。".Fmt(typeof(TData).FullName), ex));
            }
            finally
            {
                this.OnReadFinish(result);
            }
            return result;
        }
        /// <summary>
        /// 读取对象。
        /// </summary>
        /// <param name="stream">序列化的流。</param>
        /// <returns>返回序列化对象的结果。</returns>
        public Result<object> Read(Stream stream)
        {
            Result<object> result = new Result<object>();
            try
            {
                result.Value = this.Reading(stream);
            }
            catch(Exception ex)
            {
                result.ToFailded(new System.IO.IOException("对象反序列化失败。", ex));
            }
            finally
            {
                this.OnReadFinish(result);
            }
            return result;
        }

        /// <summary>
        /// 读取对象。
        /// </summary>
        /// <typeparam name="TData">可序列化对象的类型。</typeparam>
        /// <param name="content">可序列化为对象的内容文本。</param>
        /// <param name="encoding">自定义字符编码。</param>
        /// <returns>返回序列化对象的结果。</returns>
        public Result<TData> ReadString<TData>(string content, Encoding encoding = null)
        {
            if(string.IsNullOrEmpty(content))
                return new ArgumentNullException("data");
            using(MemoryStream stream = new MemoryStream((encoding ?? this.Encoding).GetBytes(content)))
            {
                return this.Read<TData>(stream);
            }
        }
        /// <summary>
        /// 读取对象。
        /// </summary>
        /// <param name="content">可序列化为对象的内容文本。</param>
        /// <param name="encoding">自定义字符编码。</param>
        /// <returns>返回序列化对象的结果。</returns>
        public Result<object> ReadString(string content, Encoding encoding = null)
        {
            if(string.IsNullOrEmpty(content))
                return new ArgumentNullException("data");
            using(MemoryStream stream = new MemoryStream((encoding ?? this.Encoding).GetBytes(content)))
            {
                return this.Read(stream);
            }
        }

        /// <summary>
        /// 读取对象。
        /// </summary>
        /// <typeparam name="TData">可序列化对象的类型。</typeparam>
        /// <param name="bytes">可序列化为对象的二进制数组。</param>
        /// <returns>返回序列化对象的结果。</returns>
        public Result<TData> ReadBytes<TData>(byte[] bytes)
        {
            using(MemoryStream stream = new MemoryStream(bytes))
            {
                return this.Read<TData>(stream);
            }
        }
        /// <summary>
        /// 读取对象。
        /// </summary>
        /// <param name="bytes">可序列化为对象的二进制数组。</param>
        /// <returns>返回序列化对象的结果。</returns>
        public Result<object> ReadBytes(byte[] bytes)
        {
            using(MemoryStream stream = new MemoryStream(bytes))
            {
                return this.Read(stream);
            }
        }

        /// <summary>
        /// 快速读取对象。
        /// </summary>
        /// <typeparam name="TData">可序列化对象的类型。</typeparam>
        /// <param name="bytes">可序列化为对象的二进制数组。</param>
        /// <returns>返回序列化对象的对象。</returns>
        public TData FastReadBytes<TData>(byte[] bytes)
        {
            return this.ReadBytes<TData>(bytes).UnsafeValue;
        }

        /// <summary>
        /// 快速读取对象。
        /// </summary>
        /// <param name="bytes">可序列化为对象的二进制数组。</param>
        /// <returns>返回序列化对象的对象。</returns>
        public object FastReadBytes(byte[] bytes)
        {
            return this.ReadBytes(bytes).UnsafeValue;
        }

        /// <summary>
        /// 写入可序列化的对象。
        /// </summary>
        /// <typeparam name="TData">可序列化对象的类型。</typeparam>
        /// <param name="path">序列化的路径。</param>
        /// <param name="data">可序列化的对象。</param>
        /// <returns>返回一个结果，指示序列化是否成功。</returns>
        public Result Write<TData>(string path, TData data)
        {
            using(var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                return this.Write<TData>(stream, data);
            }
        }

        /// <summary>
        /// 写入可序列化的对象。
        /// </summary>
        /// <typeparam name="TData">可序列化对象的类型。</typeparam>
        /// <param name="stream">可序列化的流。</param>
        /// <param name="data">可序列化的对象。</param>
        /// <returns>返回一个结果，指示序列化是否成功。</returns>
        public Result Write<TData>(Stream stream, TData data)
        {
            Result result = new Result();
            try
            {
                this.Writing<TData>(stream, data);
            }
            catch(Exception ex)
            {
                result.ToFailded(new System.IO.IOException("对象 {0} 序列化失败。".Fmt((data == null ? typeof(TData) : data.GetType()).FullName), ex));
            }
            finally
            {
                this.OnWriteFinish(result);
            }
            return result;
        }

        /// <summary>
        /// 写入可序列化的对象。
        /// </summary>
        /// <typeparam name="TData">可序列化对象的类型。</typeparam>
        /// <param name="data">可序列化为字符串的对象。</param>
        /// <param name="encoding">自定义字符编码。</param>
        /// <returns>返回一个结果，指示序列化是否成功。</returns>
        public Result<string> WriteString<TData>(TData data, Encoding encoding = null)
        {
            Result<string> result = new Result<string>();

            using(MemoryStream stream = new MemoryStream())
            {
                var r = this.Write<TData>(stream, data);
                if(r.IsSucceed)
                {
                    stream.Position = 0;
                    var v = stream.ToArray();
                    result.Value = (encoding ?? this.Encoding).GetString(v, 0, v.Length);
                }
                else
                {
                    result.ToFailded(r.Exception);
                }
            }
            return result;
        }

        /// <summary>
        /// 写入可序列化的对象。
        /// </summary>
        /// <typeparam name="TData">可序列化对象的类型。</typeparam>
        /// <param name="data">可序列化为字符串的对象。</param>
        /// <returns>返回一个结果，指示序列化是否成功。</returns>
        public Result<byte[]> WriteBytes<TData>(TData data)
        {
            using(MemoryStream stream = new MemoryStream())
            {
                var r = this.Write(stream, data);
                if(r.IsFailed) return r.Exception;
                return stream.ToArray();/*.ToArray();/*<<- 内存泄漏*/
            }
        }

        /// <summary>
        /// 快速写入可序列化的对象。
        /// </summary>
        /// <typeparam name="TData">可序列化对象的类型。</typeparam>
        /// <param name="data">可序列化为字符串的对象。</param>
        /// <returns>返回一个二进制数组。</returns>
        public byte[] FastWriteBytes<TData>(TData data)
        {
            return this.WriteBytes<TData>(data).UnsafeValue;
        }

        /// <summary>
        /// 表示引发 <see cref="System.Serializer.ReadFinish"/> 事件。
        /// </summary>
        /// <param name="result">读取的返回值。</param>
        protected virtual void OnReadFinish(object result)
        {
            if(this.ReadFinish != null)
                this.ReadFinish(result, EventArgs.Empty);
        }

        /// <summary>
        /// 表示引发 <see cref="System.Serializer.WriteFinish"/> 事件。
        /// </summary>
        /// <param name="result">写入的返回值。</param>
        protected virtual void OnWriteFinish(object result)
        {
            if(this.WriteFinish != null)
                this.WriteFinish(result, EventArgs.Empty);
        }


        /// <summary>
        /// 读取对象。
        /// </summary>
        /// <typeparam name="TData">可序列化对象的类型。</typeparam>
        /// <param name="stream">序列化的流。</param>
        /// <returns>返回序列化对象。</returns>
        protected virtual TData Reading<TData>(Stream stream)
        {
            return (TData)Reading(stream);
        }

        /// <summary>
        /// 读取对象。
        /// </summary>
        /// <param name="stream">序列化的流。</param>
        /// <returns>返回序列化对象。</returns>
        protected abstract object Reading(Stream stream);

        /// <summary>
        /// 写入可序列化的对象。
        /// </summary>
        /// <typeparam name="TData">可序列化对象的类型。</typeparam>
        /// <param name="stream">可序列化的流。</param>
        /// <param name="data">可序列化的对象。</param>
        protected abstract void Writing<TData>(Stream stream, TData data);
    }
}