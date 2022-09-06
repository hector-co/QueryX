namespace QueryX.Parser
{
    internal enum QueryToken
    {
        None,
        LParen,
        RParen,
        And,
        Or,
        Comma,
        Operator,
        String,
        Number,
        Identifier
    }
}
