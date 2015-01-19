using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Logger
{
    /// <summary>
    /// 表示一个基于文本的日志管理器。
    /// </summary>
    public class TextLogger : LoggerBase
    {
        private ITextWriterFactory _TextWriterFactory;
        /// <summary>
        /// 获取或设置文本编写器工厂。
        /// </summary>
        public ITextWriterFactory TextWriterFactory
        {
            get { return this._TextWriterFactory; }
            set
            {
                if(value == null) throw new ArgumentNullException("value");
                this._TextWriterFactory = value;
            }
        }

        private ILogDescriptor _Descriptor;
        /// <summary>
        /// 获取或设置日志描述器。
        /// </summary>
        public ILogDescriptor Descriptor
        {
            get { return this._Descriptor; }
            set
            {
                if(value == null) throw new ArgumentNullException("value");
                this._Descriptor = value;
            }
        }

        /// <summary>
        /// 指定文本编写器工厂和日志描述器，初始化一个 <see cref="Aoite.Logger.TextLogger"/> 类的新实例。
        /// </summary>
        /// <param name="textWriterfactory">文本编写器工厂。</param>
        /// <param name="descriptor">日志描述器。</param>
        public TextLogger(ITextWriterFactory textWriterfactory, ILogDescriptor descriptor)
        {
            if(textWriterfactory == null) throw new ArgumentNullException("textWriterfactory");
            if(descriptor == null) throw new ArgumentNullException("descriptor");

            this._TextWriterFactory = textWriterfactory;
            this._Descriptor = descriptor;
        }

        /// <summary>
        /// 异步写入一个或多个日志项。
        /// </summary>
        /// <param name="items">日志项的数组。</param>
        protected override void OnWrite(params LogItem[] items)
        {
            this._TextWriterFactory.Process(writer =>
            {
                foreach(var item in items)
                {
                    writer.WriteLine(this._Descriptor.Describe(this, item));
                }
                writer.Flush();
            });
        }
    }
}
