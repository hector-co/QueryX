using Superpower;
using Superpower.Parsers;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using QueryX.Parsing.Nodes;

namespace QueryX.Parsing
{
    internal static class QueryParser
    {
        private static TokenListParser<QueryToken, string> String { get; } =
            Token.EqualTo(QueryToken.String)
                .Apply(QuotedString.SqlStyle)
                .Select(s => s);

        private static TokenListParser<QueryToken, string> Number { get; } =
            Token.EqualTo(QueryToken.Number)
                .Apply(Numerics.Decimal)
                .Select(s => s.ToStringValue());

        private static TokenListParser<QueryToken, string> True { get; } =
            Token.EqualToValueIgnoreCase(QueryToken.Identifier, "true")
                .Value("true");

        private static TokenListParser<QueryToken, string> False { get; } =
            Token.EqualToValueIgnoreCase(QueryToken.Identifier, "false")
                .Value("false");

        private static TokenListParser<QueryToken, string?> Null { get; } =
            Token.EqualToValueIgnoreCase(QueryToken.Identifier, "null")
                .Value((string?)null);

        private static TokenListParser<QueryToken, QueryToken> And { get; } =
            Token.EqualTo(QueryToken.And).Value(QueryToken.And);

        private static TokenListParser<QueryToken, QueryToken> Or { get; } =
            Token.EqualTo(QueryToken.Or).Value(QueryToken.Or);

        private static TokenListParser<QueryToken, string?> Value { get; } =
            String.AsNullable()
                .Or(Number.AsNullable())
                .Or(True.AsNullable())
                .Or(False.AsNullable())
                .Or(Null);

        private static TokenListParser<QueryToken, string?[]> ValueArray { get; } =
            from values in Value
                .ManyDelimitedBy(Token.EqualTo(QueryToken.Comma))
            select values;

        private static TokenListParser<QueryToken, string> Operator { get; } =
            Token.EqualTo(QueryToken.Operator)
                .Select(s => s.ToStringValue());

        private static TokenListParser<QueryToken, string> Property { get; } =
            Token.EqualTo(QueryToken.Identifier)
                .Select(s => s.ToStringValue());

        private static TokenListParser<QueryToken, NodeBase> Filter { get; } =
            from negation in Token.EqualTo(QueryToken.Exclamation).Value(true).OptionalOrDefault()
            from property in Property
            from op in Operator
            from values in ValueArray
            select (NodeBase)new FilterNode(property, op, values, isNegated: negation);

        private static TokenListParser<QueryToken, NodeBase> CollectionFilter { get; } =
            from negation in Token.EqualTo(QueryToken.Exclamation).Value(true).OptionalOrDefault()
            from property in Property
            from allFlag in Token.EqualTo(QueryToken.Asterisk).Value(true).OptionalOrDefault()
            from objectFilter in Group!
            select (NodeBase)new ObjectFilterNode(property, objectFilter, applyAll: allFlag, isNegated: negation);

        private static TokenListParser<QueryToken, NodeBase> Group { get; } =
            from negation in Token.EqualTo(QueryToken.Exclamation).Value(true).OptionalOrDefault()
            from lParen in Token.EqualTo(QueryToken.LParen)
            from filter in Disjunction!
            from rParen in Token.EqualTo(QueryToken.RParen)
            select negation ? filter.Negated() : filter;

        private static TokenListParser<QueryToken, NodeBase> FilterExp { get; } =
            CollectionFilter.Try()
                .Or(Filter).Try()
                .Or(Group).Try()
                .Select(s => s);

        private static TokenListParser<QueryToken, NodeBase> Conjunction { get; } =
            Parse.Chain(And, FilterExp, (_, l, r) => new AndAlsoNode(l, r));

        private static TokenListParser<QueryToken, NodeBase> Disjunction { get; } =
            Parse.Chain(Or, Conjunction, (_, l, r) => new OrElseNode(l, r));

        private static TokenListParser<QueryToken, NodeBase> Instance { get; } = Disjunction.AtEnd();

        internal static NodeBase ParseNodes(string source)
        {
            var tokens = QueryTokenizer.Instance.TryTokenize(source);
            var result = Instance.Parse(tokens.Value);

            return result;
        }

        private const string CommaSeparatedValuesSplit = @",(?=(?:[^']*'[^']*')*[^']*$)";

        internal static IEnumerable<(string PropName, bool Ascending)> GetOrderingTokens(string orderByString)
        {
            var result = new List<(string PropName, bool Ascending)>();

            if (string.IsNullOrEmpty(orderByString))
                return result;

            var orderings = SplitCommaSeparatedValues(orderByString)
                .Where(s => !string.IsNullOrEmpty(s))
                .Select(s => s!.Trim());

            result.AddRange(
                orderings.Select(
                    order => order.StartsWith('-') ? (order[1..], false) : (order, true)));

            return result;
        }

        private static IEnumerable<string?> SplitCommaSeparatedValues(string values)
        {
            return Regex.Split(values, CommaSeparatedValuesSplit)
                .Select(v => v.Trim())
                .Select(v => v == "null" ? null : v.TrimStart('\'').TrimEnd('\'').Replace("''", "'"));
        }
    }
}
