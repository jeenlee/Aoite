using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Aoite.Reflection
{
    /// <summary>
    /// 表示基于 Emit 的动态实现。
    /// </summary>
    /// <typeparam name="I">实例的类型。</typeparam>
    public class Dynamic<I>
    {
        #region Fields

        private DynamicFieldHelper<I> _Fields;
        private DynamicMethodHelper<I> _Methods;
        private DynamicPropertyHelper<I> _Properties;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// 初始化一个 <see cref="Aoite.Reflection.Dynamic&lt;I&gt;"/> 类的新实例。
        /// </summary>
        public Dynamic()
            : this(typeof(I)) { }

        /// <summary>
        /// 指定绑定动态实现的实例，初始化一个 <see cref="Aoite.Reflection.Dynamic&lt;I&gt;"/> 类的新实例。
        /// </summary>
        /// <param name="instance">实例。可以为 null，表示动态实现 I 的静态字段。</param>
        public Dynamic(I instance)
        {
            this._Fields = new DynamicFieldHelper<I>(instance);
            this._Methods = new DynamicMethodHelper<I>(instance);
            this._Properties = new DynamicPropertyHelper<I>(instance);
        }

        /// <summary>
        /// 指定绑定动态实现的类型，初始化一个 <see cref="Aoite.Reflection.Dynamic&lt;I&gt;"/> 类的新实例。
        /// </summary>
        /// <param name="instanceType">实例类型。</param>
        public Dynamic(Type instanceType)
        {
            if(instanceType == null) throw new ArgumentNullException("instanceType");
            this._Fields = new DynamicFieldHelper<I>(instanceType);
            this._Methods = new DynamicMethodHelper<I>(instanceType);
            this._Properties = new DynamicPropertyHelper<I>(instanceType);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// 动态字段。
        /// </summary>
        public DynamicFieldHelper<I> Fields
        {
            get { return this._Fields; }
        }

        /// <summary>
        /// 动态方法。
        /// </summary>
        public DynamicMethodHelper<I> Methods
        {
            get { return this._Methods; }
        }

        /// <summary>
        /// 动态属性。
        /// </summary>
        public DynamicPropertyHelper<I> Properties
        {
            get { return this._Properties; }
        }


        #endregion Properties

        #region Methods

        /// <summary>
        /// 生成实体的属性信息的字符串。
        /// </summary>
        /// <param name="instance">实体。</param>
        /// <returns>返回描述实体的字符串。</returns>
        public static string DisplayProperty(I instance)
        {

            return DisplayProperty(new List<object>(), instance, 0);
        }

        private static string DisplayProperty(List<object> objectContainer, object instance, int depth)
        {
            if(instance == null) return "null";
            var helper = new DynamicPropertyHelper<object>(instance);
            string spaceString = depth == 0 ? string.Empty : new string(' ', depth * 4);

            string propertySpaceString = spaceString + "    ";
            StringBuilder builder = new StringBuilder();
            builder.Append('#');
            builder.Append(helper.InstanceType.Name);
            builder.AppendLine();
            builder.Append(spaceString);
            builder.Append('{');
            foreach(var propertyInfo in helper.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.Public))
            {
                builder.AppendLine();
                builder.Append(propertySpaceString);
                builder.Append(propertyInfo.Name);
                builder.Append(' ');
                builder.Append(':');
                builder.Append(' ');
                /*
                 * #SimpleUser
                 * {
                 *      Id : 5
                 * }
                 */
                var propertyValue = helper.CreateGetterHandler(propertyInfo)(instance);
                if(propertyValue == null)
                {
                    builder.Append("null");
                }
                else
                {
                    var index = objectContainer.IndexOf(propertyValue);

                    if(index > -1)
                    {
                        index++;
                        PropertyInfo info = (PropertyInfo)objectContainer[index];
                        builder.Append("[#");
                        builder.Append(info.DeclaringType.FullName);
                        builder.Append("].");
                        builder.Append(info.Name);
                        continue;
                    }
                    Type valueType = propertyValue.GetType();
                    if(!valueType.IsValueType)
                    {
                        objectContainer.Add(propertyValue);
                        objectContainer.Add(propertyInfo);
                    }
                    switch(Type.GetTypeCode(valueType))
                    {
                        case TypeCode.Empty:
                            builder.Append("Empty");
                            break;
                        case TypeCode.DBNull:
                            builder.Append("DBNull.Value");
                            break;
                        case TypeCode.Char:
                            builder.Append('\'');
                            builder.Append(propertyValue);
                            builder.Append('\'');
                            break;
                        case TypeCode.String:
                            builder.Append('"');
                            builder.Append(propertyValue);
                            builder.Append('"');
                            break;
                        case TypeCode.Object:
                            builder.Append(DisplayProperty(objectContainer, propertyValue, depth + 1));
                            break;
                        default:
                            builder.Append(propertyValue);
                            break;
                    }
                }
                builder.Append(",");
            }

            builder.AppendLine();
            builder.Append(spaceString);
            builder.Append('}');

            return builder.ToString();
        }

        #endregion Methods

        /// <summary>
        /// 动态创建一个默认构造函数的实例。
        /// </summary>
        /// <returns>返回动态创建的实例。</returns>
        public static I Create()
        {
            return Create(typeof(I));
        }

        /// <summary>
        /// 动态创建一个默认构造函数的实例。
        /// </summary>
        /// <param name="instanceType">实例的真实数据类型。</param>
        /// <returns>返回动态创建的实例。</returns>
        public static I Create(Type instanceType)
        {
            return Create(instanceType, Type.EmptyTypes);
        }

        /// <summary>
        /// 动态创建一个指定构造函数参数类型的实例。
        /// </summary>
        /// <param name="types">构造函数参数的类型集合。</param>
        /// <param name="parameters">参数对应的值。</param>
        /// <returns>返回动态创建的实例。</returns>
        public static I Create(Type[] types, params object[] parameters)
        {
            return Create(typeof(I), types, parameters);
        }

        /// <summary>
        /// 动态创建一个指定构造函数参数类型的实例。
        /// </summary>
        /// <param name="instanceType">实例的真实数据类型。</param>
        /// <param name="types">构造函数参数的类型集合。</param>
        /// <param name="parameters">参数对应的值。</param>
        /// <returns>返回动态创建的实例。</returns>
        public static I Create(Type instanceType, Type[] types, params object[] parameters)
        {
            if(instanceType == null) throw new ArgumentNullException("instanceType");
            return Create(instanceType.GetConstructor(Flags.InstanceAnyVisibility, null, types ?? System.Type.EmptyTypes, null), parameters);
        }

        /// <summary>
        /// 动态创建一个指定构造函数元数据的实例。
        /// </summary>
        /// <param name="constructorInfo">构造函数元数据。</param>
        /// <param name="parameters">参数对应的值。</param>
        /// <returns>返回动态创建的实例。</returns>
        public static I Create(ConstructorInfo constructorInfo, params object[] parameters)
        {
            return (I)CreateHandler(constructorInfo)(parameters);
        }

        /// <summary>
        /// 创建一个指定构造函数元数据的工厂委托。
        /// </summary>
        /// <param name="constructorInfo">构造函数元数据。</param>
        /// <returns>返回一个动态创建实例的工厂委托。</returns>
        public static ConstructorInvoker CreateHandler(ConstructorInfo constructorInfo)
        {
            return constructorInfo.DelegateForCreateInstance();
        }
    }
}