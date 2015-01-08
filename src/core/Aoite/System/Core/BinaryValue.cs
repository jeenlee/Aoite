using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    //TODO：未完成。需要完成 EntityMapper、QuicklySerializer 模块
    /// <summary>
    /// 表示一个二进制的值。
    /// </summary>
    public partial class BinaryValue
    {
        private byte[] _ByteArray;
        /// <summary>
        /// 获取字节数组。
        /// </summary>
        public byte[] ByteArray { get { return this._ByteArray; } }

        ///// <summary>
        ///// 初始化一个 <see cref="System.BinaryValue"/> 类的新实例。
        ///// </summary>
        ///// <param name="value">待序列化的对象。可以为 null 值。</param>
        //public BinaryValue(object value) : this(value == null ? null : Serializer.Quickly.FastWriteBytes(value)) { }

        /// <summary>
        /// 初始化一个 <see cref="System.BinaryValue"/> 类的新实例。
        /// </summary>
        /// <param name="bytes">字节数组。可以为 null 值。</param>
        public BinaryValue(byte[] bytes)
        {
            this._ByteArray = bytes;
        }

        ///// <summary>
        ///// 将当前字节转换为对象实例。
        ///// </summary>
        ///// <typeparam name="TModel">对象的数据类型。</typeparam>
        ///// <returns>返回一个对象。</returns>
        //public TModel ToModel<TModel>()
        //{
        //    if(this._ByteArray == null) return default(TModel);
        //    return Serializer.Quickly.ReadBytes<TModel>(this._ByteArray).UnsafeValue;
        //}
        ///// <summary>
        ///// 将当前字节转换为对象实例。
        ///// </summary>
        ///// <returns>返回一个对象。</returns>
        //public object ToModel()
        //{

        //    if(this._ByteArray == null) return null;
        //    return Serializer.Quickly.ReadBytes(this._ByteArray).UnsafeValue;
        //}

        /// <summary>
        /// <see cref="System.BinaryValue"/> 和 <see cref="System.Byte"/>[] 的隐式转换。
        /// </summary>
        /// <param name="value">一个二进制的值。</param>
        /// <returns>返回一个 <see cref="System.Byte"/>[] 的新实例。</returns>
        public static implicit operator byte[](BinaryValue value)
        {
            if(value == null) return null;
            return value._ByteArray;
        }

        /// <summary>
        /// <see cref="System.Byte"/>[] 和 <see cref="System.BinaryValue"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个 <see cref="System.Byte"/>[] 的新实例。</param>
        /// <returns>一个二进制的值。</returns>
        public static implicit operator BinaryValue(byte[] value)
        {
            return new BinaryValue(value);
        }

        /// <summary>
        /// <see cref="System.BinaryValue"/> 和 <see cref="System.Decimal"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个二进制的值。</param>
        /// <returns>返回一个 <see cref="System.Decimal"/> 的新实例。</returns>
        public static implicit operator Decimal(BinaryValue value)
        {
            if(value == null) return default(Decimal);
            var bits = new int[]
                {
                    BitConverter.ToInt32(value._ByteArray, 00),
                    BitConverter.ToInt32(value._ByteArray, 04),
                    BitConverter.ToInt32(value._ByteArray, 08),
                    BitConverter.ToInt32(value._ByteArray, 12)
                };
            return new decimal(bits);
        }
        /// <summary>
        /// <see cref="System.Decimal"/> 和 <see cref="System.BinaryValue"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个 <see cref="System.Decimal"/> 的新实例。</param>
        /// <returns>一个二进制的值。</returns>
        public static implicit operator BinaryValue(Decimal value)
        {
            var bits = Decimal.GetBits(value);
            byte[] bytesValue = new byte[16];
            Buffer.BlockCopy(BitConverter.GetBytes(bits[0]), 0, bytesValue, 00, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(bits[1]), 0, bytesValue, 04, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(bits[2]), 0, bytesValue, 08, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(bits[3]), 0, bytesValue, 12, 4);
            return new BinaryValue(bytesValue);
        }

        /// <summary>
        /// <see cref="System.BinaryValue"/> 和 <see cref="System.Guid"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个二进制的值。</param>
        /// <returns>返回一个 <see cref="System.Guid"/> 的新实例。</returns>
        public static implicit operator Guid(BinaryValue value)
        {
            if(value == null) return default(Guid);
            return new Guid(value._ByteArray);
        }
        /// <summary>
        /// <see cref="System.Guid"/> 和 <see cref="System.BinaryValue"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个 <see cref="System.Guid"/> 的新实例。</param>
        /// <returns>一个二进制的值。</returns>
        public static implicit operator BinaryValue(Guid value)
        {
            return new BinaryValue(value.ToByteArray());
        }

        /// <summary>
        /// <see cref="System.BinaryValue"/> 和 <see cref="System.String"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个二进制的值。</param>
        /// <returns>返回一个 <see cref="System.String"/> 的新实例。</returns>
        public static implicit operator String(BinaryValue value)
        {
            if(value == null) return null;
            return value.ToString();
        }
        /// <summary>
        /// <see cref="System.String"/> 和 <see cref="System.BinaryValue"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个 <see cref="System.String"/> 的新实例。</param>
        /// <returns>一个二进制的值。</returns>
        public static implicit operator BinaryValue(String value)
        {
            return new BinaryValue(Encoding.UTF8.GetBytes(value));
        }

        /// <summary>
        /// <see cref="System.BinaryValue"/> 和 <see cref="System.DateTime"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个二进制的值。</param>
        /// <returns>返回一个 <see cref="System.DateTime"/> 的新实例。</returns>
        public static implicit operator DateTime(BinaryValue value)
        {
            if(value == null) return default(DateTime);
            return DateTime.FromBinary(BitConverter.ToInt64(value._ByteArray, 0));
        }
        /// <summary>
        /// <see cref="System.DateTime"/> 和 <see cref="System.BinaryValue"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个 <see cref="System.DateTime"/> 的新实例。</param>
        /// <returns>一个二进制的值。</returns>
        public static implicit operator BinaryValue(DateTime value)
        {
            return new BinaryValue(BitConverter.GetBytes(value.ToBinary()));
        }

        /// <summary>
        /// <see cref="System.BinaryValue"/> 和 <see cref="System.TimeSpan"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个二进制的值。</param>
        /// <returns>返回一个 <see cref="System.TimeSpan"/> 的新实例。</returns>
        public static implicit operator TimeSpan(BinaryValue value)
        {
            if(value == null) return default(TimeSpan);
            return new TimeSpan(BitConverter.ToInt64(value._ByteArray, 0));
        }
        /// <summary>
        /// <see cref="System.TimeSpan"/> 和 <see cref="System.BinaryValue"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个 <see cref="System.TimeSpan"/> 的新实例。</param>
        /// <returns>一个二进制的值。</returns>
        public static implicit operator BinaryValue(TimeSpan value)
        {
            return new BinaryValue(BitConverter.GetBytes(value.Ticks));
        }

        /// <summary>
        /// 返回当前值的 UTF8 编码字符串。
        /// </summary>
        public override string ToString()
        {
            if(this._ByteArray == null) return null;
            if(this._ByteArray.Length == 0) return string.Empty;
            return Encoding.UTF8.GetString(this._ByteArray);
        }
    }
}
