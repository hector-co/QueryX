namespace QueryX.Exceptions
{
    public class InvalidFilterPropertyException : QueryException
    {
        public string PropertyName { get; private set; }

        public InvalidFilterPropertyException(string propertyName)
            : base($"Invalid filter property: '{propertyName}'")
        {
            PropertyName = propertyName;
        }
    }
}
