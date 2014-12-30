using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Aoite.Reflection
{
    /// <summary>
    /// 表示字段的动态实现。
    /// </summary>
    /// <typeparam name="I">实例的类型。</typeparam>
    public class DynamicFieldHelper<I> : DynamicHelperBase<I>
    {
        #region Constructors

        /// <summary>
        /// 初始化一个 <see cref="Aoite.Reflection.DynamicFieldHelper&lt;I&gt;"/> 类的新实例。
        /// </summary>
        public DynamicFieldHelper() { }

        /// <summary>
        /// 指定绑定动态实现的实例，初始化一个 <see cref="Aoite.Reflection.DynamicFieldHelper&lt;I&gt;"/> 类的新实例。
        /// </summary>
        /// <param name="instance">绑定的实例。可以为 null，表示动态实现 <typeparamref name="I"/> 的静态成员（或不设实例）。不为 null 的实例也可以用于以下情况：实现在父类中动态调用派生类的成员。</param>
        public DynamicFieldHelper(I instance) : base(instance) { }

        /// <summary>
        /// 指定绑定动态实现的实例类型，初始化一个 <see cref="Aoite.Reflection.DynamicFieldHelper&lt;I&gt;"/> 类的新实例。
        /// </summary>
        /// <param name="instanceType">实例类型。</param>
        public DynamicFieldHelper(Type instanceType) : base(instanceType) { }

        #endregion Constructors

        #region Indexers

        /// <summary>
        /// 指定字段名，获取或设置一个值，动态实现实例的字段值。
        /// </summary>
        /// <param name="fieldName">字段名。</param>
        /// <returns>返回字段值。</returns>
        public object this[string fieldName]
        {
            get
            {
                return this[this.GetFields(fieldName)];
            }
            set
            {
                this[this.GetFields(fieldName)] = value;
            }
        }

        /// <summary>
        /// 指定字段名和绑定标识，获取或设置一个值，动态实现实例的字段值。
        /// </summary>
        /// <param name="fieldName">字段名。</param>
        /// <param name="bindingAttr">字段绑定标识。</param>
        /// <returns>返回字段值。</returns>
        public object this[string fieldName, BindingFlags bindingAttr]
        {
            get
            {
                return this[this.GetFields(fieldName, bindingAttr)];
            }
            set
            {
                this[this.GetFields(fieldName, bindingAttr)] = value;
            }
        }

        /// <summary>
        /// 指定字段元数据，获取或设置一个值，动态实现实例的字段值。
        /// </summary>
        /// <param name="fieldInfo">字段元数据。</param>
        /// <returns>返回字段值。</returns>
        public object this[FieldInfo fieldInfo]
        {
            get
            {
                return this.CreateGetterHandler(fieldInfo)(this._Instance);
            }
            set
            {
                this.CreateSetterHandler(fieldInfo)(this._Instance, value);
            }
        }

        #endregion Indexers

        #region Methods

        /// <summary>
        /// 创建一个指定字段名的获取器工厂委托。
        /// </summary>
        /// <param name="fieldName">字段名。</param>
        /// <returns>返回一个动态字段获取器的工厂委托。</returns>
        public MemberGetter CreateGetterHandler(string fieldName)
        {
            return this.CreateGetterHandler(this.GetFields(fieldName));
        }

        /// <summary>
        /// 创建一个指定字段名和绑定标识的获取器工厂委托。
        /// </summary>
        /// <param name="fieldName">字段名。</param>
        /// <param name="bindingAttr">字段绑定标识。</param>
        /// <returns>返回一个动态字段获取器的工厂委托。</returns>
        public MemberGetter CreateGetterHandler(string fieldName, BindingFlags bindingAttr)
        {
            return this.CreateGetterHandler(this.GetFields(fieldName, bindingAttr));
        }

        /// <summary>
        /// 创建一个指定字段元数据的获取器工厂委托。
        /// </summary>
        /// <param name="fieldInfo">字段元数据。</param>
        /// <returns>返回一个动态字段获取器的工厂委托。</returns>
        public MemberGetter CreateGetterHandler(FieldInfo fieldInfo)
        {
            return fieldInfo.DelegateForGetFieldValue();
        }

        /// <summary>
        /// 创建一个指定字段名的设置器工厂委托。
        /// </summary>
        /// <param name="fieldName">字段名。</param>
        /// <returns>返回一个动态字段设置器的工厂委托。</returns>
        public MemberSetter CreateSetterHandler(string fieldName)
        {
            return this.CreateSetterHandler(this.GetFields(fieldName));
        }

        /// <summary>
        /// 创建一个指定字段名和绑定标识的设置器工厂委托。
        /// </summary>
        /// <param name="fieldName">字段名。</param>
        /// <param name="bindingAttr">字段绑定标识。</param>
        /// <returns>返回一个动态字段设置器的工厂委托。</returns>
        public MemberSetter CreateSetterHandler(string fieldName, BindingFlags bindingAttr)
        {
            return this.CreateSetterHandler(this.GetFields(fieldName, bindingAttr));
        }

        /// <summary>
        /// 创建一个指定字段元数据的设置器工厂委托。
        /// </summary>
        /// <param name="fieldInfo">字段元数据。</param>
        /// <returns>返回一个动态字段设置器的工厂委托。</returns>
        public MemberSetter CreateSetterHandler(FieldInfo fieldInfo)
        {
            return fieldInfo.DelegateForSetFieldValue();
        }

        /// <summary>
        /// 获取指定字段名的字段元数据。
        /// </summary>
        /// <param name="fieldName">字段名。</param>
        /// <returns>返回一个字段的元数据</returns>
        public FieldInfo GetFields(string fieldName)
        {
            return this.GetFields(fieldName, Flags.InstanceAnyVisibility);
        }

        /// <summary>
        /// 获取指定字段名和绑定标识的字段元数据。
        /// </summary>
        /// <param name="fieldName">字段名。</param>
        /// <param name="bindingAttr">字段绑定标识。</param>
        /// <returns>返回一个字段的元数据</returns>
        public FieldInfo GetFields(string fieldName, BindingFlags bindingAttr)
        {
            return this._InstanceType.GetField(fieldName, bindingAttr);
        }

        /// <summary>
        /// 获取所有公共的字段。
        /// </summary>
        /// <returns>返回多个字段的元数据。</returns>
        public FieldInfo[] GetFields()
        {
            return this._InstanceType.GetFields();
        }

        /// <summary>
        /// 获取指定字段绑定标识的多个字段。
        /// </summary>
        /// <param name="bindingAttr">字段绑定标识。</param>
        /// <returns>返回多个字段的元数据。</returns>
        public FieldInfo[] GetFields(BindingFlags bindingAttr)
        {
            return this._InstanceType.GetFields(bindingAttr);
        }

        #endregion Methods
    }
}