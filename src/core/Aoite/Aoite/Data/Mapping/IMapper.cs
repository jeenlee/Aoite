using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Data
{
    /// <summary>
    /// 定义一个映射器。
    /// </summary>
    public interface IMapper
    {
        /// <summary>
        /// 获取或设置映射器的名称。
        /// </summary>
        string Name { get; set; }
    }
}
