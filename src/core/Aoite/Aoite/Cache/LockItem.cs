using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Cache
{
    class LockItem : IDisposable
    {
        Action _disposing;
        public LockItem(Action disposing)
        {
            if(disposing == null) throw new ArgumentNullException("disposing");
            this._disposing = disposing;
        }
        void IDisposable.Dispose()
        {
            this._disposing();
        }

        internal static void TimeoutError(string key)
        {
            throw new TimeoutException("键 {0} 的锁已被占用，获取锁超时。".Fmt(key));
        }
    }
}
