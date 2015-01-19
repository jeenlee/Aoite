using Aoite.Net;
using Aoite.Redis.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Aoite.Redis
{
    class DefaultConnector : ObjectDisposableBase, IConnector
    {
        private Socket _socket;
        private Stream _stream;

        public TimeSpan ConnectTimeout { get; set; }
        public SocketInfo SocketInfo { get; private set; }

        public Stream ReadStream
        {
            get
            {
                if(this.Connected || this.Connect()) return this._stream;
                throw new RedisIOException("连接到 Redis 服务器失败。");
            }
        }
        public Stream WriteStream { get { return this.ReadStream; } }

        public DefaultConnector(SocketInfo socketInfo)
        {
            if(socketInfo == null) throw new ArgumentNullException("socketInfo");
            socketInfo.EndPoint.ToLoopback();
            this.SocketInfo = socketInfo;
            this.ConnectTimeout = TimeSpan.FromMilliseconds(-1);
        }

        private void Reusable()
        {
            this._socket.Shutdown(true);
            this._stream.TryDispose();
            this._socket = null;
            this._stream = null;
        }

        public bool Connected { get { return this._socket != null && this._socket.Connected; } }

        public bool Connect()
        {
            this.ThrowWhenDisposed();

            this.Reusable();
            this._socket = this.SocketInfo.CreateSocket();
            this._socket.Connect(this.SocketInfo.EndPoint, this.ConnectTimeout);
            if(this._socket.Connected)
            {
                this._stream = new BufferedStream(new NetworkStream(this._socket));
            }
            return this._socket.Connected;
        }

        protected override void DisposeManaged()
        {
            this.Reusable();
            base.DisposeManaged();
        }

    }
}
