namespace QueryX
{
    public class QueryModel
    {
        public string Filter { get; set; } = string.Empty;
        public string Sort { get; set; } = string.Empty;
        public int Offset { get; set; }
        public int Limit { get; set; }
    }
}
