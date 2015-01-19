using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Aoite.Serialization.BinarySuite
{

    internal class AssemblyInfo
    {
        public readonly Assembly Assembly;
        public readonly string Fullname;
        public readonly string Name;
        public readonly string Version;

        public AssemblyInfo(string fullname)
            : this(
#if SILVERLIGHT
            Assembly.Load(fullname)
#else
            Assembly.LoadFrom(fullname)
#endif
, fullname) { }

        public AssemblyInfo(Assembly assembly)
            : this(assembly, assembly.FullName) { }

        public AssemblyInfo(Assembly assembly, string fullname)
        {
            this.Assembly = assembly;
            BuilderNameVersion(fullname, out this.Name, out this.Version);
            this.Fullname = fullname;
        }

        public AssemblyInfo(Assembly assembly, string fullname, string name, string version)
        {
            this.Assembly = assembly;
            this.Fullname = fullname;
            this.Name = name;
            this.Version = version;
        }

        public override string ToString()
        {
            return this.Fullname;
        }

        public static void BuilderNameVersion(string fullname, out string name, out string version)
        {
            name = null;
            version = null;
            string val = string.Empty;
            foreach(var c in fullname)
            {
                switch(c)
                {
                    case ' ':
                        val = string.Empty;
                        continue;
                    case ',':
                        if(name == null)
                        {
                            name = val;
                            continue;
                        }
                        version = val;
                        return;
                    default:
                        val += c;
                        break;
                }
            }
        }
    }
}
