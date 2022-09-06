using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace QueryX.Parser
{
    static class QueryTokenizer
    {
        static TextParser<Unit> MultiOperator { get; } =
            from _ in Span.Regex("[^a-zA-Z0-9_\\s\\;'\\(\\)]{2,}")
            select Unit.Value;

        static TextParser<Unit> SingleOperator { get; } =
            from _ in Character.In('<', '>').AtLeastOnce()
            select Unit.Value;

        static TextParser<Unit> Operator { get; } =
            from _ in MultiOperator
                .Or(SingleOperator)
            select Unit.Value;

        static TextParser<Unit> CustomIdentifier { get; } =
            from _ in Identifier.CStyle.ManyDelimitedBy(Character.EqualTo('.'))
            select Unit.Value;

        internal static Tokenizer<QueryToken> Instance { get; } =
            new TokenizerBuilder<QueryToken>()
                .Ignore(Span.WhiteSpace)
                .Match(Operator, QueryToken.Operator)
                .Match(Character.EqualTo('('), QueryToken.LParen)
                .Match(Character.EqualTo(')'), QueryToken.RParen)
                .Match(Character.In('&', ';'), QueryToken.And)
                .Match(Character.EqualTo('|'), QueryToken.Or)
                .Match(Character.EqualTo(','), QueryToken.Comma)
                .Match(QuotedString.SqlStyle, QueryToken.String)
                .Match(Numerics.Decimal, QueryToken.Number)
                .Match(CustomIdentifier, QueryToken.Identifier, requireDelimiters: true)
                .Build();
    }
}
