using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Aoite.ServiceModel
{
    /// <summary>
    /// 定义一个契约的客户端。
    /// </summary>
    public interface IContractClient : IDisposable
    {
        /// <summary>
        /// 获取一个值，该值指示当契约客户端是否已释放。
        /// </summary>
        bool IsDisposed { get; }
        /// <summary>
        /// 获取契约的域。
        /// </summary>
        ContractDomain Domain { get; }

        /// <summary>
        /// 获取契约的信息。
        /// </summary>
        IContractInfo Contract { get; }
        /// <summary>
        /// 获取契约的生命周期。
        /// </summary>
        IContractLifeCycle LifeCycle { get; }
        /// <summary>
        /// 获取一个值，该值指示是否为一个持久连接的契约客户端。
        /// </summary>
        bool KeepAlive { get; }
        /// <summary>
        /// 添加一个文件到下一次契约请求。
        /// </summary>
        /// <param name="fileName">文件的完全限定名。</param>
        /// <param name="fileSize">文件的大小（以字节为单位）。</param>
        /// <param name="content">一个 <see cref="System.IO.Stream"/> 对象，该对象指向一个准备上传的内容。</param>
        void AddFile(string fileName, int fileSize, Stream content);
        /// <summary>
        /// 添加一个文件到下一次契约请求。
        /// </summary>
        /// <param name="fileName">文件的完全限定名。</param>
        void AddFile(string fileName);
        /// <summary>
        /// 添加一个文件到下一次契约请求。
        /// </summary>
        /// <param name="fileStream">文件流。</param>
        void AddFile(FileStream fileStream);
        /// <summary>
        /// 添加一个文件到下一次契约请求。
        /// </summary>
        /// <param name="file">文件。</param>
        void AddFile(ContractFile file);

        /// <summary>
        /// 获取所有正在等待上传 -或- 等待下载的文件列表。
        /// </summary>
        /// <param name="peekTime">指示是否为查看模式，如果为 true 则下一次访问此方法仍然可以获取数据，否则下一次访问之前如果没有添加新文件将会返回一个 null 值。</param>
        /// <returns>返回所有正在等待上传 -或- 等待下载的文件列表，如果没有等待上传 -或- 等待下载的文件，将返回一个 null 值。</returns>
        ContractFile[] GetFiles(bool peekTime);
    }
}
