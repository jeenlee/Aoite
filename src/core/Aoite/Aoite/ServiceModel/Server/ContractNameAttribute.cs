using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.ServiceModel
{
    /// <summary>
    /// 表示自定义契约名称的特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ContractNameAttribute : Attribute
    {
        /// <summary>
        /// 初始化一个 <see cref="Aoite.ServiceModel.ContractNameAttribute"/> 类的新实例。
        /// </summary>
        public ContractNameAttribute() { }
        /// <summary>
        /// 提供自定义契约名称，初始化一个 <see cref="Aoite.ServiceModel.ContractNameAttribute"/> 类的新实例。
        /// </summary>
        /// <param name="name">契约名称。</param>
        public ContractNameAttribute(string name)
        {
            this._Name = name;
        }

        private string _Name;
        /// <summary>
        /// 获取或设置契约名称。
        /// </summary>
        public string Name { get { return this._Name; } set { this._Name = value; } }
    }
}
