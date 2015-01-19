using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Aoite.Serialization.BinarySuite
{
    internal static partial class NumberFormatters
    {
        #region - Single -

        public static void WriteSingle(this ObjectWriter writer, Single value)
        {
            writer.WriteTag(FormatterTag.Single);
            writer.Stream.WriteBytes(BitConverter.GetBytes(value));
        }
        public static Single ReadSingle(this ObjectReader reader)
        {
            reader.Stream.Read(reader.DefaultBuffer, 0, TypeByteLength.Single);
            return BitConverter.ToSingle(reader.DefaultBuffer, 0);
        }

        public static void WriteSingleArray(this ObjectWriter writer, Single[] value)
        {
            writer.WriteTag(FormatterTag.SingleArray);
            writer.InnerWrite(value.Length);
            foreach(var item in value)
            {
                writer.Stream.WriteBytes(BitConverter.GetBytes(item));
            }
        }
        public static Single[] ReadSingleArray(this ObjectReader reader)
        {
            var length = reader.ReadInt32();
            Single[] value = new Single[length];
            for(int i = 0; i < length; i++) value[i] = reader.ReadSingle();
            return value;
        }

        #endregion

        #region - Double -

        public static void WriteDouble(this ObjectWriter writer, Double value)
        {
            writer.WriteTag(FormatterTag.Double);
            writer.Stream.WriteBytes(BitConverter.GetBytes(value));
        }
        public static Double ReadDouble(this ObjectReader reader)
        {
            reader.Stream.Read(reader.DefaultBuffer, 0, TypeByteLength.Double);
            return BitConverter.ToDouble(reader.DefaultBuffer, 0);
        }

        public static void WriteDoubleArray(this ObjectWriter writer, Double[] value)
        {
            writer.WriteTag(FormatterTag.DoubleArray);
            writer.InnerWrite(value.Length);
            foreach(var item in value)
            {
                writer.Stream.WriteBytes(BitConverter.GetBytes(item));
            }
        }
        public static Double[] ReadDoubleArray(this ObjectReader reader)
        {
            var length = reader.ReadInt32();
            Double[] value = new Double[length];
            for(int i = 0; i < length; i++) value[i] = reader.ReadDouble();
            return value;
        }

        #endregion

        #region - Decimal -

        public static void WriteDecimal(this ObjectWriter writer, Decimal value)
        {
            writer.WriteTag(FormatterTag.Decimal);
            writer.InnerWrite(value);
        }
        public static Decimal ReadDecimal(this ObjectReader reader)
        {
            reader.Stream.Read(reader.DefaultBuffer, 0, TypeByteLength.Decimal);
            var bits = new int[]
            {
                BitConverter.ToInt32(reader.DefaultBuffer, 0),
                BitConverter.ToInt32(reader.DefaultBuffer, 4),
                BitConverter.ToInt32(reader.DefaultBuffer, 8),
                BitConverter.ToInt32(reader.DefaultBuffer, 12)
            };
            return new decimal(bits);
        }

        public static void WriteDecimalArray(this ObjectWriter writer, Decimal[] value)
        {
            writer.WriteTag(FormatterTag.DecimalArray);
            writer.InnerWrite(value.Length);
            foreach(var item in value)
            {
                writer.InnerWrite(item);
            }
        }
        public static Decimal[] ReadDecimalArray(this ObjectReader reader)
        {
            var length = reader.ReadInt32();
            Decimal[] value = new Decimal[length];
            for(int i = 0; i < length; i++) value[i] = reader.ReadDecimal();
            return value;
        }

        #endregion
    }
}
