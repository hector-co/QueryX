using System.Collections.Generic;
using QueryX.Filters;

namespace QueryX
{
    public class Query
    {
        public List<(string propertyName, IFilter filter)> Filters { get; set; } = new List<(string propertyName, IFilter filter)>();
        public List<(string propertyName, bool ascending)> OrderBy { get; set; } = new List<(string propertyName, bool ascending)>();
        public int Offset { get; set; }
        public int Limit { get; set; }
    }
}
