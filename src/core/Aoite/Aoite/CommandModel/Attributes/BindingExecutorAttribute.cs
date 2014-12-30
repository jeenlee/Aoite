using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.CommandModel
{
    /// <summary>
    /// 表示一个命令模型具有指定执行器类型的特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class BindingExecutorAttribute : Attribute
    {
        private Type _Type;
        /// <summary>
        /// 获取执行器类型。
        /// </summary>
        public Type Type { get { return _Type; } }

        /// <summary>
        /// 初始化一个 <see cref="Aoite.CommandModel.BindingExecutorAttribute"/> 类的新实例。
        /// </summary>
        /// <param name="type">执行器类型。</param>
        public BindingExecutorAttribute(Type type)
        {
            if(type == null) throw new ArgumentNullException("type");
            this._Type = type;
        }
    }
}
