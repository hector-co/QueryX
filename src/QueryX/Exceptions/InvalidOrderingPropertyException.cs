namespace QueryX.Exceptions
{
    public class InvalidOrderingPropertyException : QueryException
    {
        public string PropertyName { get; private set; }

        public InvalidOrderingPropertyException(string propertyName)
            : base($"Invalid ordering property: '{propertyName}'")
        {
            PropertyName = propertyName;
        }
    }
}
