using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
namespace System
{
    public class GridDataTests
    {
        [Fact()]
        public void Test()
        {
            GridData<int> grid = new GridData<int>()
            {
                Total = 5,
                Rows = new int[] { 1, 2, 3, 4, 5 }
            };
            Assert.Equal(5, grid.Total);
            Assert.Equal(new int[] { 1, 2, 3, 4, 5 }, grid.Rows);
        }
    }
}
