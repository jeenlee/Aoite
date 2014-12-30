using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Aoite.Net.LoadBalancing
{
    class AsyncState
    {
        public TcpClient FromClient;
        public NetworkStream FromStream;
        public TcpClient ToClient;
        public NetworkStream ToStream;
        public string ToEndPoint;

        public AsyncState(TcpClient fromClient, TcpClient toClient)
        {
            this.FromClient = fromClient;
            this.FromStream = fromClient.GetStream();
            this.ToClient = toClient;
            this.ToStream = toClient.GetStream();
            this.ToEndPoint = toClient.Client.RemoteEndPoint.ToString();
        }
    }
}
