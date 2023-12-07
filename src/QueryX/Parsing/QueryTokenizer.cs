using Superpower;
using Superpower.Model;
using Superpower.Parsers;
using Superpower.Tokenizers;

namespace QueryX.Parsing
{
    internal static class QueryTokenizer
    {
        private readonly static string[] _operators = new[]
        {
            "==",
            "!=",
            ">=",
            "<=",
            "|=",
            "-=-",
            "=-",
            "-=",
            ">",
            "<",
        };

        private static TextParser<Unit> Operator { get; } =
            from _ in Span.EqualTo(_operators[0]).Try()
                .Or(Span.EqualTo(_operators[1])).Try()
                .Or(Span.EqualTo(_operators[2])).Try()
                .Or(Span.EqualTo(_operators[3])).Try()
                .Or(Span.EqualTo(_operators[4])).Try()
                .Or(Span.EqualTo(_operators[5])).Try()
                .Or(Span.EqualTo(_operators[6])).Try()
                .Or(Span.EqualTo(_operators[7])).Try()
                .Or(Span.EqualTo(_operators[8])).Try()
                .Or(Span.EqualTo(_operators[9])).Try()
            select Unit.Value;

        private static TextParser<Unit> CustomIdentifier { get; } =
            from _ in Identifier.CStyle.ManyDelimitedBy(Character.EqualTo('.'))
            select Unit.Value;

        internal static Tokenizer<QueryToken> Instance { get; } =
            new TokenizerBuilder<QueryToken>()
                .Ignore(Span.WhiteSpace)
                .Match(Operator, QueryToken.Operator)
                .Match(Character.EqualTo('*'), QueryToken.Asterisk)
                .Match(Character.EqualTo('('), QueryToken.LParen)
                .Match(Character.EqualTo(')'), QueryToken.RParen)
                .Match(Character.In('&', ';'), QueryToken.And)
                .Match(Character.EqualTo('|'), QueryToken.Or)
                .Match(Character.EqualTo(','), QueryToken.Comma)
                .Match(Character.EqualTo('!'), QueryToken.Exclamation)
                .Match(QuotedString.SqlStyle, QueryToken.String)
                .Match(Numerics.Decimal, QueryToken.Number)
                .Match(CustomIdentifier, QueryToken.Identifier, requireDelimiters: true)
                .Build();
    }
}
