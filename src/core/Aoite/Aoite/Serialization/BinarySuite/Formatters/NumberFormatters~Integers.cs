using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Aoite.Serialization.BinarySuite
{
    internal static partial class NumberFormatters
    {
        #region - SByte & Byte -

        public static void WriteSByte(this ObjectWriter writer, SByte value)
        {
            writer.WriteTag(FormatterTag.SByte);
            writer.Stream.WriteByte((Byte)value);
        }
        public static SByte ReadSByte(this ObjectReader reader)
        {
            return (SByte)reader.Stream.ReadByte();
        }

        public static void WriteSByteArray(this ObjectWriter writer, SByte[] value)
        {
            writer.WriteTag(FormatterTag.SByteArray);
            writer.InnerWrite(value.Length);
            foreach(var item in value)
            {
                writer.Stream.WriteByte((Byte)item);
            }
        }
        public static SByte[] ReadSByteArray(this ObjectReader reader)
        {
            var length = reader.ReadInt32();
            SByte[] value = new SByte[length];
            for(int i = 0; i < length; i++) value[i] = (SByte)reader.Stream.ReadByte();
            return value;
        }

        public static void WriteByte(this ObjectWriter writer, Byte value)
        {
            writer.WriteTag(FormatterTag.Byte);
            writer.Stream.WriteByte(value);
        }
        public static Byte ReadByte(this ObjectReader reader)
        {
            return (Byte)reader.Stream.ReadByte();
        }

        public static void WriteByteArray(this ObjectWriter writer, Byte[] value)
        {
            writer.WriteTag(FormatterTag.ByteArray);
            writer.InnerWrite(value.Length);
            foreach(var item in value)
            {
                writer.Stream.WriteByte(item);
            }
        }
        public static Byte[] ReadByteArray(this ObjectReader reader)
        {
            var length = reader.ReadInt32();
            return reader.ReadBuffer(length);
        }

        #endregion

        #region - Int16 & UInt16 -

        public static void WriteInt16(this ObjectWriter writer, Int16 value)
        {
            writer.WriteTag(FormatterTag.Int16);
            writer.Stream.WriteBytes(BitConverter.GetBytes(value));
        }
        public static Int16 ReadInt16(this ObjectReader reader)
        {
            reader.Stream.Read(reader.DefaultBuffer, 0, TypeByteLength.Int16);
            return BitConverter.ToInt16(reader.DefaultBuffer, 0);
        }

        public static void WriteInt16Array(this ObjectWriter writer, Int16[] value)
        {
            writer.WriteTag(FormatterTag.Int16Array);
            writer.InnerWrite(value.Length);
            foreach(var item in value)
            {
                writer.Stream.WriteBytes(BitConverter.GetBytes(item));
            }
        }
        public static Int16[] ReadInt16Array(this ObjectReader reader)
        {
            var length = reader.ReadInt32();
            Int16[] value = new Int16[length];
            for(int i = 0; i < length; i++) value[i] = reader.ReadInt16();
            return value;
        }

        public static void WriteUInt16(this ObjectWriter writer, UInt16 value)
        {
            writer.WriteTag(FormatterTag.UInt16);
            writer.Stream.WriteBytes(BitConverter.GetBytes(value));
        }
        public static UInt16 ReadUInt16(this ObjectReader reader)
        {
            reader.Stream.Read(reader.DefaultBuffer, 0, TypeByteLength.UInt16);
            return BitConverter.ToUInt16(reader.DefaultBuffer, 0);
        }

        public static void WriteUInt16Array(this ObjectWriter writer, UInt16[] value)
        {
            writer.WriteTag(FormatterTag.UInt16Array);
            writer.InnerWrite(value.Length);
            foreach(var item in value)
            {
                writer.Stream.WriteBytes(BitConverter.GetBytes(item));
            }
        }
        public static UInt16[] ReadUInt16Array(this ObjectReader reader)
        {
            var length = reader.ReadInt32();
            UInt16[] value = new UInt16[length];
            for(int i = 0; i < length; i++) value[i] = reader.ReadUInt16();
            return value;
        }

        #endregion

        #region - Int32 & UInt32 -

        public static void WriteInt32(this ObjectWriter writer, Int32 value)
        {
            writer.WriteTag(FormatterTag.Int32);
            writer.InnerWrite(value);
        }
        public static Int32 ReadInt32(this ObjectReader reader)
        {
            reader.Stream.Read(reader.DefaultBuffer, 0, TypeByteLength.Int32);
            return BitConverter.ToInt32(reader.DefaultBuffer, 0);
        }

        public static void WriteInt32Array(this ObjectWriter writer, Int32[] value)
        {
            writer.WriteTag(FormatterTag.Int32Array);
            writer.InnerWrite(value.Length);
            foreach(var item in value)
            {
                writer.Stream.WriteBytes(BitConverter.GetBytes(item));
            }
        }
        public static Int32[] ReadInt32Array(this ObjectReader reader)
        {
            var length = reader.ReadInt32();
            Int32[] value = new Int32[length];
            for(int i = 0; i < length; i++) value[i] = reader.ReadInt32();
            return value;
        }

        public static void WriteUInt32(this ObjectWriter writer, UInt32 value)
        {
            writer.WriteTag(FormatterTag.UInt32);
            writer.Stream.WriteBytes(BitConverter.GetBytes(value));
        }
        public static UInt32 ReadUInt32(this ObjectReader reader)
        {
            reader.Stream.Read(reader.DefaultBuffer, 0, TypeByteLength.UInt32);
            return BitConverter.ToUInt32(reader.DefaultBuffer, 0);
        }

        public static void WriteUInt32Array(this ObjectWriter writer, UInt32[] value)
        {
            writer.WriteTag(FormatterTag.UInt32Array);
            writer.InnerWrite(value.Length);
            foreach(var item in value)
            {
                writer.Stream.WriteBytes(BitConverter.GetBytes(item));
            }
        }
        public static UInt32[] ReadUInt32Array(this ObjectReader reader)
        {
            var length = reader.ReadInt32();
            UInt32[] value = new UInt32[length];
            for(int i = 0; i < length; i++) value[i] = reader.ReadUInt32();
            return value;
        }

        #endregion

        #region - Int64 & UInt64 -

        public static void WriteInt64(this ObjectWriter writer, Int64 value)
        {
            writer.WriteTag(FormatterTag.Int64);
            writer.Stream.WriteBytes(BitConverter.GetBytes(value));
        }
        public static Int64 ReadInt64(this ObjectReader reader)
        {
            reader.Stream.Read(reader.DefaultBuffer, 0, TypeByteLength.Int64);
            return BitConverter.ToInt64(reader.DefaultBuffer, 0);
        }

        public static void WriteInt64Array(this ObjectWriter writer, Int64[] value)
        {
            writer.WriteTag(FormatterTag.Int64Array);
            writer.InnerWrite(value.Length);
            foreach(var item in value)
            {
                writer.Stream.WriteBytes(BitConverter.GetBytes(item));
            }
        }
        public static Int64[] ReadInt64Array(this ObjectReader reader)
        {
            var length = reader.ReadInt32();
            Int64[] value = new Int64[length];
            for(int i = 0; i < length; i++) value[i] = reader.ReadInt64();
            return value;
        }

        public static void WriteUInt64(this ObjectWriter writer, UInt64 value)
        {
            writer.WriteTag(FormatterTag.UInt64);
            writer.Stream.WriteBytes(BitConverter.GetBytes(value));
        }
        public static UInt64 ReadUInt64(this ObjectReader reader)
        {
            reader.Stream.Read(reader.DefaultBuffer, 0, TypeByteLength.UInt64);
            return BitConverter.ToUInt64(reader.DefaultBuffer, 0);
        }

        public static void WriteUInt64Array(this ObjectWriter writer, UInt64[] value)
        {
            writer.WriteTag(FormatterTag.UInt64Array);
            writer.InnerWrite(value.Length);
            foreach(var item in value)
            {
                writer.Stream.WriteBytes(BitConverter.GetBytes(item));
            }
        }
        public static UInt64[] ReadUInt64Array(this ObjectReader reader)
        {
            var length = reader.ReadInt32();
            UInt64[] value = new UInt64[length];
            for(int i = 0; i < length; i++) value[i] = reader.ReadUInt64();
            return value;
        }

        #endregion
    }
}
