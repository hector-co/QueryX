using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace QueryX
{
    internal static class QueryTokenizer
    {
        const string SplitFilters = @";(?=(?:[^""]*""[^""]*"")*[^""]*$)";
        const string SplitFilter = @"\s(?=(?:[^""]*""[^""]*"")*[^""]*$)";
        const string SplitCommaSeparatedValues = @",(?=(?:[^""]*""[^""]*"")*[^""]*$)";

        public static IEnumerable<(string PropName, string Operator, IEnumerable<string?> Values)> GetFilterTokens(string filterString)
        {
            var filtersCollection = Regex.Split(filterString, SplitFilters)
                .Where(s => !string.IsNullOrEmpty(s));

            var result = new List<(string PropName, string Operator, IEnumerable<string?> Values)>();

            foreach (var filter in filtersCollection)
            {
                var filterParams = Regex.Split(filter, SplitFilter)
                    .Where(s => !string.IsNullOrEmpty(s)).ToArray();

                if (filterParams.Length != 3)
                    throw new Exception();

                result.Add((filterParams[0], filterParams[1], SplitValues(filterParams[2])));
            }

            return result;
        }

        private static IEnumerable<string?> SplitValues(string values)
        {
            return Regex.Split(values, SplitCommaSeparatedValues)
                .Select(v => v.Trim())
                .Select(v => v == "null" ? null : v.TrimStart('"').TrimEnd('"'));
        }
    }
}
