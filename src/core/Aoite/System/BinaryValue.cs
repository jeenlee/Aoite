using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// 表示一个二进制的值。
    /// </summary>
    public class BinaryValue
    {
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
            if(this._ByteArray == null) return default(TModel);
            return Serializer.Quickly.ReadBytes<TModel>(this._ByteArray).UnsafeValue;
        }
        /// <summary>
        /// 将当前字节转换为对象实例。
        /// </summary>
        /// <returns>返回一个对象。</returns>
        public object ToModel()
        {
            if(this._ByteArray == null) return null;
            return Serializer.Quickly.ReadBytes(this._ByteArray).UnsafeValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator byte[](BinaryValue value)
        {
            if(value) return null;
            return value._ByteArray;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator BinaryValue(byte[] value)
        {
            return new BinaryValue(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator Boolean(BinaryValue value)
        {
            if(value == null) return default(Boolean);
            return BitConverter.ToBoolean(value._ByteArray, 0);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator BinaryValue(Boolean value)
        {
            return new BinaryValue(BitConverter.GetBytes(value));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator Int16(BinaryValue value)
        {
            if(value == null) return default(Int16);
            return BitConverter.ToInt16(value._ByteArray, 0);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator BinaryValue(Int16 value)
        {
            return new BinaryValue(BitConverter.GetBytes(value));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator Int32(BinaryValue value)
        {
            if(value == null) return default(Int32);
            return BitConverter.ToInt32(value._ByteArray, 0);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator BinaryValue(Int32 value)
        {
            return new BinaryValue(BitConverter.GetBytes(value));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator Int64(BinaryValue value)
        {
            if(value == null) return default(Int64);
            return BitConverter.ToInt64(value._ByteArray, 0);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator BinaryValue(Int64 value)
        {
            return new BinaryValue(BitConverter.GetBytes(value));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator Double(BinaryValue value)
        {
            if(value == null) return default(Double);
            return BitConverter.ToDouble(value._ByteArray, 0);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator BinaryValue(Double value)
        {
            return new BinaryValue(BitConverter.GetBytes(value));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator Char(BinaryValue value)
        {
            if(value == null) return default(Char);
            return BitConverter.ToChar(value._ByteArray, 0);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator BinaryValue(Char value)
        {
            return new BinaryValue(BitConverter.GetBytes(value));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator Decimal(BinaryValue value)
        {
            if(value == null) return default(Decimal);
            var bits = new int[]
                {
                    BitConverter.ToInt32(value._ByteArray, 0),
                    BitConverter.ToInt32(value._ByteArray, 4),
                    BitConverter.ToInt32(value._ByteArray, 8),
                    BitConverter.ToInt32(value._ByteArray, 12)
                };
            return new decimal(bits);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator BinaryValue(Decimal value)
        {
            var bits = Decimal.GetBits(value);
            byte[] bytesValue = new byte[16];
            Buffer.BlockCopy(bytesValue, 0, BitConverter.GetBytes(bits[0]), 0, 4);
            Buffer.BlockCopy(bytesValue, 4, BitConverter.GetBytes(bits[1]), 0, 4);
            Buffer.BlockCopy(bytesValue, 8, BitConverter.GetBytes(bits[2]), 0, 4);
            Buffer.BlockCopy(bytesValue, 12, BitConverter.GetBytes(bits[3]), 0, 4);
            return new BinaryValue(bytesValue);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator Guid(BinaryValue value)
        {
            if(value == null) return default(Guid);
            return new Guid(value._ByteArray);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator BinaryValue(Guid value)
        {
            return new BinaryValue(value.ToByteArray());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator String(BinaryValue value)
        {
            if(value == null) return null;
            return value.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator BinaryValue(String value)
        {
            return new BinaryValue(Encoding.UTF8.GetBytes(value));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator DateTime(BinaryValue value)
        {
            if(value == null) return default(DateTime);
            return DateTime.FromBinary(BitConverter.ToInt64(value._ByteArray, 0));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator BinaryValue(DateTime value)
        {
            return new BinaryValue(BitConverter.GetBytes(value.ToBinary()));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator TimeSpan(BinaryValue value)
        {
            if(value == null) return default(TimeSpan);
            return new TimeSpan(BitConverter.ToInt64(value._ByteArray, 0));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
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
