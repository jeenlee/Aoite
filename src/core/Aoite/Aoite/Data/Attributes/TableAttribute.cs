using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Data
{
    /// <summary>
    /// 表示一张表的特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class TableAttribute : AliasAttribute
    {
        /// <summary>
        /// 初始化一个 <see cref="Aoite.Data.TableAttribute"/> 类的新实例。
        /// </summary>
        public TableAttribute() { }

        /// <summary>
        /// 指定名称，初始化一个 <see cref="Aoite.Data.TableAttribute"/> 类的新实例。
        /// </summary>
        /// <param name="name">名称。</param>
        public TableAttribute(string name) : base(name) { }
    }
}
