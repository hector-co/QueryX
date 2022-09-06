using QueryX.Parser.Nodes;
using Superpower;
using Superpower.Parsers;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace QueryX.Parser
{
    static class QueryParser
    {
        static TokenListParser<QueryToken, string> String { get; } =
            Token.EqualTo(QueryToken.String)
                .Apply(QuotedString.SqlStyle)
                .Select(s => s);

        static TokenListParser<QueryToken, string> Number { get; } =
            Token.EqualTo(QueryToken.Number)
            .Apply(Numerics.Decimal)
            .Select(s => s.ToStringValue());

        static TokenListParser<QueryToken, string> True { get; } =
            Token.EqualToValueIgnoreCase(QueryToken.Identifier, "true")
            .Value("true");

        static TokenListParser<QueryToken, string> False { get; } =
            Token.EqualToValueIgnoreCase(QueryToken.Identifier, "false")
            .Value("false");
        static TokenListParser<QueryToken, string?> Null { get; } =
            Token.EqualToValueIgnoreCase(QueryToken.Identifier, "null")
            .Value((string?)null);

        static TokenListParser<QueryToken, string?> Value { get; } =
            String.AsNullable()
            .Or(Number.AsNullable())
            .Or(True.AsNullable())
            .Or(False.AsNullable())
            .Or(Null);

        static TokenListParser<QueryToken, string?[]> ValueArray { get; } =
            from values in Parse.Ref(() => Value!)
                .ManyDelimitedBy(Token.EqualTo(QueryToken.Comma))
            select values;

        static TokenListParser<QueryToken, string> Operator { get; } =
            Token.EqualTo(QueryToken.Operator)
            .Select(s => s.ToStringValue());

        static TokenListParser<QueryToken, string> Property { get; } =
            Token.EqualTo(QueryToken.Identifier)
            .Select(s => s.ToStringValue());

        static TokenListParser<QueryToken, NodeBase> Filter { get; } =
            from property in Property
            from op in Operator
            from values in ValueArray
            select (NodeBase)new OperatorNode(property, op, values);

        static TokenListParser<QueryToken, NodeBase> OrElse { get; } =
            from filter1 in Parse.Ref(() => Filter!).Or(Parse.Ref(() => Group!))
            from op in Token.EqualTo(QueryToken.Or)
            from filter2 in Parse.Ref(() => Exp!)
            select (NodeBase)new OrElseNode(filter1, filter2);

        static TokenListParser<QueryToken, NodeBase> AndAlso { get; } =
            from filter1 in Parse.Ref(() => Filter!).Or(Parse.Ref(() => Group!))
            from op in Token.EqualTo(QueryToken.And)
            from filter2 in Parse.Ref(() => Exp!)
            select (NodeBase)new AndAlsoNode(filter1, filter2);

        static TokenListParser<QueryToken, NodeBase> Group { get; } =
            from lParen in Token.EqualTo(QueryToken.LParen)
            from filter in Parse.Ref(() => Exp!)
            from rParen in Token.EqualTo(QueryToken.RParen)
            select filter;

        static TokenListParser<QueryToken, NodeBase> Exp { get; } =
            OrElse.Try()
            .Or(AndAlso).Try()
            .Or(Group).Try()
            .Or(Filter).Try()
            .Select(s => s);

        static TokenListParser<QueryToken, NodeBase> Instance { get; } = Exp.AtEnd();

        internal static bool TryParse(string source, out NodeBase? root)
        {
            root = null;
            try
            {
                var tokens = QueryTokenizer.Instance.TryTokenize(source);
                var result = Instance.TryParse(tokens.Value);

                if (!result.HasValue)
                    return false;

                root = result.Value;
                return true;
            }
            catch
            {
                return false;
            }
        }

        const string CommaSeparatedValuesSplit = @",(?=(?:[^']*'[^']*')*[^']*$)";

        internal static IEnumerable<(string PropName, bool Ascending)> GetOrderingTokens(string orderByString)
        {
            var result = new List<(string PropName, bool Ascending)>();

            if (string.IsNullOrEmpty(orderByString))
                return result;

            var orderings = SplitCommaSeparatedValues(orderByString)
                .Where(s => !string.IsNullOrEmpty(s))
                .Select(s => s!.Trim());

            foreach (var order in orderings)
            {
                if (order.StartsWith('-'))
                    result.Add((order[1..], false));
                else
                    result.Add((order, true));
            }

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
