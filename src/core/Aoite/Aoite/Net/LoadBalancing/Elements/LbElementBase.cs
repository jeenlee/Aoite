using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Aoite.Net.LoadBalancing.Elements
{
    /// <summary>
    /// 表示一个负载均衡的元素基类。
    /// </summary>
    public class LbElementBase
    {
        /// <summary>
        /// 设置或获取网络主机的终结点。
        /// </summary>
        public IPEndPoint EndPoint { get; set; }
    }
}
