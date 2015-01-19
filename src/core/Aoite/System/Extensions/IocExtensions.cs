using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// 表示 <see cref="System.IIocContainer"/> 的扩展方法。
/// </summary>
public static class IocExtensions
{
    #region Service

    /// <summary>
    /// 将指定服务添加到服务容器中。
    /// </summary>
    /// <typeparam name="TService">要添加的服务类型。</typeparam>
    /// <param name="container">服务容器。</param>
    /// <param name="singletonMode">true，则启用单例模式；否则为 false。</param>
    /// <param name="promote">true，则将此请求提升到任何父服务容器；否则为 false。</param>
    public static void AddService<TService>(this IIocContainer container, bool singletonMode = false, bool promote = false)
    {
        if(container == null) throw new ArgumentNullException("container");

        container.AddService(typeof(TService), singletonMode, promote);
    }

    /// <summary>
    /// 将指定服务添加到服务容器中。
    /// </summary>
    /// <typeparam name="TService">要添加的服务类型。</typeparam>
    /// <typeparam name="TActual">实际的服务类型。</typeparam>
    /// <param name="container">服务容器。</param>
    /// <param name="singletonMode">true，则启用单例模式；否则为 false。</param>
    /// <param name="promote">true，则将此请求提升到任何父服务容器；否则为 false。</param>
    public static void AddService<TService, TActual>(this IIocContainer container, bool singletonMode = false, bool promote = false)
        where TActual : TService
    {
        if(container == null) throw new ArgumentNullException("container");

        container.AddService(typeof(TService), typeof(TActual), singletonMode, promote);
    }

    /// <summary>
    /// 将指定服务添加到服务容器中。
    /// </summary>
    /// <typeparam name="TService">要添加的服务类型。</typeparam>
    /// <param name="container">服务容器。</param>
    /// <param name="serviceInstance">要添加的服务的实例。 此对象必须实现 <typeparamref name="TService"/> 参数所指示的类型或从其继承。</param>
    /// <param name="promote">true，则将此请求提升到任何父服务容器；否则为 false。</param>
    public static void AddService<TService>(this IIocContainer container, TService serviceInstance, bool promote = false)
    {
        if(container == null) throw new ArgumentNullException("container");

        container.AddService(typeof(TService), serviceInstance, promote);
    }

    /// <summary>
    /// 将指定服务添加到服务容器中。
    /// </summary>
    /// <typeparam name="TService">要添加的服务类型。</typeparam>
    /// <param name="container">服务容器。</param>
    /// <param name="callback">用于创建服务的回调对象。这允许将服务声明为可用，但将对象的创建延迟到请求该服务之后。</param>
    /// <param name="singletonMode">true，则启用单例模式；否则为 false。</param>
    /// <param name="promote">true，则将此请求提升到任何父服务容器；否则为 false。</param>
    public static void AddService<TService>(this IIocContainer container, InstanceCreatorCallback callback, bool singletonMode = false, bool promote = false)
    {
        if(container == null) throw new ArgumentNullException("container");

        container.AddService(typeof(TService), callback, singletonMode, promote);
    }

    /// <summary>
    /// 从服务容器中移除指定的服务类型。
    /// </summary>
    /// <typeparam name="TService">要添加的服务类型。</typeparam>
    /// <param name="container">服务容器。</param>
    /// <param name="promote">true，则将此请求提升到任何父服务容器；否则为 false。</param>
    public static void RemoveService<TService>(this IIocContainer container, bool promote = false)
    {
        if(container == null) throw new ArgumentNullException("container");

        container.RemoveService(typeof(TService), promote);
    }

    /// <summary>
    /// 获取指定类型的服务对象。
    /// </summary>
    /// <typeparam name="TService">要添加的服务类型。</typeparam>
    /// <param name="container">服务容器。</param>
    /// <param name="lastMappingValues">后期映射的参数值数组。请保证数组顺序与构造函数的后期映射的参数顺序一致。</param>
    /// <returns><typeparamref name="TService"/> 类型的服务对象。- 或 -如果没有 <typeparamref name="TService"/>> 类型的服务对象，则为默认值。</returns>
    public static TService GetService<TService>(this IIocContainer container, params object[] lastMappingValues)
    {
        if(container == null) throw new ArgumentNullException("container");

        var service = container.GetService(typeof(TService), lastMappingValues);
        if(service == null) return default(TService);
        return (TService)service;
    }

    /// <summary>
    /// 尝试获取指定类型的服务对象。
    /// </summary>
    /// <typeparam name="TService">要添加的服务类型。</typeparam>
    /// <typeparam name="TValue">返回值的数据类型。</typeparam>
    /// <param name="container">服务容器。</param>
    /// <param name="callback">当服务存在时发生的回调方法。</param>
    /// <param name="lastMappingValues">后期映射的参数值数组。请保证数组顺序与构造函数的后期映射的参数顺序一致。</param>
    /// <returns>返回回调方法的值。</returns>
    public static TValue TryGetService<TService, TValue>(this IIocContainer container, Func<TService, TValue> callback, params object[] lastMappingValues)
    {
        var service = GetService<TService>(container, lastMappingValues);
        if(service != null) return callback(service);
        return default(TValue);
    }

    /// <summary>
    /// 尝试获取指定类型的服务对象。
    /// </summary>
    /// <typeparam name="TService">要添加的服务类型。</typeparam>
    /// <param name="container">服务容器。</param>
    /// <param name="callback">当服务存在时发生的回调方法。</param>
    /// <param name="lastMappingValues">后期映射的参数值数组。请保证数组顺序与构造函数的后期映射的参数顺序一致。</param>
    /// <returns><typeparamref name="TService"/> 类型的服务对象。- 或 -如果没有 <typeparamref name="TService"/>> 类型的服务对象，则为默认值。</returns>
    public static TService TryGetService<TService>(this IIocContainer container, Action<TService> callback, params object[] lastMappingValues)
    {
        var service = GetService<TService>(container, lastMappingValues);
        if(service != null) callback(service);
        return service;
    }

    /// <summary>
    /// 查找服务容器是否包含指定的服务类型。
    /// </summary>
    /// <typeparam name="TService">要添加的服务类型。</typeparam>
    /// <param name="container">服务容器。</param>
    /// <param name="promote">true，则将此请求提升到任何父服务容器；否则为 false。</param>
    /// <returns>如果存在返回 true，否则返回 false。</returns>
    public static bool ContainsService<TService>(this IIocContainer container, bool promote = false)
    {
        if(container == null) throw new ArgumentNullException("container");

        return container.ContainsService(typeof(TService), promote);
    }

    #endregion

    #region TypeValue

    /// <summary>
    /// 将指定的参数名和参数值添加到服务容器中，并绑定到关联的服务类型的构造函数。
    /// </summary>
    /// <typeparam name="TService">要添加的服务类型。</typeparam>
    /// <param name="container">服务容器。</param>
    /// <param name="name">参数名称。</param>
    /// <param name="value">参数值。</param>
    /// <param name="promote">true，则将此请求提升到任何父服务容器；否则为 false。</param>
    public static void AddValue<TService>(this IIocContainer container, string name, object value, bool promote = false)
    {
        if(container == null) throw new ArgumentNullException("container");

        container.AddValue(typeof(TService), name, value, promote);
    }
    /// <summary>
    /// 将指定的参数名和参数值添加到服务容器中，并绑定到关联的服务类型的构造函数。
    /// </summary>
    /// <typeparam name="TService">要添加的服务类型。</typeparam>
    /// <param name="container">服务容器。</param>>
    /// <param name="name">参数名称。</param>
    /// <param name="callback">用于创建参数的回调对象。这允许将参数声明为可用，但将值的创建延迟到请求该参数之后。</param>
    /// <param name="singletonMode">true，则启用单例模式；否则为 false。</param>
    /// <param name="promote">true，则将此请求提升到任何父服务容器；否则为 false。</param>
    public static void AddValue<TService>(this IIocContainer container, string name, InstanceCreatorCallback callback, bool singletonMode = false, bool promote = false)
    {
        if(container == null) throw new ArgumentNullException("container");

        container.AddValue(typeof(TService), name, callback, singletonMode, promote);
    }
    /// <summary>
    /// 从服务容器中移除指定关联的服务类型的参数。
    /// </summary>
    /// <typeparam name="TService">要添加的服务类型。</typeparam>
    /// <param name="container">服务容器。</param>
    /// <param name="name">要移除的参数名称。</param>
    /// <param name="promote">true，则将此请求提升到任何父服务容器；否则为 false。</param>
    public static void RemoveValue<TService>(this IIocContainer container, string name, bool promote = false)
    {
        if(container == null) throw new ArgumentNullException("container");

        container.RemoveValue(typeof(TService), name, promote);
    }
    /// <summary>
    /// 获取指定关联的服务类型和参数名称的值。
    /// </summary>
    /// <typeparam name="TService">要添加的服务类型。</typeparam>
    /// <param name="container">服务容器。</param>
    /// <param name="name">参数名称。</param>
    /// <param name="lastMappingValues">后期映射的参数值数组。</param>
    /// <returns>返回参数名称的值。- 或 -如果没有参数名称的值，则为 null 值。</returns>
    public static object GetValue<TService>(this IIocContainer container, string name, params object[] lastMappingValues)
    {
        if(container == null) throw new ArgumentNullException("container");

        return container.GetValue(typeof(TService), name, lastMappingValues);
    }
    /// <summary>
    /// 查找服务容器是否包含指定关联的服务类型指定的参数。
    /// </summary>
    /// <typeparam name="TService">要添加的服务类型。</typeparam>
    /// <param name="container">服务容器。</param>
    /// <param name="name">要查找的参数名称。</param>
    /// <param name="promote">true，则将此请求提升到任何父服务容器；否则为 false。</param>
    /// <returns>如果存在返回 true，否则返回 false。</returns>
    public static bool ContainsValue<TService>(this IIocContainer container, string name, bool promote = false)
    {
        if(container == null) throw new ArgumentNullException("container");

        return container.ContainsValue(typeof(TService), name, promote);
    }

    #endregion

    #region AutoMap

    /// <summary>
    /// 指定筛选器，自动映射类型。
    /// </summary>
    /// <param name="container">服务容器。</param>
    /// <param name="mapFilter">依赖注入与控制反转的映射筛选器。</param>
    /// <param name="expectTypeHandler">找到预期类型时发生。</param>
    public static void AutoMap(this IIocContainer container, IMapFilter mapFilter, Action<Type> expectTypeHandler = null)
    {
        if(mapFilter == null) throw new ArgumentNullException("mapFilter");

        var allTypes = ObjectFactory.AllTypes;
        var hasHandler = expectTypeHandler != null;
        foreach(var groupItem in allTypes)
        {
            foreach(var expectType in groupItem)
            {
                var ns = expectType.Namespace;
                if(mapFilter.IsExpectType(expectType))
                {
                    var actualType = mapFilter.FindActualType(allTypes, expectType);
                    if(actualType == null)
                        throw new Exception("无法找到预期定义类型“" + expectType.AssemblyQualifiedName + "”的实际映射类型。");
                    container.AddService(expectType, actualType, mapFilter.IsSingletonMode(expectType, actualType));
                    if(hasHandler) expectTypeHandler(expectType);
                    break;
                }
            }
        }
    }

    /// <summary>
    /// 指定配置文件“configuration/appSettings”的键，自定映射类型。
    /// </summary>
    /// <param name="container">服务容器。</param>
    /// <param name="appSettingKey">配置文件“configuration/appSettings”的键。</param>
    public static void AutoMapFromConfig(this IIocContainer container, string appSettingKey)
    {
        var namespaceExpression = System.Configuration.ConfigurationManager.AppSettings[appSettingKey];
        if(string.IsNullOrEmpty(namespaceExpression))
            throw new System.Configuration.SettingsPropertyNotFoundException("configuration/appSettings/add[key='" + appSettingKey + "'] 不存在。");
        AutoMap(container, new DefaultMapFilter(namespaceExpression));
    }
    /// <summary>
    /// 指定命名空间表达式，自动映射类型。
    /// </summary>
    /// <param name="container">服务容器。</param>
    /// <param name="namespaceExpression">筛选器的命名空间表达式。可以是一个完整的命名空间，也可以是“*”起始，或者以“*”结尾。符号“*”只能出现一次。通过“|”可以同时包含多个命名空间。</param>
    /// <param name="actualTypeFullNameFormat">获取或设置筛选器的实际类型的完全限定名的格式项，例如“{0}.Default{1}”，索引 0 表示 - 预期定义接口 - 的命名空间，索引 1 表示 - 预期定义接口 - 的名称（已去 I）。</param>
    public static void AutoMap(this IIocContainer container, string namespaceExpression, string actualTypeFullNameFormat = null)
    {
        if(string.IsNullOrEmpty(namespaceExpression)) throw new ArgumentNullException("namespaceExpression");
        AutoMap(container, new DefaultMapFilter(namespaceExpression) { ActualTypeFullNameFormat = actualTypeFullNameFormat });
    }
    /*
    /// <summary>
    /// 指定契约域，自动映射类型。
    /// </summary>
    /// <param name="container">服务容器。</param>
    /// <param name="domain">契约域。</param>
    public static void AutoMap(this IIocContainer container, Aoite.ServiceModel.ContractDomain domain)
    {
        var mapFilter = domain.CreateMapFilter();
        var allTypes = ObjectFactory.AllTypes;
        foreach(var groupItem in allTypes)
        {
            foreach(var expectType in groupItem)
            {
                var ns = expectType.Namespace;
                if(mapFilter.IsExpectType(expectType))
                {
                    var info = domain.AddOrGet(expectType);
                    container.AddService(expectType, lastMappingArguments =>
                    {
                        var keepAlive = false;
                        if(lastMappingArguments.Length == 1 && lastMappingArguments[0] is Boolean)
                            keepAlive = (bool)lastMappingArguments[0];
                        return info.CreateProxyObject(domain, keepAlive);
                    }, false);
                }
            }
        }
    }
    */
    #endregion
}