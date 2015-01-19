using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace System
{
    partial class GA
    {
        /// <summary>
        /// 提供用于 System.Net 的实用工具方法。
        /// </summary>
        public static class Net
        {
            /// <summary>
            ///  获取本机已被使用的网络端点。
            /// </summary>
            /// <returns>返回本机所有网络端点。</returns>
            public static IEnumerable<IPEndPoint> GetUsedIPEndPoint()
            {
                //获取一个对象，该对象提供有关本地计算机的网络连接和通信统计数据的信息。
                IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();

                foreach(var item in ipGlobalProperties.GetActiveTcpListeners()) yield return item;
                foreach(var item in ipGlobalProperties.GetActiveUdpListeners()) yield return item;
                foreach(var item in ipGlobalProperties.GetActiveTcpConnections()) yield return item.LocalEndPoint;
            }

            /// <summary>
            /// 判断指定的网络端点（只判断端口）是否被使用。
            /// </summary>
            /// <returns>如果端口已被占用，则返回 true，否则返回 false。</returns>
            public static bool IsUsedIPEndPoint(int port)
            {
                foreach(IPEndPoint iep in GetUsedIPEndPoint())
                {
                    if(iep.Port == port)
                    {
                        return true;
                    }
                }
                return false;
            }

            /// <summary>
            /// 判断指定的网络端点（判断IP和端口）是否被使用
            /// </summary>
            /// <param name="host">主机地址。</param>
            /// <param name="port">主机端口。</param>
            /// <returns>如果地址和端口已被占用，则返回 true，否则返回 false。</returns>
            public static bool IsUsedIPEndPoint(string host, int port)
            {
                foreach(IPEndPoint iep in GetUsedIPEndPoint())
                {
                    if(iep.Address.ToString() == host && iep.Port == port)
                    {
                        return true;
                    }
                }
                return false;
            }

            /// <summary>
            /// 将指定主机地址转换为 <see cref="System.Net.IPAddress"/> 类的新实例，并创建一个 <see cref="System.Net.IPEndPoint"/> 类的新实例。
            /// </summary>
            /// <param name="host">主机地址。</param>
            /// <param name="port">主机端口。</param>
            /// <returns>返回一个新的 <see cref="System.Net.IPEndPoint"/> 实例。</returns>
            public static IPEndPoint CreateEndPoint(string host, int port)
            {
                return new IPEndPoint((string.IsNullOrEmpty(host) || host.iEquals("localhost"))
                ? IPAddress.Any
                : IPAddress.Parse(host), port);
            }
        }
    }
}
