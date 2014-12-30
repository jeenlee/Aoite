using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Aoite.ServiceModel.Locals
{
    /// <summary>
    /// 表示一个本地的客户端。
    /// </summary>
    public abstract partial class LocalClient
    {
        const string LocalClientEndPoint = ":1";
        private ContractServerBase _server;
        private ServerConfigurationBase _config;
        private ContractService _service;
        private Dictionary<int, ContractMethod> _methods;

        /// <summary>
        /// 初始化一个 <see cref="Aoite.ServiceModel.Locals.LocalClient"/> 类的新实例。
        /// </summary>
        /// <param name="server">契约服务器。</param>
        /// <param name="service">契约服务。</param>
        /// <param name="methods">契约方法组。</param>
        public LocalClient(ContractServerBase server, ContractService service, Dictionary<int, ContractMethod> methods)
        {
            if(server == null) throw new ArgumentNullException("server");
            if(service == null) throw new ArgumentNullException("service");
            if(methods == null) throw new ArgumentNullException("methods");

            this._server = server;
            this._service = service;
            this._config = (server as IContractServer).Configuration;
            this._methods = methods;
        }

        /// <summary>
        /// 调用指定方法标识索引和参数数组的服务端方法。
        /// </summary>
        /// <param name="identity">方法的标识索引。</param>
        /// <param name="parameters">方法的参数集合。</param>
        /// <returns>一组方法调用后的返回值。索引 (0) 表示方法的返回值，其他表示 "ref" 或 "out" 的返回值。</returns>
        protected virtual object[] CallMethod(int identity, params object[] parameters)
        {
            var method = this._methods[identity];
            var ownerContext = ContractContext.Current;
            var hasOwner = ownerContext != null;
            var request = new ContractRequest(ownerContext, this._service, method, parameters, (this as IContractClient).GetFiles(false));

            request.SessionId = this._server.SessionProvider.SessionId;
            if(hasOwner && string.IsNullOrEmpty(request.SessionId)) request.SessionId = ownerContext._ResponseSessionId;

            var context = hasOwner
                ? new LocalClientContractContext(ownerContext, this._server, request, LocalClientEndPoint)
                : new ContractContext(this._server, request, LocalClientEndPoint);
            context._Service = this._service;
            context._Method = method;
            context._IsLocalClient = true;

            ContractContext.Current = context;
            try
            {
                method.Validation(parameters);
                this._server.Call(context);
                var response = context.Source.Response;
                if(response.Status != StatusCode.OK) throw new ResponseException(response.Status, response.Message);
                if(response.Files != null)
                    this._lastFiles.AddRange(response.Files);

                if(hasOwner && string.IsNullOrEmpty(response.SessionId)) response.SessionId = ownerContext._ResponseSessionId;
                if(!string.IsNullOrEmpty(response.SessionId))  this._server.SessionProvider.SessionId = response.SessionId;

                return response.Values;
            }
            catch(Exception ex)
            {
                var returnType = method.ReturnType;
                if(returnType != Types.Result
                  && !returnType.IsSubclassOf(Types.Result)) throw;

                try
                {
                    var r = Activator.CreateInstance(returnType) as Result;
                    if(ex is System.ComponentModel.DataAnnotations.ValidationException) r.ToFailded(ex, 1);
                    else if(ex is ResponseException) r.ToFailded(ex, (int)(ex as ResponseException).Status);
                    else r.ToFailded(ex);

                    var result = new object[method.ParameterTypes.Length + 1];
                    result[0] = r;
                    return result;
                }
                catch(Exception) { }

                throw;
            }
            finally
            {
                ContractContext.Current = ownerContext;
            }
        }
    }
    partial class LocalClient : ObjectDisposableBase, IContractClient
    {
        ContractDomain IContractClient.Domain
        {
            get { throw new NotImplementedException(); }
        }

        IContractInfo IContractClient.Contract { get { return this._service; } }

        IContractLifeCycle IContractClient.LifeCycle { get { throw new NotImplementedException(); } }

        bool IContractClient.KeepAlive
        {
            get { return true; }
        }

        private List<ContractFile> _lastFiles = new List<ContractFile>();

        void IContractClient.AddFile(string fileName, int fileSize, Stream content)
        {
            this._lastFiles.Add(new ContractFile(fileName, fileSize, content));
        }

        void IContractClient.AddFile(string fileName)
        {
            if(string.IsNullOrEmpty(fileName)) throw new ArgumentNullException("fileName");
            using(var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                this._lastFiles.Add(new ContractFile(fileStream.Name, (int)fileStream.Length, fileStream));
            }
        }

        void IContractClient.AddFile(FileStream fileStream)
        {
            if(fileStream == null) throw new ArgumentNullException("fileStream");
            this._lastFiles.Add(new ContractFile(fileStream.Name, (int)fileStream.Length, fileStream));
        }

        void IContractClient.AddFile(ContractFile file)
        {
            if(file == null) throw new ArgumentNullException("file");
            this._lastFiles.Add(file);
        }
        ContractFile[] IContractClient.GetFiles(bool peekTime)
        {
            if(this._lastFiles.Count == 0) return null;

            var files = this._lastFiles.ToArray();
            if(!peekTime) this._lastFiles = new List<ContractFile>();
            return files;
        }
    }
    partial class LocalClient
    {
        private const string NamePrefix = "<LOCAL_CONTRACT>";

        private static readonly Type BaseType = typeof(LocalClient);
        private static readonly Type[] BaseTypeParameters = new Type[] { typeof(ContractServerBase), typeof(ContractService), typeof(Dictionary<int, ContractMethod>) };
        private static readonly ConstructorInfo BaseTypeConstructor = BaseType.GetConstructor(BaseTypeParameters);
        private static readonly Dictionary<Type, Lazy<LocalClient>> LazyClientCache = new Dictionary<Type, Lazy<LocalClient>>();

        internal static LocalClient CreateLocalClient(ContractServerBase server, ContractService service)
        {
            var type = service.Type;
            Lazy<LocalClient> client;
            if(!LazyClientCache.TryGetValue(type, out client))
                lock(type)
                    if(!LazyClientCache.TryGetValue(type, out client))
                    {
                        var contractMethods = service.ToDictionary<ContractMethod, int>(m => m.Identity);
                        var invoker = CreateConstructorInvoker(type, contractMethods);
                        client = new Lazy<LocalClient>(() => (LocalClient)invoker(server, service, contractMethods));
                        LazyClientCache.Add(type, client);
                    }
            return client.Value;
        }

        private static Aoite.Reflection.ConstructorInvoker CreateConstructorInvoker(Type contractType, Dictionary<int, ContractMethod> contractMethods)
        {
            contractType.TestContractType();

            var typeBuilder = CreateTypeBuilder(contractType);
            foreach(var item in contractMethods)
            {
                DefineContractMethod(typeBuilder, item.Value);
            }
            var proxyType = typeBuilder.CreateType();
            return Aoite.Reflection.ConstructorInfoExtensions.DelegateForCreateInstance(proxyType.GetConstructor(BaseTypeParameters));
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
                call base.ctor(String)      调用的方法（父类构造方法，也是一个方法）
                ret
             }
             */

            ILGenerator ctorIL = ctor.GetILGenerator();
            ctorIL.Emit(OpCodes.Ldarg_0);           //load "this"
            ctorIL.Emit(OpCodes.Ldarg_1);           //load "ContractServerBase"
            ctorIL.Emit(OpCodes.Ldarg_2);           //load "ContractService"
            ctorIL.Emit(OpCodes.Ldarg_3);           //load "Dictionary<int, ContractMethod>"
            ctorIL.Emit(OpCodes.Call, BaseTypeConstructor);    //call "base(...)"
            ctorIL.Emit(OpCodes.Ret);

            return typeBuilder;
        }

        private static void DefineContractMethod(TypeBuilder typeBuilder, ContractMethod contractMethod)
        {
            var methodInfo = contractMethod.MethodInfo;
            ParameterInfo[] parameterInfos;
            Type[] parameterTypes = methodInfo.GetParameterTypes(out parameterInfos);
            var returnType = methodInfo.ReturnType;
            MethodBuilder methodBuilder = typeBuilder.DefineMethod(methodInfo.Name, MethodAttributes.Public | MethodAttributes.Virtual, returnType, parameterTypes);

            ILGenerator mIL = methodBuilder.GetILGenerator();
            GenerateILCodeForMethod(contractMethod.Identity, mIL, returnType, parameterTypes);
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
