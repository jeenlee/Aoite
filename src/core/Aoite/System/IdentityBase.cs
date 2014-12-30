using Aoite.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// 表示唯一实体的基类。
    /// </summary>
    [Serializable]
    public abstract class IdentityBase
    {
        /// <summary>
        /// 获取或设置唯一编号。
        /// </summary>
        [Column(true)]
        public Guid Id { get; set; }
    }
}
