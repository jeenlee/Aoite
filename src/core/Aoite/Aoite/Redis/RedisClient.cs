using Aoite.Net;
using Aoite.Redis.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Redis
{
    /// <summary>
    /// 表示一个 Redis 客户端。
    /// </summary>
    public class RedisClient : ObjectDisposableBase, IRedisClient
    {
        internal RedisExecutor _executor;
        internal IConnector _connector;
        internal LinkedList<RedisCommand> _tranCommands;

        /// <summary>
        /// 指定套接字的信息，初始化一个 <see cref="Aoite.Redis.RedisClient"/> 类的新实例。
        /// </summary>
        /// <param name="socketInfo">套接字的信息。</param>
        public RedisClient(SocketInfo socketInfo) : this(new DefaultConnector(socketInfo)) { }

        internal RedisClient(IConnector connector)
        {
            this._connector = connector;
            this._executor = new RedisExecutor(connector);
        }

        /// <summary>
        /// 执行指定的 Redis 命令。
        /// </summary>
        /// <typeparam name="T">命令返回值的数据类型。</typeparam>
        /// <param name="command">Redis 命令。</param>
        /// <returns>返回执行后的返回值。</returns>
        public virtual T Execute<T>(RedisCommand<T> command)
        {
            if(command == null) throw new ArgumentNullException("command");
            this.ThrowWhenDisposed();
            if(this._tranCommands != null) throw new RedisException("Redis 事务期间，禁止通过非事务方式调用。");

            return this._executor.Execute(command);
        }

        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        public override void Dispose()
        {
            this._connector.Dispose();
            base.Dispose();
        }

        /// <summary>
        /// 开始一个新的事务。
        /// </summary>
        /// <returns>如果事务已存在，将会抛出一个错误，否则返回一个新的事务。</returns>
        public virtual IRedisTransaction BeginTransaction()
        {
            this.ThrowWhenDisposed();
            if(this._tranCommands != null) throw new RedisException("Redis 不支持嵌套事务。");
            this.Execute(new RedisStatus("MULTI")).ThrowIfFailded();
            return new RedisTransaction(this);
        }

    }

    class RedisTransaction : ObjectDisposableBase, IRedisTransaction
    {
        private readonly RedisClient _client;
        public RedisTransaction(RedisClient client)
        {
            if(client == null) throw new ArgumentNullException("client");
            this._client = client;
            this._client._tranCommands = new LinkedList<RedisCommand>();
        }

        protected override void ThrowWhenDisposed()
        {
            base.ThrowWhenDisposed();
            if(this._client._tranCommands == null) throw new RedisException("Redis 的事务已结束。");
        }

        public T Execute<T>(RedisCommand<T> command)
        {
            if(command == null) throw new ArgumentNullException("command");

            this.ThrowWhenDisposed();
            this._client._tranCommands.AddLast(command);
            this._client._executor.Execute(new RedisStatus.Queue(command)).ThrowIfFailded();
            return default(T);
        }

        IRedisTransaction IRedisClient.BeginTransaction()
        {
            throw new NotImplementedException();
        }

        void IRedisTransaction.On<T>(T executor, Action<T> callback)
        {
            this.ThrowWhenDisposed();
            //executor();
            this._client._tranCommands.Last.Value.SetCallback(callback);
        }

        Result IRedisTransaction.Commit()
        {
            this.ThrowWhenDisposed();
            return this._client._executor.Execute(new RedisArray.TranExec(this._client._tranCommands));
        }

        protected override void DisposeManaged()
        {
            if(this._client._tranCommands != null && this._client._tranCommands.Count > 0)
            {
                try
                {
                    this._client._executor.Execute(new RedisStatus("DISCARD"));
                }
                catch(Exception)
                {
                    // Redis client is disposed?
                }
            }
            this._client._tranCommands = null;
            base.DisposeManaged();
        }

    }
}
