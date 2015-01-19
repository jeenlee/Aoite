using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace System
{
    partial class BinaryValue
    {
        /// <summary>
        /// <see cref="System.BinaryValue"/> 和 <see cref="System.Boolean"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个二进制的值。</param>
        /// <returns>返回一个 <see cref="System.Boolean"/> 的新实例。</returns>
        public static implicit operator Boolean(BinaryValue value)
        {
            if(!HasValue(value)) return default(Boolean);
            return BitConverter.ToBoolean(value._ByteArray, 0);
        }

        /// <summary>
        /// <see cref="System.Boolean"/> 和 <see cref="System.BinaryValue"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个 <see cref="System.Boolean"/> 的新实例。</param>
        /// <returns>一个二进制的值。</returns>
        public static implicit operator BinaryValue(Boolean value)
        {
            return new BinaryValue(BitConverter.GetBytes(value));
        }

        /// <summary>
        /// <see cref="System.BinaryValue"/> 和 <see cref="System.Char"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个二进制的值。</param>
        /// <returns>返回一个 <see cref="System.Char"/> 的新实例。</returns>
        public static implicit operator Char(BinaryValue value)
        {
            if(!HasValue(value)) return default(Char);
            return BitConverter.ToChar(value._ByteArray, 0);
        }

        /// <summary>
        /// <see cref="System.Char"/> 和 <see cref="System.BinaryValue"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个 <see cref="System.Char"/> 的新实例。</param>
        /// <returns>一个二进制的值。</returns>
        public static implicit operator BinaryValue(Char value)
        {
            return new BinaryValue(BitConverter.GetBytes(value));
        }

        /// <summary>
        /// <see cref="System.BinaryValue"/> 和 <see cref="System.Double"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个二进制的值。</param>
        /// <returns>返回一个 <see cref="System.Double"/> 的新实例。</returns>
        public static implicit operator Double(BinaryValue value)
        {
            if(!HasValue(value)) return default(Double);
            return BitConverter.ToDouble(value._ByteArray, 0);
        }

        /// <summary>
        /// <see cref="System.Double"/> 和 <see cref="System.BinaryValue"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个 <see cref="System.Double"/> 的新实例。</param>
        /// <returns>一个二进制的值。</returns>
        public static implicit operator BinaryValue(Double value)
        {
            return new BinaryValue(BitConverter.GetBytes(value));
        }

        /// <summary>
        /// <see cref="System.BinaryValue"/> 和 <see cref="System.Int16"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个二进制的值。</param>
        /// <returns>返回一个 <see cref="System.Int16"/> 的新实例。</returns>
        public static implicit operator Int16(BinaryValue value)
        {
            if(!HasValue(value)) return default(Int16);
            return BitConverter.ToInt16(value._ByteArray, 0);
        }

        /// <summary>
        /// <see cref="System.Int16"/> 和 <see cref="System.BinaryValue"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个 <see cref="System.Int16"/> 的新实例。</param>
        /// <returns>一个二进制的值。</returns>
        public static implicit operator BinaryValue(Int16 value)
        {
            return new BinaryValue(BitConverter.GetBytes(value));
        }

        /// <summary>
        /// <see cref="System.BinaryValue"/> 和 <see cref="System.Int32"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个二进制的值。</param>
        /// <returns>返回一个 <see cref="System.Int32"/> 的新实例。</returns>
        public static implicit operator Int32(BinaryValue value)
        {
            if(!HasValue(value)) return default(Int32);
            return BitConverter.ToInt32(value._ByteArray, 0);
        }

        /// <summary>
        /// <see cref="System.Int32"/> 和 <see cref="System.BinaryValue"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个 <see cref="System.Int32"/> 的新实例。</param>
        /// <returns>一个二进制的值。</returns>
        public static implicit operator BinaryValue(Int32 value)
        {
            return new BinaryValue(BitConverter.GetBytes(value));
        }

        /// <summary>
        /// <see cref="System.BinaryValue"/> 和 <see cref="System.Int64"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个二进制的值。</param>
        /// <returns>返回一个 <see cref="System.Int64"/> 的新实例。</returns>
        public static implicit operator Int64(BinaryValue value)
        {
            if(!HasValue(value)) return default(Int64);
            return BitConverter.ToInt64(value._ByteArray, 0);
        }

        /// <summary>
        /// <see cref="System.Int64"/> 和 <see cref="System.BinaryValue"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个 <see cref="System.Int64"/> 的新实例。</param>
        /// <returns>一个二进制的值。</returns>
        public static implicit operator BinaryValue(Int64 value)
        {
            return new BinaryValue(BitConverter.GetBytes(value));
        }

        /// <summary>
        /// <see cref="System.BinaryValue"/> 和 <see cref="System.Single"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个二进制的值。</param>
        /// <returns>返回一个 <see cref="System.Single"/> 的新实例。</returns>
        public static implicit operator Single(BinaryValue value)
        {
            if(!HasValue(value)) return default(Single);
            return BitConverter.ToSingle(value._ByteArray, 0);
        }

        /// <summary>
        /// <see cref="System.Single"/> 和 <see cref="System.BinaryValue"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个 <see cref="System.Single"/> 的新实例。</param>
        /// <returns>一个二进制的值。</returns>
        public static implicit operator BinaryValue(Single value)
        {
            return new BinaryValue(BitConverter.GetBytes(value));
        }

        /// <summary>
        /// <see cref="System.BinaryValue"/> 和 <see cref="System.UInt16"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个二进制的值。</param>
        /// <returns>返回一个 <see cref="System.UInt16"/> 的新实例。</returns>
        public static implicit operator UInt16(BinaryValue value)
        {
            if(!HasValue(value)) return default(UInt16);
            return BitConverter.ToUInt16(value._ByteArray, 0);
        }

        /// <summary>
        /// <see cref="System.UInt16"/> 和 <see cref="System.BinaryValue"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个 <see cref="System.UInt16"/> 的新实例。</param>
        /// <returns>一个二进制的值。</returns>
        public static implicit operator BinaryValue(UInt16 value)
        {
            return new BinaryValue(BitConverter.GetBytes(value));
        }

        /// <summary>
        /// <see cref="System.BinaryValue"/> 和 <see cref="System.UInt32"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个二进制的值。</param>
        /// <returns>返回一个 <see cref="System.UInt32"/> 的新实例。</returns>
        public static implicit operator UInt32(BinaryValue value)
        {
            if(!HasValue(value)) return default(UInt32);
            return BitConverter.ToUInt32(value._ByteArray, 0);
        }

        /// <summary>
        /// <see cref="System.UInt32"/> 和 <see cref="System.BinaryValue"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个 <see cref="System.UInt32"/> 的新实例。</param>
        /// <returns>一个二进制的值。</returns>
        public static implicit operator BinaryValue(UInt32 value)
        {
            return new BinaryValue(BitConverter.GetBytes(value));
        }

        /// <summary>
        /// <see cref="System.BinaryValue"/> 和 <see cref="System.UInt64"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个二进制的值。</param>
        /// <returns>返回一个 <see cref="System.UInt64"/> 的新实例。</returns>
        public static implicit operator UInt64(BinaryValue value)
        {
            if(!HasValue(value)) return default(UInt64);
            return BitConverter.ToUInt64(value._ByteArray, 0);
        }

        /// <summary>
        /// <see cref="System.UInt64"/> 和 <see cref="System.BinaryValue"/> 的隐式转换。
        /// </summary>
        /// <param name="value">一个 <see cref="System.UInt64"/> 的新实例。</param>
        /// <returns>一个二进制的值。</returns>
        public static implicit operator BinaryValue(UInt64 value)
        {
            return new BinaryValue(BitConverter.GetBytes(value));
        }

    }
}