using QueryX.Parser.Nodes;
using Superpower;
using Superpower.Parsers;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace QueryX.Parser
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

        private static TokenListParser<QueryToken, string?> Value { get; } =
            String.AsNullable()
            .Or(Number.AsNullable())
            .Or(True.AsNullable())
            .Or(False.AsNullable())
            .Or(Null);

        private static TokenListParser<QueryToken, string?[]> ValueArray { get; } =
            from values in Parse.Ref(() => Value)
                .ManyDelimitedBy(Token.EqualTo(QueryToken.Comma))
            select values;

        private static TokenListParser<QueryToken, string> Operator { get; } =
            Token.EqualTo(QueryToken.Operator)
            .Select(s => s.ToStringValue());

        private static TokenListParser<QueryToken, string> Property { get; } =
            Token.EqualTo(QueryToken.Identifier)
            .Select(s => s.ToStringValue());

        private static TokenListParser<QueryToken, NodeBase> Filter { get; } =
            from property in Property
            from op in Operator
            from values in ValueArray
            select (NodeBase)new OperatorNode(property, op, values);

        private static TokenListParser<QueryToken, NodeBase> AnyObjectFilter { get; } =
            from property in Property
            from objectFilter in Parse.Ref(() => Group!)
            select (NodeBase)new ObjectFilterNode(property, objectFilter);

        private static TokenListParser<QueryToken, NodeBase> AllObjectFilter { get; } =
            from property in Property
            from flag in Token.EqualTo(QueryToken.Asterisk)
            from objectFilter in Parse.Ref(() => Group!)
            select (NodeBase)new ObjectFilterNode(property, objectFilter, true);

        private static TokenListParser<QueryToken, NodeBase> Group { get; } =
            from lParen in Token.EqualTo(QueryToken.LParen)
            from filter in Parse.Ref(() => Exp!)
            from rParen in Token.EqualTo(QueryToken.RParen)
            select filter;

        private static TokenListParser<QueryToken, NodeBase> OrElse { get; } =
            from filter1 in Parse.Ref(() => AnyObjectFilter).Try()
                .Or(Parse.Ref(() => AllObjectFilter)).Try()
                .Or(Parse.Ref(() => Filter)).Try()
                .Or(Parse.Ref(() => Group)).Try()
            from op in Token.EqualTo(QueryToken.Or)
            from filter2 in Parse.Ref(() => Exp!)
            select (NodeBase)new OrElseNode(filter1, filter2);

        private static TokenListParser<QueryToken, NodeBase> AndAlso { get; } =
            from filter1 in Parse.Ref(() => AnyObjectFilter).Try()
                .Or(Parse.Ref(() => AllObjectFilter)).Try()
                .Or(Parse.Ref(() => Filter)).Try()
                .Or(Parse.Ref(() => Group)).Try()
            from op in Token.EqualTo(QueryToken.And)
            from filter2 in Parse.Ref(() => Exp!)
            select (NodeBase)new AndAlsoNode(filter1, filter2);

        private static TokenListParser<QueryToken, NodeBase> Exp { get; } =
            OrElse.Try()
            .Or(AndAlso).Try()
            .Or(AnyObjectFilter).Try()
            .Or(AllObjectFilter).Try()
            .Or(Filter).Try()
            .Or(Group).Try()
            .Select(s => s);

        private static TokenListParser<QueryToken, NodeBase> Instance { get; } = Exp.AtEnd();


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
