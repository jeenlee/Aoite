using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// 定义复制的策略。
    /// </summary>
    public enum CopyToStrategy
    {
        /// <summary>
        /// 默认方式。
        /// </summary>
        Default,
        /// <summary>
        /// 仅限主键方式。
        /// </summary>
        OnlyPrimaryKey,
        /// <summary>
        /// 仅限非主键方式。
        /// </summary>
        ExcludePrimaryKey,
        /// <summary>
        /// 仅限被修改过的值。
        /// </summary>
        OnlyChangeValues,
    }
    //TODO：未完成。需要完成 Cache
    /// <summary>
    /// 提供公共的实用工具方法。
    /// </summary>
    public static class CommonExtensions
    {
        /// <summary>
        /// 尝试将给定值转换为指定的数据类型。
        /// </summary>
        /// <typeparam name="T">要转换的数据类型。</typeparam>
        /// <param name="value">请求类型转换的值。</param>
        /// <returns>返回一个数据类型的值，或一个 null 值。</returns>
        public static T CastTo<T>(this object value)
        {
            return (T)CastTo(value, typeof(T));
        }

        /// <summary>
        /// 尝试将给定值转换为指定的数据类型。
        /// </summary>
        /// <param name="value">请求类型转换的值。</param>
        /// <param name="type">要转换的数据类型。</param>
        /// <returns>返回一个数据类型的值，或一个 null 值。</returns>
        public static object CastTo(this object value, Type type)
        {
            return type.ChangeType(value);
        }
      
        /// <summary>
        /// 将 <paramref name="target"/> 所有的属性值复制到当前对象。
        /// </summary>
        /// <typeparam name="TSource">源的数据类型。</typeparam>
        /// <typeparam name="TTarget">目标的数据类型。</typeparam>
        /// <param name="source">复制的源对象。</param>
        /// <param name="target">复制的目标对象。</param>
        /// <param name="targetStrategy">复制目标的策略。</param>
        /// <returns>返回 <paramref name="source"/>。</returns>
        public static TSource CopyFrom<TSource, TTarget>(this TSource source, TTarget target, CopyToStrategy targetStrategy = CopyToStrategy.Default)
        {
            return CopyTo<TSource>(target, source, targetStrategy);
        }

        /// <summary>
        /// 将当前对象所有的属性值复制成一个新的 <typeparamref name="TTarget"/> 实例。
        /// </summary>
        /// <typeparam name="TTarget">新的数据类型。</typeparam>
        /// <param name="source">复制的源对象。</param>
        /// <param name="targetStrategy">复制目标的策略。</param>
        /// <returns>返回一个 <typeparamref name="TTarget"/> 的心实例。</returns>
        public static TTarget CopyTo<TTarget>(this object source, CopyToStrategy targetStrategy = CopyToStrategy.Default)
        {
            if(source == null) return default(TTarget);
            TTarget t2 = Activator.CreateInstance<TTarget>();
            return CopyTo(source, t2, targetStrategy);
        }

        /// <summary>
        /// 将当前对象所有的属性值复制到 <paramref name="target"/>。
        /// </summary>
        /// <typeparam name="TTarget">目标的数据类型。</typeparam>
        /// <param name="source">复制的源对象。</param>
        /// <param name="target">复制的目标对象。</param>
        /// <param name="targetStrategy">复制目标的策略。</param>
        /// <returns>返回 <paramref name="target"/>。</returns>
        public static TTarget CopyTo<TTarget>(this object source, TTarget target, CopyToStrategy targetStrategy = CopyToStrategy.Default)
        {
            if(source == null) throw new ArgumentNullException("source");
            if(target == null) throw new ArgumentNullException("target");

            var sMapper = TypeMapper.Create(source.GetType());
            var tMapper = TypeMapper.Create(target.GetType());
            foreach(var sProperty in sMapper.Properties)
            {
                var tProperty = tMapper[sProperty.Name];
                if(tProperty == null) continue;
                if(targetStrategy == CopyToStrategy.OnlyPrimaryKey && !tProperty.IsKey
                    || (targetStrategy == CopyToStrategy.ExcludePrimaryKey && tProperty.IsKey)
                    || !tProperty.Property.CanWrite) continue;

                object sValue = sProperty.GetValue(source);

                if(targetStrategy == CopyToStrategy.OnlyChangeValues) if(object.Equals(sValue, sProperty.TypeDefaultValue)) continue;

                var spType = sProperty.Property.PropertyType;
                var tpType = tProperty.Property.PropertyType;

                if(spType != tpType)
                {
                    if(tpType.IsValueType && sValue == null)
                        throw new ArgumentNullException("{0}.{1}".Fmt(sMapper.Type.Name, sProperty.Property.Name), "目标属性 {0}.{1} 不能为 null 值。".Fmt(tMapper.Type.Name, tProperty.Property.Name));
                    tProperty.SetValue(target, tpType.ChangeType(sValue));
                }
                else tProperty.SetValue(target, sValue);
            }
            return target;
        }
      
        /// <summary>
        /// 对当前集合的每个元素执行指定操作。
        /// </summary>
        /// <typeparam name="T">集合的数据类型。</typeparam>
        /// <param name="collection">当前集合。</param>
        /// <param name="action">执行的委托。</param>
        public static void Each<T>(this IEnumerable<T> collection, Action<T> action)
        {
            if(collection == null) return;
            if(action == null) throw new ArgumentNullException("action");
            foreach(var item in collection) action(item);
        }

        /// <summary>
        /// 对当前集合的每个元素执行指定操作，并返回一个特定的结果集合。
        /// </summary>
        /// <typeparam name="T">集合的数据类型。</typeparam>
        /// <typeparam name="T2">返回的数据类型。</typeparam>
        /// <param name="collection">当前集合。</param>
        /// <param name="func">执行的委托。</param>
        /// <returns>返回一个集合。</returns>
        public static T2[] Each<T, T2>(this IEnumerable<T> collection, Func<T, T2> func)
        {
            if(collection == null) return null;
            if(func == null) throw new ArgumentNullException("action");
            return InnerEach(collection, func).ToArray();
        }
        private static IEnumerable<T2> InnerEach<T, T2>(IEnumerable<T> collection, Func<T, T2> func)
        {
            foreach(var item in collection) yield return func(item);
        }
        /// <summary>
        /// 返回表示当前对象的 <see cref="System.String"/>，如果 <paramref name="obj"/> 是一个 null 值，将返回 <see cref="System.String.Empty"/>。
        /// </summary>
        /// <param name="obj">一个对象。</param>
        /// <returns>返回 <paramref name="obj"/> 的 <see cref="System.String"/> 或 <see cref="System.String.Empty"/>。</returns>
        public static string ToStringOrEmpty(this object obj)
        {
            return obj == null ? string.Empty : obj.ToString();
        }

        /// <summary>
        /// 尝试释放当前对象使用的所有资源
        /// </summary>
        /// <param name="obj">释放的对象。</param>
        public static void TryDispose(this IDisposable obj)
        {
            if(obj != null) obj.Dispose();
        }

        /// <summary>
        /// 判定指定的二进制值是否包含有效的值。
        /// </summary>
        /// <param name="value">一个二进制值。</param>
        /// <returns>如果包含返回 true，否则返回 false。</returns>
        public static bool HasValue(this BinaryValue value)
        {
            return BinaryValue.HasValue(value);
        }
        /*
        /// <summary>
        /// 提供批量锁的功能。
        /// </summary>
        /// <param name="provider">缓存提供程序。</param>
        /// <param name="keys">锁的键名列表。</param>
        /// <returns>返回一个批量锁。</returns>
        public static IDisposable MultipleLock(this Aoite.Cache.ICacheProvider provider, params string[] keys)
        {
            return MultipleLock(provider, null, keys);
        }
        /// <summary>
        /// 提供批量锁的功能。
        /// </summary>
        /// <param name="provider">缓存提供程序。</param>
        /// <param name="timeout">锁的超时设定。</param>
        /// <param name="keys">锁的键名列表。</param>
        /// <returns>返回一个批量锁。</returns>
        public static IDisposable MultipleLock(this Aoite.Cache.ICacheProvider provider, TimeSpan timeout, params string[] keys)
        {
            return MultipleLock(provider, timeout, keys);
        }

        private static IDisposable MultipleLock(Aoite.Cache.ICacheProvider provider, TimeSpan? timeout, params string[] keys)
        {
            if(provider == null) throw new ArgumentNullException("provider");
            if(keys == null || keys.Length == 0) throw new ArgumentNullException("keys");

            Stack<IDisposable> stack = new Stack<IDisposable>(keys.Length);
            foreach(var key in keys)
            {
                stack.Push(provider.Lock(key, timeout));
            }
            return new MultipleLockKeys(stack);
        }
        class MultipleLockKeys : IDisposable
        {
            Stack<IDisposable> _stack;
            public MultipleLockKeys(Stack<IDisposable> stack)
            {
                this._stack = stack;
            }
            public void Dispose()
            {
                while(this._stack.Count > 0)
                {
                    this._stack.Pop().Dispose();
                }
            }
        }
        */
    }
}
