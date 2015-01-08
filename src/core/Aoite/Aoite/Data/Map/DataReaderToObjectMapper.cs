using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Aoite.Data
{
    class DataReaderToObjectMapper : MapperBase<object>
    {
        public IDataReader FromValue;
        protected override void Fill()
        {
            for(int i = 0; i < FromValue.FieldCount; i++)
                this.SetPropertyValue(FromValue.GetName(i), FromValue.GetFieldType(i), FromValue.GetValue(i));
        }
    }
}
