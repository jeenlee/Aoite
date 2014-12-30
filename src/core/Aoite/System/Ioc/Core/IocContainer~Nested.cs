using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    partial class IocContainer
    {
        /// <summary>
        /// 表示独立生命周期的实例盒。
        /// </summary>
        private class InstanceBox
        {
            public readonly string Name;
            public readonly Type LastMappingType;
            protected readonly InstanceCreatorCallback Callback;

            public InstanceBox(string name, InstanceCreatorCallback callback, Type lastMappingType = null)
            {
                this.Name = name;
                this.Callback = callback;
                this.LastMappingType = lastMappingType;
            }

            public virtual object GetInstance(params object[] lastMappingValues)
            {
                return this.Callback(lastMappingValues);
            }
        }

        /// <summary>
        /// 表示单例生命周期的实例盒。
        /// </summary>
        private class SingletonInstanceBox : InstanceBox
        {
            public SingletonInstanceBox(string name, InstanceCreatorCallback callback) : base(name, callback) { }

            private object Instance;
            public override object GetInstance(params object[] lastMappingValues)
            {
                if(this.Instance == null) lock(this) if(this.Instance == null)
                            this.Instance = this.Callback(lastMappingValues);
                return this.Instance;

            }

        }

        private sealed class EmbeddedTypeAwareTypeComparer : IEqualityComparer<Type>
        {
            // Methods
            public bool Equals(Type x, Type y)
            {
                return x.IsEquivalentTo(y);
            }

            public int GetHashCode(Type obj)
            {
                return obj.FullName.GetHashCode();
            }
        }
    }
}
