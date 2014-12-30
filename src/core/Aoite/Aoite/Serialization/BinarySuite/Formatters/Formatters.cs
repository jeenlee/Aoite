using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;

namespace Aoite.Serialization.BinarySuite
{
    internal static class Formatters
    {
        #region Serialize

        public static void Serialize(this ObjectWriter writer, object value)
        {
            if(value is Aoite.Data.IDbResult)
                throw new NotSupportedException("请不要将数据源执行的结果（如：Aoite.Data.DbResult）直接序列化，因为其包含数据源连接的相关信息。");

            #region Null & DbNull & SuccessfullyResult

            if(value == null)
            {
                writer.WriteNull();
                return;
            }
            if(value is DBNull)
            {
                writer.WriteDBNull();
                return;
            }
            if(value is SuccessfullyResult)
            {
                writer.WriteSuccessfullyResult();
                return;
            }

            #endregion

            #region ValueType

            if(value is ValueType)
            {
                if(value is Enum) value = Convert.ChangeType(value, Enum.GetUnderlyingType(value.GetType()), null);

                if(value is Guid) writer.WriteGuid((Guid)value);
                else if(value is DateTime) writer.WriteDateTime((DateTime)value);
                else if(value is TimeSpan) writer.WriteTimeSpan((TimeSpan)value);
                else if(value is Boolean) writer.WriteBoolean((Boolean)value);
                else if(value is Byte) writer.WriteByte((Byte)value);
                else if(value is SByte) writer.WriteSByte((SByte)value);
                else if(value is Char) writer.WriteChar((Char)value);
                else if(value is Single) writer.WriteSingle((Single)value);
                else if(value is Double) writer.WriteDouble((Double)value);
                else if(value is Decimal) writer.WriteDecimal((Decimal)value);
                else if(value is Int16) writer.WriteInt16((Int16)value);
                else if(value is Int32) writer.WriteInt32((Int32)value);
                else if(value is Int64) writer.WriteInt64((Int64)value);
                else if(value is UInt16) writer.WriteUInt16((UInt16)value);
                else if(value is UInt32) writer.WriteUInt32((UInt32)value);
                else if(value is UInt64) writer.WriteUInt64((UInt64)value);
                else writer.WriteValueTypeObject(value);
                return;
            }

            #endregion

            if(writer.TryWriteReference(value)) return;

            #region Simple

            if(value is String)
            {
                writer.WriteString((String)value);
                return;
            }

            if(value is StringBuilder)
            {
                writer.WriteStringBuilder((StringBuilder)value);
                return;
            }
            if(value is Type)
            {
                writer.WriteType((Type)value);
                return;
            }

            #endregion

            #region Array

            if(value is Array)
            {
                if(value is String[]) writer.WriteStringArray((String[])value);
                else if(value is Guid[]) writer.WriteGuidArray((Guid[])value);
                else if(value is DateTime[]) writer.WriteDateTimeArray((DateTime[])value);
                else if(value is TimeSpan[]) writer.WriteTimeSpanArray((TimeSpan[])value);
                else if(value is Boolean[]) writer.WriteBooleanArray((Boolean[])value);
                else if(value is Byte[]) writer.WriteByteArray((Byte[])value);
                else if(value is SByte[]) writer.WriteSByteArray((SByte[])value);
                else if(value is Char[]) writer.WriteCharArray((Char[])value);
                else if(value is Single[]) writer.WriteSingleArray((Single[])value);
                else if(value is Double[]) writer.WriteDoubleArray((Double[])value);
                else if(value is Decimal[]) writer.WriteDecimalArray((Decimal[])value);
                else if(value is Int16[]) writer.WriteInt16Array((Int16[])value);
                else if(value is Int32[]) writer.WriteInt32Array((Int32[])value);
                else if(value is Int64[]) writer.WriteInt64Array((Int64[])value);
                else if(value is UInt16[]) writer.WriteUInt16Array((UInt16[])value);
                else if(value is UInt32[]) writer.WriteUInt32Array((UInt32[])value);
                else if(value is UInt64[]) writer.WriteUInt64Array((UInt64[])value);
                else if(value is StringBuilder[]) writer.WriteStringBuilderArray((StringBuilder[])value);
                else if(value is Type[]) writer.WriteTypeArray((Type[])value);
                else if(value.GetType() == Types.ObjectArray) writer.WriteObjectArray((Object[])value);
                else
                {
                    writer.WriteArray(value as Array);
                }
                return;
            }

            #endregion

            var type = value.GetType();

            #region Result &　Generic

            if(type == Types.Result)
            {
                writer.WriteResult((Result)value);
                return;
            }
            if(type == Types.HybridDictionary)
            {
                writer.WriteHybridDictionary((System.Collections.Specialized.HybridDictionary)value);
                return;
            }
            if(type.IsGenericType)
            {
                var defineType = type.GetGenericTypeDefinition();
                if(Types.GResult == defineType)
                {
                    writer.WriteGResult((Result)value, type);
                    return;
                }
                if(Types.GList == defineType)
                {
                    writer.WriteGList((IList)value, type);
                    return;
                }
                if(Types.GConcurrentDictionary == defineType)
                {
                    writer.WriteGConcurrentDictionary((IDictionary)value, type);
                    return;
                }
                if(Types.GDictionary == defineType)
                {
                    writer.WriteGDictionary((IDictionary)value, type);
                    return;
                }
            }

            #endregion

            writer.WriteObject(value, type);
        }

        public static void InnerWrite(this ObjectWriter writer, FormatterTag tag, byte[] bytes)
        {
            writer.WriteTag(tag);
            writer.Stream.WriteBytes(bytes);
        }

        public static void InnerWrite(this ObjectWriter writer, Decimal value)
        {
            var bits = Decimal.GetBits(value);
            writer.Stream.WriteBytes(BitConverter.GetBytes(bits[0]));
            writer.Stream.WriteBytes(BitConverter.GetBytes(bits[1]));
            writer.Stream.WriteBytes(BitConverter.GetBytes(bits[2]));
            writer.Stream.WriteBytes(BitConverter.GetBytes(bits[3]));
        }

        public static void InnerWrite(this ObjectWriter writer, Int32 value)
        {
            writer.Stream.WriteBytes(BitConverter.GetBytes(value));
        }

        public static void InnerWrite(this ObjectWriter writer, Array array)
        {
            writer.InnerWrite(array.Length);
            foreach(var item in array) writer.Serialize(item);
        }

        public static void InnerWrite(this ObjectWriter writer, Type type)
        {
            writer.WriteStringOrReference(SerializationHelper.SimplifyQualifiedName(type));
        }

        public static void InnerWriteObject(this ObjectWriter writer, object value, Type type, FormatterTag tagType)
        {
            writer.WriteTag(tagType);
            writer.InnerWrite(type);
            SerializableFieldInfo[] fields = SerializationHelper.GetSerializableMembers(type);

            foreach(var field in fields)
            {
                writer.Serialize(field.GetValue(value));
            }
        }

        public static void WriteStringOrReference(this ObjectWriter writer, String value)
        {
            if(value == null) writer.WriteNull();
            else if(!writer.TryWriteReference(value)) writer.WriteString(value);
        }
        public static string ReadStringOrReference(this ObjectReader reader)
        {
            return reader.Deserialize() as string;
        }

        public static bool TryWriteReference(this ObjectWriter writer, string value)
        {
            if(value == null) return false;
            value = string.Intern(value);
            var index = writer.ReferenceContainer.IndexOf(value);
            if(index < 0)
            {
                writer.ReferenceContainer.Add(value);
                return false;
            }
            writer.WriteTag(FormatterTag.Reference);
            writer.InnerWrite(index);
            return true;
        }
        public static bool TryWriteReference(this ObjectWriter writer, object value)
        {
            if(value == null) return false;
            var index = writer.ReferenceContainer.IndexOf(value);
            if(index < 0)
            {
                writer.ReferenceContainer.Add(value);
                return false;
            }
            writer.WriteTag(FormatterTag.Reference);
            writer.InnerWrite(index);
            return true;
        }

        #endregion

        #region Deserialize

        public static object ReadReference(this ObjectReader reader)
        {
            var index = reader.ReadInt32();
            return reader.ReferenceContainer[index];
        }

        public static object Deserialize(this ObjectReader reader)
        {
            var tag = (FormatterTag)reader.Stream.ReadByte();
            switch(tag)
            {
                case FormatterTag.Reference: return reader.ReadReference();
                case FormatterTag.Null: return null;
                case FormatterTag.SuccessfullyResult: return Result.Successfully;
                case FormatterTag.DBNull: return DBNull.Value;
                case FormatterTag.Guid: return reader.ReadGuid();
                case FormatterTag.DateTime: return reader.ReadDateTime();
                case FormatterTag.TimeSpan: return reader.ReadTimeSpan();
                case FormatterTag.Boolean: return reader.ReadBoolean();
                case FormatterTag.Byte: return reader.ReadByte();
                case FormatterTag.SByte: return reader.ReadSByte();
                case FormatterTag.Char: return reader.ReadChar();
                case FormatterTag.Int16: return reader.ReadInt16();
                case FormatterTag.Int32: return reader.ReadInt32();
                case FormatterTag.Int64: return reader.ReadInt64();
                case FormatterTag.UInt16: return reader.ReadUInt16();
                case FormatterTag.UInt32: return reader.ReadUInt32();
                case FormatterTag.UInt64: return reader.ReadUInt64();
                case FormatterTag.Single: return reader.ReadSingle();
                case FormatterTag.Double: return reader.ReadDouble();
                case FormatterTag.Decimal: return reader.ReadDecimal();
                case FormatterTag.ValueTypeObject: return reader.ReadValueTypeObject();
            }

            var index = reader.ReferenceContainer.Count;
            reader.ReferenceContainer.Add(null);
            object value;
            switch(tag)
            {
                case FormatterTag.Result: return reader.ReadResult(index);
                case FormatterTag.GResult: return reader.ReadGResult(index);
                case FormatterTag.Array: return reader.ReadSimpleArray(index);
                case FormatterTag.MultiRankArray: return reader.ReadMultiRankArray(index);
                case FormatterTag.GList: return reader.ReadGList(index);
                case FormatterTag.GDictionary: return reader.ReadGDictionary(index);
                case FormatterTag.GConcurrentDictionary: return reader.ReadGConcurrentDictionary(index);
                case FormatterTag.HybridDictionary: return reader.ReadHybridDictionary(index);

                case FormatterTag.Object: return reader.ReadObject(index);
                case FormatterTag.ObjectArray: return reader.ReadObjectArray(index);

                case FormatterTag.Type:
                    value = reader.ReadType();
                    break;
                case FormatterTag.TypeArray:
                    value = reader.ReadTypeArray();
                    break;
                case FormatterTag.GuidArray:
                    value = reader.ReadGuidArray();
                    break;
                case FormatterTag.DateTimeArray:
                    value = reader.ReadDateTimeArray();
                    break;
                case FormatterTag.TimeSpanArray:
                    value = reader.ReadTimeSpanArray();
                    break;
                case FormatterTag.BooleanArray:
                    value = reader.ReadBooleanArray();
                    break;
                case FormatterTag.ByteArray:
                    value = reader.ReadByteArray();
                    break;
                case FormatterTag.SByteArray:
                    value = reader.ReadSByteArray();
                    break;
                case FormatterTag.CharArray:
                    value = reader.ReadCharArray();
                    break;
                case FormatterTag.Int16Array:
                    value = reader.ReadInt16Array();
                    break;
                case FormatterTag.Int32Array:
                    value = reader.ReadInt32Array();
                    break;
                case FormatterTag.Int64Array:
                    value = reader.ReadInt64Array();
                    break;
                case FormatterTag.UInt16Array:
                    value = reader.ReadUInt16Array();
                    break;
                case FormatterTag.UInt32Array:
                    value = reader.ReadUInt32Array();
                    break;
                case FormatterTag.UInt64Array:
                    value = reader.ReadUInt64Array();
                    break;
                case FormatterTag.SingleArray:
                    value = reader.ReadSingleArray();
                    break;
                case FormatterTag.DoubleArray:
                    value = reader.ReadDoubleArray();
                    break;
                case FormatterTag.DecimalArray:
                    value = reader.ReadDecimalArray();
                    break;

                case FormatterTag.String:
                    value = reader.ReadString();
                    break;
                case FormatterTag.StringArray:
                    value = reader.ReadStringArray();
                    break;
                case FormatterTag.StringBuilder:
                    value = reader.ReadStringBuilder();
                    break;
                case FormatterTag.StringBuilderArray:
                    value = reader.ReadStringBuilderArray();
                    break;

                default:
                    throw new ArgumentException(tag + "：无法识别的标识。");
            }
            reader.ReferenceContainer[index] = value;
            return value;
        }

        public static byte[] ReadBuffer(this ObjectReader reader, int length)
        {
            byte[] buffer = new byte[length];
            reader.Stream.Read(buffer, 0, length);
            return buffer;
        }

        #endregion
    }
}
