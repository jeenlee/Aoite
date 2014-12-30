using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Net.LoadBalancing.Configurations
{
    /// <summary>
    /// 表示一个负载均衡的节点配置。
    /// </summary>
    public class LbNodeConfiguration : LbConfigurationBase
    {
        /// <summary>
        /// 设置或获取负载均衡元素的权重。
        /// </summary>
        public int Weight { get; set; }
        /// <summary>
        /// 初始化一个 <see cref="Aoite.Net.LoadBalancing.Configurations.LbNodeConfiguration"/> 类的新实例。
        /// </summary>
        public LbNodeConfiguration() { }
    }
}
