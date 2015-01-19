using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// 表示一个二进制的值。
    /// </summary>
    public partial class BinaryValue
    {
        internal static bool HasValue(BinaryValue value)
        {
            return value != null && value._ByteArray != null && value._ByteArray.Length > 0;
        }

        private byte[] _ByteArray;
        /// <summary>
        /// 获取字节数组。
        /// </summary>
        public byte[] ByteArray { get { return this._ByteArray; } }

        /// <summary>
        /// 初始化一个 <see cref="System.BinaryValue"/> 类的新实例。
        /// </summary>
        /// <param name="value">待序列化的对象。可以为 null 值。</param>
        public BinaryValue(object value) : this(value == null ? null : Serializer.Quickly.FastWriteBytes(value)) { }

        /// <summary>
        /// 初始化一个 <see cref="System.BinaryValue"/> 类的新实例。
        /// </summary>
        /// <param name="bytes">字节数组。可以为 null 值。</param>
        public BinaryValue(byte[] bytes)
        {
            this._ByteArray = bytes;
        }

        /// <summary>
        /// 将当前字节转换为对象实例。
        /// </summary>
        /// <typeparam name="TModel">对象的数据类型。</typeparam>
        /// <returns>返回一个对象。</returns>
        public TModel ToModel<TModel>()
        {
            if(!HasValue(this)) return default(TModel);
            return Serializer.Quickly.FastReadBytes<TModel>(this._ByteArray);
        }

        /// <summary>
        /// 将当前字节转换为对象实例。
        /// </summary>
        /// <returns>返回一个对象。</returns>
        public object ToModel()
        {
            if(!HasValue(this)) return null;
            return Serializer.Quickly.FastReadBytes(this._ByteArray);
        }

        /// <summary>
        /// 提供未知的数据类型，创建一个二进制值。
        /// </summary>
        /// <param name="value">一个未知类型的值。</param>
        /// <returns>返回一个二进制值。</returns>
        public static BinaryValue Create(object value)
        {
            if(value == null) return null;
            if(value is BinaryValue) return (BinaryValue)value;
            if(value is byte[]) return new BinaryValue((byte[])value);
            if(value is Decimal) return (Decimal)value;
            if(value is Guid) return (Guid)value;
            if(value is String) return (String)value;
            if(value is DateTime) return (DateTime)value;
            if(value is DateTimeOffset) return (DateTimeOffset)value;
            if(value is TimeSpan) return (TimeSpan)value;
            if(value is Boolean) return (Boolean)value;
            if(value is Char) return (Char)value;
            if(value is Double) return (Double)value;
            if(value is Int16) return (Int16)value;
            if(value is Int32) return (Int32)value;
            if(value is Int64) return (Int64)value;
            if(value is Single) return (Single)value;
            if(value is UInt16) return (UInt16)value;
            if(value is UInt32) return (UInt32)value;
            if(value is UInt64) return (UInt64)value;
            return new BinaryValue(value);
        }

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
            if(!HasValue(value)) return default(Decimal);
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
            if(!HasValue(value)) return default(Guid);
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
            if(!HasValue(value)) return null;
            return value.ToString();
        }

        /// <summary>
        /// <see cref="System.String"/> 和 <see cref="System.BinaryValue"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个 <see cref="System.String"/> 的新实例。</param>
        /// <returns>一个二进制的值。</returns>
        public static implicit operator BinaryValue(String value)
        {
            if(value == null) return null; 
            return new BinaryValue(GA.UTF8.GetBytes(value));
        }

        /// <summary>
        /// <see cref="System.BinaryValue"/> 和 <see cref="System.DateTime"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个二进制的值。</param>
        /// <returns>返回一个 <see cref="System.DateTime"/> 的新实例。</returns>
        public static implicit operator DateTime(BinaryValue value)
        {
            if(!HasValue(value)) return default(DateTime);
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
        /// <see cref="System.BinaryValue"/> 和 <see cref="System.DateTimeOffset"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个二进制的值。</param>
        /// <returns>返回一个 <see cref="System.DateTimeOffset"/> 的新实例。</returns>
        public static implicit operator DateTimeOffset(BinaryValue value)
        {
            if(!HasValue(value)) return default(DateTimeOffset);
            return new DateTimeOffset(DateTime.FromBinary(BitConverter.ToInt64(value._ByteArray, 0)));
        }
        /// <summary>
        /// <see cref="System.DateTimeOffset"/> 和 <see cref="System.BinaryValue"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个 <see cref="System.DateTimeOffset"/> 的新实例。</param>
        /// <returns>一个二进制的值。</returns>
        public static implicit operator BinaryValue(DateTimeOffset value)
        {
            return new BinaryValue(BitConverter.GetBytes(value.DateTime.ToBinary()));
        }

        /// <summary>
        /// <see cref="System.BinaryValue"/> 和 <see cref="System.TimeSpan"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个二进制的值。</param>
        /// <returns>返回一个 <see cref="System.TimeSpan"/> 的新实例。</returns>
        public static implicit operator TimeSpan(BinaryValue value)
        {
            if(!HasValue(value)) return default(TimeSpan);
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
            return GA.UTF8.GetString(this._ByteArray);
        }
    }
}
