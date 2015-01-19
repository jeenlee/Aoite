using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// 提供用于 System.IO 命名空间下的实用工具方法。
    /// </summary>
    public static class IOExtensions
    {
        /// <summary>
        /// 从当前流中读取所有字节并将其写入到目标流中（使用指定的缓冲区大小）。
        /// </summary>
        /// <param name="stream">源流。</param>
        /// <param name="source">将包含当前流的内容的流。</param>
        /// <param name="startPosition">指定开始复制的流位置。</param>
        /// <param name="bufferSize">缓冲区的大小。此值必须大于零。默认大小为 4096。</param>
        public static void CopyTo(this Stream stream, Stream source, long startPosition, int bufferSize)
        {
            long oldPosition = 0L;
            if(stream.CanSeek)
            {
                oldPosition = stream.Position;
                stream.Position = startPosition;
            }
            byte[] array = new byte[bufferSize];
            while(true)
            {
                int size;
                if((size = stream.Read(array, 0, array.Length)) == 0)
                {
                    break;
                }
                source.Write(array, 0, size);
            }
            if(stream.CanSeek) stream.Position = oldPosition;
        }

        /// <summary>
        /// 向当前流写入字节序列。
        /// </summary>
        /// <param name="stream">当前流。</param>
        /// <param name="bytes">字节序列。</param>
        public static void WriteBytes(this Stream stream, byte[] bytes)
        {
            IOExtensions.WriteBytes(stream, bytes, bytes.Length);
        }

        /// <summary>
        /// 向当前流写入字节序列。
        /// </summary>
        /// <param name="stream">当前流。</param>
        /// <param name="bytes">字节序列。</param>
        /// <param name="count">要写入当前流的字节数。</param>
        public static void WriteBytes(this Stream stream, byte[] bytes, int count)
        {
            stream.Write(bytes, 0, count);
        }
    }
}
