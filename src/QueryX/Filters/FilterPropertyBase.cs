using System.Linq.Expressions;

namespace QueryX.Filters
{
    public abstract class FilterPropertyBase<TValue> : IFilterProperty
    {
        private string _modelPropertyName = string.Empty;
        public string PropertyName { get; internal set; } = string.Empty;
        public string ModelPropertyName
        {
            get
            {
                return string.IsNullOrEmpty(_modelPropertyName)
                    ? PropertyName
                    : _modelPropertyName;
            }
            internal set { _modelPropertyName = value; }
        }
        public bool IsCustomFilter { get; internal set; }

        public abstract void SetValueFromString(params string?[] values);

        public virtual Expression? GetFilterExpression<TModel>(ParameterExpression modelParameter)
        {
            var propExp = ModelPropertyName.GetPropertyExpression<TModel>(modelParameter);
            if (propExp == null)
                return null;

            return GetExpression(propExp);
        }

        protected abstract Expression GetExpression(Expression property);

        public void SetOptions(string propertyName, string modelPropertyName, bool isCustomFilter)
        {
            PropertyName = propertyName;
            ModelPropertyName = modelPropertyName;
            IsCustomFilter = isCustomFilter;
        }
    }
}