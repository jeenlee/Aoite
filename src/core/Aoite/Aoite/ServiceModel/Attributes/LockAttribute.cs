using Aoite.Cache;
using Aoite.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// 定义一个自定义锁。
    /// </summary>
    public interface ICustomLockKey
    {
        /// <summary>
        /// 获取锁的唯一键名称。
        /// </summary>
        /// <param name="context">契约的上下文信息。</param>
        /// <returns>返回锁的唯一键名称。返回值不能为 null 值。</returns>
        string GetKey(ContractContext context);
    }

    /// <summary>
    /// 表示一个契约方法具有分布式锁的特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class LockAttribute : ContractFilterAttribute
    {
        /// <summary>
        /// 获取或设置分布式锁的键名。
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 获取或设置分布式锁的超时设定。
        /// </summary>
        public TimeSpan? Timeout { get; set; }

        /// <summary>
        /// 获取或设置分布锁的键名后缀。
        /// </summary>
        public string KeySuffix { get; set; }

        /// <summary>
        /// 初始化一个 <see cref="System.LockAttribute"/> 类的新实例。
        /// </summary>
        public LockAttribute() { }

        /// <summary>
        /// 提供分布式锁的超时设定，初始化一个 <see cref="System.LockAttribute"/> 类的新实例。
        /// </summary>
        /// <param name="timeout">分布式锁的超时设定。</param>
        public LockAttribute(TimeSpan? timeout) : this(null, timeout) { }
        /// <summary>
        /// 提供分布式锁的键名和超时设定，初始化一个 <see cref="System.LockAttribute"/> 类的新实例。
        /// </summary>
        /// <param name="key">分布式锁的键名。允许为 null 或 空值，表示使用契约方法作为键名。</param>
        /// <param name="timeout">分布式锁的超时设定。</param>
        public LockAttribute(string key, TimeSpan? timeout = null)
        {
            this.Key = key;
            this.Timeout = timeout;
        }

        private const string DataName = "$LockAttribute$";

        /// <summary>
        /// 创建锁的唯一名称。
        /// </summary>
        /// <param name="context">执行的上下文。</param>
        /// <returns>返回锁的唯一名称。</returns>
        protected virtual string CreateKey(ContractContext context)
        {
            if(string.IsNullOrEmpty(this.Key))
            {
                lock(this)
                {
                    if(string.IsNullOrEmpty(this.Key))
                    {

                        this.Key = context.Source.Service.Type.FullName
                                   + "->"
                                   + context.Source.Method.MethodInfo.Name;
                    }
                }
            }
            return this.Key;
        }

        /// <summary>
        /// 创建一个锁。
        /// </summary>
        /// <param name="context">契约的上下文信息。</param>
        /// <returns>返回一个可释放的锁。</returns>
        protected virtual IDisposable CreateLock(ContractContext context)
        {
            var key = this.CreateKey(context);
            if(string.IsNullOrEmpty(key)) throw new NotSupportedException("契约方法 {0}->{1} 找不到需要锁的唯一键名称。".Fmt(context.Source.Service.Type.FullName, context.Source.Method.Name));
            return context.CacheProvider.Lock(key + this.KeySuffix, this.Timeout);
        }

        /// <summary>
        /// 摧毁一个锁。
        /// </summary>
        /// <param name="context">契约的上下文信息。</param>
        /// <param name="lockObject">一个可释放的锁。</param>
        protected virtual void DestroyLock(ContractContext context, IDisposable lockObject)
        {
            lockObject.TryDispose();
            context.Items.Remove(DataName);
        }

        /// <summary>
        /// 契约方法调用前发生。
        /// </summary>
        /// <param name="context">契约的上下文信息。</param>
        protected override void OnCalling(ContractContext context)
        {
            context.Items[DataName] = this.CreateLock(context);
        }

        /// <summary>
        /// 契约方法调用后发生。
        /// </summary>
        /// <param name="context">契约的上下文信息。</param>
        protected override void OnCalled(ContractContext context)
        {
            DestroyLock(context, context.Items[DataName] as IDisposable);
        }
    }

    /// <summary>
    /// 表示一个命令模型执行器具有分布式锁的特性，锁的唯一键名称来自用户。使用此特性必须保证用户已登录。
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class UserLockAttribute : LockAttribute
    {
        readonly static System.Collections.Concurrent.ConcurrentDictionary<Type, Aoite.Reflection.MemberGetter>
            PropertiesCache = new System.Collections.Concurrent.ConcurrentDictionary<Type, Aoite.Reflection.MemberGetter>();

        /// <summary>
        /// 初始化一个 <see cref="System.UserLockAttribute"/> 类的新实例。
        /// </summary>
        public UserLockAttribute() { }
        /// <summary>
        /// 提供分布式锁的超时设定，初始化一个 <see cref="System.UserLockAttribute"/> 类的新实例。
        /// </summary>
        /// <param name="timeout">分布式锁的超时设定。</param>
        public UserLockAttribute(TimeSpan? timeout = null) : base(timeout) { }
        /// <summary>
        /// 提供分布式锁的键名和超时设定，初始化一个 <see cref="System.UserLockAttribute"/> 类的新实例。
        /// </summary>
        /// <param name="key">分布式锁的键名。允许为 null 或 空值，表示调用用户实例的 ToString 方法。</param>
        /// <param name="timeout">分布式锁的超时设定。</param>
        public UserLockAttribute(string key, TimeSpan? timeout = null) : base(key, timeout) { }

        /// <summary>
        /// 创建锁的唯一名称。
        /// </summary>
        /// <param name="context">契约的上下文信息。</param>
        /// <returns>返回锁的唯一名称。</returns>
        protected override string CreateKey(ContractContext context)
        {
            var user = context.User;
            if(user == null) throw new NotSupportedException("使用用户锁时，当前用户不能为空。");
            if(user is ICustomLockKey) return (user as ICustomLockKey).GetKey(context);

            var objectUser = (object)user;
            var key = this.Key;
            if(string.IsNullOrEmpty(key))
            {
                key = objectUser.ToString();
            }
            else
            {
                var getter = PropertiesCache.GetOrAdd(objectUser.GetType(), type =>
                {
                    var p = type.GetProperty(key, Aoite.Reflection.Flags.AnyVisibility);
                    if(p == null) return null;
                    return Aoite.Reflection.PropertyInfoExtensions.DelegateForGetPropertyValue(p);
                });
                if(getter == null) throw new ArgumentOutOfRangeException(key, "用户对象找不到需要锁的属性（" + key + "）。");
                key = Convert.ToString(getter(objectUser));
            }
            return key;
        }
    }
}
