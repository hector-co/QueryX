namespace QueryX.Filters
{
    public class SortValue
    {
        private string _modelPropertyName = string.Empty;

        public string PropertyName { get; set; } = string.Empty;
        public string ModelPropertyName
        {
            get
            {
                return string.IsNullOrEmpty(_modelPropertyName)
                    ? PropertyName
                    : _modelPropertyName;
            }
            set { _modelPropertyName = value; }
        }
        public bool Ascending { get; set; }
    }
}
