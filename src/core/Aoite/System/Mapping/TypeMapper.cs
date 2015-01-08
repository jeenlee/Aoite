using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
namespace System
{
    /// <summary>
    /// 表示一个类型的映射器。
    /// </summary>
    public class TypeMapper
    {
        /// <summary>
        /// 实体的属性映射集合。
        /// </summary>
        protected readonly Dictionary<string, PropertyMapper> _Properties;

        /// <summary>
        /// 获取或设置映射器的名称。
        /// </summary>
        public virtual string Name { get; set; }
        /// <summary>
        /// 获取实体的类型。
        /// </summary>
        public virtual Type Type { get; private set; }
        /// <summary>
        /// 获取实体的属性映射集合。
        /// </summary>
        public IEnumerable<PropertyMapper> Properties { get { return this._Properties.Values; } }
        /// <summary>
        /// 获取指定属性名称的属性映射。
        /// </summary>
        /// <param name="propertyName">属性名称。</param>
        public PropertyMapper this[string propertyName]
        {
            get
            {
                return this._Properties.TryGetValue(propertyName);
            }
        }
        /// <summary>
        /// 获取实体的属性映射集合的元素数。
        /// </summary>
        public int Count { get { return this._Properties.Count; } }

        internal TypeMapper(int capacity)
        {
            this._Properties = new Dictionary<string, PropertyMapper>(capacity, StringComparer.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// 指定属性名，判断指定的属性是否存在。
        /// </summary>
        /// <param name="propertyName">属性名称。</param>
        /// <returns>存在返回 true，否则返回 false。</returns>
        public bool Contains(string propertyName)
        {
            return this._Properties.ContainsKey(propertyName);
        }

        private static readonly ConcurrentDictionary<Type, TypeMapper> Cacher = new ConcurrentDictionary<Type, TypeMapper>();

        /// <summary>
        /// 指定实例的数据类型，创建或从缓存读取一个实体的映射器。
        /// </summary>
        /// <param name="type">实例的数据类型。</param>
        /// <returns>返回一个实体映射器。</returns>
        public static TypeMapper Create(Type type)
        {
            if(type == null) throw new ArgumentNullException("type");
            return Cacher.GetOrAdd(type, OnCreate);
        }

        static TypeMapper OnCreate(Type type)
        {
            var ps = type.GetProperties();
            TypeMapper mapper = new TypeMapper(ps.Length) { Type = type };

            var aliasAttr = type.GetAttribute<IAliasAttribute>();
            mapper.Name = aliasAttr != null && aliasAttr.Name != null
                ? aliasAttr.Name
                : type.Name;

            foreach(var p in ps)
            {
                var propertyMapper = new PropertyMapper(mapper, p);
                mapper._Properties.Add(propertyMapper.Name, propertyMapper);
            }

            return mapper;
        }

        /// <summary>
        /// 表示类型映射器泛型单例模式。
        /// </summary>
        /// <typeparam name="TEntity">实体的数据类型。</typeparam>
        public static class Instance<TEntity>
        {
            /// <summary>
            /// 获取类型的映射器。
            /// </summary>
            public readonly static TypeMapper Mapper = TypeMapper.Create(typeof(TEntity));
        }
    }
}
