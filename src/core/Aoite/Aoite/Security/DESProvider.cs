using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Aoite.Security
{
    /// <summary>
    /// 表示 DES 可逆对称加密提供程序。
    /// </summary>
    public class DESProvider : SecurityProviderBase
    {
        /// <summary>
        /// 单例。
        /// </summary>
        public static readonly DESProvider Instance = new DESProvider();

        //默认密钥向量，使用加密方法后，最好不要轻易更改。
        //private static readonly byte[] Keys = { 32, 0xE1, 113, 0x2E, 65, 0xAB, 8, 0xAF };
        private static readonly byte[] Keys = { 32, 0xE1, 113, 0x2E, 55, 0xAB, 8, 0xAF };

        /// <summary>
        /// 初始化一个 <see cref="Aoite.Security.DESProvider"/> 类的新实例。
        /// </summary>
        public DESProvider() { }

        /// <summary>
        /// 给定密钥，解密数据。
        /// </summary>
        /// <param name="data">暗文。</param>
        /// <param name="key">解密的密钥。</param>
        /// <returns>返回解密后的明文。</returns>
        public override string Decrypt(string data, string key)
        {
            if(key == null || key.Length != 8) throw new ArgumentException("解密密钥的长度必须为 8 位！", key);

            if(string.IsNullOrEmpty(data)) return data;

            byte[] rgbKey = this.Encoding.GetBytes(key);
            byte[] rgbIV = Keys;
            byte[] inputByteArray = Convert.FromBase64String(data);
            using(DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider())
            {
                using(MemoryStream mStream = new MemoryStream())
                {
                    using(CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write))
                    {
                        cStream.Write(inputByteArray, 0, inputByteArray.Length);
                        cStream.FlushFinalBlock();
                        return this.Encoding.GetString(mStream.ToArray());
                    }
                }
            }

        }

        /// <summary>
        /// 给定密钥，加密数据。
        /// </summary>
        /// <param name="data">明文。</param>
        /// <param name="key">加密的密钥。</param>
        /// <returns>返回加密后的暗文。</returns>
        public override string Encrypt(string data, string key)
        {
            if(key == null || key.Length != 8) throw new ArgumentException("加密密钥的长度必须为 8 位！", "key");

            if(string.IsNullOrEmpty(data)) return data;
            byte[] rgbKey = this.Encoding.GetBytes(key);
            byte[] rgbIV = Keys;
            byte[] inputByteArray = this.Encoding.GetBytes(data);
            using(DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider())
            {
                using(MemoryStream mStream = new MemoryStream())
                {
                    using(CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write))
                    {
                        cStream.Write(inputByteArray, 0, inputByteArray.Length);
                        cStream.FlushFinalBlock();
                        return Convert.ToBase64String(mStream.ToArray());
                    }
                }
            }

        }
    }
}