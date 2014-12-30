using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Aoite.Net
{
    /// <summary>
    /// 定义一个接受连接的客户端。
    /// </summary>
    public interface IAcceptedClient
    {
        /// <summary>
        /// 获取一个值，该值指示客户端是否处于忙碌状态。
        /// </summary>
        bool IsBusy { get; }
        /// <summary>
        /// 获取客户端自定义数据的集合。
        /// </summary>
        AsyncDataCollection Data { get; }
        /// <summary>
        /// 获取或设置与此客户端关联的用户或应用程序对象。
        /// </summary>
        object Tag { get; set; }
        /// <summary>
        /// 获取客户端的接入时间。
        /// </summary>
        DateTime AcceptTime { get; }
        /// <summary>
        /// 获取客户端的终结点地址。
        /// </summary>
        string EndPoint { get; }
        /// <summary>
        /// 获取一个值，该值指示客户端是否已成功连接。
        /// </summary>
        bool Connected { get; }
        /// <summary>
        /// 使用 <paramref name="key"/> 获取或设置自定义数据的集合的值。
        /// </summary>
        /// <param name="key">自定义数据的键。</param>
        /// <returns>获取一个已存在的自定义数据的值，或一个 null 值。</returns>
        object this[string key] { get; set; }
        /// <summary>
        /// 将文件集合或者内存中的数据缓冲区以异步方法发送给连接的 <see cref="System.Net.Sockets.Socket"/> 对象。
        /// </summary>
        /// <param name="packets">要发送的缓冲区数组的 <see cref="System.Net.Sockets.SendPacketsElement"/> 对象数组。</param>
        /// <returns>返回一个值，指示异步发送的指令是否已完成。</returns>
        bool SendPacketsAsync(params SendPacketsElement[] packets);
        /// <summary>
        /// 将数据异步发送到连接的 <see cref="System.Net.Sockets.Socket"/> 对象。
        /// </summary>
        /// <param name="buffer">要用于异步套接字方法的数据缓冲区。</param>
        /// <param name="offset">数据缓冲区中操作开始位置处的偏移量，以字节为单位。</param>
        /// <param name="count">可在缓冲区中发送或接收的最大数据量（以字节为单位）。</param>
        /// <returns>返回一个值，指示异步发送的指令是否已完成。</returns>
        bool SendAsync(byte[] buffer, int offset, int count);
        /// <summary>
        /// 强制中断当前客户端的连接。
        /// </summary>
        void Shutdown();
        /// <summary>
        /// 开始一个操作。
        /// </summary>
        void BeginOperation();
        /// <summary>
        /// 结束一个操作。
        /// </summary>
        void EndOperation();
    }
}
