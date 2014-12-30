using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Aoite.CommandModel
{
    /// <summary>
    /// 定义一个命令模型的缓存策略。
    /// </summary>
    public interface ICommandCacheStrategy
    {
        /// <summary>
        /// 获取缓存项的键。
        /// </summary>
        string Key { get; }
        /// <summary>
        /// 获取缓存项的弹性的过期间隔。
        /// </summary>
        TimeSpan SlidingExpiration { get; }
        /// <summary>
        /// 获取缓存中的项。
        /// </summary>
        /// <param name="group">缓存键的分组。</param>
        /// <returns>返回缓存中的项，如果缓存不存在项，则返回 null 值。</returns>
        object GetCache(string group);
        /// <summary>
        /// 提供缓存键的分组，设置缓存项。
        /// </summary>
        /// <param name="group">缓存键的分组。</param>
        /// <param name="value">缓存的值。</param>
        void SetCache(string group, object value);
    }

    /// <summary>
    /// 表示一个命令模型默认的缓存策略。
    /// </summary>
    public class CommandCacheStrategy : ICommandCacheStrategy
    {
        private string _Key;
        /// <summary>
        /// 获取缓存项的键。
        /// </summary>
        public string Key { get { return _Key; } }

        private TimeSpan _SlidingExpiration;
        /// <summary>
        /// 获取缓存项的弹性的过期间隔。
        /// </summary>
        public TimeSpan SlidingExpiration { get { return _SlidingExpiration; } }

        private ICommand _Command;
        /// <summary>
        /// 获取执行的命令模型。
        /// </summary>
        public ICommand Command { get { return _Command; } }

        private IContext _Context;
        /// <summary>
        /// 获取执行的上下文。
        /// </summary>
        public IContext Context { get { return _Context; } }

        /// <summary>
        /// 初始化一个 <see cref="Aoite.CommandModel.CommandCacheStrategy"/> 类的新实例。
        /// </summary>
        /// <param name="key">缓存项的键。</param>
        /// <param name="slidingExpiration">缓存项的弹性的过期间隔。</param>
        /// <param name="command">执行的命令模型。</param>
        /// <param name="context">执行的上下文。</param>
        public CommandCacheStrategy(string key, TimeSpan slidingExpiration, ICommand command, IContext context)
        {
            if(string.IsNullOrEmpty(key)) throw new ArgumentNullException("key");
            if(slidingExpiration.TotalSeconds < 1) throw new ArgumentOutOfRangeException("slidingExpiration");
            if(command == null) throw new ArgumentNullException("command");
            if(context == null) throw new ArgumentNullException("context");

            this._Key = key;
            this._SlidingExpiration = slidingExpiration;
            this._Command = command;
            this._Context = context;
        }

        /// <summary>
        /// 获取缓存中的项。
        /// </summary>
        /// <param name="group">缓存键的分组。</param>
        /// <returns>返回缓存中的项，如果缓存不存在项，则返回 null 值。</returns>
        public virtual object GetCache(string group)
        {
            var cacheProvider = this._Context.Container.GetService<Aoite.Cache.ICacheProvider>();
            var key = group + this._Key;
            var value = cacheProvider.Get(key);
            if(value == null) return null;
            cacheProvider.Expire(key, this._SlidingExpiration);
            return value;
        }

        /// <summary>
        /// 提供缓存键的分组，设置缓存项。
        /// </summary>
        /// <param name="group">缓存键的分组。</param>
        /// <param name="value">缓存的值。</param>
        public virtual void SetCache(string group, object value)
        {
            var cacheProvider = this._Context.Container.GetService<Aoite.Cache.ICacheProvider>();
            cacheProvider.Set(group + this._Key, value, this._SlidingExpiration);
        }

    }

    /// <summary>
    /// 定义一个支持缓存的命令模型。
    /// </summary>
    public interface ICommandCache
    {
        /// <summary>
        /// 提供执行的上下文，获取缓存策略。
        /// </summary>
        /// <param name="context">执行的上下文。</param>
        /// <returns>返回一个缓存策略。</returns>
        ICommandCacheStrategy CreateStrategy(IContext context);
        /// <summary>
        /// 设置缓存的值。
        /// </summary>
        /// <param name="value">缓存值。</param>
        /// <returns>返回一个值，表示缓存值是否有效的赋值。返回 false 表示缓存值无效。</returns>
        bool SetCacheValue(object value);
        /// <summary>
        /// 获取需缓存的值。
        /// </summary>
        /// <returns>返回缓存值。</returns>
        object GetCacheValue();
    }

    /// <summary>
    /// 表示一个命令模型执行器具有缓存的特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class CacheAttribute : Attribute, IExecutorAttribute
    {
        private string _Group;
        /// <summary>
        /// 设置或获取缓存的分组。
        /// </summary>
        public string Group { get { return _Group; } }

        /// <summary>
        /// 初始化一个 <see cref="Aoite.CommandModel.CacheAttribute"/> 类的新实例。
        /// </summary>
        /// <param name="group">缓存的分组。</param>
        public CacheAttribute(string group)
        {
            if(string.IsNullOrEmpty(group)) throw new ArgumentNullException("group");
            this._Group = group;
        }

        bool ICommandHandler<ICommand>.RaiseExecuting(IContext context, ICommand command)
        {
            var commandCache = command as ICommandCache;
            if(commandCache == null) throw new NotSupportedException(command.GetType().FullName + "：命令模型没有实现缓存接口。");
            var strategy = commandCache.CreateStrategy(context);
            if(strategy == null || string.IsNullOrEmpty(strategy.Key))
                throw new NotSupportedException(command.GetType().FullName + "：命令模型返回了无效的策略信息。");

            var value = strategy.GetCache(this._Group);
            if(value == null) return true;
            return !commandCache.SetCacheValue(value);
        }

        void ICommandHandler<ICommand>.RaiseExecuted(IContext context, ICommand command)
        {
            var commandCache = command as ICommandCache;
            commandCache.CreateStrategy(context).SetCache(this._Group, commandCache.GetCacheValue());
        }

        void ICommandHandler<ICommand>.RaiseException(IContext context, ICommand command, Exception exception) { }
    }
}
