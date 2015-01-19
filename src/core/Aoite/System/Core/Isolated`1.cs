using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System
{
    /// <summary>
    /// 表示一个跨应用程序域孤立的对象。
    /// </summary>
    /// <typeparam name="T">对象的数据类型。</typeparam>
    public class Isolated<T> : ObjectDisposableBase where T : MarshalByRefObject
    {
        private readonly static Type TType = typeof(T);
        private AppDomain _domain;

        /// <summary>
        /// 初始化一个 <see cref="System.Isolated&lt;T&gt;"/> 类的新实例。
        /// </summary>
        public Isolated() : this(TType.Assembly.FullName, TType.FullName) { }

        /// <summary>
        /// 提供程序集的显示名称和类型的完全限定名称，初始化一个 <see cref="System.Isolated&lt;T&gt;"/> 类的新实例。
        /// </summary>
        /// <param name="assemblyFullName">程序集的显示名称。</param>
        /// <param name="typeFullName">类型的完全限定名称，包含命名空间而不是程序集。</param>
        public Isolated(string assemblyFullName, string typeFullName)
        {
            if(string.IsNullOrEmpty(assemblyFullName)) throw new ArgumentNullException("assemblyFullName");
            if(string.IsNullOrEmpty(typeFullName)) throw new ArgumentNullException("typeFullName");
            this._domain = AppDomain.CreateDomain("Isolated:" + Guid.NewGuid(), null, AppDomain.CurrentDomain.SetupInformation);
            this._Instance = new Lazy<T>(() => (T)this._domain.CreateInstanceAndUnwrap(assemblyFullName, typeFullName));
            //this._domain.ReflectionOnlyAssemblyResolve += _domain_ReflectionOnlyAssemblyResolve;
        }

        private Lazy<T> _Instance;
        /// <summary>
        /// 获取孤立的对象实例。
        /// </summary>
        public T Instance
        {
            get
            {
                this.ThrowWhenDisposed();
                return this._Instance.Value;
            }
        }

        /// <summary>
        /// 执行与释放或重置托管资源相关的应用程序定义的任务。
        /// </summary>
        protected override void DisposeManaged()
        {
            this._Instance = null;
            AppDomain.Unload(this._domain);
            this._domain = null;
        }
    }

    /// <summary>
    /// 表示一个应用程序域孤立的代理。
    /// </summary>
    public class IsolatedProxy : MarshalByRefObject
    {
        /// <summary>
        /// 初始化一个 <see cref="System.IsolatedProxy"/> 类的新实例。
        /// </summary>
        public IsolatedProxy() { }

        /// <summary>
        /// 加载指定路径的程序集。
        /// </summary>
        /// <param name="assemblyPath">包含程序集清单的文件的路径。</param>
        public void LoadAssembly(string assemblyPath)
        {
            try
            {
                Assembly.ReflectionOnlyLoadFrom(assemblyPath);
            }
            catch(FileNotFoundException) { }
        }

        /// <summary>
        /// 执行一个孤立的回调方法。
        /// </summary>
        /// <param name="callback">回调方法。</param>
        public void Callback(Action callback)
        {
            callback();
        }

        /// <summary>
        /// 获取控制此实例的生存期策略的生存期服务对象。
        /// </summary>
        /// <returns><see cref="System.Runtime.Remoting.Lifetime.ILease"/> 类型的对象，用于控制此实例的生存期策略。这是此实例当前的生存期服务对象（如果存在）；否则为初始化为 <see cref="System.Runtime.Remoting.Lifetime.LifetimeServices.LeaseManagerPollTime"/> 属性的值的新生存期服务对象。</returns>
        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}
