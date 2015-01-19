using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Aoite.Data
{
    /// <summary>
    /// 表示一个 SQL 脚本管理工具。
    /// </summary>
    public class SqlScriptsManager : Dictionary<string, string>
    {
        /// <summary>
        /// 初始化 <see cref="Aoite.Data.SqlScriptsManager"/> 类的新实例。
        /// </summary>
        public SqlScriptsManager() : base(StringComparer.OrdinalIgnoreCase) { }

        /// <summary>
        /// 从指定的目录解析所有脚本文件。
        /// </summary>
        /// <param name="folder">要搜索的目录。</param>
        /// <param name="searchPattern">要与 <paramref name="folder"/> 中的文件名匹配的搜索字符串。此参数不能以两个句点（“..”）结束，不能在 <see cref="System.IO.Path.DirectorySeparatorChar"/> 或 <see cref="System.IO.Path.AltDirectorySeparatorChar"/> 的前面包含两个句点（“..”），也不能包含 <see cref="System.IO.Path.InvalidPathChars"/> 中的任何字符。</param>
        public void ParseFolder(string folder, string searchPattern = "*.sql")
        {
            if(Directory.Exists(folder))
            {
                foreach(var script in Directory.GetFiles(folder, searchPattern))
                {
                    this.ParsePath(script);
                }
            }
        }

        /// <summary>
        /// 从指定的 SQL 文件解析脚本。
        /// </summary>
        /// <param name="path">SQL 文件路径。</param>
        public void ParsePath(string path)
        {
            var lines = File.ReadAllLines(path);
            this.Parse(lines);
        }

        /// <summary>
        /// 从指定的 SQL 文本解析脚本。
        /// </summary>
        /// <param name="content">SQL 文本。</param>
        public void ParseContent(string content)
        {
            this.Parse(content.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries));
        }

        private void Parse(string[] lines)
        {
            string key = null;
            StringBuilder script = new StringBuilder();
            foreach(var line in lines)
            {
                if(line.StartsWith("/*[") && line.EndsWith("]*/"))
                {
                    if(key != null)
                    {
                        this.Add(key, script.ToString());
                        script.Clear();
                    }
                    key = line.RemoveStarts(3).RemoveEnds(3).Trim();
                }
                else script.AppendLine(line);
            }

            if(key != null) this.Add(key, script.ToString());
        }

        /// <summary>
        /// 获取指定键名的脚本。
        /// </summary>
        /// <typeparam name="TKey">键的数据类型。</typeparam>
        /// <param name="key">脚本的键名。</param>
        /// <returns>返回脚本。</returns>
        public string GetScript<TKey>(TKey key)
        {
            return this.TryGetValue(Convert.ToString(key));
        }

        /// <summary>
        /// 获取指定键名的脚本。
        /// </summary>
        /// <param name="key">脚本的键名。</param>
        /// <returns>返回脚本。</returns>
        public string GetScript(string key)
        {
            return this.TryGetValue(key);
        }
    }
}
