using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

namespace System
{
    /// <summary>
    /// 表示数据安全处理。
    /// </summary>
    public static class DataSecurity
    {
        /// <summary>
        /// 指定加密算法，加密指定的文本。
        /// </summary>
        /// <param name="alog">加密算法。</param>
        /// <param name="text">需加密的字符串。</param>
        /// <param name="encoding">编码方式。</param>
        /// <returns>返回加密后的字符串。</returns>
        public static string Crypto(SecurityAlgorithms alog, string text, Encoding encoding = null)
        {
            HashAlgorithm algorithm;
            switch(alog)
            {
                case SecurityAlgorithms.SHA1:
                    algorithm = CreateSHA1();
                    break;
                case SecurityAlgorithms.SHA256:
                    algorithm = CreateSHA256();
                    break;
                case SecurityAlgorithms.SHA384:
                    algorithm = CreateSHA384();
                    break;
                case SecurityAlgorithms.SHA512:
                    algorithm = CreateSHA512();
                    break;
                case SecurityAlgorithms.MD5:
                    algorithm = CreateMD5();
                    break;
                default: throw new NotSupportedException();
            }
            using(algorithm)
            {
                return algorithm.ComputeHash((encoding ?? Encoding.UTF8).GetBytes(text)).ToHexString();
            }
        }

        /// <summary>
        /// 生产成指定字符串，生成 32位加盐值，并返回 44 位加盐散列后的文本。
        /// </summary>
        /// <param name="text">原始文本。</param>
        /// <param name="salt">加盐值。</param>
        /// <param name="encoding">编码方式。</param>
        /// <returns>返回 44 位加盐散列后的文本。</returns>
        public static string GenerateSaltedHash(string text, out Guid salt, Encoding encoding = null)
        {
            return GenerateSaltedHash(text, salt = Guid.NewGuid(), encoding);
        }

        /// <summary>
        /// 生产成指定字符串和加盐值，并返回 44 位加盐散列后的文本。
        /// </summary>
        /// <param name="text">原始文本。</param>
        /// <param name="salt">加盐值。</param>
        /// <param name="encoding">编码方式。</param>
        /// <returns>返回 44 位加盐散列后的文本。</returns>
        public static string GenerateSaltedHash(string text, Guid salt, Encoding encoding = null)
        {
            if(encoding == null) encoding = Encoding.UTF8;
            return Convert.ToBase64String(GenerateSaltedHash(encoding.GetBytes(text), encoding.GetBytes(salt.ToString("N"))));
        }

        static byte[] GenerateSaltedHash(byte[] plainText, byte[] salt)
        {
            HashAlgorithm algorithm = new SHA256Managed();

            var plainTextWithSaltBytes = new byte[plainText.Length + salt.Length];

            for(int i = 0; i < plainText.Length; i++)
            {
                plainTextWithSaltBytes[i] = plainText[i];
            }
            for(int i = 0; i < salt.Length; i++)
            {
                plainTextWithSaltBytes[plainText.Length + i] = salt[i];
            }

            return algorithm.ComputeHash(plainTextWithSaltBytes);
        }

        internal static SHA1 CreateSHA1()
        {
            return SHA1.Create();
        }

        internal static SHA256 CreateSHA256()
        {
            return SHA256.Create();
        }

        internal static SHA384 CreateSHA384()
        {
            return SHA384.Create();
        }

        internal static SHA512 CreateSHA512()
        {
            return SHA512.Create();
        }

        internal static MD5 CreateMD5()
        {
            return MD5CryptoServiceProvider.Create();
        }
    }

    /// <summary>
    /// 表示安全加密算法。
    /// </summary>
    public enum SecurityAlgorithms
    {
        /// <summary>
        /// 使用 <see cref="System.Security.Cryptography.SHA1"/> 哈希函数计算基于哈希值的消息验证代码 (HMAC)。
        /// </summary>
        SHA1,
        /// <summary>
        /// 使用 <see cref="System.Security.Cryptography.SHA256"/> 哈希函数计算基于哈希值的消息验证代码 (HMAC)。
        /// </summary>
        SHA256,
        /// <summary>
        /// 使用 <see cref="System.Security.Cryptography.SHA384"/> 哈希函数计算基于哈希值的消息验证代码 (HMAC)。
        /// </summary>
        SHA384,
        /// <summary>
        /// 使用 <see cref="System.Security.Cryptography.SHA512"/> 哈希函数计算基于哈希值的消息验证代码 (HMAC)。
        /// </summary>
        SHA512,
        /// <summary>
        ///  提供 MD5（消息摘要 5）128 位哈希算法的实现。
        /// </summary>
        MD5
    }
}
