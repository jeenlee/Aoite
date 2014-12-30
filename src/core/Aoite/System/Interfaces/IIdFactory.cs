using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// 唯一键生成工厂。
    /// </summary>
    public interface IIdFactory
    {
        /// <summary>
        /// 提供一个键名，生成一个唯一的 64 位键值。
        /// </summary>
        /// <param name="key">键名。</param>
        /// <returns>返回一个 64 位值。</returns>
        long New(string key);
    }
}
