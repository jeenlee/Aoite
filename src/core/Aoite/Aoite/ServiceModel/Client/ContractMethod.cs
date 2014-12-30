using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Aoite.ServiceModel
{
    /// <summary>
    /// 表示一个契约的方法。
    /// </summary>
    public class ContractMethod
    {
        private int _Identity;
        /// <summary>
        /// 获取契约方法的标识索引。
        /// </summary>
        public int Identity { get { return this._Identity; } }

        private MethodInfo _MethodInfo;
        /// <summary>
        /// 获取契约方法的方法元数据。
        /// </summary>
        public MethodInfo MethodInfo { get { return this._MethodInfo; } }

        private Type[] _ParameterTypes;
        /// <summary>
        /// 获取契约方法的参数类型列表。
        /// </summary>
        public Type[] ParameterTypes { get { return this._ParameterTypes; } }

        /// <summary>
        /// 获取契约方法的返回类型。
        /// </summary>
        public Type ReturnType { get { return this._MethodInfo.ReturnType; } }

        private string _Name;
        /// <summary>
        /// 设置或获取契约方法的名称。
        /// </summary>
        public string Name { get { return this._Name; } }

        private bool[] _ParameterByRefs;
        /// <summary>
        /// 获取一组布尔值，表示方法参数是否为引用传递。
        /// </summary>
        public bool[] ParameterByRefs { get { return this._ParameterByRefs; } }

        private string[] _ParameterNames;
        /// <summary>
        /// 获取参数名称的集合。
        /// </summary>
        public string[] ParameterNames { get { return _ParameterNames; } }

        private bool _AllowAnonymous;
        /// <summary>
        /// 获取一个值，该值指示当前方法是否允许匿名访问。
        /// </summary>
        public bool AllowAnonymous { get { return this._AllowAnonymous; } }

        private HybridDictionary _Data;
        /// <summary>
        /// 获取契约方法的自定义数据。
        /// </summary>
        public HybridDictionary Data { get { return (this._Data ?? (this._Data = new HybridDictionary(true))); } }

        private ContractFilterAttribute[] _Filters;
        /// <summary>
        /// 获取筛选器的集合。
        /// </summary>
        public ContractFilterAttribute[] Filters { get { return this._Filters; } }

        private Aoite.Reflection.MethodInvoker _CallMethod;

        internal object CallMethod(object obj, params object[] parameters)
        {
            if(this._CallMethod == null)
                lock(this)
                    if(this._CallMethod == null) this._CallMethod = Aoite.Reflection.MethodInfoExtensions.DelegateForCallMethod(this._MethodInfo);

            return _CallMethod(obj, parameters);
        }

        private ParameterInfo[] _parameterInfos;
        private ValidationAttribute[][] _validations;

        internal ContractMethod(int identity, MethodInfo methodInfo, ParameterInfo[] parameterInfos, Type[] parameterTypes)
        {
            this._Identity = identity;
            this._MethodInfo = methodInfo;
            this._parameterInfos = parameterInfos;
            this._Name = methodInfo.GetContractName();
            this._AllowAnonymous = methodInfo.IsDefined(AllowAnonymousAttribute.Type, true);
            this._Filters = methodInfo.GetAttributes<ContractFilterAttribute>().OrderBy(filter => filter.Order).ToArray();
            this._ParameterTypes = parameterTypes;
            this._ParameterByRefs = new bool[parameterTypes.Length];
            this._ParameterNames = new string[parameterTypes.Length];
            this._validations = new ValidationAttribute[parameterTypes.Length][];
            for(int i = 0; i < parameterTypes.Length; i++)
            {
                var pInfo = parameterInfos[i];
                this._validations[i] = pInfo
                                            .GetCustomAttributes(typeof(ValidationAttribute), true)
                                            .Cast<ValidationAttribute>().ToArray();
                this._ParameterByRefs[i] = this._ParameterTypes[i].IsByRef;
                this._ParameterNames[i] = pInfo.Name;
            }
        }

        internal void Validation(object[] values)
        {
            for(int i = 0; i < values.Length; i++)
            {
                var validations = this._validations[i];
                var value = values[i];
                if(value == null)
                {
                    var ra = validations.FirstOrDefault(a => a is RequiredAttribute) as RequiredAttribute;
                    if(ra != null)
                    {

                        var p = _parameterInfos[i];
                        var name = p.Name;
                        var disAttr = p.GetCustomAttributes(typeof(DisplayAttribute), true).SingleOrDefault() as DisplayAttribute;
                        if(disAttr != null) name = disAttr.Name;

                        throw new ValidationException(new ValidationResult(ra.FormatErrorMessage(name), null)
                            , ra, value);
                    }
                    return;
                }
                var context = new ValidationContext(value, null, null);
                if(validations.Length > 0) Validator.ValidateValue(value, context, validations);
                Validator.ValidateObject(value, context, true);
            }
        }

        internal void ToJsonMetadata(StringBuilder builder, string aoiteName, string contractName)
        {
            var methodName = this.Name.ToCamelCase();
            builder.Append("    ")
                .Append(aoiteName).Append(".")
                .Append(contractName).Append(".")
                .Append(methodName).Append(" = ");
            builder.Append("function(");
            for(int i = 0; i < this._ParameterNames.Length; i++)
            {
                if(i > 0) builder.Append(", ");
                var pName = this._ParameterNames[i].ToCamelCase();
                builder.Append(pName);
            }
            builder.AppendLine(") {")
                .Append("        return ").Append(aoiteName).Append(".remote(\"" + contractName + "/" + methodName + "\", { ");
            for(int i = 0; i < this._ParameterNames.Length; i++)
            {
                if(i > 0) builder.Append(", ");
                var pName = this._ParameterNames[i].ToCamelCase();
                builder.Append(pName).Append(": ").Append(pName);
            }
            builder.AppendLine("});");
            builder.AppendLine("    };");
        }

    }
}
