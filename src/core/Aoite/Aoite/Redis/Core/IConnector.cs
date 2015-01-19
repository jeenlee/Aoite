using Aoite.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Aoite.Redis
{
    interface IConnector : IObjectDisposable
    {
        TimeSpan ConnectTimeout { get; set; }
        SocketInfo SocketInfo { get; }
        Stream ReadStream { get; }
        Stream WriteStream { get; }
        bool Connected { get; }
        bool Connect();
    }
}
