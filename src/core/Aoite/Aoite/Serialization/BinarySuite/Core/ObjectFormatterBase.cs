using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Aoite.Serialization.BinarySuite
{
    internal abstract class ObjectFormatterBase : IDisposable
    {
        public readonly Stream Stream;
        public readonly Encoding Encoding;
        public readonly List<object> ReferenceContainer = new List<object>(11);

        public ObjectFormatterBase(Stream stream, Encoding encoding)
        {
            if(stream == null) throw new ArgumentNullException("stream");
            this.Stream = stream;
            this.Encoding = encoding ?? Encoding.UTF8;
        }

        void IDisposable.Dispose()
        {
            Stream.Dispose();
        }
    }
}
