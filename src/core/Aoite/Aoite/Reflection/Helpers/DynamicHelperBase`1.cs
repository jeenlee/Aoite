namespace Aoite.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// 表示成员动态实现的基类。
    /// </summary>
    /// <typeparam name="I">实例的类型。</typeparam>
    public abstract class DynamicHelperBase<I>
    {
        #region Fields

        /// <summary>
        /// 表示动态实现的实例。
        /// </summary>
        protected object _Instance;

        /// <summary>
        /// 表示动态实现实例的类型。
        /// </summary>
        protected Type _InstanceType;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// 初始化一个 <see cref="Aoite.Reflection.DynamicHelperBase&lt;I&gt;"/> 类的新实例。
        /// </summary>
        protected DynamicHelperBase()
            : this(typeof(I))
        {
        }

        /// <summary>
        /// 指定绑定动态实现的实例，初始化一个 <see cref="Aoite.Reflection.DynamicHelperBase&lt;I&gt; "/> 类的新实例。
        /// </summary>
        /// <param name="instance">绑定的实例。可以为 null，表示动态实现 <typeparamref name="I"/> 的静态成员（或不设实例）。不为 null 的实例也可以用于以下情况：实现在父类中动态调用派生类的成员。</param>
        protected DynamicHelperBase(I instance)
            : this(instance == null ? typeof(I) : instance.GetType())
        {
            this._Instance = instance;
        }

        /// <summary>
        /// 指定绑定动态实现的实例类型，初始化一个 <see cref="Aoite.Reflection.DynamicHelperBase&lt;I&gt; "/> 类的新实例。
        /// </summary>
        /// <param name="instanceType">实例类型。</param>
        protected DynamicHelperBase(Type instanceType)
        {
            if(instanceType == null) throw new ArgumentNullException("instanceType");
            this._InstanceType = instanceType;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// 获取或设置一个值，表示动态实现的实例。
        /// </summary>
        public object Instance
        {
            get { return this._Instance; }
        }

        /// <summary>
        /// 获取或设置一个值，表示动态实现实例的类型。
        /// </summary>
        public Type InstanceType
        {
            get
            {
                return this._InstanceType;
            }
        }

        #endregion Properties
    }
}