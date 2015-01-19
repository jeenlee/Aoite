using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System
{

    /// <summary>
    /// 一个依赖注入与控制反转的工厂对象。
    /// </summary>
    public static partial class ObjectFactory
    {
        /// <summary>
        /// 表示解析映射类型时发生。
        /// </summary>
        public static event MapResolveEventHandler MapResolve;
        /// <summary>
        /// 表示解析后期映射类型时发生。
        /// </summary>
        public static event MapResolveEventHandler LastMappingResolve;
        /// <summary>
        /// 获取默认的全局服务容器。
        /// </summary>
        public readonly static IocContainer Global = new IocContainer();

        /// <summary>
        /// 获取当前应用程序域的所有有效类型。
        /// </summary>
        public static IEnumerable<IGrouping<string, Type>> AllTypes    { get { return AllTypesCreateFactory(); }   }
        /*
        static ObjectFactory()
        {
            var dominsConfPath = GA.FullPath("domains.conf");
            if(File.Exists(dominsConfPath))
            {
                var conf = File.ReadAllText(dominsConfPath, GA.UTF8);
                var domains = JsonConf.LoadFromConf<Aoite.ServiceModel.ContractDomain[]>("[" + conf + "]");
                foreach(var domain in domains) Global.AutoMap(domain);
            }
        }
        */
        private static MapResolveEventArgs InternalOnEvent(MapResolveEventHandler handler, object sender, Type expectType)
        {
            if(handler != null)
            {
                var e = new MapResolveEventArgs(expectType);
                handler(sender, e);
                if(e.Callback == null) return null;
                return e;
            }
            return null;
        }
        internal static MapResolveEventArgs InternalOnMapResolve(MapResolveEventHandler handler, object sender, Type expectType)
        {
            return InternalOnEvent(handler ?? MapResolve, sender, expectType);
        }
        internal static MapResolveEventArgs InternalOnLastMappingResolve(MapResolveEventHandler handler, object sender, Type expectType)
        {
            return InternalOnEvent(handler ?? LastMappingResolve, sender, expectType);
        }

        private static IEnumerable<IGrouping<string, Type>> AllTypesCreateFactory()
        {
            try
            {
                return from a in AppDomain.CurrentDomain.GetAssemblies()
                       let pkToken = BitConverter.ToString(a.GetName().GetPublicKeyToken())
                       where pkToken != "B7-7A-5C-56-19-34-E0-89" && pkToken != "B0-3F-5F-7F-11-D5-0A-3A" && !a.IsDynamic
                         && a.ManifestModule.Name != "<In Memory Module>"
                         && !a.FullName.StartsWith("System")
                         && !a.FullName.StartsWith("Microsoft")
                         && a.Location.IndexOf("App_Web") == -1
                         && a.Location.IndexOf("App_global") == -1
                         && a.FullName.IndexOf("CppCodeProvider") == -1
                         && a.FullName.IndexOf("WebMatrix") == -1
                         && a.FullName.IndexOf("SMDiagnostics") == -1
                         && !String.IsNullOrEmpty(a.Location)
                       from t in a.GetTypes()
                       where !t.Name.StartsWith("<>") && !t.IsSpecialName && !t.IsCOMObject
                       group t by t.FullName into g
                       select g;
            }
            catch(Exception)
            {

                throw;
            }
        }

        //private readonly static Mean<IEnumerable<IGrouping<string, Type>>> meanAllTypes = new Mean<IEnumerable<IGrouping<string, Type>>>(AllTypesCreateFactory);

        //static ObjectFactory()
        //{
        //    AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
        //}

        //static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        //{

        //}
        ///// <summary>
        ///// 刷新当前应用程序域的所有有效类型。
        ///// </summary>
        //public static void RefreshAllTypes()
        //{
        //    meanAllTypes.Reset();
        //}

    }
}
