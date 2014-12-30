using System;
using System.Security.Cryptography;
using System.Text;

namespace Aoite.Security
{
    /*
     * “非对称加密”结合“对称加密”，是一种安全系数较高的加密方式。采用“非对称加密”方式来加密“对称加密”的密钥。
     * 因此“非对称加密”，最长明文长度为 117 位。
     */
    /// <summary>
    /// RSA 公钥私钥加解密提供程序( 非对称加密 )。
    /// <para>说明：提供“密钥加密”&lt; - &gt;“私钥解密”或“私钥加密”&lt; - &gt;“密钥解密”的服务。</para>
    /// </summary>
    public class RSAProvider : SecurityProviderBase
    {
        /// <summary>
        /// 单例。
        /// </summary>
        public static readonly RSAProvider Instance = new RSAProvider();

        /// <summary>
        /// 初始化一个 <see cref="Aoite.Security.RSAProvider"/> 类的新实例。
        /// </summary>
        public RSAProvider() { }

        /// <summary>
        /// 创建密钥(公钥+私钥)以及公钥。
        /// </summary>
        /// <param name="privateKey">密钥(公钥+私钥)。</param>
        /// <param name="publicKey">公钥。</param>
        public static void CreateKey(out string privateKey, out string publicKey)
        {
            using(RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                privateKey = rsa.ToXmlString(true);
                publicKey = rsa.ToXmlString(false);
            }
        }

        /// <summary>
        /// 指定暗文以及私钥进行数据解密。
        /// </summary>
        /// <param name="data">暗文。</param>
        /// <param name="privateKey">私钥。</param>
        /// <returns>返回解密后的明文。</returns>
        public override string Decrypt(string data, string privateKey)
        {
            using(RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(privateKey);
                return Encoding.GetString(rsa.Decrypt(Convert.FromBase64String(data), false));
            }
        }

        /// <summary>
        /// 指定明文以及公钥进行数据加密。
        /// </summary>
        /// <param name="data">明文。</param>
        /// <param name="publicKey">公钥。</param>
        /// <returns>返回加密后的暗文。</returns>
        public override string Encrypt(string data, string publicKey)
        {
            if(data.Length > 117) throw new ArgumentOutOfRangeException("data", "RSA 算法加密长度最多不可超过 117 位。");

            using(RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(publicKey);

                byte[] bytResult = rsa.Encrypt(Encoding.GetBytes(data), false);

                return Convert.ToBase64String(bytResult);
            }
        }
    }
}
