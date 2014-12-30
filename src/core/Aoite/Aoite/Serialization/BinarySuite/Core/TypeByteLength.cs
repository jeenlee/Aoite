using System;
using System.Collections.Generic;
using System.Text;

namespace Aoite.Serialization.BinarySuite
{
    internal static class TypeByteLength
    {

        public readonly static int Guid = System.Runtime.InteropServices.Marshal.SizeOf(Types.Guid);
        public const int Boolean = sizeof(Boolean);

        public const int Single = sizeof(Single);
        public const int Double = sizeof(Double);
        public const int Decimal = sizeof(Decimal);

        public const int Int16 = sizeof(Int16);
        public const int Int32 = sizeof(Int32);
        public const int Int64 = sizeof(Int64);
        public const int UInt16 = sizeof(UInt16);
        public const int UInt32 = sizeof(UInt32);
        public const int UInt64 = sizeof(UInt64);

        public const int Char = sizeof(Char);

    }
}
