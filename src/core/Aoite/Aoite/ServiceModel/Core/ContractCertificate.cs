using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.ServiceModel
{
    /// <summary>
    /// 表示一个契约证书。
    /// </summary>
    public class ContractCertificate
    {
        /// <summary>
        /// 获取或设置证书的应用标识。
        /// </summary>
        public string AppKey { get; set; }

        /// <summary>
        /// 获取或设置证书的应用密钥。
        /// </summary>
        public string AppSecret { get; set; }

        /// <summary>
        /// 初始化一个 <see cref="Aoite.ServiceModel.ContractCertificate"/> 类的新实例。
        /// </summary>
        public ContractCertificate() { }
    }
}
