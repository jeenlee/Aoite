using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Aoite.Logger
{
    /// <summary>
    /// 表示一个基于流的文本编写器工厂。
    /// </summary>
    public class StreamTextWriterFactory : ITextWriterFactory
    {
        private string _LogFolder;
        /// <summary>
        /// 获取或设置日志的文件目录。
        /// </summary>
        public string LogFolder
        {
            get { return this._LogFolder; }
            set
            {
                if(string.IsNullOrEmpty(value)) throw new ArgumentNullException("value");
                this._LogFolder = value;
            }
        }

        private string _LogExtension;
        /// <summary>
        /// 获取或设置日志的文件后缀。
        /// </summary>
        public string LogExtension
        {
            get { return this._LogExtension; }
            set
            {
                if(string.IsNullOrEmpty(value)) throw new ArgumentNullException("value");

                if(value[0] != '.') this._LogExtension = "." + value;
                else this._LogExtension = value;
            }
        }

        private Encoding _Encoding;
        /// <summary>
        /// 获取或设置日志的编码格式。
        /// </summary>
        public Encoding Encoding
        {
            get { return this._Encoding; }
            set
            {
                if(value == null) throw new ArgumentNullException("value");
                this._Encoding = value;
            }
        }

        /// <summary>
        /// 获取或设置日志的路径生成工厂。
        /// </summary>
        public ILogPathFactory LogPathFactory { get; private set; }

        /// <summary>
        /// 指定日志的路径生成工厂，初始化一个 <see cref="Aoite.Logger.StreamTextWriterFactory"/> 类的新实例。
        /// </summary>
        /// <param name="logPathFactory">日志的路径生成工厂。</param>
        public StreamTextWriterFactory(ILogPathFactory logPathFactory)
        {
            if(logPathFactory == null) throw new ArgumentNullException("logPathFactory");
            this.LogPathFactory = logPathFactory;

            this.LogFolder = GA.FullPath("Logs");
            this.LogExtension = ".log";
            this.Encoding = Encoding.UTF8;
        }

        internal StreamWriter _lastWriter;
        internal string _lastPath;
        internal Func<DateTime> NowGetter = () => DateTime.Now;
        private StreamWriter CreateWriter()
        {
            var now = this.NowGetter();
            if(this._lastWriter == null || !this.LogPathFactory.IsCreated(now))
            {
                this._lastPath = this.LogPathFactory.CreatePath(now, this.LogFolder, this.LogExtension);
                this._lastWriter = new StreamWriter(new FileStream(this._lastPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite | FileShare.Delete), this.Encoding);
            }
            return this._lastWriter;
        }

        /// <summary>
        /// 生成流的编写器，处理指定的回调方法。
        /// </summary>
        /// <param name="callback">回调方法。</param>
        public void Process(Action<TextWriter> callback)
        {
            if(callback == null) throw new ArgumentNullException("callback");
            var writer = this.CreateWriter();
            callback(writer);
            writer.Flush();
        }

        /// <summary>
        /// 析构函数。
        /// </summary>
        ~StreamTextWriterFactory()
        {
            if(this._lastWriter != null)
            {
                try
                {
                    this._lastWriter.Dispose();
                }
                catch(Exception) { }
                this._lastWriter = null;
            }
        }

    }
}
