using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Data
{
    /// <summary>
    /// 定义一个映射器的特性。
    /// </summary>
    public interface IMapperAttribute
    {
        /// <summary>
        /// 初始化映射器。
        /// </summary>
        /// <param name="mapper">映射器。</param>
        void Initialization(IMapper mapper);
    }
}
