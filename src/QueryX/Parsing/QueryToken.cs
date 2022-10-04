namespace QueryX.Parsing
{
    internal enum QueryToken
    {
        Unknown,
        LParen,
        RParen,
        And,
        Or,
        Comma,
        Asterisk,
        Exclamation,
        Operator,
        String,
        Number,
        Identifier
    }
}
