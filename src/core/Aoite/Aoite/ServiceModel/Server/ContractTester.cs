using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.ServiceModel
{
    /// <summary>
    /// 表示一个契约的测试用例。
    /// </summary>
    public class ContractTester : IDisposable
    {
        ContractDomain _Domain;
        ContractServer _Server;
        /// <summary>
        /// 获取正在测试的服务端。
        /// </summary>
        public ContractServer Server { get { return this._Server; } }
        /// <summary>
        /// 获取正在测试的客户端。
        /// </summary>
        public ContractDomain Domain { get { return this._Domain; } }

        /// <summary>
        /// 初始化一个 <see cref="Aoite.ServiceModel.ContractTester"/> 类的新实例。
        /// </summary>
        /// <param name="serverAccessor">实现服务创建的函数。</param>
        /// <param name="allowAnonymous">指示默认情况下是否允许匿名访问。</param>
        /// <param name="namespaceExpression">服务的命名空间表达式。可以是一个完整的命名空间，也可以是“*”起始，或者以“*”结尾。符号“*”只能出现一次。通过“|”可以同时包含多个命名空间。</param>
        public ContractTester(Func<ServerConfiguration, ContractServer> serverAccessor,
            bool allowAnonymous, string namespaceExpression)
        {
            ServerConfiguration config = new ServerConfiguration();
            config.Host = "localhost";
            config.Port = 0;
            config.NamespaceExpression = namespaceExpression;
            config.ReturnInternalErrors = true;

            this._Server = serverAccessor(config);
            if(allowAnonymous)
            {
                this._Server.Calling += (ss, ee) =>
                {
                    ContractContext.Current.IsAuthenticated = true;
                };
            }
            var r = this._Server.Open();
            if(r.IsFailed) throw r.Exception;

            this._Domain = new ContractDomain()
            {
                Host = config.Host,
                Port = this._Server.Port,
                ResponseTimeout = -1
            };
        }
        /// <summary>
        /// 初始化一个 <see cref="Aoite.ServiceModel.ContractTester"/> 类的新实例。
        /// </summary>
        /// <param name="allowAnonymous">指示默认情况下是否允许匿名访问。</param>
        /// <param name="namespaceExpression">服务的命名空间表达式。可以是一个完整的命名空间，也可以是“*”起始，或者以“*”结尾。符号“*”只能出现一次。通过“|”可以同时包含多个命名空间。</param>
        public ContractTester(bool allowAnonymous, string namespaceExpression)
            : this((config) => new ContractServer(config), allowAnonymous, namespaceExpression) { }

        /// <summary>
        /// 创建指定契约类型的代理实例。
        /// </summary>
        /// <typeparam name="TContract">契约的数据类型。</typeparam>
        /// <param name="keepAlive">指示是否为一个持久连接的契约客户端。</param>
        /// <returns>返回一个代理实例。</returns>
        public TContract Create<TContract>(bool keepAlive = false)
        {
            return this._Domain.Create<TContract>(keepAlive);
        }

        void IDisposable.Dispose()
        {
            if(this._Server == null) return;
            this._Server.Close();
            this._Server = null;
        }
    }
}
