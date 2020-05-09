using Superpower;
using Superpower.Parsers;
using System;

namespace SetTheory
{
    using ExpressionParser = TokenListParser<TokenType, Expression>;

    public class Grammar
    {
        ExpressionParser Set { get; }
        ExpressionParser UniverseSet { get; }
        ExpressionParser EmptySet { get; }
        ExpressionParser Variable { get; }
        ExpressionParser ExpressionInParens { get; }
        ExpressionParser Factor { get; }
        ExpressionParser PrefixNegation { get; }
        ExpressionParser PostfixNegation { get; }
        ExpressionParser Term { get; }
        TokenListParser<TokenType, string> Union { get; }
        TokenListParser<TokenType, string> Intersection { get; }
        TokenListParser<TokenType, string> Difference { get; }
        TokenListParser<TokenType, string> SymmetricDifference { get; }
        ExpressionParser Expr { get; }

        public Grammar(ISettings settings)
        {
            var defaultSettings = new DefaultSettings();

            var universeSign = settings.UniverseSign ?? defaultSettings.UniverseSign;
            var emptySetSign = settings.EmptySetSign ?? defaultSettings.EmptySetSign;
            var postfixNegationSign = settings.PostfixNegation ?? defaultSettings.PostfixNegation;
            var unionSign = settings.Union ?? defaultSettings.Union;
            var intersectionSign = settings.Intersection ?? defaultSettings.Intersection;
            var differenceSign = settings.Difference ?? defaultSettings.Difference;
            var symmetricDifferenceSign = settings.SymmetricDifference ?? defaultSettings.SymmetricDifference;

            Set = Token.EqualTo(TokenType.Set).Select(x => Expression.Create<Set>(x.ToStringValue()));
            UniverseSet = Token.EqualTo(TokenType.UniverseSet).Select(_ => Expression.Create<Set>(universeSign));
            EmptySet = Token.EqualTo(TokenType.EmptySet).Select(_ => Expression.Create<Set>(emptySetSign));
            Variable = Token.EqualTo(TokenType.Variable).Select(x => Expression.Create<Variable>(x.ToStringValue()));
            ExpressionInParens =
                from lparen in Token.EqualTo(TokenType.LParen)
                from expr in Parse.Ref(() => Expr)
                from rparen in Token.EqualTo(TokenType.RParen)
                select expr;
            Factor = Set.Try()
               .Or(UniverseSet).Try()
               .Or(EmptySet).Try()
               .Or(Variable).Try()
               .Or(ExpressionInParens);
            PrefixNegation =
                from token in Token.EqualTo(TokenType.PrefixNegation)
                from factor in Factor
                select Expression.Create<UnaryOperation>(postfixNegationSign, factor);
            PostfixNegation =
                from factor in Factor
                from token in Token.EqualTo(TokenType.PostfixNegation)
                select Expression.Create<UnaryOperation>(postfixNegationSign, factor);
            Term = PostfixNegation.Try().Or(PrefixNegation).Try().Or(Factor);
            Union = Token.EqualTo(TokenType.Union).Select(_ => unionSign);
            Intersection = Token.EqualTo(TokenType.Intersection).Select(_ => intersectionSign);
            Difference = Token.EqualTo(TokenType.Difference).Select(_ => differenceSign);
            SymmetricDifference = Token.EqualTo(TokenType.SymmetricDifference).Select(_ => symmetricDifferenceSign);
            Expr = Parse.Chain(
                Union.Try().Or(Intersection).Try().Or(Difference).Try().Or(SymmetricDifference),
                Term,
                CreateBinaryOperation());
        }

        Func<string, Expression, Expression, Expression> CreateBinaryOperation()
            => (value, child1, child2) => Expression.Create<BinaryOperation>(value, child1, child2);

        public ExpressionParser BuildTree => Expr.AtEnd().Select(x => Expression.Create<Tree>("Tree", x));
    }
}