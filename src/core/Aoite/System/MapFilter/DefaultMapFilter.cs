using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System
{
    /// <summary>
    /// 默认实现依赖注入与控制反转的映射筛选器。
    /// </summary>
    public class DefaultMapFilter : IMapFilter
    {
        /// <summary>
        /// 获取或设置筛选器的实际类型的完全限定名的格式项，例如“{0}.Default{1}”，索引 0 表示 - 预期定义接口 - 的命名空间，索引 1 表示 - 预期定义接口 - 的名称（已去 I）。
        /// </summary>
        public readonly static HashSet<string> DefaultFormats = new HashSet<string>() { "{0}.Default{1}", "{0}.{1}", "{0}.Fake{1}", "{0}.Mock{1}" };

        internal static string GetActualType(string format, Type expectType)
        {
            var name = expectType.Name;
            var ns = expectType.Namespace;
            if(name[0] == 'I') name = name.Remove(0, 1);
            return string.Format(format, expectType.Namespace, name);
        }
        internal static HashSet<string> GetAllActualType(Type expectType)
        {
            if(expectType.DeclaringType != null) throw new ArgumentException("为了设计规范，预期定义“" + expectType.FullName + "”不能是一个嵌套类。", "expectType");

            HashSet<string> fullNames = new HashSet<string>();
            var name = expectType.Name;
            var ns = expectType.Namespace;
            if(name[0] == 'I') name = name.Remove(0, 1);
            foreach(var item in DefaultFormats)
            {
                if(item == null) continue;
                fullNames.Add(string.Format(item, expectType.Namespace, name));
            }
            return fullNames;
        }

        private string _ActualTypeFullNameFormat = null;
        /// <summary>
        /// 获取或设置筛选器的实际类型的完全限定名的格式项，例如“{0}.Default{1}”，索引 0 表示 - 预期定义接口 - 的命名空间，索引 1 表示 - 预期定义接口 - 的名称（已去 I）。
        /// </summary>
        public virtual string ActualTypeFullNameFormat { get { return this._ActualTypeFullNameFormat; } set { this._ActualTypeFullNameFormat = value; } }

        private NamesapceRule[] _Rules;
        /// <summary>
        /// 获取筛选器的命名空间规则列表。
        /// </summary>
        public virtual NamesapceRule[] Rules { get { return this._Rules; } }

        private void LoadAssembly(string name)
        {
            try
            {
                AssemblyName assemblyName = name.iEndsWith(".dll")
                    ? AssemblyName.GetAssemblyName(name)
                    : new AssemblyName(name);
                Assembly.Load(assemblyName);
                return;
            }
            catch(Exception) { }
        }

        /// <summary>
        /// 指定筛选器的命名空间表达式，初始化一个 <see cref="System.DefaultMapFilter"/> 类的新实例。
        /// </summary>
        /// <param name="namespaceExpression">筛选器的命名空间表达式。可以是一个完整的命名空间，也可以是“*”起始，或者以“*”结尾。符号“*”只能出现一次。通过“|”可以同时包含多个命名空间。</param>
        public DefaultMapFilter(string namespaceExpression)
        {
            if(string.IsNullOrEmpty(namespaceExpression)) throw new ArgumentNullException("namespaceExpression");
            var nes = namespaceExpression.Split('|');
            List<NamesapceRule> rules = new List<NamesapceRule>(nes.Length);
            //var refreshAllTypes = false;
            foreach(var ne in nes)
            {
                var t_ne = ne.Trim();
                if(string.IsNullOrEmpty(t_ne)) continue;
                if(t_ne[0] == '$')
                {
                    this.LoadAssembly(t_ne.Remove(0, 1));
                    //refreshAllTypes = true;
                }
                else rules.Add(new NamesapceRule(t_ne));
            }
            //if(refreshAllTypes) ObjectFactory.RefreshAllTypes();
            if(rules.Count == 0) throw new ArgumentOutOfRangeException("namespaceExpression");
            this._Rules = rules.ToArray();
        }

        /// <summary>
        /// 判断指定命名空间是否匹配规则。
        /// </summary>
        /// <param name="namespace">一个命名空间。</param>
        /// <returns>如果匹配返回 true，否则返回 false。</returns>
        public virtual bool NamespaceIsMatch(string @namespace)
        {
            foreach(var rule in _Rules)
            {
                if(rule.IsMatch(@namespace)) return true;
            }
            return false;
        }

        /// <summary>
        /// 判断指定 <paramref name="expectType"/> 和 <paramref name="actualType"/> 是否开启单例模式。
        /// </summary>
        /// <param name="expectType">预期定义的类型。</param>
        /// <param name="actualType">实际映射的类型。</param>
        /// <returns>如果启用单例模式则返回 true，否则返回 false。</returns>
        public virtual bool IsSingletonMode(Type expectType, Type actualType)
        {
            if(expectType == null) throw new ArgumentNullException("expectType");
            if(actualType == null) throw new ArgumentNullException("actualType");
            return false;
        }

        /// <summary>
        /// 判断指定 <paramref name="type"/> 是否是一个预期定义的类型。
        /// </summary>
        /// <param name="type">类型。</param>
        /// <returns>如果 <paramref name="type"/> 是一个预期的类型返回 true，否则返回 false。</returns>
        public virtual bool IsExpectType(Type type)
        {
            if(type == null) throw new ArgumentNullException("expectType");
            return type.IsInterface
                && type.IsPublic
                && !type.IsDefined(IgnoreAttribute.Type, true)
                && this.NamespaceIsMatch(type.Namespace);
        }

        /// <summary>
        /// 根据指定的 <paramref name="expectType"/> ，查找在 <paramref name="allTypes"/> 集合中对应的实际定义的类型。
        /// </summary>
        /// <param name="allTypes">当前应用程序所有已加载的类型。</param>
        /// <param name="expectType">预期定义的类型。</param>
        /// <returns>如果找到返回一个 <see cref="System.Type"/> 的实例，否则返回 null 值。</returns>
        public virtual Type FindActualType(IEnumerable<IGrouping<string, Type>> allTypes, Type expectType)
        {
            if(allTypes == null) throw new ArgumentNullException("allTypes");
            if(expectType == null) throw new ArgumentNullException("expectType");
            var fullNames = GetAllActualType(expectType);
            if(this._ActualTypeFullNameFormat != null) fullNames.Add(GetActualType(this._ActualTypeFullNameFormat, expectType));
            return FindActualType(allTypes, expectType, fullNames);
        }

        internal static Type FindActualType(IEnumerable<IGrouping<string, Type>> allTypes, Type expectType, HashSet<string> fullNames)
        {
            return (from item in allTypes
                    where fullNames.Contains(item.Key)
                    from type in item
                    where expectType.IsAssignableFrom(type) && !type.IsDefined(IgnoreAttribute.Type, true)
                    select type).FirstOrDefault();
        }
    }
}
