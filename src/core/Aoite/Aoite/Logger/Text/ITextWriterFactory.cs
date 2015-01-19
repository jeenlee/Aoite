using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Aoite.Logger
{
    /// <summary>
    /// 定义一个文本编写器工厂。
    /// </summary>
    [DefaultMapping(typeof(CustomTextWriterFactory))]
    public interface ITextWriterFactory
    {
        /// <summary>
        /// 生成流的编写器，处理指定的回调方法。
        /// </summary>
        /// <param name="callback">回调方法。</param>
        void Process(Action<TextWriter> callback);
    }
    
}
