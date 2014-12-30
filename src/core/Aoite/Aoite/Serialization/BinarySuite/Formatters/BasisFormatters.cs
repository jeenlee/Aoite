using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Serialization.BinarySuite
{
    internal static class BasisFormatters
    {
        private readonly static Dictionary<string, Type> TypeCache = new Dictionary<string, Type>();

        #region - FormatterTag -

        public static void WriteTag(this ObjectWriter writer, FormatterTag value)
        {
            writer.Stream.WriteByte((byte)value);
        }

        public static FormatterTag ReadTag(this ObjectReader reader)
        {
            return (FormatterTag)reader.Stream.ReadByte();
        }

        #endregion

        #region - Null & Type -

        public static void WriteNull(this ObjectWriter writer)
        {
            writer.WriteTag(FormatterTag.Null);
        }

        public static void WriteDBNull(this ObjectWriter writer)
        {
            writer.WriteTag(FormatterTag.DBNull);
        }
        public static void WriteType(this ObjectWriter writer, Type value)
        {
            writer.WriteTag(FormatterTag.Type);
            writer.InnerWrite(value);
        }
        public static Type ReadType(this ObjectReader reader)
        {
            var simplifyQualifiedName = reader.ReadStringOrReference();
            Type type;
            if(!TypeCache.TryGetValue(simplifyQualifiedName, out type))
                lock(TypeCache)
                    if(!TypeCache.TryGetValue(simplifyQualifiedName, out type))
                    {
                        type = SerializationHelper.RecoveryQualifiedName(simplifyQualifiedName);
                        TypeCache.Add(simplifyQualifiedName, type);
                    }
            return type;
        }

        public static void WriteTypeArray(this ObjectWriter writer, Type[] value)
        {
            writer.WriteTag(FormatterTag.TypeArray);
            writer.InnerWrite(value.Length);
            foreach(var v in value)
            {
                writer.InnerWrite(v);
            }
        }
        public static Type[] ReadTypeArray(this ObjectReader reader)
        {
            var length = reader.ReadInt32();
            Type[] value = new Type[length];
            for(int i = 0; i < length; i++) value[i] = reader.ReadType();
            return value;
        }

        #endregion

        #region - Guid & Boolean -

        public static void WriteGuid(this ObjectWriter writer, Guid value)
        {
            writer.InnerWrite(FormatterTag.Guid, value.ToByteArray());
        }
        public static Guid ReadGuid(this ObjectReader reader)
        {
            reader.Stream.Read(reader.DefaultBuffer, 0, TypeByteLength.Guid);
            return new Guid(reader.DefaultBuffer);
        }

        public static void WriteGuidArray(this ObjectWriter writer, Guid[] value)
        {
            writer.WriteTag(FormatterTag.GuidArray);
            writer.InnerWrite(value.Length);
            foreach(var v in value)
            {
                writer.Stream.WriteBytes(v.ToByteArray());
            }
        }
        public static Guid[] ReadGuidArray(this ObjectReader reader)
        {
            var length = reader.ReadInt32();
            Guid[] value = new Guid[length];
            for(int i = 0; i < length; i++) value[i] = reader.ReadGuid();
            return value;
        }

        public static void WriteBoolean(this ObjectWriter writer, Boolean value)
        {
            writer.WriteTag(FormatterTag.Boolean);
            writer.Stream.WriteByte((value ? (byte)1 : (byte)0));
        }
        public static Boolean ReadBoolean(this ObjectReader reader)
        {
            return reader.Stream.ReadByte() == 1;
        }

        public static void WriteBooleanArray(this ObjectWriter writer, Boolean[] value)
        {
            writer.WriteTag(FormatterTag.BooleanArray);
            writer.InnerWrite(value.Length);
            foreach(var v in value)
            {
                writer.Stream.WriteByte((v ? (byte)1 : (byte)0));
            }
        }
        public static Boolean[] ReadBooleanArray(this ObjectReader reader)
        {
            var length = reader.ReadInt32();
            Boolean[] value = new Boolean[length];
            for(int i = 0; i < length; i++) value[i] = reader.ReadBoolean();
            return value;
        }

        #endregion

        #region - DateTime & TimeSpan -

        public static void WriteDateTime(this ObjectWriter writer, DateTime value)
        {
            writer.InnerWrite(FormatterTag.DateTime, BitConverter.GetBytes(value.ToBinary()));
        }
        public static DateTime ReadDateTime(this ObjectReader reader)
        {
            reader.Stream.Read(reader.DefaultBuffer, 0, TypeByteLength.Int64);
            return DateTime.FromBinary(BitConverter.ToInt64(reader.DefaultBuffer, 0));
        }

        public static void WriteDateTimeArray(this ObjectWriter writer, DateTime[] value)
        {
            writer.WriteTag(FormatterTag.DateTimeArray);
            writer.InnerWrite(value.Length);

            foreach(var v in value)
            {
                writer.Stream.WriteBytes(BitConverter.GetBytes(v.ToBinary()));
            }
        }

        public static DateTime[] ReadDateTimeArray(this ObjectReader reader)
        {
            var length = reader.ReadInt32();
            DateTime[] value = new DateTime[length];
            for(int i = 0; i < length; i++) value[i] = reader.ReadDateTime();
            return value;
        }

        public static void WriteTimeSpan(this ObjectWriter writer, TimeSpan value)
        {
            writer.InnerWrite(FormatterTag.TimeSpan, BitConverter.GetBytes(value.Ticks));
        }
        public static TimeSpan ReadTimeSpan(this ObjectReader reader)
        {
            reader.Stream.Read(reader.DefaultBuffer, 0, TypeByteLength.Int64);
            return new TimeSpan(BitConverter.ToInt64(reader.DefaultBuffer, 0));
        }

        public static void WriteTimeSpanArray(this ObjectWriter writer, TimeSpan[] value)
        {
            writer.WriteTag(FormatterTag.TimeSpanArray);
            writer.InnerWrite(value.Length);

            foreach(var v in value)
            {
                writer.Stream.WriteBytes(BitConverter.GetBytes(v.Ticks));
            }
        }
        public static TimeSpan[] ReadTimeSpanArray(this ObjectReader reader)
        {
            var length = reader.ReadInt32();
            TimeSpan[] value = new TimeSpan[length];
            for(int i = 0; i < length; i++) value[i] = reader.ReadTimeSpan();
            return value;
        }

        #endregion
    }
}
