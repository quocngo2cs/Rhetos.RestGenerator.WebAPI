
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rhetos.WebApiRestGenerator.Utilities
{
    public class RecordsAndTotalCountResult<T>
    {
        public T[] Records { get; set; }
        public int TotalCount { get; set; }
    }
}
