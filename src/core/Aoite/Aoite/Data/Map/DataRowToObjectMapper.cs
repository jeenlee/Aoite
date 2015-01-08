using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Aoite.Data
{
    class DataRowToObjectMapper : MapperBase<object>
    {
        public DataRow FromValue;
        protected override void Fill()
        {
            foreach(DataColumn column in FromValue.Table.Columns)
                this.SetPropertyValue(column.ColumnName, column.DataType, FromValue[column]);
        }

    }
}
