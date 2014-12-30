using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Serialization.BinarySuite
{
    internal enum FormatterTag : byte
    {
        Reference,
        Null,
        DBNull,

        Guid,
        GuidArray,
        DateTime,
        DateTimeArray,
        TimeSpan,
        TimeSpanArray,

        Boolean,
        BooleanArray,


        Char,
        CharArray,
        String,
        StringArray,

        Byte,
        ByteArray,
        SByte,
        SByteArray,

        Int16,
        Int16Array,
        Int32,
        Int32Array,
        Int64,
        Int64Array,

        UInt16,
        UInt16Array,
        UInt32,
        UInt32Array,
        UInt64,
        UInt64Array,

        Single,
        SingleArray,
        Double,
        DoubleArray,
        Decimal,
        DecimalArray,

        Array,
        MultiRankArray,
        StringBuilder,
        StringBuilderArray,

        GList,
        GDictionary,
        HybridDictionary,
        GConcurrentDictionary,

        Object,
        ObjectArray,
        ValueTypeObject,

        Type,
        TypeArray,
        Result,
        GResult,
        SuccessfullyResult,
    }
}
