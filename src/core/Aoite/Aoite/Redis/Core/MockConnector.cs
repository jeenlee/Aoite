using Aoite.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Aoite.Redis
{
    class MockConnector : ObjectDisposableBase, IConnector
    {
        public TimeSpan ConnectTimeout { get; set; }
        public SocketInfo SocketInfo { get; private set; }

        private bool _Connected;
        public bool Connected { get { return this._Connected; } }

        public MockConnector(params string[] mockResponses) : this("localhost", 9999, mockResponses) { }

        public MockConnector(string host, int port, params string[] mockResponses)
        {
            this.SocketInfo = new SocketInfo(host, port);

            this._ReadStream = new MemoryStream();
            this._WriteStream = new MemoryStream();

            for(int i = 0; i < mockResponses.Length; i++)
            {
                var data = GA.UTF8.GetBytes(mockResponses[i]);
                this._ReadStream.Write(data, 0, data.Length);
            }
            this._ReadStream.Position = 0;
        }

        private MemoryStream _ReadStream;
        public Stream ReadStream { get { return _ReadStream; } }

        private MemoryStream _WriteStream;
        public Stream WriteStream { get { return _WriteStream; } }

        public bool Connect()
        {
            return this._Connected = true;
        }

        public string GetMessage()
        {
            var data = this._WriteStream.ToArray();
            this._WriteStream = new MemoryStream();
            return GA.UTF8.GetString(data);
        }

        protected override void DisposeManaged()
        {
            this._Connected = false;
            this._ReadStream.TryDispose();
            this._ReadStream = null;
            this._WriteStream.TryDispose();
            this._WriteStream = null;

            base.DisposeManaged();
        }
    }
}
