using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Aoite.Data
{
    class DynamicEntityValue : System.Dynamic.DynamicObject
    {
        private Dictionary<string, object> _nameValues = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        public DynamicEntityValue(DbDataReader reader)
        {
            for(int i = 0; i < reader.FieldCount; i++)
            {
                this._nameValues.Add(reader.GetName(i), reader.GetValue(i));
            }
        }

        public override bool TryGetMember(System.Dynamic.GetMemberBinder binder, out object result)
        {
            return this._nameValues.TryGetValue(binder.Name, out result);
        }
        public override bool TrySetMember(System.Dynamic.SetMemberBinder binder, object value)
        {
            this._nameValues[binder.Name] = value;
            return true;
        }
    }
}
