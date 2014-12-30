using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// 定义一个服务容器。
    /// </summary>
    public interface IIocContainer : IServiceProvider
    {
        /// <summary>
        /// 获取父级服务容器。
        /// </summary>
        IIocContainer Parent { get; }
        /// <summary>
        /// 将指定服务添加到服务容器中。
        /// </summary>
        /// <param name="serviceType">要添加的服务类型。</param>
        /// <param name="singletonMode">true，则启用单例模式；否则为 false。</param>
        /// <param name="promote">true，则将此请求提升到任何父服务容器；否则为 false。</param>
        void AddService(Type serviceType, bool singletonMode = false, bool promote = false);
        /// <summary>
        /// 将指定服务添加到服务容器中。
        /// </summary>
        /// <param name="serviceType">要添加的服务类型。</param>
        /// <param name="actualType">实际的服务类型。</param>
        /// <param name="singletonMode">true，则启用单例模式；否则为 false。</param>
        /// <param name="promote">true，则将此请求提升到任何父服务容器；否则为 false。</param>
        void AddService(Type serviceType, Type actualType, bool singletonMode = false, bool promote = false);
        /// <summary>
        /// 将指定服务添加到服务容器中。
        /// </summary>
        /// <param name="serviceType">要添加的服务类型。</param>
        /// <param name="serviceInstance">要添加的服务的实例。 此对象必须实现 <paramref name="serviceType"/> 参数所指示的类型或从其继承。</param>
        /// <param name="promote">true，则将此请求提升到任何父服务容器；否则为 false。</param>
        void AddService(Type serviceType, object serviceInstance, bool promote = false);
        /// <summary>
        /// 将指定服务添加到服务容器中。
        /// </summary>
        /// <param name="serviceType">要添加的服务类型。</param>
        /// <param name="callback">用于创建服务的回调对象。这允许将服务声明为可用，但将对象的创建延迟到请求该服务之后。</param>
        /// <param name="singletonMode">true，则启用单例模式；否则为 false。</param>
        /// <param name="promote">true，则将此请求提升到任何父服务容器；否则为 false。</param>
        void AddService(Type serviceType, InstanceCreatorCallback callback, bool singletonMode = false, bool promote = false);
        /// <summary>
        /// 从服务容器中移除指定的服务类型。
        /// </summary>
        /// <param name="serviceType">要移除的服务类型。</param>
        /// <param name="promote">true，则将此请求提升到任何父服务容器；否则为 false。</param>
        void RemoveService(Type serviceType, bool promote = false);
        /// <summary>
        /// 获取指定类型的服务对象。
        /// </summary>
        /// <param name="serviceType">一个对象，它指定要获取的服务对象的类型。。</param>
        /// <param name="lastMappingValues">后期映射的参数值数组。请保证数组顺序与构造函数的后期映射的参数顺序一致。</param>
        /// <returns><paramref name="serviceType"/> 类型的服务对象。- 或 -如果没有 <paramref name="serviceType"/> 类型的服务对象，则为 null。</returns>
        object GetService(Type serviceType, params object[] lastMappingValues);
        /// <summary>
        /// 查找服务容器是否包含指定的服务类型。
        /// </summary>
        /// <param name="serviceType">要查找的服务类型。</param>
        /// <param name="promote">true，则将此请求提升到任何父服务容器；否则为 false。</param>
        /// <returns>如果存在返回 true，否则返回 false。</returns>
        bool ContainsService(Type serviceType, bool promote = false);

        /// <summary>
        /// 将指定的参数名和参数值添加到服务容器中。
        /// </summary>
        /// <param name="name">参数名称。</param>
        /// <param name="value">参数值。</param>
        /// <param name="promote">true，则将此请求提升到任何父服务容器；否则为 false。</param>
        void AddValue(string name, object value, bool promote = false);
        /// <summary>
        /// 将指定的参数名和参数值添加到服务容器中。
        /// </summary>
        /// <param name="name">参数名称。</param>
        /// <param name="callback">用于创建参数的回调对象。这允许将参数声明为可用，但将值的创建延迟到请求该参数之后。</param>
        /// <param name="singletonMode">true，则启用单例模式；否则为 false。</param>
        /// <param name="promote">true，则将此请求提升到任何父服务容器；否则为 false。</param>
        void AddValue(string name, InstanceCreatorCallback callback, bool singletonMode = false, bool promote = false);
        /// <summary>
        /// 从服务容器中移除指定的参数。
        /// </summary>
        /// <param name="name">要移除的参数名称。</param>
        /// <param name="promote">true，则将此请求提升到任何父服务容器；否则为 false。</param>
        void RemoveValue(string name, bool promote = false);
        /// <summary>
        /// 获取指定参数名称的值。
        /// </summary>
        /// <param name="name">参数名称。</param>
        /// <param name="lastMappingValues">后期映射的参数值数组。</param>
        /// <returns>返回参数名称的值。- 或 -如果没有参数名称的值，则为 null 值。</returns>
        object GetValue(string name, params object[] lastMappingValues);
        /// <summary>
        /// 查找服务容器是否包含指定的参数。
        /// </summary>
        /// <param name="name">要查找的参数名称。</param>
        /// <param name="promote">true，则将此请求提升到任何父服务容器；否则为 false。</param>
        /// <returns>如果存在返回 true，否则返回 false。</returns>
        bool ContainsValue(string name, bool promote = false);

        /// <summary>
        /// 将指定的参数名和参数值添加到服务容器中，并绑定到关联的服务类型的构造函数。
        /// </summary>
        /// <param name="serviceType">关联的服务类型。</param>
        /// <param name="name">参数名称。</param>
        /// <param name="value">参数值。</param>
        /// <param name="promote">true，则将此请求提升到任何父服务容器；否则为 false。</param>
        void AddValue(Type serviceType, string name, object value, bool promote = false);
        /// <summary>
        /// 将指定的参数名和参数值添加到服务容器中，并绑定到关联的服务类型的构造函数。
        /// </summary>
        /// <param name="serviceType">关联的服务类型。</param>
        /// <param name="name">参数名称。</param>
        /// <param name="callback">用于创建参数的回调对象。这允许将参数声明为可用，但将值的创建延迟到请求该参数之后。</param>
        /// <param name="singletonMode">true，则启用单例模式；否则为 false。</param>
        /// <param name="promote">true，则将此请求提升到任何父服务容器；否则为 false。</param>
        void AddValue(Type serviceType, string name, InstanceCreatorCallback callback, bool singletonMode = false, bool promote = false);
        /// <summary>
        /// 从服务容器中移除指定关联的服务类型的参数。
        /// </summary>
        /// <param name="serviceType">关联的服务类型。</param>
        /// <param name="name">要移除的参数名称。</param>
        /// <param name="promote">true，则将此请求提升到任何父服务容器；否则为 false。</param>
        void RemoveValue(Type serviceType, string name, bool promote = false);
        /// <summary>
        /// 获取指定关联的服务类型和参数名称的值。
        /// </summary>
        /// <param name="serviceType">关联的服务类型。</param>
        /// <param name="name">参数名称。</param>
        /// <param name="lastMappingValues">后期映射的参数值数组。</param>
        /// <returns>返回参数名称的值。- 或 -如果没有参数名称的值，则为 null 值。</returns>
        object GetValue(Type serviceType, string name, params object[] lastMappingValues);
        /// <summary>
        /// 查找服务容器是否包含指定关联的服务类型指定的参数。
        /// </summary>
        /// <param name="serviceType">关联的服务类型。</param>
        /// <param name="name">要查找的参数名称。</param>
        /// <param name="promote">true，则将此请求提升到任何父服务容器；否则为 false。</param>
        /// <returns>如果存在返回 true，否则返回 false。</returns>
        bool ContainsValue(Type serviceType, string name, bool promote = false);

        /// <summary>
        /// 销毁所有的映射。
        /// </summary>
        void DestroyAll();
    }
}
