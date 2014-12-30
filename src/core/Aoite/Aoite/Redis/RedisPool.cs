using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Redis
{
    /// <summary>
    /// 表示一个 Redis 池。
    /// </summary>
    public class RedisPool : ObjectPool<RedisClient>
    {
        private bool _HasPassword;
        private string _Password;
        /// <summary>
        /// 获取授权密码。
        /// </summary>
        public string Password { get { return _Password; } }

        private string _Host;
        /// <summary>
        /// 获取主机。
        /// </summary>
        public string Host { get { return _Host; } }

        private int _Port;
        /// <summary>
        /// 获取端口。
        /// </summary>
        public int Port { get { return _Port; } }

        /// <summary>
        /// 初始化一个 <see cref="Aoite.Redis.RedisPool"/> 类的新实例。
        /// </summary>
        /// <param name="host">主机</param>
        /// <param name="port">端口。</param>
        /// <param name="password">授权密码。</param>
        public RedisPool(string host, int port = RedisClient.DefaultPort, string password = null)
        {
            if(string.IsNullOrEmpty(host)) throw new ArgumentNullException("host");
            if(port < 1 || port > 65535) throw new ArgumentOutOfRangeException("port");
            this._Host = host;
            this._Port = port;
            this._Password = password;
            this._HasPassword = !string.IsNullOrEmpty(password);
        }

        /// <summary>
        /// 获取一个对象池的对象。
        /// </summary>
        /// <returns>返回一个已释放或新的对象。</returns>
        public override RedisClient Acquire()
        {
            var client = base.Acquire();
            if(!client.Connected)
            {
                if(!client.Connect(3000)) throw new TimeoutException("连接 Redis 服务器超时。");
                if(this._HasPassword) client.Auth(this._Password).ThrowIfFailded();
            }
            return client;
        }

        /// <summary>
        /// 创建对象时发生。 
        /// </summary>
        /// <returns>返回一个新的对象。</returns>
        protected override RedisClient OnCreateObject()
        {
            return new RedisClient(this._Host, this._Port);
        }
    }
}
