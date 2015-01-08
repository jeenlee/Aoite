//using Aoite.Reflection;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Text;

//namespace System
//{
//    /// <summary>
//    /// 定义一个属性的映射器。
//    /// </summary>
//    public interface IPropertyMapper
//    {
//        /// <summary>
//        /// 获取属性的设置器。
//        /// </summary>
//        MemberSetter SetValue { get; }
//        /// <summary>
//        /// 获取属性的读取器。
//        /// </summary>
//        MemberGetter GetValue { get; }
//        /// <summary>
//        /// 获取成员的属性元数据。
//        /// </summary>
//        PropertyInfo Property { get; }
//        /// <summary>
//        /// 获取或设置映射器的名称。
//        /// </summary>
//        string Name { get; }
//        /// <summary>
//        /// 获取或设置一个值，指示是否为唯一标识。
//        /// </summary>
//        bool IsKey { get; }
//        /// <summary>
//        /// 获取属性所属的类型映射器。
//        /// </summary>
//        ITypeMapper TypeMapper { get; }
//        /// <summary>
//        /// 获取或设置一个值，该值指示当前成员是否已标识忽略标识。
//        /// </summary>
//        bool IsIgnore { get; }
//        /// <summary>
//        /// 获取类型的默认值。
//        /// </summary>
//        object TypeDefaultValue { get; }
//    }
//}
