using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace System
{
    partial class IocContainer
    {
        private readonly static EmbeddedTypeAwareTypeComparer serviceTypeComparer = new EmbeddedTypeAwareTypeComparer();
        /// <summary>
        /// 表示一个缓存列表，映射到指定类型的构造函数的参数名称的实例盒。优先级最高（1）。
        /// </summary>
        private readonly ConcurrentDictionary<Type, ConcurrentDictionary<string, InstanceBox>> CacheTypeName = new ConcurrentDictionary<Type, ConcurrentDictionary<string, InstanceBox>>(serviceTypeComparer);
        /// <summary>
        /// 表示一个缓存列表，映射对应类型的实例盒。优先级中（2）。
        /// </summary>
        private readonly ConcurrentDictionary<Type, InstanceBox> CacheType = new ConcurrentDictionary<Type, InstanceBox>(serviceTypeComparer);
        /// <summary>
        /// 表示一个缓存列表，映射所有类型的构造函数的参数名称的实例盒。优先级最低（3）。
        /// </summary>
        private readonly ConcurrentDictionary<string, InstanceBox> CacheName = new ConcurrentDictionary<string, InstanceBox>();

        private void Map(Type type, InstanceBox box)
        {
            CacheType[type] = box;
        }
        private void MapRemove(Type type)
        {
            InstanceBox box;
            CacheType.TryRemove(type, out box);
        }
        private bool MapContains(Type type)
        {
            return CacheType.ContainsKey(type);
        }
        private InstanceBox FindInstanceBox(Type type, bool autoResolve)
        {
            InstanceBox box;
            if(!CacheType.TryGetValue(type, out box))
                lock(type)
                {
                    if(!CacheType.TryGetValue(type, out box))
                    {
                        if(autoResolve) box = AutoResolveExpectType(type);
                    }
                }
            return box;
        }

        private void Map(string name, InstanceBox box)
        {
            CacheName[name] = box;
        }
        private void MapRemove(string name)
        {
            InstanceBox box;
            CacheName.TryRemove(name, out box);
        }
        private bool MapContains(string name)
        {
            return CacheName.ContainsKey(name);
        }
        private InstanceBox FindInstanceBox(string name)
        {
            InstanceBox box;
            if(CacheName.TryGetValue(name, out box)) return box;
            return null;
        }

        private void Map(Type type, string name, InstanceBox box)
        {
            ConcurrentDictionary<string, InstanceBox> typeObjectCaches = null;
            typeObjectCaches = CacheTypeName.GetOrAdd(type, (t) => new ConcurrentDictionary<string, InstanceBox>());
            typeObjectCaches[name] = box;
        }
        private void MapRemove(Type type, string name)
        {
            ConcurrentDictionary<string, InstanceBox> typeObjectCaches = null;
            lock(type)
            {
                if(!CacheTypeName.TryGetValue(type, out typeObjectCaches)) return;
                InstanceBox box;
                typeObjectCaches.TryRemove(name, out box);
                if(typeObjectCaches.Count == 0) CacheTypeName.TryRemove(type, out typeObjectCaches);
            }
        }
        private bool MapContains(Type type, string name)
        {
            ConcurrentDictionary<string, InstanceBox> typeObjectCaches = null;
            lock(type)
            {
                if(!CacheTypeName.TryGetValue(type, out typeObjectCaches)) return false;
                return typeObjectCaches.ContainsKey(name);
            }
        }
        private InstanceBox FindInstanceBox(Type type, string name)
        {
            ConcurrentDictionary<string, InstanceBox> typeObjectCaches = null;
            lock(type)
            {
                if(!CacheTypeName.TryGetValue(type, out typeObjectCaches)) return null;
                InstanceBox box;
                if(typeObjectCaches.TryGetValue(name, out box)) return box;
            }
            return null;
        }
    }
}
