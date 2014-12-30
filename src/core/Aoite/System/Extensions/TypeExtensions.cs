using Aoite.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System
{
    /// <summary>
    /// 基本数据类型的实用工具方法。
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// 返回一个类型的默认值。
        /// </summary>
        /// <param name="type">值类型或引用类型。</param>
        /// <returns>返回类型的默认值。</returns>
        public static object GetDefaultValue(this Type type)
        {
            return type.IsValueType && !(type.IsGenericType && type.GetGenericTypeDefinition().Equals(Types.Nullable))
                ? Activator.CreateInstance(type)
                : null;
        }
        /// <summary>
        /// 判断一个类型是否为 <see cref="System.Data.DataTable"/> 或 <see cref="System.Data.DataSet"/> 的类型。
        /// </summary>
        /// <param name="type">数据类型。</param>
        /// <returns>如果类型为 <see cref="System.Data.DataTable"/> 或 <see cref="System.Data.DataSet"/>，则返回 true，否则返回 false。</returns>
        public static bool IsDataType(this Type type)
        {
            return Types.DataTable.IsAssignableFrom(type) || Types.DataSet.IsAssignableFrom(type);
        }
        /// <summary>
        /// 判断类型是否为匿名类型。
        /// </summary>
        /// <param name="type">数据类型。</param>
        /// <returns>如果为匿名类型返回 true，否则返回 false。</returns>
        public static bool IsAnonymous(this Type type)
        {
            if(type == null) return false;

            return !type.IsPublic
                && Attribute.IsDefined(type, typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), false)
                && type.IsGenericType
                && type.Name.Contains("AnonymousType")
                && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"));
        }
        /// <summary>
        /// 判断一个类型是否为可空类型。
        /// </summary>
        /// <param name="type">需要判断的类型。</param>
        /// <returns>如果为 true 则是一个可空类型，否则为 false。</returns>
        public static bool IsNullable(this Type type)
        {
            return Nullable.GetUnderlyingType(type) != null;
            //return type.IsValueType && type.IsGenericType && type.GetGenericTypeDefinition().Equals(Types.Nullable);
        }
        /// <summary>
        /// 尝试获取可空类型的真实类型。
        /// </summary>
        /// <param name="type">需要判断的类型。</param>
        /// <returns>返回可空类型的真实类型，若当前类型非可空类型则返回原始值。</returns>
        public static Type GetNullableType(this Type type)
        {
            return Nullable.GetUnderlyingType(type)??type;
            //return (type.IsValueType && type.IsGenericType && type.GetGenericTypeDefinition().Equals(Types.Nullable))
            //    ? type.GetGenericArguments()[0]
            //    : type;
        }

        /// <summary>
        /// 判断当前类型是否可以从 <see cref="System.String"/> 类型进行转换。
        /// </summary>
        /// <param name="type">需要判断的类型。</param>
        /// <returns>如果可以转换返回 true，否则返回 false。</returns>
        public static bool HasStringConverter(this Type type)
        {
            return System.ComponentModel.TypeDescriptor.GetConverter(type).CanConvertFrom(Types.String);
        }
        /// <summary>
        /// 获取一个值，指示当前类型是否为简单类型。
        /// </summary>
        /// <param name="type">需要判断的类型。</param>
        /// <returns>如果为简单类型返回 true，否则返回 false。</returns>
        public static bool IsSimpleType(this Type type)
        {
            return type.IsPrimitive ||
                   type.Equals(Types.String) ||
                   type.Equals(Types.DateTime) ||
                   type.Equals(Types.Decimal) ||
                   type.Equals(Types.Guid) ||
                   type.Equals(Types.DateTimeOffset) ||
                   type.Equals(Types.TimeSpan);
        }
        /// <summary>
        /// 判断一个类型是否为数字类型。
        /// </summary>
        /// <param name="type">数据类型。</param>
        /// <returns>如果类型为任意数字类型则返回 true，否则返回 false。</returns>
        public static bool IsNumber(this Type type)
        {
            return Array.IndexOf(Types.NumberTypes, type) > -1;
        }
        /// <summary>
        /// 判断一个类型是否为浮点数类型。
        /// </summary>
        /// <param name="type">数据类型。</param>
        /// <returns>如果类型为 <see cref="System.Single"/>、<see cref="System.Double"/> 或 <see cref="System.Decimal"/> 则返回 true，否则返回 false。</returns>
        public static bool IsNumberFloat(this Type type)
        {
            return Array.IndexOf(Types.NumberFloatTypes, type) > -1;
        }
        private static string GetSimpleType(Type type, short numericPrecision, short numericScale)
        {
            switch(Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    return "bool";
                case TypeCode.Byte:
                    return "byte";
                case TypeCode.Char:
                    return "char";
                case TypeCode.Decimal:
                    if(numericScale == 0)
                    {
                        if(numericPrecision < 5) goto case TypeCode.Int16;
                        else if(numericPrecision < 10) goto case TypeCode.Int32;
                        goto case TypeCode.Int64;
                    }
                    return "decimal";
                case TypeCode.Double:
                    return "double";
                case TypeCode.Int16:
                    return "short";
                case TypeCode.Int32:
                    return "int";
                case TypeCode.Int64:
                    return "long";
                case TypeCode.String:
                    return "string";
            }
            return type.Name;
        }
        /// <summary>
        /// 获取指定类型的缩写。
        /// </summary>
        /// <param name="type">数据类型。</param>
        /// <param name="numericPrecision">数字长度。</param>
        /// <param name="numericScale">数字精度。</param>
        /// <param name="allowDBNull">是否允许为空。</param>
        public static string GetSimpleType(this Type type, short numericPrecision, short numericScale, bool allowDBNull)
        {
            var typeName = GetSimpleType(type, numericPrecision, numericScale);
            if(allowDBNull && type.IsValueType) return typeName + "?";
            return typeName;
        }

        /// <summary>
        /// 查找指定名称的字段（不区分大小写、静态、实例、公有和私有）。
        /// </summary>
        /// <param name="type">数据类型。</param>
        /// <param name="name">字段名称。</param>
        /// <returns>返回一个字段或一个 null 值。</returns>
        public static FieldInfo FindField(this Type type, string name)
        {
            if(type == null) throw new ArgumentNullException("type");

            return type.GetField(name, Flags.StaticInstanceAnyVisibility);
        }

        /// <summary>
        /// 查找指定名称的属性（不区分大小写、静态、实例、公有和私有）。
        /// </summary>
        /// <param name="type">数据类型。</param>
        /// <param name="name">属性名称。</param>
        /// <returns>返回一个属性或一个 null 值。</returns>
        public static PropertyInfo FindProperty(this Type type, string name)
        {
            if(type == null) throw new ArgumentNullException("type");
            return type.GetProperty(name, Flags.StaticInstanceAnyVisibility);
        }

        /// <summary>
        /// 查找指定参数类型数组和名称的方法（不区分大小写、静态、实例、公有和私有）。
        /// </summary>
        /// <param name="type">数据类型。</param>
        /// <param name="name">方法名称。</param>
        /// <param name="types">方法的参数类型数组。</param>
        /// <returns>返回一个方法或一个 null 值。</returns>
        public static MethodInfo FindMethod(this Type type, string name, params Type[] types)
        {
            if(type == null) throw new ArgumentNullException("type");
            if(types == null || types.Length == 0) return type.GetMethod(name, Flags.StaticInstanceAnyVisibility);
            return type.GetMethod(name, Flags.StaticInstanceAnyVisibility, null, types, null);
        }

        /// <summary>
        /// 查找指定参数类型数组的构造函数。
        /// </summary>
        /// <param name="type">数据类型。</param>
        /// <param name="types">构造函数的参数类型数组。</param>
        /// <returns>返回一个构造函数或一个 null 值。</returns>
        public static ConstructorInfo FindConstructor(this Type type, params Type[] types)
        {
            if(type == null) throw new ArgumentNullException("type");
            return type.GetConstructor(Flags.InstanceAnyVisibility, null, types ?? System.Type.EmptyTypes, null);
        }

        #region MemberInfo

        /// <summary>
        /// 返回由 <typeparamref name="T"/> 标识的特性（包括继承链）。
        /// </summary>
        /// <typeparam name="T">特性的数据类型。</typeparam>
        /// <param name="member">成员。</param>
        /// <returns>如果存在标志，则返回这个值，否则返回一个默认值。</returns>
        public static T GetAttribute<T>(this MemberInfo member)
        {
            return GetAttribute<T>(member, true);
        }
        /// <summary>
        /// 返回由 <typeparamref name="T"/> 标识的特性。
        /// </summary>
        /// <typeparam name="T">特性的数据类型。</typeparam>
        /// <param name="member">成员。</param>
        /// <param name="inherit">指定是否搜索该成员的继承链以查找这些属性。</param>
        /// <returns>如果存在标志，则返回这个值，否则返回一个默认值。</returns>
        public static T GetAttribute<T>(this MemberInfo member, bool inherit)
        {
            var ats = member.GetCustomAttributes(typeof(T), inherit);
            if(ats.Length > 0) return (T)ats[0];
            return default(T);
        }
        /// <summary>
        /// 返回由 <typeparamref name="T"/> 标识的特性（包括继承链）。
        /// </summary>
        /// <typeparam name="T">特性的数据类型。</typeparam>
        /// <param name="member">成员。</param>
        /// <returns>返回特性的数组。</returns>
        public static T[] GetAttributes<T>(this MemberInfo member)
        {
            return GetAttributes<T>(member, true);
        }
        /// <summary>
        /// 返回由 <typeparamref name="T"/> 标识的特性的数组。
        /// </summary>
        /// <typeparam name="T">特性的数据类型。</typeparam>
        /// <param name="member">成员。</param>
        /// <param name="inherit">指定是否搜索该成员的继承链以查找这些属性。</param>
        /// <returns>返回特性的数组。</returns>
        public static T[] GetAttributes<T>(this MemberInfo member, bool inherit)
        {
            var ats = member.GetCustomAttributes(typeof(T), inherit);
            T[] result = new T[ats.Length];
            for(int i = 0; i < ats.Length; i++)
            {
                result[i] = (T)ats[i];
            }
            return result;
        }

        #endregion

        /// <summary>
        /// 将指定的值转换为枚举类型。
        /// </summary>
        /// <param name="type">类型。</param>
        /// <param name="value">实例。</param>
        /// <returns>返回类型转换的实例。</returns>
        public static object ToEnumValue(this Type type, object value)
        {
            if(value is string)
            {
                var s_value = Convert.ToString(value);
                foreach(var c in s_value)
                {
                    if(!Char.IsNumber(c))
                        return Enum.Parse(type, Convert.ToString(value), true);
                }
                value = Convert.ChangeType(value, Enum.GetUnderlyingType(type));
            }
            return Enum.ToObject(type, value);
        }

        /// <summary>
        /// 将指定的值转换为当前类型。
        /// </summary>
        /// <param name="type">类型。</param>
        /// <param name="value">实例。</param>
        /// <returns>返回类型转换的实例。</returns>
        public static object ChangeType(this Type type, object value)
        {
            if(value == null || Convert.IsDBNull(value)) return type.GetDefaultValue();
            var realType = type.GetNullableType();

            if(realType.IsInstanceOfType(value)) return value;
            if(realType == Types.Boolean) return Types.TrueStrings.IndexOf<string>(value.ToString(), StringComparer.OrdinalIgnoreCase) != -1;
            try
            {
                if(realType == Types.Guid)
                {
                    if(value is byte[]) return new Guid((byte[])value);
                    return new Guid(value.ToString());
                }
                if(realType == Types.TimeSpan)
                {
                    if(value is Int64) return new TimeSpan((Int64)value);
                    return TimeSpan.Parse(value.ToString());
                }
                if(realType.IsEnum)
                {
                    if(value is string) return Enum.Parse(realType, Convert.ToString(value), true);
                    return Enum.ToObject(realType, value);
                }

                if(realType == Types.Uri) return new Uri(value.ToString());

                switch(Type.GetTypeCode(realType))
                {
                    case TypeCode.Byte: return Convert.ToByte(value);
                    case TypeCode.Char: return Convert.ToChar(value);
                    case TypeCode.DBNull: return DBNull.Value;
                    case TypeCode.DateTime: return Convert.ToDateTime(value);
                    case TypeCode.Decimal: return Convert.ToDecimal(value);
                    case TypeCode.Double: return Convert.ToDouble(value);
                    case TypeCode.Int16: return Convert.ToInt16(value);
                    case TypeCode.Int32: return Convert.ToInt32(value);
                    case TypeCode.Int64: return Convert.ToInt64(value);
                    case TypeCode.SByte: return Convert.ToSByte(value);
                    case TypeCode.Single: return Convert.ToSingle(value);
                    case TypeCode.String: return value.ToString();
                    case TypeCode.UInt16: return Convert.ToUInt16(value);
                    case TypeCode.UInt32: return Convert.ToUInt32(value);
                    case TypeCode.UInt64: return Convert.ToUInt64(value);
                }
                return Convert.ChangeType(value, realType);
            }
            catch(Exception)
            {
                return type.GetDefaultValue();
            }
        }
    }
}
