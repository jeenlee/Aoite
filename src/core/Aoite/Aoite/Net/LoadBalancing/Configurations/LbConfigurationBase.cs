using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Net.LoadBalancing.Configurations
{
    /// <summary>
    /// 表示一个负载均衡的配置基类。
    /// </summary>
    public class LbConfigurationBase : IHostPort
    {
        /// <summary>
        /// 设置或获取网络主机地址。
        /// </summary>
        public string Host { get; set; }
        /// <summary>
        /// 设置或获取网络主机端口。
        /// </summary>
        public int Port { get; set; }
    }
}
