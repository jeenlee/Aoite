using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Resources;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace System.ComponentModel.DataAnnotations
{
    /// <summary>
    /// 提供比较两个属性的属性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class CompareAttribute : ValidationAttribute
    {
        /// <summary>
        /// 初始化一个 <see cref="System.ComponentModel.DataAnnotations"/> 类的新实例。
        /// </summary>
        /// <param name="otherProperty">要与当前属性进行比较的属性。</param>
        public CompareAttribute(string otherProperty)
            : base("相等匹配失败。")
        {
            if(otherProperty == null)
            {
                throw new ArgumentNullException("otherProperty");
            }
            OtherProperty = otherProperty;
        }

        /// <summary>
        /// 获取要与当前属性进行比较的属性。
        /// </summary>
        public string OtherProperty { get; private set; }

        /// <summary>
        /// 获取其他属性的显示名称。
        /// </summary>
        public string OtherPropertyDisplayName { get; internal set; }

        /// <summary>
        /// 基于发生错误的数据字段对错误消息应用格式设置。
        /// </summary>
        /// <param name="name">导致验证失败的字段的名称。</param>
        /// <returns>带有格式的错误消息。</returns>
        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, OtherPropertyDisplayName ?? OtherProperty);
        }

        /// <summary>
        /// 确定指定的对象是否有效。
        /// </summary>
        /// <param name="value">要验证的对象。</param>
        /// <param name="validationContext">一个对象，该对象包含有关验证请求的信息。</param>
        /// <returns>如果 <paramref name="value"/> 有效，则为 true；否则为 false。</returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            PropertyInfo otherPropertyInfo = validationContext.ObjectType.GetProperty(OtherProperty);
            if(otherPropertyInfo == null)
            {
                return new ValidationResult(String.Format(CultureInfo.CurrentCulture, "没有找到属性 {0}。", OtherProperty));
            }

            object otherPropertyValue = otherPropertyInfo.GetValue(validationContext.ObjectInstance, null);
            if(!Equals(value, otherPropertyValue))
            {
                if(OtherPropertyDisplayName == null)
                {
                    OtherPropertyDisplayName = GetDisplayNameForProperty(validationContext.ObjectType, OtherProperty);
                }
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }
            return null;
        }

        private static string GetDisplayNameForProperty(Type containerType, string propertyName)
        {
            ICustomTypeDescriptor typeDescriptor = GetTypeDescriptor(containerType);
            PropertyDescriptor property = typeDescriptor.GetProperties().Find(propertyName, true);
            if(property == null)
            {
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture,

                    "找不到 {0} 的属性 {1}。", containerType.FullName, propertyName));
            }
            IEnumerable<Attribute> attributes = property.Attributes.Cast<Attribute>();
            DisplayAttribute display = attributes.OfType<DisplayAttribute>().FirstOrDefault();
            if(display != null)
            {
                return display.GetName();
            }
            DisplayNameAttribute displayName = attributes.OfType<DisplayNameAttribute>().FirstOrDefault();
            if(displayName != null)
            {
                return displayName.DisplayName;
            }
            return propertyName;
        }

        private static ICustomTypeDescriptor GetTypeDescriptor(Type type)
        {
            return new AssociatedMetadataTypeTypeDescriptionProvider(type).GetTypeDescriptor(type);
        }
    }
}