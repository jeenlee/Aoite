using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Serialization.BinarySuite
{
    internal static class StringFormatters
    {
        #region - Char -

        public static void WriteChar(this ObjectWriter writer, Char value)
        {
            writer.InnerWrite(FormatterTag.Char, BitConverter.GetBytes(value));
        }
        public static Char ReadChar(this ObjectReader reader)
        {
            reader.Stream.Read(reader.DefaultBuffer, 0, TypeByteLength.Char);
            return BitConverter.ToChar(reader.DefaultBuffer, 0);
        }
        public static void WriteCharArray(this ObjectWriter writer, Char[] value)
        {
            writer.WriteTag(FormatterTag.CharArray);
            var bytes = writer.Encoding.GetBytes(value);
            writer.InnerWrite(bytes.Length);
            writer.Stream.WriteBytes(bytes);
        }
        public static Char[] ReadCharArray(this ObjectReader reader)
        {
            var bytes = reader.ReadByteArray();
            return reader.Encoding.GetChars(bytes);
        }

        #endregion

        #region - String -

        public static void WriteString(this ObjectWriter writer, String value)
        {
            writer.WriteTag(FormatterTag.String);
            if(value == null) writer.InnerWrite(-1);
            else if(value.Length == 0) writer.InnerWrite(0);
            else
            {
                var bytes = writer.Encoding.GetBytes(value);
                writer.InnerWrite(bytes.Length);
                writer.Stream.WriteBytes(bytes);
            }
        }
        public static string ReadString(this ObjectReader reader)
        {
            var byteLength = reader.ReadInt32();
            if(byteLength == -1) return null;
            else if(byteLength == 0) return string.Empty;
            var bytes = reader.ReadBuffer(byteLength);
            return reader.Encoding.GetString(bytes);
        }

        public static void WriteStringArray(this ObjectWriter writer, String[] value)
        {
            writer.WriteTag(FormatterTag.StringArray);
            writer.InnerWrite(value.Length);
            foreach(var item in value)
            {
                writer.WriteStringOrReference(item);
            }
        }
        public static string[] ReadStringArray(this ObjectReader reader)
        {
            var length = reader.ReadInt32();
            String[] value = new String[length];
            for(int i = 0; i < length; i++) value[i] = reader.ReadStringOrReference();
            return value;
        }

        #endregion

        #region - StringBuilder -

        public static void WriteStringBuilder(this ObjectWriter writer, StringBuilder value)
        {
            writer.WriteTag(FormatterTag.StringBuilder);
            writer.WriteStringOrReference(value.ToString());
        }
        public static StringBuilder ReadStringBuilder(this ObjectReader reader)
        {
            var str = reader.ReadStringOrReference();
            return new StringBuilder(str);
        }

        public static void WriteStringBuilderArray(this ObjectWriter writer, StringBuilder[] value)
        {
            writer.WriteTag(FormatterTag.StringBuilderArray);
            writer.InnerWrite(value.Length);
            foreach(var v in value)
            {
                if(v == null) writer.WriteNull();
                else writer.WriteStringOrReference(v.ToString());
            }
        }
        public static StringBuilder[] ReadStringBuilderArray(this ObjectReader reader)
        {
            var length = reader.ReadInt32();
            StringBuilder[] value = new StringBuilder[length];
            for(int i = 0; i < length; i++)
            {
                var str = reader.ReadStringOrReference();
                if(str == null) value[i] = null;
                else value[i] = new StringBuilder(str);
            }
            return value;
        }

        #endregion
    }
}
