using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    /// <summary>
    /// 表示一个默认的服务容器。
    /// </summary>
    public partial class IocContainer : IIocContainer
    {
        private readonly IIocContainer _parentLocator;
        private readonly bool _hasParent;
        /// <summary>
        /// 获取父级服务容器。
        /// </summary>
        public IIocContainer Parent { get { return this._parentLocator; } }

        /// <summary>
        /// 获取或设置一个值，表示是都禁用自动智能解析的功能。
        /// </summary>
        public bool DisabledAutoResolving { get; set; }
        /// <summary>
        /// 获取所有服务类型。
        /// </summary>
        public Type[] ServiceTypes { get { return CacheType.Keys.ToArray(); } }
        /// <summary>
        /// 获取所有值的名称。
        /// </summary>
        public string[] ValueNames { get { return CacheName.Keys.ToArray(); } }
        /// <summary>
        /// 获取所有绑定到类型的值的名称。
        /// </summary>
        public IEnumerable<KeyValuePair<Type, string[]>> TypeValueNames
        {
            get
            {
                foreach(var item in CacheTypeName)
                {
                    yield return new KeyValuePair<Type, string[]>(item.Key, item.Value.Keys.ToArray());
                }
            }
        }

        /// <summary>
        /// 初始化一个 <see cref="System.IocContainer"/> 类的新实例。
        /// </summary>
        public IocContainer()
        {
            this.AddService<IIocContainer>(this);
            this.AddService<IocContainer>(this);
        }

        /// <summary>
        /// 提供父级服务容器，初始化一个 <see cref="System.IocContainer"/> 类的新实例。
        /// </summary>
        /// <param name="parentLocator">父级服务容器，</param>
        public IocContainer(IIocContainer parentLocator)
            : this()
        {
            if(parentLocator == null) return;

            this._parentLocator = parentLocator;
            this._hasParent = true;
        }

        /// <summary>
        /// 创建基于当前服务容器的子服务容器。
        /// </summary>
        /// <returns>返回一个新的服务容器。</returns>
        public virtual IIocContainer CreateChildLocator()
        {
            return new IocContainer(this);
        }

        /// <summary>
        /// 将指定服务添加到服务容器中。
        /// </summary>
        /// <param name="serviceType">要添加的服务类型。</param>
        /// <param name="singletonMode">true，则启用单例模式；否则为 false。</param>
        /// <param name="promote">true，则将此请求提升到任何父服务容器；否则为 false。</param>
        public virtual void AddService(Type serviceType, bool singletonMode = false, bool promote = false)
        {
            if(serviceType == null) throw new ArgumentNullException("serviceType");

            if(this._hasParent && promote) this._parentLocator.AddService(serviceType, singletonMode, promote);

            this.Map(serviceType, this.AutoResolveExpectType(serviceType, singletonMode));
        }

        /// <summary>
        /// 将指定服务添加到服务容器中。
        /// </summary>
        /// <param name="serviceType">要添加的服务类型。</param>
        /// <param name="actualType">实际的服务类型。</param>
        /// <param name="singletonMode">true，则启用单例模式；否则为 false。</param>
        /// <param name="promote">true，则将此请求提升到任何父服务容器；否则为 false。</param>
        public virtual void AddService(Type serviceType, Type actualType, bool singletonMode = false, bool promote = false)
        {
            if(serviceType == null) throw new ArgumentNullException("serviceType");
            if(actualType == null) throw new ArgumentNullException("actualType");

            if(this._hasParent && promote) this._parentLocator.AddService(serviceType, actualType, singletonMode, promote);

            this.InnerMapType(actualType, serviceType, singletonMode);
        }

        /// <summary>
        /// 将指定服务添加到服务容器中。
        /// </summary>
        /// <param name="serviceType">要添加的服务类型。</param>
        /// <param name="serviceInstance">要添加的服务的实例。 此对象必须实现 <paramref name="serviceType"/> 参数所指示的类型或从其继承。</param>
        /// <param name="promote">true，则将此请求提升到任何父服务容器；否则为 false。</param>
        public void AddService(Type serviceType, object serviceInstance, bool promote = false)
        {
            this.AddService(serviceType, lmp => serviceInstance, true, promote);
        }
        /// <summary>
        /// 将指定服务添加到服务容器中。
        /// </summary>
        /// <param name="serviceType">要添加的服务类型。</param>
        /// <param name="callback">用于创建服务的回调对象。这允许将服务声明为可用，但将对象的创建延迟到请求该服务之后。</param>
        /// <param name="singletonMode">true，则启用单例模式；否则为 false。</param>
        /// <param name="promote">true，则将此请求提升到任何父服务容器；否则为 false。</param>
        public virtual void AddService(Type serviceType, InstanceCreatorCallback callback, bool singletonMode = false, bool promote = false)
        {
            if(serviceType == null) throw new ArgumentNullException("serviceType");
            if(callback == null) throw new ArgumentNullException("callback");

            if(this._hasParent && promote) this._parentLocator.AddService(serviceType, callback, singletonMode, promote);

            this.Map(serviceType, singletonMode
                ? new SingletonInstanceBox(serviceType.FullName, callback)
                : new InstanceBox(serviceType.FullName, callback));
        }
        /// <summary>
        /// 从服务容器中移除指定的服务类型。
        /// </summary>
        /// <param name="serviceType">要移除的服务类型。</param>
        /// <param name="promote">true，则将此请求提升到任何父服务容器；否则为 false。</param>
        public virtual void RemoveService(Type serviceType, bool promote = false)
        {
            if(serviceType == null) throw new ArgumentNullException("serviceType");

            if(this._hasParent && promote) this._parentLocator.RemoveService(serviceType, promote);

            this.MapRemove(serviceType);
        }
        /// <summary>
        /// 获取指定类型的服务对象。
        /// </summary>
        /// <param name="serviceType">一个对象，它指定要获取的服务对象的类型。。</param>
        /// <returns><paramref name="serviceType"/> 类型的服务对象。- 或 -如果没有 <paramref name="serviceType"/> 类型的服务对象，则为 null。</returns>
        public object GetService(Type serviceType)
        {
            return this.GetService(serviceType, null);
        }

        private readonly string UUID = Guid.NewGuid().ToString();

        /// <summary>
        /// 获取指定类型的服务对象。
        /// </summary>
        /// <param name="serviceType">一个对象，它指定要获取的服务对象的类型。。</param>
        /// <param name="lastMappingValues">后期映射的参数值数组。请保证数组顺序与构造函数的后期映射的参数顺序一致。</param>
        /// <returns><paramref name="serviceType"/> 类型的服务对象。- 或 -如果没有 <paramref name="serviceType"/> 类型的服务对象，则为 null。</returns>
        public virtual object GetService(Type serviceType, params object[] lastMappingValues)
        {
            if(serviceType == null) throw new ArgumentNullException("serviceType");

            //- 确保类型在当前容器的获取行为是独立的。
            lock(string.Intern(this.UUID + serviceType.AssemblyQualifiedName))
            {
                //- 通过非智能解析，获取实例盒
                var box = this.FindInstanceBox(serviceType, false);
                if(box == null)
                {
                    if(this._hasParent && this._parentLocator.ContainsService(serviceType, true))  //- 尝试从父级获取服务对象
                    {// 
                        var service = this._parentLocator.GetService(serviceType, lastMappingValues);
                        if(service != null) return service;
                    }
                    box = this.AutoResolveExpectType(serviceType); //- 尝试智能解析出实例盒
                }
                return box == null ? null : box.GetInstance(lastMappingValues);
            }
        }

        /// <summary>
        /// 查找服务容器是否包含指定的服务类型。
        /// </summary>
        /// <param name="serviceType">要查找的服务类型。</param>
        /// <param name="promote">true，则将此请求提升到任何父服务容器；否则为 false。</param>
        /// <returns>如果存在返回 true，否则返回 false。</returns>
        public virtual bool ContainsService(Type serviceType, bool promote = false)
        {
            if(serviceType == null) throw new ArgumentNullException("serviceType");
            return this.MapContains(serviceType)
                || (promote && this._hasParent && this._parentLocator.ContainsService(serviceType, promote));
        }

        /// <summary>
        /// 将指定的参数名和参数值添加到服务容器中。
        /// </summary>
        /// <param name="name">参数名称。</param>
        /// <param name="value">参数值。</param>
        /// <param name="promote">true，则将此请求提升到任何父服务容器；否则为 false。</param>
        public void AddValue(string name, object value, bool promote = false)
        {
            this.AddValue(name, lmp => value, true, promote);
        }
        /// <summary>
        /// 将指定的参数名和参数值添加到服务容器中。
        /// </summary>
        /// <param name="name">参数名称。</param>
        /// <param name="callback">用于创建参数的回调对象。这允许将参数声明为可用，但将值的创建延迟到请求该参数之后。</param>
        /// <param name="singletonMode">true，则启用单例模式；否则为 false。</param>
        /// <param name="promote">true，则将此请求提升到任何父服务容器；否则为 false。</param>
        public virtual void AddValue(string name, InstanceCreatorCallback callback, bool singletonMode = false, bool promote = false)
        {
            if(string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            if(callback == null) throw new ArgumentNullException("callback");

            if(this._hasParent && promote) this._parentLocator.AddValue(name, callback, singletonMode, promote);
            this.Map(name, singletonMode ? new SingletonInstanceBox(name, callback) : new InstanceBox(name, callback));
        }
        /// <summary>
        /// 从服务容器中移除指定的参数。
        /// </summary>
        /// <param name="name">要移除的参数名称。</param>
        /// <param name="promote">true，则将此请求提升到任何父服务容器；否则为 false。</param>
        public virtual void RemoveValue(string name, bool promote = false)
        {
            if(string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

            if(this._hasParent && promote) this._parentLocator.RemoveValue(name, promote);
            this.MapRemove(name);
        }
        /// <summary>
        /// 获取指定参数名称的值。
        /// </summary>
        /// <param name="name">参数名称。</param>
        /// <param name="lastMappingValues">后期映射的参数值数组。</param>
        /// <returns>返回参数名称的值。- 或 -如果没有参数名称的值，则为 null 值。</returns>
        public virtual object GetValue(string name, params object[] lastMappingValues)
        {
            if(string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

            var box = this.FindInstanceBox(name);
            if(box != null) return box.GetInstance(lastMappingValues);
            if(this._hasParent) return this._parentLocator.GetValue(name, lastMappingValues);
            return null;
        }
        /// <summary>
        /// 查找服务容器是否包含指定的参数。
        /// </summary>
        /// <param name="name">要查找的参数名称。</param>
        /// <param name="promote">true，则将此请求提升到任何父服务容器；否则为 false。</param>
        /// <returns>如果存在返回 true，否则返回 false。</returns>
        public virtual bool ContainsValue(string name, bool promote = false)
        {
            if(string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

            return this.MapContains(name)
                || (promote && this._hasParent && this._parentLocator.ContainsValue(name, promote));
        }

        /// <summary>
        /// 将指定的参数名和参数值添加到服务容器中，并绑定到关联的服务类型的构造函数。
        /// </summary>
        /// <param name="serviceType">关联的服务类型。</param>
        /// <param name="name">参数名称。</param>
        /// <param name="value">参数值。</param>
        /// <param name="promote">true，则将此请求提升到任何父服务容器；否则为 false。</param>
        public void AddValue(Type serviceType, string name, object value, bool promote = false)
        {
            this.AddValue(serviceType, name, lmp => value, true, promote);
        }
        /// <summary>
        /// 将指定的参数名和参数值添加到服务容器中，并绑定到关联的服务类型的构造函数。
        /// </summary>
        /// <param name="serviceType">关联的服务类型。</param>
        /// <param name="name">参数名称。</param>
        /// <param name="callback">用于创建参数的回调对象。这允许将参数声明为可用，但将值的创建延迟到请求该参数之后。</param>
        /// <param name="singletonMode">true，则启用单例模式；否则为 false。</param>
        /// <param name="promote">true，则将此请求提升到任何父服务容器；否则为 false。</param>
        public virtual void AddValue(Type serviceType, string name, InstanceCreatorCallback callback, bool singletonMode = false, bool promote = false)
        {
            if(serviceType == null) throw new ArgumentNullException("serviceType");
            if(string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");
            if(callback == null) throw new ArgumentNullException("callback");

            if(this._hasParent && promote) this._parentLocator.AddValue(serviceType, name, callback, singletonMode, promote);

            this.Map(serviceType, name, singletonMode ? new SingletonInstanceBox(name, callback) : new InstanceBox(name, callback));
        }
        /// <summary>
        /// 从服务容器中移除指定关联的服务类型的参数。
        /// </summary>
        /// <param name="serviceType">关联的服务类型。</param>
        /// <param name="name">要移除的参数名称。</param>
        /// <param name="promote">true，则将此请求提升到任何父服务容器；否则为 false。</param>
        public virtual void RemoveValue(Type serviceType, string name, bool promote = false)
        {
            if(serviceType == null) throw new ArgumentNullException("serviceType");
            if(string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

            if(this._hasParent && promote) this._parentLocator.RemoveValue(serviceType, name, promote);

            this.MapRemove(serviceType, name);
        }
        /// <summary>
        /// 获取指定关联的服务类型和参数名称的值。
        /// </summary>
        /// <param name="serviceType">关联的服务类型。</param>
        /// <param name="name">参数名称。</param>
        /// <param name="lastMappingValues">后期映射的参数值数组。</param>
        /// <returns>返回参数名称的值。- 或 -如果没有参数名称的值，则为 null 值。</returns>
        public virtual object GetValue(Type serviceType, string name, params object[] lastMappingValues)
        {
            if(serviceType == null) throw new ArgumentNullException("serviceType");
            if(string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

            var box = this.FindInstanceBox(serviceType, name);
            if(box != null) return box.GetInstance(lastMappingValues);
            if(this._hasParent) return this._parentLocator.GetValue(serviceType, name, lastMappingValues);

            return null;
        }
        /// <summary>
        /// 查找服务容器是否包含指定关联的服务类型指定的参数。
        /// </summary>
        /// <param name="serviceType">关联的服务类型。</param>
        /// <param name="name">要查找的参数名称。</param>
        /// <param name="promote">true，则将此请求提升到任何父服务容器；否则为 false。</param>
        /// <returns>如果存在返回 true，否则返回 false。</returns>
        public virtual bool ContainsValue(Type serviceType, string name, bool promote = false)
        {
            if(serviceType == null) throw new ArgumentNullException("serviceType");
            if(string.IsNullOrEmpty(name)) throw new ArgumentNullException("name");

            return this.MapContains(serviceType, name)
                || (promote && this._hasParent && this._parentLocator.ContainsValue(serviceType, name, promote));
        }

        /// <summary>
        /// 销毁所有的映射。
        /// </summary>
        public virtual void DestroyAll()
        {
            CacheTypeName.Clear();
            CacheName.Clear();
            CacheType.Clear();
            GC.WaitForFullGCComplete();
        }
    }
}
