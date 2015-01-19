using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System
{
    partial class IocContainer
    {
        /// <summary>
        /// 表示解析映射类型时发生。
        /// </summary>
        public event MapResolveEventHandler MapResolve;
        /// <summary>
        /// 表示解析后期映射类型时发生。
        /// </summary>
        public event MapResolveEventHandler LastMappingResolve;

        bool InnerMapCtor(Type actualType, Type expectedType, bool singletonMode
                                          , ConstructorInfo ctor, ParameterInfo[] ps
                                          , out InstanceBox typeInstance
                                          , out Exception exception)
        {
            bool hasExpectedType = expectedType != null;

            if(!singletonMode
                && (actualType.IsDefined(SingletonMappingAttribute.Type, true) || (hasExpectedType && expectedType.IsDefined(SingletonMappingAttribute.Type, true))))
                singletonMode = true;

            InstanceCreatorCallback callback = null;
            if(ps.Length == 0) callback = lastMappingValues => this.WrappingObject(Activator.CreateInstance(actualType, true));
            else
            {
                var psValues = new InstanceBox[ps.Length];
                for(int i = 0; i < ps.Length; i++)
                {
                    var p = ps[i];
                    var pType = p.ParameterType;
                    var pName = p.Name;
                    InstanceBox p_instance = null;
                    if(p.IsDefined(LastMappingAttribute.Type, true)
                        || (pType.IsDefined(LastMappingAttribute.Type, true) && !p.IsDefined(IgnoreAttribute.Type, true)))
                    {
                        p_instance = new InstanceBox(pName, null, pType);
                    }

                    #region 从当前容器获取

                    else if(this.MapContains(actualType, pName)) p_instance = this.FindInstanceBox(actualType, pName);
                    else if(hasExpectedType && this.MapContains(expectedType, pName)) p_instance = this.FindInstanceBox(expectedType, pName);

                    else if(this.MapContains(pName)) p_instance = this.FindInstanceBox(pName);

                    else if(this.MapContains(actualType)) p_instance = this.FindInstanceBox(actualType, false);
                    else if(hasExpectedType && this.MapContains(expectedType)) p_instance = this.FindInstanceBox(expectedType, false);

                    #endregion

                    #region 从父级容器获取

                    else if(this._hasParent && this._parentLocator.ContainsValue(actualType, pName, true)) p_instance = new InstanceBox(pName, lmp => this._parentLocator.GetValue(actualType, pName));
                    else if(this._hasParent && hasExpectedType && this._parentLocator.ContainsValue(expectedType, pName, true)) p_instance = new InstanceBox(pName, lmp => this._parentLocator.GetValue(expectedType, pName));

                    else if(this._hasParent && this._parentLocator.ContainsValue(pName, true)) p_instance = new InstanceBox(pName, lmp => this._parentLocator.GetValue(pName));
                    else if(this._hasParent && this._parentLocator.ContainsService(actualType, true)) p_instance = new InstanceBox(pName, lmp => this._parentLocator.GetService(actualType));
                    else if(this._hasParent && hasExpectedType && this._parentLocator.ContainsService(expectedType, true)) p_instance = new InstanceBox(pName, lmp => this._parentLocator.GetService(expectedType));

                    #endregion

                    else if(!pType.IsPrimitive && pType != Types.Decimal)
                    {
                        //- 从映射事件获取 -or- 尝试智能解析
                        p_instance = this.OnMapResolve(pType) ?? this.AutoResolveExpectType(pType);
                    }

                    if(p_instance == null)
                    {
                        exception = new ArgumentException(actualType.FullName + "：构造函数的参数“" + pName + "”尚未配置映射！", pName);
                        typeInstance = null;
                        return false;
                    }
                    psValues[i] = p_instance;
                }

                var cotrHandler = Aoite.Reflection.ConstructorInfoExtensions.DelegateForCreateInstance(ctor);
                callback = lastMappingValues =>
                {
                    System.Collections.IEnumerator cpe = null;
                    Func<bool> moveNextLastMap = () =>
                    {
                        if(cpe == null)
                        {
                            if(lastMappingValues == null) return false;
                            cpe = lastMappingValues.GetEnumerator();
                        }
                        return cpe.MoveNext();
                    };

                    object[] values = new object[psValues.Length];
                    for(int i = 0; i < psValues.Length; i++)
                    {
                        var instanceBox = psValues[i];
                        if(instanceBox.LastMappingType != null)
                        {
                            if(moveNextLastMap())
                            {
                                values[i] = cpe.Current;
                                continue;
                            }
                            var lastMappingBox = OnLastMappingResolve(instanceBox.LastMappingType);
                            if(lastMappingBox == null) throw new ArgumentException(actualType.FullName + "：构造函数的参数“" + instanceBox.Name + "”指定了后期映射关系，但调用方却没有传递映射值！", instanceBox.Name);
                            instanceBox = lastMappingBox;
                        }

                        values[i] = instanceBox.GetInstance();
                    }
                    return WrappingObject(cotrHandler(values));
                };
            }
            typeInstance = singletonMode ? new SingletonInstanceBox(actualType.FullName, callback) : new InstanceBox(actualType.FullName, callback);
            this.Map(expectedType ?? actualType, typeInstance);
            exception = null;
            return true;

        }
        private object WrappingObject(object obj)
        {
            //if(!GA.IsUnitTestRuntime && obj is IService)
            //{
            //    var context = Aoite.ServiceModel.ContractContext.Current;
            //    if(context != null)
            //        context.Source.Server.InstanceWapper(obj);
            //}
            return obj;
        }


        private InstanceBox InnerMapType(Type actualType, Type expectType = null, bool singletonMode = false)
        {
            if(actualType.IsAbstract || actualType.IsInterface || actualType.IsValueType)
                throw new NotSupportedException(actualType.FullName + "：不支持对基类、接口、值类型进行映射。可能原因：1、这是一个值类型；2、没有注册接口或基类的映射关联；3、通过默认规则没有找到接口的相同程序集中的关联类型。");

            var ctors = actualType.GetConstructors(Aoite.Reflection.Flags.InstanceAnyVisibility);

            InstanceBox typeInstance = null;
            Exception exception;
            if(ctors.Length == 1)
            {
                var ctor = ctors[0];
                if(!InnerMapCtor(actualType, expectType, singletonMode, ctor, ctor.GetParameters(), out typeInstance, out exception))
                    throw exception;
            }
            else
            {
                List<Exception> exceptions = new List<Exception>();
                foreach(var item in ctors.Select(ctor => new { ctor, ps = ctor.GetParameters() }).OrderByDescending(g => g.ps.Length))
                {
                    if(InnerMapCtor(actualType, expectType, singletonMode, item.ctor, item.ps, out typeInstance, out exception))
                    {
                        exceptions = null;
                        break;
                    }
                    else exceptions.Add(exception);
                }
                if(exceptions != null) throw new AggregateException(actualType.FullName + "没有在映射列表中找到匹配的构造函数。错误内容可以详见 System.Exception.Data 字典。", exceptions);
            }
            return typeInstance;

        }

        private InstanceBox OnMapResolve(Type expectType)
        {
            var e = ObjectFactory.InternalOnMapResolve(MapResolve, this, expectType)
                ?? ObjectFactory.InternalOnMapResolve(null, this, expectType);

            if(e != null)
            {
                return e.SingletonMode
                        ? new SingletonInstanceBox(expectType.FullName, e.Callback)
                        : new InstanceBox(expectType.FullName, e.Callback);
            }
            return null;
        }

        private InstanceBox OnLastMappingResolve(Type expectType)
        {
            var e = ObjectFactory.InternalOnLastMappingResolve(LastMappingResolve, this, expectType)
                ?? ObjectFactory.InternalOnLastMappingResolve(null, this, expectType);

            if(e != null)
            {
                return e.SingletonMode
                        ? new SingletonInstanceBox(expectType.FullName, e.Callback)
                        : new InstanceBox(expectType.FullName, e.Callback);
            }
            return null;
        }
        private InstanceBox AutoResolveExpectType(Type expectType, bool singletonMode = false)
        {
            if(this.DisabledAutoResolving) return null;

            Type actualType;
            var defaultMappingAttr = expectType.GetAttribute<DefaultMappingAttribute>();
            if(defaultMappingAttr != null)
            {
                actualType = defaultMappingAttr.ActualType;
            }
            else
            {
                //- 智能解析方式，不支持基类或值类型
                if((expectType.IsClass && expectType.IsAbstract) || expectType.IsValueType) return null;
                if(!expectType.IsInterface) return this.InnerMapType(expectType, null, singletonMode);

                //- 从映射事件获取
                var instance = this.OnMapResolve(expectType);
                if(instance != null)
                {
                    this.Map(expectType, instance);
                    return instance;
                }

                actualType = null;

                //- 从智能映射表中获取（当前程序集）
                var fullNames = DefaultMapFilter.GetAllActualType(expectType);
                foreach(var fullName in fullNames)
                {
                    actualType = expectType.Assembly.GetType(fullName, false);
                    if(actualType != null) break;
                }

                //- 从智能映射表中获取（所有程序集）
                if(actualType == null)
                {
                    actualType = DefaultMapFilter.FindActualType(ObjectFactory.AllTypes, expectType, fullNames);
                    if(actualType == null) return null;
                }
                //- 判定实际类型是否已注册
                if(this.CacheType.TryGetValue(actualType, out instance))
                {
                    this.Map(expectType, instance);
                    return instance;
                }
            }
            //- 解析实际类型和预期类型
            return this.InnerMapType(actualType, expectType, singletonMode);
        }
    }
}
