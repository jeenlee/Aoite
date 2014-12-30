using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Aoite.Serialization.BinarySuite
{
    internal static class AssemblyInfoCollection
    {
        private static readonly List<AssemblyInfo> AssemblyInfos;
        public static readonly AssemblyInfo mscorlib;

        static AssemblyInfoCollection()
        {
            var assems = AppDomain.CurrentDomain.GetAssemblies();
            AssemblyInfos = new List<AssemblyInfo>(assems.Length);
            var ms = Types.String.Assembly;
            foreach(var assembly in assems)
            {
                var info = new AssemblyInfo(assembly);
                if(ms == assembly)
                {
                    mscorlib = info;
                }
                AssemblyInfos.Add(info);
            }
        }

        static void Refresh()
        {
            var assems = AppDomain.CurrentDomain.GetAssemblies();
            foreach(var assembly in assems)
            {
                AssemblyInfos.Add(new AssemblyInfo(assembly));
            }
        }

        public static void Clear()
        {
            AssemblyInfos.Clear();
        }

        public static AssemblyInfo Find(Assembly assembly)
        {
            foreach(var item in AssemblyInfos)
                if(item.Assembly == assembly) return item;
            return null;
        }

        public static AssemblyInfo Find(string name)
        {
            foreach(var item in AssemblyInfos)
                if(item.Name == name) return item;
            return null;
        }

        public static AssemblyInfo TryCreate(string assemblyFullname)
        {
            if(AssemblyInfos.Count == 0)
            {
                Refresh();
            }
            string rightName, rightVersion;
            AssemblyInfo.BuilderNameVersion(assemblyFullname, out rightName, out rightVersion);

            AssemblyInfo topLeft = null;
            foreach(var left in AssemblyInfos)
            {
                if(left.Fullname == assemblyFullname)
                {
                    topLeft = left;
                    break;
                }
                else if(left.Name == rightName)
                {
                    if(topLeft != null && string.Compare(topLeft.Version, left.Version) > 0)
                    {
                        continue;
                    }
                    topLeft = left;
                }
            }
            if(topLeft == null)
            {
                topLeft = new AssemblyInfo(assemblyFullname);
                AssemblyInfos.Add(topLeft);
            }
            return topLeft;
        }

        public static Type GetTypeFromQualifiedName(string assemblyQualifiedName)
        {
            int index = TypeNameIndex(assemblyQualifiedName);
            var typeName = assemblyQualifiedName.Substring(0, index);
            string assemblyName = assemblyQualifiedName.Substring(index + 2);
            return AssemblyInfoCollection.TryCreate(assemblyName).Assembly.GetType(typeName);
        }

        private static int TypeNameIndex(string assemblyQualifiedName)
        {
            int index = 0;
            int depth = 0;
            foreach(var c in assemblyQualifiedName)
            {
                switch(c)
                {
                    case '{':
                    case '(':
                    case '[':
                        depth++;
                        break;
                    case '}':
                    case ')':
                    case ']':
                        depth--;
                        break;
                    case ',':
                        if(depth == 0) return index;
                        break;
                }
                index++;
            }
            return -1;
        }
    }
}
