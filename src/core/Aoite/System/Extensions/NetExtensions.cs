using Aoite.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace System
{
    /// <summary>
    /// 提供用于 System.Net 的实用工具方法。
    /// </summary>
    public static class NetExtensions
    {
        /// <summary>
        /// 强制停止。
        /// </summary>
        /// <param name="socket">一个 <see cref="System.Net.Sockets.Socket"/>。</param>
        /// <param name="disposing">指示是否释放 Socket 对象，并且不再使用。</param>
        public static void Shutdown(this Socket socket, bool disposing)
        {
            if(socket == null) return;
            if(socket.Connected)
            {
                try
                {
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Disconnect(!disposing);
                }
                catch(Exception) { }
                socket.Close(1);
            }
            if(disposing) socket.Dispose();
        }
        /// <summary>
        /// 将指定主机地址转换为 <see cref="System.Net.IPAddress"/> 类的新实例，并创建一个 <see cref="System.Net.IPEndPoint"/> 类的新实例。
        /// </summary>
        /// <param name="hp">一个 <see cref="Aoite.Net.IHostPort"/> 类的实现。</param>
        /// <returns>返回一个新的 <see cref="System.Net.IPEndPoint"/> 实例。</returns>
        public static IPEndPoint ToIPEndPoint(this IHostPort hp)
        {
            if(hp == null) throw new ArgumentNullException("hp");
            return GA.Net.CreateEndPoint(hp.Host, hp.Port);
        }

        /// <summary>
        /// 将当前网络地址转换为环回地址。
        /// </summary>
        /// <param name="endPoint">网络地址。</param>
        /// <returns>返回当前实例。</returns>
        public static IPEndPoint ToLoopback(this IPEndPoint endPoint)
        {
            if(endPoint == null) throw new ArgumentNullException("endPoint");
            if(endPoint.Address == IPAddress.Any) endPoint.Address = IPAddress.Loopback;
            return endPoint;
        }

        /// <summary>
        /// 开始一个对远程主机连接的异步请求。
        /// </summary>
        /// <param name="server">服务端。</param>
        /// <param name="endPoint">远程主机。</param>
        /// <param name="timeout">表示等待的毫秒数的 <see cref="System.TimeSpan"/>，或表示 -1 毫秒（无限期等待）的 <see cref="System.TimeSpan"/>。</param>
        public static void Connect(this Socket server, EndPoint endPoint, TimeSpan timeout)
        {
            var asyncResult = server.BeginConnect(endPoint, null, null);
            if(!asyncResult.AsyncWaitHandle.WaitOne(timeout)) throw new SocketException((int)SocketError.TimedOut);
            server.EndConnect(asyncResult);
        }

        /// <summary>
        /// 开始一个对远程主机连接的异步请求。
        /// </summary>
        /// <param name="server">服务端。</param>
        /// <param name="host">远程主机的名称。</param>
        /// <param name="port">远程主机的端口号。</param>
        /// <param name="timeout">表示等待的毫秒数的 <see cref="System.TimeSpan"/>，或表示 -1 毫秒（无限期等待）的 <see cref="System.TimeSpan"/>。</param>
        public static void Connect(this TcpClient server, string host, int port, TimeSpan timeout)
        {
            var asyncResult = server.BeginConnect(host, port, null, null);
            if(!asyncResult.AsyncWaitHandle.WaitOne(timeout)) throw new SocketException((int)SocketError.TimedOut);
            server.EndConnect(asyncResult);
        }

        /// <summary>
        /// 开始一个对远程主机连接的异步请求。
        /// </summary>
        /// <param name="server">服务端。</param>
        /// <param name="endPoint">打算连接到的 <see cref="System.Net.IPEndPoint"/>。</param>
        /// <param name="timeout">表示等待的毫秒数的 <see cref="System.TimeSpan"/>，或表示 -1 毫秒（无限期等待）的 <see cref="System.TimeSpan"/>。</param>
        public static void Connect(this TcpClient server, IPEndPoint endPoint, TimeSpan timeout)
        {
            var asyncResult = server.BeginConnect(endPoint.Address, endPoint.Port, null, null);
            if(!asyncResult.AsyncWaitHandle.WaitOne(timeout)) throw new SocketException((int)SocketError.TimedOut);
            server.EndConnect(asyncResult);
        }

        /// <summary>
        /// 设置以秒为单位的心跳侦测间隔。
        /// </summary>
        /// <param name="socket">套接字。</param>
        /// <param name="ms">心跳侦测间隔。</param>
        public static void SetKeepAlive(this Socket socket, uint ms)
        {
            var bytes = new byte[12];
            //- 是否启用Keep-Alive
            BitConverter.GetBytes((uint)1).CopyTo(bytes, 0);
            //- 首次探测时间
            BitConverter.GetBytes(ms).CopyTo(bytes, 4);
            //- 间隔探测时间
            BitConverter.GetBytes(ms).CopyTo(bytes, 8);
            socket.IOControl(IOControlCode.KeepAliveValues, bytes, null);
        }
    }
}
