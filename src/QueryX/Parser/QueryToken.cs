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
        Asterisk,
        Operator,
        String,
        Number,
        Identifier
    }
}
