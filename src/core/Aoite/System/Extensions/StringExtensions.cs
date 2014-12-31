using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// 提供用于字符串值的实用工具方法。
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// 返回当前字符串的 MD5 加密后的字符串。
        /// </summary>
        /// <param name="text">需加密的字符串。</param>
        /// <param name="encoding">编码方式。</param>
        /// <returns>返回加密后的字符串。</returns>
        public static string ToMd5(this string text, Encoding encoding = null)
        {
            return DataSecurity.Crypto(SecurityAlgorithms.MD5, text, encoding);
        }

        /// <summary>
        /// 将当前字符串转换为智能小写模式。
        /// </summary>
        /// <param name="s">当前字符串。</param>
        /// <returns>返回一个新的字符串。</returns>
        public static string ToCamelCase(this string s)
        {
            if(string.IsNullOrEmpty(s)) return s;

            if(!char.IsUpper(s[0])) return s;

            string camelCase = char.ToLower(s[0], CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture);
            if(s.Length > 1) camelCase += s.Substring(1);
            return camelCase;
        }
        /// <summary>
        /// 将当前字符串转换为 UTF8 的字节组。
        /// </summary>
        /// <param name="value">当前字符串。</param>
        /// <returns>返回一个字节组。</returns>
        public static byte[] ToUtf8Bytes(this string value)
        {
            return Encoding.UTF8.GetBytes(value);
        }

        /// <summary>
        /// 忽略被比较字符串的大小写，确定两个指定的 <see cref="System.String"/> 实例是否具有同一值。
        /// </summary>
        /// <param name="a"><see cref="System.String"/>第一个 <see cref="System.String"/> 的实例。</param>
        /// <param name="b"><see cref="System.String"/>第二个 <see cref="System.String"/> 的实例。</param>
        /// <returns>如果 <paramref name="a"/> 参数的值等于 <paramref name="b"/> 参数的值，则为 true；否则为 false。</returns>
        public static bool iEquals(this string a, string b)
        {
            return string.Equals(a, b, StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// 忽略被比较字符串的大小写，确定在使用指定的比较选项进行比较时此字符串实例的开头是否与指定的字符串匹配。
        /// </summary>
        /// <param name="a"><see cref="System.String"/>第一个 <see cref="System.String"/> 的实例。</param>
        /// <param name="b"><see cref="System.String"/>第二个 <see cref="System.String"/> 的实例。</param>
        /// <returns>如果 <paramref name="b"/> 参数与此字符串的开头匹配，则为 true；否则为 false。 </returns>
        public static bool iStartsWith(this string a, string b)
        {
            if(a == null || b == null) return false;
            return a.StartsWith(b, StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// 忽略被比较字符串的大小写，确定使用指定的比较选项进行比较时此字符串实例的结尾是否与指定的字符串匹配。
        /// </summary>
        /// <param name="a"><see cref="System.String"/>第一个 <see cref="System.String"/> 的实例。</param>
        /// <param name="b"><see cref="System.String"/>第二个 <see cref="System.String"/> 的实例。</param>
        /// <returns>如果 <paramref name="b"/> 参数与此字符串的结尾匹配，则为 true；否则为 false。 </returns>
        public static bool iEndsWith(this string a, string b)
        {
            if(a == null || b == null) return false;
            return a.EndsWith(b, StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// 忽略被比较字符串的大小写，返回一个值，该值指示指定的 <see cref="System.String"/> 对象是否出现在此字符串中。
        /// </summary>
        /// <param name="a"><see cref="System.String"/>第一个 <see cref="System.String"/> 的实例。</param>
        /// <param name="b"><see cref="System.String"/>第二个 <see cref="System.String"/> 的实例。</param>
        /// <returns>如果 <paramref name="b"/> 参数出现在此字符串中，或者 <paramref name="b"/> 为空字符串 ("")，则为 true；否则为 false。 </returns>
        public static bool iContains(this string a, string b)
        {
            if(a == null || b == null) return false;
            return a.ToLower().Contains(b.ToLower());
        }

        /// <summary>
        /// 在当前字符串的前后增加“%”符号。
        /// </summary>
        /// <param name="input">当前字符串。</param>
        /// <returns>返回一个新的字符串。</returns>
        public static string ToLiking(this string input)
        {
            return string.Concat("%", input, "%");
        }

        /// <summary>
        /// 将指定字符串中的格式项替换为指定数组中相应对象的字符串表示形式。
        /// </summary>
        /// <param name="format">复合格式字符串。</param>
        /// <param name="args">一个对象数组，其中包含零个或多个要设置格式的对象。</param>
        /// <returns><paramref name="format"/> 的副本，其中的格式项已替换为 <paramref name="args"/> 中相应对象的字符串表示形式。</returns>
        public static string Fmt(this string format, params object[] args)
        {
            return string.Format(format, args);
        }

        /// <summary>
        /// 返回表示当前 <see cref="System.String"/>，如果 <paramref name="input"/> 是一个 null 值，将返回 <see cref="System.String.Empty"/>。
        /// </summary>
        /// <param name="input">一个字符串。</param>
        /// <returns>返回 <paramref name="input"/> 的 <see cref="System.String"/> 或 <see cref="System.String.Empty"/>。</returns>
        public static string ToStringOrEmpty(this string input)
        {
            return input ?? string.Empty;
        }

        /// <summary>
        /// 判定当前字符串是否是一个空的字符串。
        /// </summary>
        /// <param name="input">当前字符串。</param>
        /// <returns>如果字符串为 null、空 或 空白，将返回 true，否则返回 false。</returns>
        public static bool IsNull(this string input)
        {
            return string.IsNullOrWhiteSpace(input);
        }

        /// <summary>
        /// 将指定的字节数组转换成十六进制的字符串。
        /// </summary>
        /// <param name="source">一个字节数组。</param>
        /// <returns>由字节数组转换后的十六进制的字符串。</returns>
        public static string ToHexString(this byte[] source)
        {
            return BitConverter.ToString(source).Replace("-", string.Empty);
        }

        /// <summary>
        /// 指定整串字符串的最大长度，剪裁字符串数据，超出部分将会在结尾添加“...”。
        /// </summary>
        /// <param name="input">一个字符串。</param>
        /// <param name="maxLength">字符串的最大长度（含）。</param>
        /// <param name="ellipsis">指定省略号的字符串，默认为“...”。</param>
        /// <returns>返回一个新的字符串 -或- 原字符串，该字符串的最大长度不超过 <paramref name="maxLength"/>。</returns>
        public static string CutString(this string input, int maxLength, string ellipsis = "...")
        {
            if(input == null || input.Length <= maxLength) return input;
            maxLength = maxLength - ellipsis.Length;
            return input.Substring(0, maxLength) + ellipsis;
        }

        /// <summary>
        /// 获取字符串开头的内容。
        /// </summary>
        /// <param name="input">一个字符串。</param>
        /// <param name="length">获取的字符串长度。</param>
        /// <returns>返回一个新的字符串。</returns>
        public static string Starts(this string input, int length)
        {
            if(string.IsNullOrEmpty(input)) return string.Empty;
            return length >= input.Length ? input : input.Substring(0, length);
        }

        /// <summary>
        /// 获取字符串结尾的内容。
        /// </summary>
        /// <param name="input">一个字符串。</param>
        /// <param name="length">获取的字符串长度。</param>
        /// <returns>返回一个新的字符串。</returns>
        public static string Ends(this string input, int length)
        {
            if(string.IsNullOrEmpty(input)) return string.Empty;
            return length >= input.Length ? input : input.Substring(input.Length - length);
        }

        /// <summary>
        /// 删除当前字符串的开头的字符串。
        /// </summary>
        /// <param name="val">目标字符串。</param>
        /// <param name="count">要删除的字长度。</param>
        /// <returns>返回删除后的字符串。</returns>
        public static string RemoveStart(this string val, int count = 1)
        {
            if(string.IsNullOrEmpty(val) || val.Length < count) return val;
            return val.Remove(0, count);
        }

        /// <summary>
        /// 删除当前字符串的结尾的字符串。
        /// </summary>
        /// <param name="val">目标字符串。</param>
        /// <param name="count">要删除的字长度。</param>
        /// <returns>返回删除后的字符串。</returns>
        public static string RemoveEnd(this string val, int count = 1)
        {
            if(string.IsNullOrEmpty(val) || val.Length < count) return val;
            return val.Remove(val.Length - count);
        }
    }
}
