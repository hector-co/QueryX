using System.Linq;
using System;
using QueryX.Utils;

namespace QueryX
{
    internal interface ICustomFilter
    {

    }

    internal interface ICustomFilter<TModel> : ICustomFilter
    {
        IQueryable<TModel> Apply(IQueryable<TModel> source, string?[] values, FilterOperator @operator);
    }

    internal class CustomFilter<TModel, TValue> : ICustomFilter<TModel>
    {
        private readonly Func<IQueryable<TModel>, TValue[], FilterOperator, IQueryable<TModel>> _customFilterDelegate;

        internal CustomFilter(Func<IQueryable<TModel>, TValue[], FilterOperator, IQueryable<TModel>> customFilterDeleagate)
        {
            _customFilterDelegate = customFilterDeleagate;
        }

        IQueryable<TModel> ICustomFilter<TModel>.Apply(IQueryable<TModel> source, string?[] values, FilterOperator @operatpr)
        {
            var typedValues = values.Select(v => v.ConvertValue(typeof(TValue))).Cast<TValue>().ToArray();
            return _customFilterDelegate(source, typedValues, @operatpr);
        }
    }
}
