using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Aoite.Net
{
    class DefaultAcceptedClient : IAcceptedClient
    {
        private bool _IsAttached;
        public bool IsAttached { get { return this._IsAttached; } }

        private long _activeOperationCount;
        public bool IsBusy
        {
            get { return Interlocked.Read(ref this._activeOperationCount) > 0L; }
        }

        private AsyncSocketServer _Server;
        public AsyncSocketServer Server { get { return this._Server; } }

        private SaeaEventArgs _ReceiveSaea;
        public SaeaEventArgs ReceiveSaea { get { return this._ReceiveSaea; } }

        private DateTime _AcceptTime;
        public DateTime AcceptTime { get { return this._AcceptTime; } }

        private AsyncDataCollection _Data;
        public AsyncDataCollection Data
        {
            get
            {
                this.TestIsAttached();

                if(this._Data == null) lock(this) if(this._Data == null) this._Data = new AsyncDataCollection();
                return this._Data;
            }
        }

        public object Tag { get; set; }

        private string _EndPoint;
        public string EndPoint { get { return this._EndPoint; } }

        public object this[string key]
        {
            get
            {
                this.TestIsAttached();
                return this.Data.TryGetValue(key);
            }
            set
            {
                this.TestIsAttached();
                this.Data[key] = value;
            }
        }

        public bool Connected
        {
            get
            {
                return this._IsAttached && this._ReceiveSaea.AcceptSocket.Connected;
            }
        }

        public DefaultAcceptedClient(AsyncSocketServer server)
        {
            this._Server = server;
        }

        public void Attach(SaeaEventArgs saea, string endPoint)
        {
            saea.Client = this;

            this._ReceiveSaea = saea;
            this._EndPoint = endPoint;
            this._AcceptTime = DateTime.Now;
            this._IsAttached = true;
        }

        public void Release()
        {
            this._IsAttached = false;
            this._ReceiveSaea = null;
            this._EndPoint = null;
            this._Data = null;
        }

        public bool SendPacketsAsync(params SendPacketsElement[] packets)
        {
            this.TestIsAttached();
            return this._Server.SendPacketsAsync(this._ReceiveSaea.AcceptSocket, packets);
        }

        public bool SendAsync(byte[] buffer, int offset, int count)
        {
            this.TestIsAttached();
            return this._Server.SendAsync(this._ReceiveSaea.AcceptSocket, buffer, offset, count);
        }

        public void BeginOperation()
        {
            this.TestIsAttached();
            Interlocked.Increment(ref this._activeOperationCount);
        }
        public void EndOperation()
        {
            this.TestIsAttached();
            Interlocked.Decrement(ref this._activeOperationCount);
        }

        private void TestIsAttached()
        {
            if(!this._IsAttached) throw new ObjectDisposedException(this.GetType().FullName);
        }

        public void Shutdown()
        {
            this.TestIsAttached();
            var socket = this._ReceiveSaea.AcceptSocket;
            socket.Shutdown(true);
        }
    }
}
