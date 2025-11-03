using QueryX.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
            var valueType = typeof(TValue);

            if (!valueType.IsArray)
            {
                var typedValues = values.Select(v => v.ConvertValue(typeof(TValue))).Cast<TValue>().ToArray();
                return _customFilterDelegate(source, typedValues, @operatpr);
            }
            else
            {
                var typedValues = values.Select(v => v.ConvertValue(valueType.GetElementType()));

                var listType = valueType.IsArray ? typeof(List<>).MakeGenericType(valueType.GetElementType()) : typeof(List<>).MakeGenericType(valueType);
                var typedList = (IList)Activator.CreateInstance(listType)!;

                foreach (var item in typedValues)
                {
                    typedList.Add(item);
                }

                var arrayValues = new TValue[] { (typedList as dynamic).ToArray() };
                return _customFilterDelegate(source, arrayValues, @operatpr);
            }
        }
    }
}
