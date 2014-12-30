using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.ServiceModel
{
    /// <summary>
    /// 定义一个契约客户端的生命周期。
    /// </summary>
    public interface IContractLifeCycle : IDisposable
    {
        /// <summary>
        /// 设置或获取所属的契约客户端。
        /// </summary>
        IContractClient Client { get; set; }
        /// <summary>
        /// 获取指定请求的响应。
        /// </summary>
        /// <param name="request">契约的请求。</param>
        /// <returns>返回一个请求的响应。</returns>
        ContractResponse GetResponse(ContractRequest request);
        /// <summary>
        /// 打开契约客户端的连接。
        /// </summary>
        /// <returns>返回一个结果。</returns>
        Result Open();
        /// <summary>
        /// 关闭契约客户端的连接。
        /// </summary>
        void Close();
    }

}
