using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.ServiceModel
{
    /// <summary>
    /// 表示一个契约筛选器的特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public abstract class ContractFilterAttribute : Attribute
    {
        internal void Calling(ContractContext context) { this.OnCalling(context); }
        internal void Called(ContractContext context) { this.OnCalled(context); }
        /// <summary>
        /// 契约方法调用前发生。
        /// </summary>
        /// <param name="context">契约的上下文信息。</param>
        protected abstract void OnCalling(ContractContext context);
        /// <summary>
        /// 契约方法调用后发生。
        /// </summary>
        /// <param name="context">契约的上下文信息。</param>
        protected abstract void OnCalled(ContractContext context);

        /// <summary>
        /// 契约方法在相同范围内的调用顺序。
        /// </summary>
        public int Order { get; set; }
    }
}
