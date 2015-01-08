//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace System
//{
//    /// <summary>
//    /// 定义一个类型的映射器。
//    /// </summary>
//    public interface ITypeMapper
//    {
//        /// <summary>
//        /// 获取或设置映射器的名称。
//        /// </summary>
//        string Name { get; set; }
//        /// <summary>
//        /// 获取实体的类型。
//        /// </summary>
//        Type Type { get; }
//        /// <summary>
//        /// 获取实体的属性映射集合。
//        /// </summary>
//        IEnumerable<IPropertyMapper> Properties { get; }
//        /// <summary>
//        /// 获取指定属性名称的属性映射。
//        /// </summary>
//        /// <param name="propertyName">属性名称。</param>
//        IPropertyMapper this[string propertyName] { get; }
//        /// <summary>
//        /// 获取实体的属性映射集合的元素数。
//        /// </summary>
//        int Count { get; }

//        /// <summary>
//        /// 指定属性名，判断指定的属性是否存在。
//        /// </summary>
//        /// <param name="propertyName">属性名称。</param>
//        /// <returns>存在返回 true，否则返回 false。</returns>
//        bool Contains(string propertyName);
//    }
//}
