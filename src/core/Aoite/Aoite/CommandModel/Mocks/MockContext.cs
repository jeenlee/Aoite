using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.CommandModel
{
    /// <summary>
    /// 表示一个模拟执行的命令模型的上下文。
    /// </summary>
    public class MockContext : Context
    {
        private dynamic _User;
        /// <summary>
        /// 获取执行命令模型的用户。该属性可能返回 null 值。
        /// </summary>
        public override dynamic User { get { return this._User; } }

        /// <summary>
        /// 初始化 <see cref="Aoite.CommandModel.Context"/> 类的新实例。
        /// </summary>
        /// <param name="user">模拟的用户。</param>
        /// <param name="command">命令模型。</param>
        public MockContext(object user, ICommand command)
            : base(ObjectFactory.Global, command)
        {
            this._User = user;
        }
    }
}
