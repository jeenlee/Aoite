using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Aoite.Serialization.BinarySuite
{
    internal static class ObjectFormatters
    {
        private readonly static Aoite.Reflection.MemberGetter GetCaseInsensitive;
        static ObjectFormatters()
        {
            var field = Types.HybridDictionary.GetField("caseInsensitive", Aoite.Reflection.Flags.InstancePrivateDeclaredOnly);
            GetCaseInsensitive = Aoite.Reflection.FieldInfoExtensions.DelegateForGetFieldValue(field);
        }

        #region - Result -

        public static void WriteSuccessfullyResult(this ObjectWriter writer)
        {
            writer.Stream.WriteByte((byte)FormatterTag.SuccessfullyResult);
        }

        public static void WriteResult(this ObjectWriter writer, Result value)
        {
            writer.WriteTag(FormatterTag.Result);
            writer.InnerWrite(value._Status);
            writer.WriteStringOrReference(value._Message);
        }
        public static Result ReadResult(this ObjectReader reader, int index)
        {
            var status = reader.ReadInt32();
            var exceptionMessage = reader.ReadStringOrReference();

            var value = new Result();
            reader.ReferenceContainer[index] = value;
            value._Status = status;
            value._Message = exceptionMessage;
            return value;
        }

        public static void WriteGResult(this ObjectWriter writer, Result value, Type type)
        {
            writer.WriteTag(FormatterTag.GResult);
            writer.InnerWrite(value._Status);
            writer.WriteStringOrReference(value._Message);
            writer.InnerWrite(type.GetGenericArguments()[0]);
            writer.Serialize(value.GetValue());
        }
        public static Result ReadGResult(this ObjectReader reader, int index)
        {
            var status = reader.ReadInt32();
            var exceptionMessage = reader.ReadStringOrReference();
            var elementType = reader.ReadType();

            var value = Activator.CreateInstance(Types.GResult.MakeGenericType(elementType)) as Result;
            reader.ReferenceContainer[index] = value;
            value._Status = status;
            value._Message = exceptionMessage;
            var resultValue = reader.Deserialize();
            value.SetValue(resultValue);

            return value;
        }

        #endregion

        #region - Array -

        private static void WriteSimpleArray(this ObjectWriter writer, Array value)
        {
            writer.WriteTag(FormatterTag.Array);

            var elementType = value.GetType().GetElementType();
            writer.InnerWrite(elementType);
            writer.InnerWrite(value);
        }
        public static Array ReadSimpleArray(this ObjectReader reader, int index)
        {
            var elementType = reader.ReadType();
            var length = reader.ReadInt32();
            var value = Array.CreateInstance(elementType, length);
            reader.ReferenceContainer[index] = value;
            for(int i = 0; i < length; i++) value.SetValue(reader.Deserialize(), i);
            return value;
        }

        private static void WriteMultiRankArray(this ObjectWriter writer, Array value)
        {
            writer.WriteTag(FormatterTag.MultiRankArray);

            var elementType = value.GetType().GetElementType();
            writer.InnerWrite(elementType);

            int rank = value.Rank;
            //- 写入维数
            writer.InnerWrite(rank);

            int i, j;
            int[,] des = new int[rank, 2];
            int[] loc = new int[rank];
            //- 写入维数长度
            for(i = 0; i < rank; i++)
            {
                writer.InnerWrite(value.GetLength(i));
            }

            //- 写入元素
            //- 设置每一个 数组维 的上下标。
            for(i = 0; i < rank; i++)
            {
                j = value.GetLowerBound(i);//- 上标
                des[i, 0] = j;
                des[i, 1] = value.GetUpperBound(i);  //- 下标
                loc[i] = j;
            }
            i = rank - 1;
            while(loc[0] <= des[0, 1])
            {
                writer.Serialize(value.GetValue(loc));
                loc[i]++;
                for(j = rank - 1; j > 0; j--)
                {
                    if(loc[j] > des[j, 1])
                    {
                        loc[j] = des[j, 0];
                        loc[j - 1]++;
                    }
                }
            }
        }

        public static Array ReadMultiRankArray(this ObjectReader reader, int index)
        {
            var elementType = reader.ReadType();
            var rank = reader.ReadInt32();
            Array value;

            int i, j;
            var des = new int[rank, 2];
            var loc = new int[rank];

            int[] rankLengths = new int[rank];

            for(i = 0; i < rank; i++) rankLengths[i] = reader.ReadInt32();

            value = Array.CreateInstance(elementType, rankLengths);
            reader.ReferenceContainer[index] = value;


            // 设置每一个 数组维 的上下标。
            for(i = 0; i < rank; i++)
            {
                j = value.GetLowerBound(i);//- 上标
                des[i, 0] = j;
                des[i, 1] = value.GetUpperBound(i);  //- 下标
                loc[i] = j;
            }
            i = rank - 1;
            while(loc[0] <= des[0, 1])
            {
                value.SetValue(reader.Deserialize(), loc);
                loc[i]++;
                for(j = rank - 1; j > 0; j--)
                {
                    if(loc[j] > des[j, 1])
                    {
                        loc[j] = des[j, 0];
                        loc[j - 1]++;
                    }
                }
            }
            return value;
        }

        public static void WriteArray(this ObjectWriter writer, Array value)
        {
            if(value.Rank == 1) writer.WriteSimpleArray(value);
            else writer.WriteMultiRankArray(value);
        }

        #endregion

        #region - GList & GDictionary -

        public static void WriteGList(this ObjectWriter writer, IList value, Type type)
        {
            writer.WriteTag(FormatterTag.GList);

            writer.InnerWrite(type.GetGenericArguments()[0]);
            writer.InnerWrite(value.Count);

            foreach(var item in value) writer.Serialize(item);
        }
        public static IList ReadGList(this ObjectReader reader, int index)
        {
            var elementType = reader.ReadType();
            var count = reader.ReadInt32();

            var value = Activator.CreateInstance(Types.GList.MakeGenericType(elementType), count) as IList;
            reader.ReferenceContainer[index] = value;
            for(int i = 0; i < count; i++) value.Add(reader.Deserialize());

            return value;
        }

        private static void WriteGDictionary(ObjectWriter writer, FormatterTag tag, IDictionary value, Type type)
        {
            writer.WriteTag(tag);
            var genericArguments = type.GetGenericArguments();
            writer.InnerWrite(genericArguments[0]);
            writer.InnerWrite(genericArguments[1]);
            writer.InnerWrite(value.Count);

            foreach(DictionaryEntry item in value)
            {
                writer.Serialize(item.Key);
                writer.Serialize(item.Value);
            }
        }
        public static void WriteGDictionary(this ObjectWriter writer, IDictionary value, Type type)
        {
            WriteGDictionary(writer, FormatterTag.GDictionary, value, type);
        }
        public static void WriteGConcurrentDictionary(this ObjectWriter writer, IDictionary value, Type type)
        {
            WriteGDictionary(writer, FormatterTag.GConcurrentDictionary, value, type);
        }

        private static int ProcessorCount = Environment.ProcessorCount;
        public static IDictionary ReadGConcurrentDictionary(this ObjectReader reader, int index)
        {
            Type keyType = reader.ReadType()
                , valueType = reader.ReadType();
            var count = reader.ReadInt32();

            var value = Activator.CreateInstance(Types.GConcurrentDictionary.MakeGenericType(keyType, valueType), ProcessorCount * 4, count) as IDictionary;
            reader.ReferenceContainer[index] = value;
            for(int i = 0; i < count; i++) value.Add(reader.Deserialize(), reader.Deserialize());

            return value;
        }
        public static IDictionary ReadGDictionary(this ObjectReader reader, int index)
        {
            Type keyType = reader.ReadType()
                , valueType = reader.ReadType();
            var count = reader.ReadInt32();

            var value = Activator.CreateInstance(Types.GDictionary.MakeGenericType(keyType, valueType), count) as IDictionary;
            reader.ReferenceContainer[index] = value;
            for(int i = 0; i < count; i++) value.Add(reader.Deserialize(), reader.Deserialize());

            return value;
        }

        public static void WriteHybridDictionary(this ObjectWriter writer, HybridDictionary value)
        {
            writer.WriteTag(FormatterTag.HybridDictionary);
            writer.InnerWrite(value.Count);
            var caseInsensitive = (bool)GetCaseInsensitive(value);
            writer.Stream.WriteByte((caseInsensitive ? (byte)1 : (byte)0));

            foreach(DictionaryEntry item in value)
            {
                writer.Serialize(item.Key);
                writer.Serialize(item.Value);
            }
        }

        public static HybridDictionary ReadHybridDictionary(this ObjectReader reader, int index)
        {
            var count = reader.ReadInt32();
            var caseInsensitive = reader.ReadBoolean();
            HybridDictionary value = new HybridDictionary(count, caseInsensitive);
            reader.ReferenceContainer[index] = value;
            for(int i = 0; i < count; i++) value.Add(reader.Deserialize(), reader.Deserialize());
            return value;
        }

        #endregion

        #region - Object & ObjectArray -

        public static void WriteObject(this ObjectWriter writer, object value, Type type)
        {
            writer.InnerWriteObject(value
                , type
                , FormatterTag.Object);
        }
        public static object ReadObject(this ObjectReader reader, int index)
        {
            var type = reader.ReadType();
            var value = FormatterServices.GetUninitializedObject(type);
            reader.ReferenceContainer[index] = value;

            var fields = SerializationHelper.GetSerializableMembers(type);
            foreach(var field in fields)
            {
                var fieldValue = reader.Deserialize();
                var fieldInfo = field.Field;
                if(fieldValue != null && fieldInfo.FieldType.IsNullable())
                {
                    var enumType = fieldInfo.FieldType.GetGenericArguments()[0];
                    if(enumType.IsEnum) fieldValue = Enum.ToObject(enumType, fieldValue);
                }
                field.SetValue(value, fieldValue);
            }
            return value;
        }

        public static void WriteObjectArray(this ObjectWriter writer, object[] value)
        {
            writer.WriteTag(FormatterTag.ObjectArray);
            writer.InnerWrite(value);
        }
        public static object[] ReadObjectArray(this ObjectReader reader, int index)
        {
            var length = reader.ReadInt32();
            object[] value = new object[length];
            for(int i = 0; i < length; i++) value[i] = reader.Deserialize();
            return value;
        }

        public static void WriteValueTypeObject(this ObjectWriter writer, object value)
        {
            writer.InnerWriteObject(new Aoite.Reflection.Emitter.ValueTypeHolder(value)
                , value.GetType()
                , FormatterTag.ValueTypeObject);
        }
        public static object ReadValueTypeObject(this ObjectReader reader)
        {
            var type = reader.ReadType();
            var value = FormatterServices.GetUninitializedObject(type);
            Aoite.Reflection.Emitter.ValueTypeHolder holder = new Aoite.Reflection.Emitter.ValueTypeHolder(value); //- 值类型

            var fields = SerializationHelper.GetSerializableMembers(type);
            foreach(var field in fields)
            {
                var fieldValue = reader.Deserialize();
                field.SetValue(holder, fieldValue);
            }
            return holder.Value;
        }

        #endregion
    }
}
