using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// 一个依赖注入与控制反转的实例对象获取器，支持更高级的扩展。
    /// </summary>
    /// <typeparam name="T">类型。</typeparam>
    public static class Object<T>
    {
        private readonly static Type Type = typeof(T);
        /// <summary>
        /// 当获取 <typeparamref name="T"/> 实例，并且调用方没有指定后期映射的参数值数组时发生。
        /// </summary>
        public static event LastMappingHandler LastMapping;
        /// <summary>
        /// 获取或创建指定数据类型的实例。如果 <typeparamref name="T"/> 是一个接口、基类，将返回接口映射的实例；如果 <typeparamref name="T"/> 是一个普通的类型，将会自动创建新的映射，并返回实例。
        /// </summary>
        public static T Instance { get { return Create(); } }

        /// <summary>
        /// 指定后期映射的参数值列表，获取或创建指定数据类型的实例。
        /// </summary>
        /// <param name="lastMappingValues">后期映射的参数值数组。请保证数组顺序与构造函数的后期映射的参数顺序一致。</param>
        /// <returns>如果 <typeparamref name="T"/> 是一个接口、基类，将返回接口映射的实例；如果 <typeparamref name="T"/> 是一个普通的类型，将会自动创建新的映射，并返回实例。</returns>
        public static T Create(params object[] lastMappingValues)
        {
            var handler = LastMapping;
            if(handler != null && (lastMappingValues == null || lastMappingValues.Length == 0))
                lastMappingValues = handler(Type) ?? new object[0];

            return ObjectFactory.Global.GetService<T>(lastMappingValues);
        }

        /// <summary>
        /// 执行一次性对象操作。
        /// </summary>
        /// <param name="callback">回调方法。</param>
        /// <param name="lastMappingValues">后期映射的参数值数组。请保证数组顺序与构造函数的后期映射的参数顺序一致。</param>
        public static void Once(Action<T> callback, params object[] lastMappingValues)
        {
            var instance = Create(lastMappingValues);
            try
            {
                callback(instance);
            }
            finally
            {
                (instance as IDisposable).TryDispose();
            }
        }

        /// <summary>
        /// 执行一次性对象操作，并返回指定的值。
        /// </summary>
        /// <typeparam name="V">值的数据类型</typeparam>
        /// <param name="callback">回调方法。</param>    
        /// <param name="lastMappingValues">后期映射的参数值数组。请保证数组顺序与构造函数的后期映射的参数顺序一致。</param>
        /// <returns>返回指定的值。</returns>
        public static V Once<V>(Func<T, V> callback, params object[] lastMappingValues)
        {
            var instance = Create(lastMappingValues);
            try
            {
                return callback(instance);
            }
            finally
            {
                (instance as IDisposable).TryDispose();
            }
        }
    }
}
