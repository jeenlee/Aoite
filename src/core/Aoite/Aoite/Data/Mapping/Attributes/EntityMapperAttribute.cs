using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Data
{
    /// <summary>
    /// 表示自定义实体映射器类型的特性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class EntityMapperAttribute : Attribute
    {
        /// <summary>
        /// 实体行标识的类型。
        /// </summary>
        public readonly static Type Type = typeof(EntityMapperAttribute);

        private readonly static Type IType = typeof(IEntityMapper);
        private Type _MapperType;
        /// <summary>
        /// 获取实体映射的类型。
        /// </summary>
        public Type MapperType { get { return this._MapperType; } }

        /// <summary>
        /// 初始化一个 <see cref="Aoite.Data.EntityMapperAttribute"/> 类的新实例。
        /// </summary>
        /// <param name="mapperType">实体映射的类型。</param>
        public EntityMapperAttribute(Type mapperType)
        {
            if(mapperType == null) throw new ArgumentNullException("mapperType");
            if(!mapperType.IsAssignableFrom(IType) || mapperType.GetConstructor(new Type[] { Types.Type }) == null)
                throw new ArgumentException("实体映射类型必须是实现 Aoite.Data.IEntityMapper 接口，并具备一个 System.Type 的构造参数。", "mapperType");

            this._MapperType = mapperType;
        }
    }
}
