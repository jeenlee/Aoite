using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Aoite.Serialization.BinarySuite
{
    internal class ObjectWriter : ObjectFormatterBase
    {
        public ObjectWriter(Stream stream, Encoding encoding)
            : base(stream, encoding)
        {
            if(!stream.CanWrite) throw new NotSupportedException("无法写入数据。");
        }
    }
}
