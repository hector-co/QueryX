namespace QueryX.Filters
{
    public enum OperatorType
    {
        None,
        Equals,
        CiEquals,
        NotEquals,
        CiNotEquals,
        LessThan,
        LessThanOrEquals,
        GreaterThan,
        GreaterThanOrEquals,
        Contains,
        CiContains,
        StartsWith,
        CiStartsWith,
        EndsWith,
        CiEndsWith,
        In,
        NotIn
    }
}
