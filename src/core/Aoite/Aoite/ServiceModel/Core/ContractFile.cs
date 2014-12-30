using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Aoite.ServiceModel
{
    /// <summary>
    /// 
    /// </summary>
    public interface IContractFile
    {
        /// <summary>
        /// 获取不具有扩展名的指定路径字符串的文件名。
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 获取文件的扩展名。
        /// </summary>
        string Extension { get; }
        /// <summary>
        /// 获取文件的大小。
        /// </summary>
        int FileSize { get; }
        /// <summary>
        /// 将上传的文件保存到指定的路径。
        /// </summary>
        /// <param name="path">指定的路径。</param>
        void SaveAs(string path);
    }

    /// <summary>
    /// 表示一个契约的上传文件。
    /// </summary>
    public class ContractFile
    {
        private string _Name;
        /// <summary>
        /// 获取不具有扩展名的指定路径字符串的文件名。
        /// </summary>
        public string Name { get { return this._Name; } }

        private string _Extension;
        /// <summary>
        /// 获取文件的扩展名。
        /// </summary>
        public string Extension { get { return this._Extension; } }
        /// <summary>
        /// 获取完整的文件名。
        /// </summary>
        public string FullName { get { return this._Name + this._Extension; } }

        private int _FileSize;
        /// <summary>
        /// 获取文件的大小。
        /// </summary>
        public int FileSize { get { return this._FileSize; } }

        internal readonly byte[] _FileBytes;
        [Ignore]
        private Stream _Content;
        /// <summary>
        /// 获取文件的内容。
        /// </summary>
        public Stream Content
        {
            get
            {
                if(this._Content == null && this._FileBytes != null) this._Content = new MemoryStream(this._FileBytes);
                return this._Content;
            }
        }

        internal ContractFile(string fileName)
        {
            this._Name = fileName;
        }

        /// <summary>
        /// 初始化一个 <see cref="Aoite.ServiceModel.ContractFile"/> 类的新实例。
        /// </summary>
        /// <param name="fileName">文件的完全限定名。</param>
        /// <param name="fileSize">文件的大小（以字节为单位）。</param>
        /// <param name="content">一个 <see cref="System.IO.Stream"/> 对象，该对象指向一个准备上传的内容。</param>
        public ContractFile(string fileName, int fileSize, Stream content)
            : this(fileName, fileSize, content, true) { }

        internal ContractFile(string fileName, int fileSize, Stream content, bool useBytes)
        {
            if(string.IsNullOrEmpty(fileName)) throw new ArgumentNullException("fileName");
            if(fileSize < 1) throw new ArgumentOutOfRangeException("fileSize");
            if(content == null) throw new ArgumentNullException("content");

            this._Name = Path.GetFileNameWithoutExtension(fileName);
            this._Extension = Path.GetExtension(fileName);
            this._FileSize = fileSize;
            this._Content = content;
            if(useBytes)
            {
                MemoryStream ms = new MemoryStream(fileSize);
                content.CopyTo(ms);
                this._FileBytes = ms.ToArray();
                this._Content = ms;
            }
        }

        /// <summary>
        /// 将上传的文件保存到指定的路径。
        /// </summary>
        /// <param name="path">指定的路径。</param>
        public void SaveAs(string path)
        {
            if(this._Content != null)
            {
                using(var s = File.Create(path))
                {
                    this._Content.CopyTo(s);
                }
            }
            else if(this._FileBytes != null)  File.WriteAllBytes(path, this._FileBytes);
            else throw new NotSupportedException();
        }
    }
}
