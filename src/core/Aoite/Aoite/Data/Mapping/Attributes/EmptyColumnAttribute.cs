using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aoite.Data
{
    internal class EmptyColumnAttribute : ColumnAttribute
    {
        public static readonly ColumnAttribute Empty = new EmptyColumnAttribute();
        private EmptyColumnAttribute() { }
    }
}
