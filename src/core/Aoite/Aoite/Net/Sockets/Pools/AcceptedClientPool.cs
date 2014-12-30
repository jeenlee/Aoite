using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Net
{
    internal class AcceptedClientPool : ObjectPool<DefaultAcceptedClient>
    {
        private AsyncSocketServer _server;

        public AcceptedClientPool(AsyncSocketServer server)
        {
            this._server = server;
        }

        protected override DefaultAcceptedClient OnCreateObject()
        {
            return new DefaultAcceptedClient(this._server);
        }

        public override void Release(DefaultAcceptedClient obj)
        {
            obj.Release();
            base.Release(obj);
        }
    }
}
