using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// 定义一个动态的映射器。
    /// </summary>
    public interface IDynamicMapper
    {
        /// <summary>
        /// 获取或设置映射器的名称。
        /// </summary>
        string Name { get; set; }
    }
}
