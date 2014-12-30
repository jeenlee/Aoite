using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Aoite.ServiceModel
{
    /// <summary>
    /// 定义一个契约的信息。
    /// </summary>
    public interface IContractInfo
    {
        /// <summary>
        /// 获取契约的数据类型。
        /// </summary>
        Type Type { get; }
        /// <summary>
        /// 获取契约的名称。
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 获取一个值，该值指示当前服务是否允许匿名访问。
        /// </summary>
        bool AllowAnonymous { get; }
        /// <summary>
        /// 获取契约方法的列表。
        /// </summary>
        ContractMethod[] Methods { get; }
    }
    /// <summary>
    /// 表示一个契约的信息。
    /// </summary>
    public partial class ContractInfo : IContractInfo
    {
        private Type _Type;
        /// <summary>
        /// 获取契约的数据类型。
        /// </summary>
        public Type Type { get { return this._Type; } }

        private string _Name;
        /// <summary>
        /// 获取契约的名称。
        /// </summary>
        public string Name { get { return this._Name; } }

        private bool _AllowAnonymous;
        /// <summary>
        /// 获取一个值，该值指示当前服务是否允许匿名访问。
        /// </summary>
        public bool AllowAnonymous { get { return this._AllowAnonymous; } }

        private Type _ProxyType;

        private ContractMethod[] _Methods;
        /// <summary>
        /// 获取契约方法的列表。
        /// </summary>
        public ContractMethod[] Methods { get { return this._Methods; } }

        private Aoite.Reflection.ConstructorInvoker CreateServiceHandler;

        /// <summary>
        /// 创建一个契约代理的实例。
        /// </summary>
        /// <param name="domain">契约域。</param>
        /// <param name="keepAlive">指示是否为一个持久连接的契约客户端。</param>
        /// <returns>返回一个契约代理的实例</returns>
        public object CreateProxyObject(ContractDomain domain, bool keepAlive = false)
        {
            return CreateServiceHandler(domain, this, keepAlive);
        }

        private ContractInfo() { }
    }

    partial class ContractInfo
    {
        private const string NamePrefix = "<CONTRACT>";
        private static readonly Type BaseType = typeof(ContractClient);
        private static readonly Type[] BaseTypeParameters = new Type[] { typeof(ContractDomain), typeof(ContractInfo), Types.Boolean };
        private static readonly ConstructorInfo BaseTypeConstructor = BaseType.GetConstructor(BaseTypeParameters);
        private static readonly Dictionary<Type, ContractInfo> ContractCache = new Dictionary<Type, ContractInfo>();

        /// <summary>
        /// 指定契约的数据类型，获取或生成契约的信息。
        /// </summary>
        /// <param name="type">契约的数据类型。</param>
        /// <returns>返回一个契约的信息。</returns>
        internal static ContractInfo GetContractInfo(Type type)
        {
            ContractInfo info;
            if(!ContractCache.TryGetValue(type, out info))
                lock(type)
                    if(!ContractCache.TryGetValue(type, out info))
                    {
                        info = CreateContractInfo(type);
                        ContractCache.Add(type, info);

                    }
            return info;
        }

        private static ContractInfo CreateContractInfo(Type contractType)
        {
            contractType.TestContractType();

            var typeBuilder = CreateTypeBuilder(contractType);
            var contractMethods = new List<ContractMethod>(11);
            foreach(var m in ServiceModelShared.GetAllMethods(contractType))
            {
                DefineContractMethod(typeBuilder, m, contractMethods);
            }

            var info = new ContractInfo()
            {
                _Type = contractType,
                _ProxyType = typeBuilder.CreateType(),
                _Methods = contractMethods.ToArray()
            };
            info._Name = contractType.GetServiceName();
            info._AllowAnonymous = contractType.IsDefined(AllowAnonymousAttribute.Type, true);
            info.CreateServiceHandler = Aoite.Reflection.ConstructorInfoExtensions.DelegateForCreateInstance(info._ProxyType.GetConstructor(BaseTypeParameters));

            return info;
        }

        private static TypeBuilder CreateTypeBuilder(Type contractType)
        {
            string serviceTypeName = NamePrefix + contractType.FullName;
            if(serviceTypeName[0] == 'I') serviceTypeName = serviceTypeName.Remove(0, 1);

            var typeBuilder = TypeBuilderHelper.Module.DefineType(serviceTypeName, TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Sealed, BaseType);
            typeBuilder.AddInterfaceImplementation(contractType);

            ConstructorBuilder ctor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis, BaseTypeParameters);


            /*
             ctor()
             {
                load this                   调用方法的实例
                load endpoint               调用方法的参数
                call base.ctor(String)    调用的方法（父类构造方法，也是一个方法）
                ret
             }
             */

            ILGenerator ctorIL = ctor.GetILGenerator();
            ctorIL.Emit(OpCodes.Ldarg_0);           //load "this"
            ctorIL.Emit(OpCodes.Ldarg_1);           //load "ContractDomain"
            ctorIL.Emit(OpCodes.Ldarg_2);           //load "ContractInfo"
            ctorIL.Emit(OpCodes.Ldarg_3);           //load "Boolean"
            ctorIL.Emit(OpCodes.Call, BaseTypeConstructor);    //call "base(...)"
            ctorIL.Emit(OpCodes.Ret);

            return typeBuilder;
        }

        #region 暂时对泛型的支持有待更深一步的研究

        private static string[] GetGenericParameterNames(Type[] genericArguments)
        {
            var genericParameterNames = new string[genericArguments.Length];
            for(int i = 0; i < genericArguments.Length; i++) genericParameterNames[i] = genericArguments[i].Name;
            return genericParameterNames;
        }
        private static void DeinfeGenericParameter(Type[] genericArguments, GenericTypeParameterBuilder[] gtpBuilders)
        {
            for(int i = 0; i < genericArguments.Length; i++)
            {
                gtpBuilders[i].SetGenericParameterAttributes(genericArguments[i].GenericParameterAttributes);

                Type[] constraints = genericArguments[i].GetGenericParameterConstraints();
                System.Collections.Generic.List<Type> interfaces = new System.Collections.Generic.List<Type>(constraints.Length);
                foreach(Type constraint in constraints)
                {
                    if(constraint.IsClass)
                        gtpBuilders[i].SetBaseTypeConstraint(constraint);
                    else
                        interfaces.Add(constraint);
                }
                gtpBuilders[i].SetInterfaceConstraints(interfaces.ToArray());
            }
        }

        #endregion

        private static void DefineContractMethod(TypeBuilder typeBuilder
            , MethodInfo methodInfo
            , List<ContractMethod> contractMethods)
        {
            ParameterInfo[] parameterInfos;
            Type[] parameterTypes = methodInfo.GetParameterTypes(out parameterInfos);
            var returnType = methodInfo.ReturnType;
            MethodBuilder methodBuilder = typeBuilder.DefineMethod(methodInfo.Name, MethodAttributes.Public | MethodAttributes.Virtual, returnType, parameterTypes);

            ILGenerator mIL = methodBuilder.GetILGenerator();
            var identity = contractMethods.Count;
            contractMethods.Add(new ContractMethod(identity, methodInfo, parameterInfos, parameterTypes));
            GenerateILCodeForMethod(identity, mIL, returnType, parameterTypes);
        }

        private static void GenerateILCodeForMethod(int ident
            , ILGenerator mIL
            , Type returnType
            , Type[] parameterTypes)
        {
            int pLength = parameterTypes.Length;
            //get the MethodInfo for CallMethod

            MethodInfo callMethod = BaseType.GetMethod("CallMethod", Aoite.Reflection.Flags.InstancePrivate, null, new Type[] { Types.Int32, Types.ObjectArray }, null);

            //declare local variables
            LocalBuilder resultLB = mIL.DeclareLocal(Types.ObjectArray); // object[] value

            mIL.Emit(OpCodes.Ldarg_0); //load "this"
            mIL.Emit(OpCodes.Ldc_I4, ident);
            mIL.Emit(OpCodes.Ldc_I4, pLength); //push the number of arguments
            mIL.Emit(OpCodes.Newarr, Types.Object); //create an array of objects, the length is inputArgTypes.Length.

            //store every input argument in the args array
            for(int i = 0; i < pLength; i++)
            {
                Type inputType = parameterTypes[i].IsByRef ? parameterTypes[i].GetElementType() : parameterTypes[i];

                mIL.Emit(OpCodes.Dup);
                mIL.Emit(OpCodes.Ldc_I4, i); //push the index onto the stack
                mIL.Emit(OpCodes.Ldarg, i + 1); //load the i'th argument. This might be an address			
                if(parameterTypes[i].IsByRef)
                {
                    if(inputType.IsValueType)
                    {
                        TypeBuilderHelper.LoadIndexByType(mIL, inputType);
                        mIL.Emit(OpCodes.Box, inputType);
                    }
                    else
                        mIL.Emit(OpCodes.Ldind_Ref);
                }
                else
                {
                    if(parameterTypes[i].IsValueType)
                        mIL.Emit(OpCodes.Box, parameterTypes[i]);
                }
                mIL.Emit(OpCodes.Stelem_Ref); //store the reference in the args array
            }
            mIL.Emit(OpCodes.Call, callMethod);
            mIL.Emit(OpCodes.Stloc, resultLB.LocalIndex); //store the value
            //store the results in the arguments
            for(int i = 0; i < pLength; i++)
            {
                if(parameterTypes[i].IsByRef)
                {
                    Type pType = parameterTypes[i].GetElementType();
                    mIL.Emit(OpCodes.Ldarg, i + 1); //load the address of the argument
                    mIL.Emit(OpCodes.Ldloc, resultLB.LocalIndex); //load the value array
                    mIL.Emit(OpCodes.Ldc_I4, i + 1); //load the index into the value array
                    mIL.Emit(OpCodes.Ldelem_Ref); //load the value in the index of the array
                    if(pType.IsValueType)
                    {
                        mIL.Emit(OpCodes.Unbox, pType);
                        TypeBuilderHelper.LoadIndexByType(mIL, pType);
                        TypeBuilderHelper.StoreIndexByType(mIL, pType);
                    }
                    else
                    {
                        mIL.Emit(OpCodes.Castclass, pType);
                        mIL.Emit(OpCodes.Stind_Ref); //store the unboxed value at the argument address
                    }
                }
            }
            if(returnType != Types.Void)
            {
                mIL.Emit(OpCodes.Ldloc, resultLB.LocalIndex); //load the value array
                mIL.Emit(OpCodes.Ldc_I4, 0); //load the index of the return value. Alway 0
                mIL.Emit(OpCodes.Ldelem_Ref); //load the value in the index of the array

                if(returnType.IsValueType)
                {
                    mIL.Emit(OpCodes.Unbox, returnType); //unbox it
                    TypeBuilderHelper.LoadIndexByType(mIL, returnType);
                }
                else mIL.Emit(OpCodes.Castclass, returnType);
            }
            mIL.Emit(OpCodes.Ret);
        }
    }
}
