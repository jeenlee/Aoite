//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Text;

//namespace Aoite.ServiceModel
//{
//    /// <summary>
//    /// 表示契约处理模块。
//    /// </summary>
//    public abstract class ContractModule
//    {
//        private Uri _Url;
//        /// <summary>
//        /// 获取处理模块的服务地址。
//        /// </summary>
//        public Uri Url
//        {
//            get { return this._Url; }
//        }

//        private Encoding _Encoding;
//        /// <summary>
//        /// 获取或设置处理模块的通讯编码。默认编码为 <see cref="System.Text.Encoding.UTF8"/>。
//        /// </summary>
//        public virtual Encoding Encoding
//        {
//            get { return this._Encoding ?? Encoding.UTF8; }
//            set { this._Encoding = value; }
//        }
        
//        /// <summary>
//        /// 提供处理模块的服务地址，初始化一个 <see cref="Aoite.ServiceModel.ContractModule"/> 类的新实例。
//        /// </summary>
//        /// <param name="url">处理模块的服务地址。</param>
//        public ContractModule(Uri url)
//        {
//            if(url == null) throw new ArgumentNullException("url");
//            this._Url = url;
//        }
//    }

//    public class TcpContractModule : ContractModule
//    {
//        private readonly IPEndPoint ServerEndPoint;

//        /// <summary>
//        /// 提供处理模块的服务地址，初始化一个 <see cref="Aoite.ServiceModel.TcpContractModule"/> 类的新实例。
//        /// </summary>
//        /// <param name="url">处理模块的服务地址。</param>
//        public TcpContractModule(Uri url)
//            : base(url)
//        {
//            url.TestTcpScheme();
//            this.ServerEndPoint = new IPEndPoint(ANetExtensions.ParseAddress(url.Host), url.Port);
//        }
//    }
//}
