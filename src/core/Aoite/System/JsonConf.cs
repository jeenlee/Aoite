using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;

namespace System
{
    /// <summary>
    /// 配置帮助器。
    /// </summary>
    public static class JsonConf
    {
        readonly static JavaScriptSerializer DefaultSerializer = new JavaScriptSerializer();
        readonly static Regex RemoveCommentRegex = new Regex(@"^\s*#(.*)", RegexOptions.Multiline | RegexOptions.Compiled);
        readonly static Regex RemoveExcessComma = new Regex(@",(?=\s*[\}\]])", RegexOptions.Multiline | RegexOptions.Compiled);
        /// <summary>
        /// 提供配置内容，创建一个 <typeparamref name="T"/> 类的新实例。
        /// </summary>
        /// <param name="conf">配置内容。</param>
        /// <returns>返回一个 <typeparamref name="T"/> 类的新实例。</returns>
        public static T LoadFromConf<T>(string conf)
        {
            conf = RemoveCommentRegex.Replace(conf, string.Empty).Trim();
            if(conf[0] != '{' && conf[0] != '[') conf = "{" + conf + "}";
            conf = RemoveExcessComma.Replace(conf, string.Empty);
            return DefaultSerializer.Deserialize<T>(conf);
        }

        /// <summary>
        /// 提供配置路径，创建一个 <typeparamref name="T"/> 类的新实例。
        /// </summary>
        /// <param name="path">配置路径。</param>
        /// <returns>返回一个 <typeparamref name="T"/> 类的新实例。</returns>
        public static T LoadFromFile<T>(string path)
        {
            return LoadFromConf<T>(File.ReadAllText(path, Encoding.UTF8));
        }
    }
}
