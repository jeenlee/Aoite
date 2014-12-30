using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Aoite.Net
{
    internal static class MessageHelper
    {
        const int MessageLengthByteSizes = sizeof(int);

        public static int ReadMessageLength(this byte[] buffer, ref int offset, ref int count)
        {
            var messageLength = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, offset));
            offset += MessageLengthByteSizes;
            count -= MessageLengthByteSizes;
            return messageLength;
        }

        public static byte[] WriteMessageLength(this byte[] buffer, int offset, int count, MessageMode mode = MessageMode.Full)
        {
            byte[] bytes = new byte[count + 5];
            bytes[0] = (byte)mode;
            BitConverter.GetBytes(IPAddress.HostToNetworkOrder(count)).CopyTo(bytes, 1);
            buffer.CopyTo(bytes, 5);
            return bytes;

            //SendPacketsElement[] els = new SendPacketsElement[2];
            //els[0] = CreateSendPacketsElement(mode, count);
            //els[1] = new SendPacketsElement(buffer, offset, count);
            //return els;
        }

        private static SendPacketsElement CreateSendPacketsElement(MessageMode mode, int count)
        {
            byte[] bytes = new byte[5];
            bytes[0] = (byte)mode;
            BitConverter.GetBytes(IPAddress.HostToNetworkOrder(count)).CopyTo(bytes, 1);
            return new SendPacketsElement(bytes);
        }

        public static SendPacketsElement[] WriteMessageLength(this SendPacketsElement[] packets, MessageMode mode = MessageMode.Full)
        {
            SendPacketsElement[] newPackets = new SendPacketsElement[packets.Length + 1];
            int totalLength = 0;
            for(int i = 0; i < packets.Length; i++)
            {
                var p = packets[i];
                newPackets[i + 1] = p;
                checked
                {
                    if(p.Count > 0) totalLength += p.Count;
                    else if(p.Buffer != null) totalLength += p.Buffer.Length;
                    else
                    {
                        System.IO.FileInfo fileInfo = new System.IO.FileInfo(p.FilePath);
                        var fileLength = fileInfo.Length;
                        if((fileLength - totalLength) >= int.MaxValue) throw new System.IO.InvalidDataException();
                        totalLength += (int)fileLength;
                    }
                }
            }
            newPackets[0] = CreateSendPacketsElement(mode, totalLength);
            return newPackets;
        }
    }
}
