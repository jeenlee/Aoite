using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Aoite.Logger
{
    /// <summary>
    /// 表示一个基于可自定义写入流的文本编写器工厂。
    /// </summary>
    public class CustomTextWriterFactory : ITextWriterFactory
    {
        private Func<TextWriter> _CreateWriter;
        /// <summary>
        /// 获取或设置自定义的编写器创建方法。
        /// </summary>
        public Func<TextWriter> CreateWriter
        {
            get { return this._CreateWriter; }
            set
            {
                if(value == null) throw new ArgumentNullException("value");
                this._CreateWriter = value;
            }
        }

        static TextWriter DefaultCreateWriter() { return Console.Out; }

        /// <summary>
        /// 初始化一个 <see cref="Aoite.Logger.CustomTextWriterFactory"/> 类的新实例。
        /// </summary>
        public CustomTextWriterFactory()
        {
            this._CreateWriter = DefaultCreateWriter;
        }

        /// <summary>
        /// 生成流的编写器，处理指定的回调方法。
        /// </summary>
        /// <param name="callback">回调方法。</param>
        public void Process(Action<TextWriter> callback)
        {
            if(callback == null) throw new ArgumentNullException("callback");
            callback(this._CreateWriter());
        }
    }
}
