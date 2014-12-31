using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    class TypeMapper : ITypeMapper
    {
        //protected readonly Dictionary<string, PropertyMapper> _Properties;
        /// <summary>
        /// 初始化一个 <see cref="System.TypeMapper"/> 类的新实例。
        /// </summary>
        public TypeMapper() : this(0) { }

        internal TypeMapper(int capacity)
        {
            //this._Properties = new Dictionary<string, PropertyMapper>(capacity, StringComparer.CurrentCultureIgnoreCase);
        }
    }
}
