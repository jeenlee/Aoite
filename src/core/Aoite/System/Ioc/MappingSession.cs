//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Collections.Concurrent;
//using System.Reflection;

//namespace System
//{
//    using InstanceBox = System.ObjectFactory.InstanceBox;
//    using SingletonInstanceBox = System.ObjectFactory.SingletonInstanceBox;
//    public class MappingSession
//    {

//        /// <summary>
//        /// 表示一个缓存列表，映射到指定类型的构造函数的参数名称的实例盒。优先级最高（1）。
//        /// </summary>
//        internal readonly Lazy<ConcurrentDictionary<Type, ConcurrentDictionary<string, InstanceBox>>>
//            TypeNameCache = new Lazy<ConcurrentDictionary<Type, ConcurrentDictionary<string, InstanceBox>>>();
//        /// <summary>
//        /// 表示一个缓存列表，映射对应类型的实例盒。优先级中（2）。
//        /// </summary>
//        internal readonly Lazy<ConcurrentDictionary<Type, InstanceBox>>
//            TypeCache = new Lazy<ConcurrentDictionary<Type, InstanceBox>>();
//        /// <summary>
//        /// 表示一个缓存列表，映射所有类型的构造函数的参数名称的实例盒。优先级最低（3）。
//        /// </summary>
//        internal readonly Lazy<ConcurrentDictionary<string, InstanceBox>>
//            NameCache = new Lazy<ConcurrentDictionary<string, InstanceBox>>();
//        private MappingSession _Parent;
//        public MappingSession Parent { get { return _Parent; } }

//        public MappingSession() : this(null) { }
//        public MappingSession(MappingSession parent)
//        {
//            this._Parent = parent;
//        }

//        /// <summary>
//        /// 获取指定名称的映射实例。
//        /// </summary>
//        /// <param name="name">名称。</param>
//        /// <returns>返回一个实例。</returns>
//        public object GetNameValue(string name)
//        {
//            InstanceBox box;
//            if(NameCache.Value.TryGetValue(name, out box))
//            {
//                return box.GetInstance();
//            }
//            return this._Parent == null ? null : this._Parent.GetNameValue(name);
//        }

//        /// <summary>
//        /// 判断指定的类型是否已映射。
//        /// </summary>
//        /// <param name="type">映射的类型。</param>
//        /// <returns>如果存在返回 true，否则返回 false。</returns>
//        public bool Exists(Type type)
//        {
//            return TypeCache.Value.ContainsKey(type) || (this._Parent != null && this._Parent.Exists(type));
//        }

//        /// <summary>
//        /// 判断指定的类型是否已映射。
//        /// </summary>
//        /// <typeparam name="T">映射的类型。</typeparam>
//        /// <returns>如果存在返回 true，否则返回 false。</returns>
//        public bool Exists<T>()
//        {
//            return Exists(typeof(T));
//        }

//        /// <summary>
//        /// 销毁所有的映射。
//        /// </summary>
//        public void DestroyAll()
//        {
//            if(TypeNameCache.IsValueCreated) TypeNameCache.Value.Clear();
//            if(TypeCache.IsValueCreated) TypeCache.Value.Clear();
//            if(NameCache.IsValueCreated) NameCache.Value.Clear();
//        }

//        private void Map(Type type, InstanceBox box)
//        {
//            TypeCache.Value[type] = box;
//        }

//        private object HandlerOnContractObject(object obj)
//        {
//            if(!GA.IsUnitTestRuntime && obj is IService)
//            {
//                var context = Aoite.ServiceModel.ContractContext.Current;
//                if(context != null)
//                    context.Source.Server.InstanceWapper(obj);
//            }
//            return obj;
//        }
//        internal bool InnerMapCtor(Type actualType, Type defaultInterfaceType, bool singletonMode
//                                           , ConstructorInfo ctor, ParameterInfo[] ps
//                                           , out InstanceBox typeInstance
//                                           , out Exception exception)
//        {
//            string className = actualType.FullName;
//            if(!singletonMode && actualType.IsDefined(SingletonMappingAttribute.Type, true)) singletonMode = true;

//            InstanceCreateHandler handler = null;
//            if(ps.Length == 0) handler = lastMappingValues => HandlerOnContractObject(Activator.CreateInstance(actualType, true));
//            else
//            {
//                bool isFound = false;
//                ConcurrentDictionary<string, InstanceBox> typeObjectCaches = null;
//                Func<ConcurrentDictionary<string, InstanceBox>> getTypeObjectCaches = () =>
//                {
//                    if(!isFound)
//                    {
//                        if(!TypeNameCache.Value.TryGetValue(actualType, out typeObjectCaches)
//                            && (defaultInterfaceType == null
//                                || !TypeNameCache.Value.TryGetValue(defaultInterfaceType, out typeObjectCaches)))
//                        {
//                            foreach(var interfaceType in actualType.GetInterfaces())
//                            {
//                                if(TypeNameCache.Value.TryGetValue(interfaceType, out typeObjectCaches)) break;
//                            }
//                        }
//                        isFound = true;
//                    }
//                    return typeObjectCaches;
//                };

//                var psValues = new InstanceBox[ps.Length];
//                for(int i = 0; i < ps.Length; i++)
//                {
//                    var p = ps[i];
//                    var pType = p.ParameterType;
//                    InstanceBox p_instance;
//                    if(p.IsDefined(LastMappingAttribute.Type, true)
//                        || (pType.IsDefined(LastMappingAttribute.Type, true) && !p.IsDefined(IgnoreAttribute.Type, true)))
//                    {
//                        p_instance = new InstanceBox(p.Name, null) { IsLastMapping = true, LastMappintType = pType };
//                    }
//                    else if(pType.IsInterface)
//                    {
//                        if(!TypeCache.Value.TryGetValue(pType, out p_instance))
//                        {
//                            p_instance = this.FindInstanceBox(pType);
//                            if(p_instance == null)
//                            {
//                                exception = new ArgumentException(className + "：构造函数的参数类型“" + pType.FullName + "”尚未 Ioc 映射！", pType.FullName);
//                                typeInstance = null;
//                                return false;
//                            }
//                        }
//                    }
//                    else
//                    {
//                        if((getTypeObjectCaches() == null
//                             || !typeObjectCaches.TryGetValue(p.Name, out p_instance))
//                            && (!NameCache.Value.TryGetValue(p.Name, out p_instance)
//                                && !TypeCache.Value.TryGetValue(pType, out p_instance)))
//                        {
//                            exception = new ArgumentException(className + "：构造函数的参数“" + p.Name + "”尚未 Ioc 映射！", p.Name);
//                            typeInstance = null;
//                            return false;
//                        }
//                    }
//                    psValues[i] = p_instance;
//                }
//                var cotrHandler = Aoite.Reflection.Dynamic<object>.CreateHandler(ctor);
//                handler = lastMappingValues =>
//                {
//                    System.Collections.IEnumerator cpe = null;
//                    Func<bool> moveNextLastMap = () =>
//                    {
//                        if(cpe == null) cpe = (lastMappingValues ?? new string[0]).GetEnumerator();
//                        return cpe.MoveNext();
//                    };

//                    object[] values = new object[psValues.Length];
//                    for(int i = 0; i < psValues.Length; i++)
//                    {
//                        var instanceBox = psValues[i];
//                        if(instanceBox.IsLastMapping)
//                        {
//                            if(!moveNextLastMap())
//                            {
//                                // 当最后一个参数是一个 System.IDbEngine/IDbManager 时，允许为空值，系统将为以 Db.Engine/Manager 进行赋值。
//                                if(instanceBox.LastMappintType == Types.DbEngineManager) values[i] = Db.Manager;
//                                else if(instanceBox.LastMappintType == Types.IDbEngine) values[i] = Db.Engine;
//                                else throw new ArgumentException(className + "：构造函数的参数“" + instanceBox.Name + "”指定了后期映射关系，但调用方却没有传递映射值！", instanceBox.Name);
//                            }
//                            else values[i] = cpe.Current;
//                        }
//                        else
//                        {
//                            values[i] = instanceBox.GetInstance();
//                        }
//                    }
//                    return HandlerOnContractObject(cotrHandler(values));
//                };
//            }
//            typeInstance = singletonMode ? new SingletonInstanceBox(actualType.FullName, handler) : new InstanceBox(actualType.FullName, handler);
//            Map(actualType, typeInstance);
//            exception = null;
//            return true;

//        }
//        internal InstanceBox InnerMapType(Type actualType, Type expectType = null, bool singletonMode = false)
//        {
//            if(actualType.IsAbstract || actualType.IsInterface || actualType.IsValueType)
//                throw new Exception(actualType.FullName + "：不支持对基类、接口、值类型进行映射。可能原因：1、这是一个值类型；2、没有注册接口或基类的映射关联；3、通过默认规则没有找到接口的相同程序集中的关联类型。");

//            var ctors = actualType.GetConstructors(Aoite.Reflection.Flags.InstanceAnyVisibility);

//            InstanceBox typeInstance = null;
//            Exception exception;
//            if(ctors.Length == 1)
//            {
//                var ctor = ctors[0];
//                if(!InnerMapCtor(actualType, expectType, singletonMode, ctor, ctor.GetParameters(), out typeInstance, out exception))
//                    throw exception;
//            }
//            else
//            {
//                Dictionary<ConstructorInfo, Exception> exceptions = new Dictionary<ConstructorInfo, Exception>();
//                foreach(var item in ctors.Select(ctor => new { ctor, ps = ctor.GetParameters() }).OrderByDescending(g => g.ps.Length))
//                {
//                    if(InnerMapCtor(actualType, expectType, singletonMode, item.ctor, item.ps, out typeInstance, out exception))
//                    {
//                        exceptions = null;
//                        break;
//                    }
//                    else exceptions.Add(item.ctor, exception);
//                }
//                if(exceptions != null) throw new System.ObjectFactory.MapException(actualType.FullName + "没有在映射列表中找到匹配的构造函数。错误内容可以详见 System.Exception.Data 字典。", exceptions);
//            }
//            return typeInstance;

//        }

//        InstanceBox AutoResolveExpectType(Type expectType)
//        {
//            if(expectType.IsInterface)
//            {
//                Type actualType = null;
//                var fullNames = DefaultMapFilter.GetAllActualType(expectType);
//                foreach(var fullName in fullNames)
//                {
//                    actualType = expectType.Assembly.GetType(fullName, false);
//                    if(actualType != null) break;
//                }
//                if(actualType == null)
//                {
//                    actualType = DefaultMapFilter.FindActualType(ObjectFactory.GetAllTypes(), expectType, fullNames);
//                }

//                var instance = actualType != null
//                    ? InnerMapType(actualType, expectType)
//                    : OnMapResolve(expectType);

//                if(instance != null)
//                {
//                    Map(expectType, instance);
//                    return instance;
//                }
//            }
//            else if(Aoite.Data.Database.DatabaseType.IsAssignableFrom(expectType))
//            {
//                var instance = new InstanceBox(expectType.FullName, ps => Db.Engine.CreateDatabase(expectType, ps));
//                Map(expectType, instance);
//                return instance;
//            }
//            return InnerMapType(expectType);
//        }

//        InstanceBox FindInstanceBox(Type type)
//        {
//            return TypeCache.Value.GetOrAdd(type, t => AutoResolveExpectType(type));
//        }


//        /// <summary>
//        /// 表示解析映射类型时发生。
//        /// </summary>
//        public event MapResolveEventHandler MapResolve;

//        InstanceBox OnMapResolve(Type expectType)
//        {
//            var handler = MapResolve;
//            if(handler != null)
//            {
//                var e = new MapResolveEventArgs(expectType);
//                handler(e);
//                if(e.ActualHandler != null)
//                {
//                    return e.SingletonMode
//                            ? new SingletonInstanceBox(expectType.FullName, e.ActualHandler)
//                            : new InstanceBox(expectType.FullName, e.ActualHandler);
//                }
//            }
//            return null;
//        }

//    }
//}
