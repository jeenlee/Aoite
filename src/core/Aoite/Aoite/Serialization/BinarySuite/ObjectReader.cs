using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Aoite.Serialization.BinarySuite
{
    internal class ObjectReader : ObjectFormatterBase
    {
        public readonly byte[] DefaultBuffer = new byte[16];
        public ObjectReader(Stream stream, Encoding encoding)
            : base(stream, encoding)
        {
            if(!stream.CanRead) throw new NotSupportedException("无法读取数据。");
        }
    }
}
