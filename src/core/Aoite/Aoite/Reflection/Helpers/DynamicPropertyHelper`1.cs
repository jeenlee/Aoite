using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Aoite.Reflection
{
    /// <summary>
    /// 表示属性的动态实现。
    /// </summary>
    /// <typeparam name="I">实例的类型。</typeparam>
    public class DynamicPropertyHelper<I> : DynamicHelperBase<I>
    {
        #region Constructors

        /// <summary>
        /// 初始化一个 <see cref="Aoite.Reflection.DynamicPropertyHelper&lt;I&gt;"/> 类的新实例。
        /// </summary>
        public DynamicPropertyHelper() { }

        /// <summary>
        /// 指定绑定动态实现的实例，初始化一个 <see cref="Aoite.Reflection.DynamicPropertyHelper&lt;I&gt;"/> 类的新实例。
        /// </summary>
        /// <param name="instance">绑定的实例。可以为 null，表示动态实现 <typeparamref name="I"/> 的静态成员（或不设实例）。不为 null 的实例也可以用于以下情况：实现在父类中动态调用派生类的成员。</param>
        public DynamicPropertyHelper(I instance) : base(instance) { }

        /// <summary>
        /// 指定绑定动态实现的实例类型，初始化一个 <see cref="Aoite.Reflection.DynamicPropertyHelper&lt;I&gt;"/> 类的新实例。
        /// </summary>
        /// <param name="instanceType">实例类型。</param>
        public DynamicPropertyHelper(Type instanceType) : base(instanceType) { }

        #endregion Constructors

        #region Indexers

        /// <summary>
        /// 指定属性名，获取或设置一个值，动态实现实例的字段值。
        /// </summary>
        /// <param name="propertyName">属性名。</param>
        /// <returns>返回属性值。</returns>
        public object this[string propertyName]
        {
            get
            {
                return this[this.GetProperty(propertyName)];
            }
            set
            {
                this[this.GetProperty(propertyName)] = value;
            }
        }

        /// <summary>
        /// 指定属性名名和绑定标识，获取或设置一个值，动态实现实例的属性值。
        /// </summary>
        /// <param name="propertyName">属性名。</param>
        /// <param name="bindingAttr">属性绑定标识。</param>
        /// <returns>返回属性值。</returns>
        public object this[string propertyName, BindingFlags bindingAttr]
        {
            get
            {
                return this[this.GetProperty(propertyName, bindingAttr)];
            }
            set
            {
                this[this.GetProperty(propertyName, bindingAttr)] = value;
            }
        }

        /// <summary>
        /// 指定属性元数据，获取或设置一个值，动态实现实例的属性值。
        /// </summary>
        /// <param name="propertyInfo">属性元数据。</param>
        /// <returns>返回属性值。</returns>
        public object this[PropertyInfo propertyInfo]
        {
            get
            {
                return this.CreateGetterHandler(propertyInfo)(this._Instance);
            }
            set
            {
                this.CreateSetterHandler(propertyInfo)(this._Instance, value);
            }
        }

        #endregion Indexers

        #region Methods

        /// <summary>
        /// 创建一个指定属性名的获取器工厂委托。
        /// </summary>
        /// <param name="propertyName">属性名。</param>
        /// <returns>返回一个动态属性获取器的工厂委托。</returns>
        public MemberGetter CreateGetterHandler(string propertyName)
        {
            return this.CreateGetterHandler(this.GetProperty(propertyName));
        }

        /// <summary>
        /// 创建一个指定属性名和绑定标识的获取器工厂委托。
        /// </summary>
        /// <param name="propertyName">属性名。</param>
        /// <param name="bindingAttr">属性绑定标识。</param>
        /// <returns>返回一个动态属性获取器的工厂委托。</returns>
        public MemberGetter CreateGetterHandler(string propertyName, BindingFlags bindingAttr)
        {
            return this.CreateGetterHandler(this.GetProperty(propertyName, bindingAttr));
        }

        /// <summary>
        /// 创建一个指定属性元数据的读取器工厂委托。
        /// </summary>
        /// <param name="propertyInfo">属性元数据。</param>
        /// <returns>返回一个动态属性读取器的工厂委托。</returns>
        public MemberGetter CreateGetterHandler(PropertyInfo propertyInfo)
        {
            if(propertyInfo == null) return null;
            else if(propertyInfo.CanRead) return propertyInfo.DelegateForGetPropertyValue();
            throw new MethodAccessException(string.Format("{0}::{1} 没有实现 get 访问器！", propertyInfo.DeclaringType.Name, propertyInfo.Name));
        }

        /// <summary>
        /// 创建一个指定属性名的设置器工厂委托。
        /// </summary>
        /// <param name="propertyName">属性名。</param>
        /// <returns>返回一个动态属性设置器的工厂委托。</returns>
        public MemberSetter CreateSetterHandler(string propertyName)
        {
            return this.CreateSetterHandler(this.GetProperty(propertyName));
        }

        /// <summary>
        /// 创建一个指定属性名和绑定标识的设置器工厂委托。
        /// </summary>
        /// <param name="propertyName">属性名。</param>
        /// <param name="bindingAttr">属性绑定标识。</param>
        /// <returns>返回一个动态属性设置器的工厂委托。</returns>
        public MemberSetter CreateSetterHandler(string propertyName, BindingFlags bindingAttr)
        {
            return this.CreateSetterHandler(this.GetProperty(propertyName, bindingAttr));
        }

        /// <summary>
        /// 创建一个指定属性元数据的设置器工厂委托。
        /// </summary>
        /// <param name="propertyInfo">属性元数据。</param>
        /// <returns>返回一个动态属性设置器的工厂委托。</returns>
        public MemberSetter CreateSetterHandler(PropertyInfo propertyInfo)
        {
            if(propertyInfo == null) return null;
            else if(propertyInfo.CanWrite) return propertyInfo.DelegateForSetPropertyValue();
            throw new MethodAccessException(string.Format("{0}::{1} 没有实现 set 访问器！", propertyInfo.DeclaringType.Name, propertyInfo.Name));
        }

        /// <summary>
        /// 获取所有公共的属性。
        /// </summary>
        /// <returns>返回多个属性的元数据。</returns>
        public PropertyInfo[] GetProperties()
        {
            return this._InstanceType.GetProperties();
        }

        /// <summary>
        /// 获取指定属性绑定标识的多个属性。
        /// </summary>
        /// <param name="bindingAttr">属性绑定标识。</param>
        /// <returns>返回多个属性的元数据。</returns>
        public PropertyInfo[] GetProperties(BindingFlags bindingAttr)
        {
            return this._InstanceType.GetProperties(bindingAttr);
        }

        /// <summary>
        /// 获取指定属性名的属性元数据。
        /// </summary>
        /// <param name="propertyName">属性名。</param>
        /// <returns>返回一个属性的元数据</returns>
        public PropertyInfo GetProperty(string propertyName)
        {
            return this.GetProperty(propertyName, Flags.InstanceAnyVisibility);
        }

        /// <summary>
        /// 获取指定属性名的属性元数据。
        /// </summary>
        /// <param name="propertyName">属性名。</param>
        /// <param name="bindingAttr">属性绑定标识。</param>
        /// <returns>返回一个属性的元数据</returns>
        public PropertyInfo GetProperty(string propertyName, BindingFlags bindingAttr)
        {
            return this._InstanceType.GetProperty(propertyName, bindingAttr);
        }

        #endregion Methods
    }
}