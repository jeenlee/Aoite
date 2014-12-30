using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Aoite.Reflection
{
    /// <summary>
    /// 表示动态调用指定实例的 方法 或 构造函数 的委托。
    /// </summary>
    /// <param name="parameters">方法或构造函数的参数值。</param>
    /// <returns>返回一个实例或一个方法的返回值。</returns>
    public delegate object LocalMethodInvoker(params object[] parameters);

    /// <summary>
    /// 表示方法的动态实现。
    /// </summary>
    /// <typeparam name="I">实例的类型。</typeparam>
    public class DynamicMethodHelper<I> : DynamicHelperBase<I>
    {
        /// <summary>
        /// 初始化一个 <see cref="Aoite.Reflection.DynamicMethodHelper&lt;I&gt;"/> 类的新实例。
        /// </summary>
        public DynamicMethodHelper() { }

        /// <summary>
        /// 指定绑定动态实现的实例，初始化一个 <see cref="Aoite.Reflection.DynamicMethodHelper&lt;I&gt;"/> 类的新实例。
        /// </summary>
        /// <param name="instance">绑定的实例。可以为 null，表示动态实现 <typeparamref name="I"/> 的静态成员（或不设实例）。不为 null 的实例也可以用于以下情况：实现在父类中动态调用派生类的成员。</param>
        public DynamicMethodHelper(I instance) : base(instance) { }

        /// <summary>
        /// 指定绑定动态实现的实例类型，初始化一个 <see cref="Aoite.Reflection.DynamicMethodHelper&lt;I&gt;"/> 类的新实例。
        /// </summary>
        /// <param name="instanceType">实例类型。</param>
        public DynamicMethodHelper(Type instanceType) : base(instanceType) { }

        #region This Methods

        /// <summary>
        /// 获取指定方法名和参数类型的动态方法委托。
        /// </summary>
        /// <param name="methodName">方法名。</param>
        /// <param name="types">方法参数的类型集合。</param>
        /// <returns>返回一个动态绑定的方法委托。</returns>
        public LocalMethodInvoker this[string methodName, params Type[] types]
        {
            get
            {
                return this[this.GetMethodInfo(methodName, types)];
            }
        }

        /// <summary>
        /// 获取指定方法名、绑定标识和参数类型的动态方法委托。
        /// </summary>
        /// <param name="methodName">方法名。</param>
        /// <param name="bindingAttr">方法绑定标识。</param>
        /// <param name="types">方法参数的类型集合。</param>
        /// <returns>返回一个动态绑定的方法委托。</returns>
        public LocalMethodInvoker this[string methodName, BindingFlags bindingAttr, params Type[] types]
        {
            get
            {
                return this[this.GetMethodInfo(methodName, bindingAttr, types)];
            }
        }

        /// <summary>
        /// 获取指定方法元数据的动态方法委托。
        /// </summary>
        /// <param name="methodInfo">方法元数据。</param>
        /// <returns>返回一个动态绑定的方法委托。</returns>
        public LocalMethodInvoker this[MethodInfo methodInfo]
        {
            get
            {
                var handler = this.CreateHandler(methodInfo);
                return parameters =>
                {
                    return handler(this._Instance, parameters);
                };
            }
        }

        #endregion

        #region Invoke Methods

        /// <summary>
        /// 动态调用方法。
        /// </summary>
        /// <param name="methodName">方法名。</param>
        /// <param name="parameters">参数对应的值。</param>
        /// <returns>返回实例方法调用的返回值。</returns>
        public object Invoke(string methodName, params object[] parameters)
        {
            return this.Invoke(this.GetMethodInfo(methodName), parameters);
        }

        /// <summary>
        /// 动态调用指定参数类型的方法。
        /// </summary>
        /// <param name="methodName">方法名。</param>
        /// <param name="types">方法参数的类型集合。</param>
        /// <param name="parameters">参数对应的值。</param>
        /// <returns>返回实例方法调用的返回值。</returns>
        public object Invoke(string methodName, Type[] types, params object[] parameters)
        {
            return this.Invoke(this.GetMethodInfo(methodName, types), parameters);
        }

        /// <summary>
        /// 动态调用指定方法绑定标识的方法。
        /// </summary>
        /// <param name="methodName">方法名。</param>
        /// <param name="bindingAttr">方法绑定标识。</param>
        /// <param name="parameters">参数对应的值。</param>
        /// <returns>返回实例方法调用的返回值。</returns>
        public object Invoke(string methodName, BindingFlags bindingAttr, params object[] parameters)
        {
            return this.Invoke(this.GetMethodInfo(methodName, bindingAttr), parameters);
        }

        /// <summary>
        /// 动态调用指定方法绑定标识和参数类型的方法。
        /// </summary>
        /// <param name="methodName">方法名。</param>
        /// <param name="bindingAttr">方法绑定标识。</param>
        /// <param name="types">方法参数的类型集合。</param>
        /// <param name="parameters">参数对应的值。</param>
        /// <returns>返回实例方法调用的返回值。</returns>
        public object Invoke(string methodName, BindingFlags bindingAttr, Type[] types, params object[] parameters)
        {
            return this.Invoke(this.GetMethodInfo(methodName, bindingAttr, types), parameters);
        }

        /// <summary>
        /// 动态调用指定方法元数据的方法。
        /// </summary>
        /// <param name="methodInfo">方法元数据。</param>
        /// <param name="parameters">参数对应的值。</param>
        /// <returns>返回实例方法调用的返回值。</returns>
        public object Invoke(MethodInfo methodInfo, params object[] parameters)
        {
            return this.CreateHandler(methodInfo)(this._Instance, parameters);
        }

        #endregion

        #region Create Handler

        /// <summary>
        /// 创建一个指定方法名的工厂委托。
        /// </summary>
        /// <param name="methodName">方法名。</param>
        /// <returns>返回一个动态创建实例的工厂委托。</returns>
        public MethodInvoker CreateHandler(string methodName)
        {
            return this.CreateHandler(this.GetMethodInfo(methodName));
        }

        /// <summary>
        /// 创建一个指定方法名和绑定标识的工厂委托。
        /// </summary>
        /// <param name="methodName">方法名。</param>
        /// <param name="bindingAttr">方法绑定标识。</param>
        /// <returns>返回一个动态创建实例的工厂委托。</returns>
        public MethodInvoker CreateHandler(string methodName, BindingFlags bindingAttr)
        {
            return this.CreateHandler(this.GetMethodInfo(methodName, bindingAttr));
        }

        /// <summary>
        /// 创建一个指定方法名和参数类型的工厂委托。
        /// </summary>
        /// <param name="methodName">方法名。</param>
        /// <param name="types">方法参数的类型集合。</param>
        /// <returns>返回一个动态创建实例的工厂委托。</returns>
        public MethodInvoker CreateHandler(string methodName, params Type[] types)
        {
            return this.CreateHandler(this.GetMethodInfo(methodName, types));
        }

        /// <summary>
        /// 创建一个指定方法名、绑定标识和参数类型的工厂委托。
        /// </summary>
        /// <param name="methodName">方法名。</param>
        /// <param name="bindingAttr">方法绑定标识。</param>
        /// <param name="types">方法参数的类型集合。</param>
        /// <returns>返回一个动态创建实例的工厂委托。</returns>
        public MethodInvoker CreateHandler(string methodName, BindingFlags bindingAttr, params Type[] types)
        {
            return this.CreateHandler(this.GetMethodInfo(methodName, bindingAttr, types));
        }

        /// <summary>
        /// 创建一个指定方法元数据的工厂委托。
        /// </summary>
        /// <param name="methodInfo">方法元数据。</param>
        /// <returns>返回一个动态创建实例的工厂委托。</returns>
        public MethodInvoker CreateHandler(MethodInfo methodInfo)
        {
            return methodInfo.DelegateForCallMethod();
        }

        #endregion

        #region Get Method/s

        /// <summary>
        /// 获取指定方法名的方法元数据。
        /// </summary>
        /// <param name="methodName">方法名。</param>
        /// <returns>返回一个方法的元数据。</returns>
        public MethodInfo GetMethodInfo(string methodName)
        {
            return this._InstanceType.GetMethod(methodName);
        }

        /// <summary>
        /// 获取指定方法名和绑定标识的方法元数据。
        /// </summary>
        /// <param name="methodName">方法名。</param>
        /// <param name="bindingAttr">方法绑定标识。</param>
        /// <returns>返回一个方法的元数据。</returns>
        public MethodInfo GetMethodInfo(string methodName, BindingFlags bindingAttr)
        {
            return this._InstanceType.GetMethod(methodName, bindingAttr);
        }

        /// <summary>
        /// 获取指定方法名和参数类型的方法元数据。
        /// </summary>
        /// <param name="methodName">方法名。</param>
        /// <param name="types">方法参数的类型集合。</param>
        /// <returns>返回一个方法的元数据。</returns>
        public MethodInfo GetMethodInfo(string methodName, params Type[] types)
        {
            return this.GetMethodInfo(methodName, Flags.InstanceAnyVisibility, types);
        }

        /// <summary>
        /// 获取指定方法名、绑定标识和参数类型的方法元数据。
        /// </summary>
        /// <param name="methodName">方法名。</param>
        /// <param name="bindingAttr">方法绑定标识。</param>
        /// <param name="types">方法参数的类型集合。</param>
        /// <returns>返回一个方法的元数据。</returns>
        public MethodInfo GetMethodInfo(string methodName, BindingFlags bindingAttr, params Type[] types)
        {
            return this._InstanceType.GetMethod(methodName, bindingAttr, null, CallingConventions.Any, types, null);
        }

        /// <summary>
        /// 获取所有公共的方法。
        /// </summary>
        /// <returns>返回多个方法的元数据。</returns>
        public MethodInfo[] GetMethodInfos()
        {
            return this._InstanceType.GetMethods();
        }

        /// <summary>
        /// 获取指定方法绑定标识的多个方法。
        /// </summary>
        /// <param name="bindingAttr">方法绑定标识。</param>
        /// <returns>返回多个方法的元数据。</returns>
        public MethodInfo[] GetMethodInfos(BindingFlags bindingAttr)
        {
            return this._InstanceType.GetMethods(bindingAttr);
        }

        #endregion

    }
}
