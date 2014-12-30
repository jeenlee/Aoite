using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Aoite.Net
{
    internal class SaeaPool : ObjectPool<SaeaEventArgs>
    {
        private EventHandler<SocketAsyncEventArgs> _onCompleted;

        public SaeaPool(EventHandler<SocketAsyncEventArgs> onCompleted, Func<SaeaEventArgs> objectFactory = null)
            : base(objectFactory)
        {
            this._onCompleted = onCompleted;
        }

        public SaeaPool() { }

        public override SaeaEventArgs Acquire()
        {
            var saea = base.Acquire();
            if(this._onCompleted != null) saea.Completed += this._onCompleted;
            return saea;
        }

        public override void Release(SaeaEventArgs item)
        {
            if(this._onCompleted != null) item.Completed -= this._onCompleted;
            item.Release();
            base.Release(item);
        }

        protected override void DisposeItem(SaeaEventArgs item)
        {
            //item.Release();
            item.Dispose();
        }
    }
}