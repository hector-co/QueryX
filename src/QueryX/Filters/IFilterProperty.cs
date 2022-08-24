namespace QueryX.Filters
{
    public interface IFilterProperty : IFilter
    {
        string PropertyName { get; }
        string ModelPropertyName { get; }
        bool IsCustomFilter { get; }
        void SetValueFromString(params string?[] values);

        void SetOptions(string propertyName, string modelPropertyName, bool isCustomFilter);
    }
}
