using SetTheory.AST;
using SetTheory.Structs;
using Superpower;
using Superpower.Parsers;
using System;

namespace SetTheory
{
    using ExpressionParser = TokenListParser<TokenType, Expression>;

    public static class Grammar
    {
        static ExpressionParser Set { get; } =
            Token.EqualTo(TokenType.Set)
            .Select(x => Expression.Create<Set>(x.ToStringValue()));

        static ExpressionParser UniverseSet { get; } =
            Token.EqualTo(TokenType.UniverseSet)
            .Select(_ => Expression.Create<Set>("U"));

        static ExpressionParser EmptySet { get; } =
            Token.EqualTo(TokenType.EmptySet)
            .Select(_ => Expression.Create<Set>("∅"));

        static ExpressionParser Variable { get; } =
            Token.EqualTo(TokenType.Variable)
            .Select(x => Expression.Create<Variable>(x.ToStringValue()));

        static ExpressionParser ExpressionInParens { get; } =
            from lparen in Token.EqualTo(TokenType.LParen)
            from expr in Parse.Ref(() => Expr)
            from rparen in Token.EqualTo(TokenType.RParen)
            select expr;

        static ExpressionParser Factor { get; } =
            Set.Try()
            .Or(UniverseSet).Try()
            .Or(EmptySet).Try()
            .Or(Variable).Try()
            .Or(ExpressionInParens);

        static ExpressionParser PrefixNegation { get; } =
            from token in Token.EqualTo(TokenType.PrefixNegation)
            from factor in Factor
            select Expression.Create<UnaryOperation>("'", factor);

        static ExpressionParser PostfixNegation { get; } =
            from factor in Factor
            from token in Token.EqualTo(TokenType.PostfixNegation)
            select Expression.Create<UnaryOperation>("'", factor);

        static TokenListParser<TokenType, string> Union { get; } = Token.EqualTo(TokenType.Union).Select(_ => "∪");
        static TokenListParser<TokenType, string> Intersection { get; } = Token.EqualTo(TokenType.Intersection).Select(_ => "∩");
        static TokenListParser<TokenType, string> Difference { get; } = Token.EqualTo(TokenType.Difference).Select(_ => @"\");
        static TokenListParser<TokenType, string> SymmetricDifference { get; } =
            Token.EqualTo(TokenType.SymmetricDifference).Select(_ => "△");

        static ExpressionParser Term { get; } = PostfixNegation.Try().Or(PrefixNegation).Try().Or(Factor);

        static Func<string, Expression, Expression, Expression> CreateExpr { get; } =
            (value, child1, child2) => Expression.Create<BinaryOperation>(value, child1, child2);

        public static ExpressionParser Expr { get; } =
            Parse.Chain(Union.Try().Or(Intersection).Try().Or(Difference).Try().Or(SymmetricDifference), Term, CreateExpr);

        public static ExpressionParser Tree { get; } = Expr.AtEnd().Select(x => Expression.Create<Tree>("Tree", x));
    }
}