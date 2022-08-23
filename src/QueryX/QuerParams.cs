namespace QueryX
{
    public class QuerParams
    {
        public string Filter { get; set; } = string.Empty;
        public string OrderBy { get; set; } = string.Empty;
        public int Offset { get; set; }
        public int Limit { get; set; }
    }
}
