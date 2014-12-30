using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Aoite.CommandModel
{
    /// <summary>
    /// 表示一个执行命令模型的上下文。
    /// </summary>
    public class Context : CommandModelContainerProviderBase, IContext
    {
        /// <summary>
        /// 获取执行命令模型的用户。该属性可能返回 null 值。
        /// </summary>
        public virtual dynamic User
        {
            get
            {
                if(this.Container.ContainsService<IUserFactory>())
                    return this.Container.GetService<IUserFactory>().GetUser(this.Container);
                return null;
            }
        }

        private HybridDictionary _Data;
        /// <summary>
        /// 获取执行命令模型的其他参数，参数名称若为字符串则不区分大小写的序号字符串比较。
        /// </summary>
        public virtual HybridDictionary Data { get { return _Data ?? (_Data = new HybridDictionary(true)); } }

        private ICommand _Command;
        /// <summary>
        /// 获取正在执行的命令模型。
        /// </summary>
        public ICommand Command { get { return _Command; } }

        /// <summary>
        /// 初始化 <see cref="Aoite.CommandModel.Context"/> 类的新实例。
        /// </summary>
        /// <param name="container">服务容器。</param>
        /// <param name="command">命令模型。</param>
        public Context(IIocContainer container, ICommand command)
            : base(container)
        {
            if(command == null) throw new ArgumentNullException("command");
            this._Command = command;
        }


        /// <summary>
        /// 获取或设置键的值。
        /// </summary>
        /// <param name="key">键。</param>
        /// <returns>返回一个值。</returns>
        public object this[object key]
        {
            get
            {
                return Data[key];
            }
            set
            {
                Data[key] = value;
            }
        }
    }
}
