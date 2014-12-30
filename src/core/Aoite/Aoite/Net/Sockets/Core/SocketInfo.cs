using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Aoite.Net
{
    /// <summary>
    /// 表示套接字的信息。
    /// </summary>
    public class SocketInfo
    {
        private IPEndPoint _EndPoint;
        /// <summary>
        /// 获取网络端点。
        /// </summary>
        public IPEndPoint EndPoint { get { return this._EndPoint; } }

        /// <summary>
        /// 提供主机地址和主机端口，初始化一个 <see cref="Aoite.Net.SocketInfo"/> 类的新实例。
        /// </summary>
        /// <param name="host">主机地址。</param>
        /// <param name="port">主机端口。</param>
        public SocketInfo(string host, int port)
            : this(GA.Net.CreateEndPoint(host, port)) { }

        /// <summary>
        /// 提供网络端点，初始化一个 <see cref="Aoite.Net.SocketInfo"/> 类的新实例。
        /// </summary>
        /// <param name="endPoint">网络端点。</param>
        public SocketInfo(IPEndPoint endPoint)
        {
            if(endPoint == null) throw new ArgumentNullException("endPoint");
            this._EndPoint = endPoint;
        }

        private uint _KeepAliveMilliseconds = 60000U;
        /// <summary>
        /// 获取或设置以秒为单位的心跳侦测间隔。默认为 60 秒。当为 0 时表示采用操作系统的默认心跳侦测间隔。
        /// </summary>
        public uint KeepAliveSeconds
        {
            get
            {
                return this._KeepAliveMilliseconds / 1000U;
            }
            set
            {
                this._KeepAliveMilliseconds = value * 1000U;
            }
        }

        /// <summary>
        /// 创建一个 <see cref="System.Net.Sockets.Socket"/>。
        /// </summary>
        /// <returns>返回一个 <see cref="System.Net.Sockets.Socket"/>。</returns>
        public Socket CreateSocket()
        {
            var socket = new Socket(this._EndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            //socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            //socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, false);
            if(this._KeepAliveMilliseconds > 0U)
            {
                socket.SetKeepAlive(this._KeepAliveMilliseconds);
            }
            return socket;
        }

        /// <summary>
        /// 将指定的端口隐式转换为 <see cref="Aoite.Net.SocketInfo"/> 类的新实例。
        /// </summary>
        /// <param name="port">主机端口。</param>
        /// <returns>返回一个 <see cref="Aoite.Net.SocketInfo"/> 类的新实例。</returns>
        public static implicit operator SocketInfo(int port)
        {
            return new SocketInfo(null, port);
        }

        /// <summary>
        /// 将指定的网络端点隐式转换为 <see cref="Aoite.Net.SocketInfo"/> 类的新实例。
        /// </summary>
        /// <param name="endPoint">网络端点。</param>
        /// <returns>返回一个 <see cref="Aoite.Net.SocketInfo"/> 类的新实例。</returns>
        public static implicit operator SocketInfo(IPEndPoint endPoint)
        {
            return new SocketInfo(endPoint);
        }
    }
}
