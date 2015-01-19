using Aoite.Redis.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Redis
{
    /// <summary>
    /// 定义一个 Redis 客户端。
    /// </summary>
    public interface IRedisClient : IObjectDisposable
    {
        /// <summary>
        /// 执行指定的 Redis 命令。
        /// </summary>
        /// <typeparam name="T">命令返回值的数据类型。</typeparam>
        /// <param name="command">Redis 命令。</param>
        /// <returns>返回执行后的返回值。</returns>
        T Execute<T>(RedisCommand<T> command);
        /// <summary>
        /// 开始一个新的事务。
        /// </summary>
        /// <returns>如果事务已存在，将会抛出一个错误，否则返回一个新的事务。</returns>
        IRedisTransaction BeginTransaction();
    }
    /// <summary>
    /// 表示一个 Redis 事务。
    /// </summary>
    public interface IRedisTransaction : IRedisClient
    {
        /// <summary>
        /// 指示当事务成功执行后，需要获取返回值的回调的方法。
        /// </summary>
        /// <typeparam name="T">返回值的数据类型。</typeparam>
        /// <param name="executor">执行的委托的返回值。</param>
        /// <param name="callback">毁掉的委托。</param>
        void On<T>(T executor, Action<T> callback);
        /// <summary>
        /// 提交当前事务。
        /// <para>该方法有可能会直接抛出错误。举个例子，事务中的命令可能处理了错误类型的键，比如将列表命令用在了字符串键上面，诸如此类。</para> 
        /// </summary>
        /// <returns>返回一个结果。</returns>
        Result Commit();
    }
}
