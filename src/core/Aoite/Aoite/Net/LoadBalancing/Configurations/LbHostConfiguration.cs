using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

namespace Aoite.Net.LoadBalancing.Configurations
{
    /// <summary>
    /// 表示一个负载均衡的主机配置。
    /// </summary>
    public class LbHostConfiguration : LbConfigurationBase
    {
        /// <summary>
        /// 设置或获取一个值，该值指示当前主机是否允许远程管理。默认为 false。
        /// </summary>
        public bool AllowRemoteManage { get; set; }
        /// <summary>
        /// 设置或获取一个值，表示判定负载均衡节点的故障时间（秒）。默认为 5s。
        /// </summary>
        public int Timeout { get; set; }
        /// <summary>
        /// 设置或获取一个值，表示尝试恢复故障节点的时间（秒）。默认为 600s。
        /// </summary>
        public int RecoveryTimes { get; set; }

        #region <<通讯属性>>


        private int _MaxBufferSize = 2048;
        /// <summary>
        /// 设置或获取最大缓冲区大小。默认为 2048 字节。
        /// </summary>
        public int MaxBufferSize { get { return this._MaxBufferSize; } set { this._MaxBufferSize = value; } }

        private int _MaxConnectionCount = 1111;
        /// <summary>
        /// 设置或获取允许最大的连接数。默认为 1111 个连接数。
        /// </summary>
        public int MaxConnectionCount { get { return this._MaxConnectionCount; } set { this._MaxConnectionCount = value; } }

        private int _ListenBacklog = 111;
        /// <summary>
        /// 设置或获取一个值，表示挂起连接队列的最大长度。默认为 111 并发数。
        /// </summary>
        public int ListenBacklog { get { return this._ListenBacklog; } set { this._ListenBacklog = value; } }

        #endregion
        /// <summary>
        /// 设置或获取负载均衡的节点策略。
        /// </summary>
        public LbStrategy Strategy { get; set; }

        private Dictionary<string, object> _ExtendData = new Dictionary<string,object>( StringComparer.CurrentCultureIgnoreCase);
        /// <summary>
        /// 设置或获取扩展的用户数据集合。
        /// </summary>
        public Dictionary<string, object> ExtendData
        {
            get { return this._ExtendData; }
            set { this._ExtendData = new Dictionary<string, object>(value, StringComparer.CurrentCultureIgnoreCase); }
        }

        /// <summary>
        /// 设置或获取负载均衡的节点集。
        /// </summary>
        public List<LbNodeConfiguration> Nodes { get; set; }

        /// <summary>
        /// 初始化一个 <see cref="Aoite.Net.LoadBalancing.Configurations.LbHostConfiguration"/> 类的新实例。
        /// </summary>
        public LbHostConfiguration()
        {
            this.Timeout = 5;
            this.RecoveryTimes = 600;
            this.Strategy = LbStrategy.IPQueue;
            this.Nodes = new List<LbNodeConfiguration>();
        }
    }
}
