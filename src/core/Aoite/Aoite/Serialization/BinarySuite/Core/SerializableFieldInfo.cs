using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Aoite.Reflection;

namespace Aoite.Serialization.BinarySuite
{
    internal class SerializableFieldInfo
    {
        public readonly FieldInfo Field;
        public readonly string Name;
        //public readonly int NameHashCode;
        public readonly MemberGetter GetValue;
        public readonly MemberSetter SetValue;

        public SerializableFieldInfo(FieldInfo field, int depth)
        {
            this.Field = field;
            if(depth == 0) this.Name = field.Name;
            else this.Name = depth.ToString() + "#" + field.Name;
            this.GetValue = field.DelegateForGetFieldValue();
            this.SetValue = field.DelegateForSetFieldValue();
            
            //this.Name
            //this.NameHashCode = this.Name.GetHashCode();
        }

    }
}
