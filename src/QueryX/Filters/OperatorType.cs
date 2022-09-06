namespace QueryX.Filters
{
    public class OperatorType
    {
        public const string EqualsFilter = "==";
        public const string CiEqualsFilter = "==*";
        public const string NotEqualsFilter = "!=";
        public const string CiNotEqualsFilter = "!=*";
        public const string LessThanFilter = "<";
        public const string LessThanOrEqualsFilter = "<=";
        public const string GreaterThanFilter = ">";
        public const string GreaterThanOrEqualsFilter = ">=";
        public const string ContainsFilter = "-=-";
        public const string CiContainsFilter = "-=-*";
        public const string StartsWithFilter = "=-";
        public const string CiStartsWithFilter = "=-*";
        public const string EndsWithFilter = "-=";
        public const string CiEndsWithFilter = "-=*";
        public const string InFilter = "|=";
        public const string NotInFilter = "!|=";
    }
}
