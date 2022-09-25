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
        Exclamation,
        Operator,
        String,
        Number,
        Identifier
    }
}
