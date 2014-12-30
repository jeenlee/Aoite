using Aoite.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Aoite.ServiceModel
{
    static class ServiceModelShared
    {
        const string ServiceNameSuffix = "Service";
        private const string TcpScheme = "tcp";
        public static void TestTcpScheme(this Uri url)
        {
            if(!TcpScheme.iEquals(url.Scheme))
                throw new ArgumentException("服务地址的 URI 方案名称必须是“" + TcpScheme + "”。", "domain");
        }
        public static string GetContractName(this MethodInfo info)
        {
            var cna = info.GetAttribute<ContractNameAttribute>();
            if(cna != null && !string.IsNullOrEmpty(cna.Name)) return cna.Name;
            return info.Name;
        }
        public static IEnumerable<MethodInfo> GetAllMethods(this Type type)
        {
            foreach(var m in type.GetMethods())
            {
                m.TestContractMethod();
                yield return m;
            }
            foreach(var i in type.GetInterfaces())
            {
                if(i == Types.IDisposable) continue;
                type.TestContractType();
                foreach(var m in i.GetMethods())
                {
                    m.TestContractMethod();
                    yield return m;
                }
            }
        }
        public static string GetServiceName(this Type type)
        {
            var cna = type.GetAttribute<ContractNameAttribute>();
            if(cna != null && !string.IsNullOrEmpty(cna.Name)) return cna.Name;

            var name = type.Name;
            if(name[0] == 'I') name = name.Remove(0, 1);
            if(name.EndsWith(ServiceNameSuffix)) name = name.RemoveEnd(ServiceNameSuffix.Length);
            return name;
        }

        public static Type[] GetParameterTypes(this MethodInfo methodInfo, out ParameterInfo[] parameterInfos)
        {
            parameterInfos = methodInfo.GetParameters();
            int pLength = parameterInfos.Length;

            Type[] parameterTypes = new Type[pLength];
            for(int i = 0; i < pLength; i++)
            {
                parameterTypes[i] = parameterInfos[i].ParameterType;
            }
            return parameterTypes;
        }
        public static void TestContractMethod(this MethodInfo methodInfo)
        {
            if(methodInfo.IsGenericMethod) throw new NotSupportedException("类型“" + methodInfo.DeclaringType.FullName + "." + methodInfo.Name + "”不能是一个泛型方法。");
        }

        public static void TestContractType(this Type contractType)
        {
            if(contractType == null) throw new ArgumentNullException("contractType");
            if(!contractType.IsPublic || !contractType.IsInterface) throw new NotSupportedException("类型“" + contractType.FullName + "”必须是一个公共接口。");
            if(contractType.IsGenericType) throw new NotSupportedException("类型“" + contractType.FullName + "”不能是一个泛型类型。");

            var firstMember = contractType.GetEvents().FirstOrDefault() as MemberInfo
                ?? contractType.GetProperties().FirstOrDefault() as MemberInfo;
            if(firstMember != null) throw new NotSupportedException("不支持“事件”和“属性”的接口定义。发生错误的是“" + contractType.FullName + "." + firstMember.Name + "”");
        }


        internal static ICacheProvider CreateCacheProvider(this IContractServer server, string cacheProviderType)
        {
            Dictionary<string, object> options = new Dictionary<string, object>(server.Configuration.ExtendData, StringComparer.OrdinalIgnoreCase)
            {
                { CacheOptions.SessionExpiration, server.Configuration.SessionExpiration }
            };
            if(string.IsNullOrEmpty(cacheProviderType)) return new MemoryCacheProvider(options);
            else
            {
                var type = Type.GetType(cacheProviderType, true, true);
                if(!typeof(ICacheProvider).IsAssignableFrom(type)) throw new TypeLoadException("类型 {0} 没有实现 Aoite.ServiceModel.ICacheProvider 接口。".Fmt(type.FullName));
                if(type.GetConstructor(new Type[] { typeof(Dictionary<string, object>) }) == null) throw new TypeLoadException("类型 {0} 必须包含一个 ctor(System.Collections.Generic.Dictionary<string, object>) 的构造函数。".Fmt(type.FullName));
                try
                {
                    return Activator.CreateInstance(type, options) as ICacheProvider;
                }
                catch(Exception ex)
                {
                    if(ex.InnerException != null) throw ex.InnerException;
                    throw;
                }
            }
        }
    }
}
