using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.ServiceModel
{
    /// <summary>
    /// 表示一个契约的服务提供程序。
    /// </summary>
    public class ContractService : IEnumerable<ContractMethod>, IContractInfo
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

        ContractMethod[] IContractInfo.Methods
        {
            get { return GetAllMethods().ToArray(); }
        }
        private IEnumerable<ContractMethod> GetAllMethods()
        {
            foreach(var list in _Methods.Values)
            {
                foreach(var item in list)
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// 设置或获取后期映射的参数值数组的参数委托。
        /// </summary>
        public Func<Type, object[]> LastMappingValues { get; set; }
        /// <summary>
        /// 获取或设置契约的真正服务类型。
        /// </summary>
        public Type RealType { get; set; }

        /// <summary>
        /// 获取一个契约的服务实例。
        /// </summary>
        /// <param name="server">契约服务器。</param>
        /// <param name="lastMappingValues">后期映射的参数值数组。请保证数组顺序与构造函数的后期映射的参数顺序一致。</param>
        /// <returns>返回一个契约的服务实例。</returns>
        public virtual object GetServiceInstance(IContractServer server, params object[] lastMappingValues)
        {
            return server.Container.GetService(this.RealType ?? this._Type, lastMappingValues);
        }

        private Dictionary<string, List<ContractMethod>> _Methods;
        internal List<ContractMethod> FindMethods(string methodName)
        {
            List<ContractMethod> ms;
            if(_Methods.TryGetValue(methodName, out ms)) return ms;
            return null;
        }
        internal ContractMethod FindMethod(ContractRequest request)
        {
            List<ContractMethod> ms;
            if(_Methods.TryGetValue(request.MethodName, out ms))
            {
                var ps = request.Parameters;
                foreach(var m in ms)
                {
                    var pts = m.ParameterTypes;
                    var ispr = m.ParameterByRefs;
                    if(pts.Length != ps.Length) continue;
                    int matchIndex = 0;
                    for(int i = 0; i < ps.Length; i++)
                    {
                        var pt = pts[i];
                        if(ispr[i]) pt = pt.GetElementType();
                        var pv = ps[i];
                        if((pt.IsEnum && Enum.IsDefined(pt, pv)) || (pv == null && pt.IsClass) || pt.IsInstanceOfType(pv))
                        {
                            matchIndex++;
                        }
                    }
                    if(matchIndex == ps.Length) return m;
                }
            }
            return null;
        }

        private ContractFilterAttribute[] _Filters;
        /// <summary>
        /// 获取筛选器的集合。
        /// </summary>
        public ContractFilterAttribute[] Filters { get { return this._Filters; } }
        private ContractService(Type contractType)
        {
            ServiceModelShared.TestContractType(contractType);
            this._AllowAnonymous = contractType.IsDefined(AllowAnonymousAttribute.Type, true);
            this._Type = contractType;
            this._Name = contractType.GetServiceName();
            this._Methods = new Dictionary<string, List<ContractMethod>>(StringComparer.CurrentCultureIgnoreCase);
            int identity = 0;
            this._Filters = contractType.GetAttributes<ContractFilterAttribute>().OrderBy(filter => filter.Order).ToArray();
            foreach(var methodInfo in ServiceModelShared.GetAllMethods(contractType))
            {
                var name = methodInfo.GetContractName();
                List<ContractMethod> ms;
                if(!this._Methods.TryGetValue(name, out ms))
                {
                    _Methods.Add(name, ms = new List<ContractMethod>(1));
                }
                System.Reflection.ParameterInfo[] parameterInfos;
                var parameterTypes = methodInfo.GetParameterTypes(out parameterInfos);
                ms.Add(new ContractMethod(identity++, methodInfo, parameterInfos, parameterTypes));
            }
        }

        private readonly static Dictionary<Type, ContractService> Cache = new Dictionary<Type, ContractService>();

        /// <summary>
        /// 创建或获取一个契约的服务提供程序。
        /// </summary>
        /// <param name="contractType">契约的数据类型。</param>
        /// <returns>返回一个契约的服务提供程序。</returns>
        public static ContractService GetContractService(Type contractType)
        {
            ContractService contract;
            if(!Cache.TryGetValue(contractType, out contract))
            {
                lock(Cache)
                {
                    if(!Cache.TryGetValue(contractType, out contract))
                    {
                        Cache.Add(contractType, contract = new ContractService(contractType));
                    }
                }
            }
            return contract;
        }

        internal void ToJsonMetadata(StringBuilder builder, string aoiteName)
        {
            var name = this._Name.ToCamelCase();
            builder.AppendLine()
                .Append("    //").Append('=', 20).Append(this._Name).Append('=', 20).AppendLine().AppendLine();
            builder.Append("    ").Append(aoiteName).Append(".").Append(name).AppendLine(" = {};");

            foreach(var methods in this._Methods.Values)
            {
                if(methods.Count == 1)
                {
                    methods.First().ToJsonMetadata(builder, aoiteName, name);
                }
            }

            builder.Append("    //").Append('=', 20).Append(this._Name).Append('=', 20).AppendLine().AppendLine();
        }

        /// <summary>
        /// 返回一个循环访问集合的枚举器。
        /// </summary>
        /// <returns>可用于循环访问集合的 <see cref="System.Collections.Generic.IEnumerator&lt;ContractMethod&gt;"/>。</returns>
        public IEnumerator<ContractMethod> GetEnumerator()
        {
            foreach(var item in _Methods)
            {
                foreach(var item2 in item.Value)
                {
                    yield return item2;
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

       
    }
}
