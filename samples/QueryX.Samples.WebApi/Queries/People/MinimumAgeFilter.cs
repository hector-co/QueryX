using QueryX.Filters;
using QueryX.Samples.WebApi.Domain;

namespace QueryX.Samples.WebApi.Queries.People
{
    public class MinimumAgeFilter : CustomFilter<int?>
    {
        public MinimumAgeFilter(OperatorType @operator, IEnumerable<int?> values) : base(@operator, values)
        {
            var age = values.First() ?? 0;
            var minimumYear = DateTime.Now.Year - age;

            SetFilterExpression<Person>(p => p.Birthday.Year <= minimumYear);
        }
    }
}
