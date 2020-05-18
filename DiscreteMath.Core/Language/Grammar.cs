using DiscreteMath.Core.Language.AST;
using DiscreteMath.Core.Structs;
using Superpower;
using Superpower.Parsers;
using System;

namespace DiscreteMath.Core.Language
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
        ExpressionParser Difference { get; }
        ExpressionParser Intersection { get; }
        ExpressionParser Union { get; }
        ExpressionParser SymmetricDifference { get; }

        public Grammar(ISettings settings)
        {
            var universeSign = settings.UniverseSign;
            var emptySetSign = settings.EmptySetSign;
            var isPrefixNegation = settings.IsPrefixNegation;
            var prefixNegationSign = settings.PrefixNegation;
            var postfixNegationSign = settings.PostfixNegation;
            var negationSign = isPrefixNegation ? prefixNegationSign : postfixNegationSign;
            var unionSign = settings.Union;
            var intersectionSign = settings.Intersection;
            var differenceSign = settings.Difference;
            var symmetricDifferenceSign = settings.SymmetricDifference;

            Set = Token.EqualTo(TokenType.Set).Select(x => (Expression)new Set(x.ToStringValue()));
            UniverseSet = Token.EqualTo(TokenType.UniverseSet).Select(_ => (Expression)new Set(universeSign));
            EmptySet = Token.EqualTo(TokenType.EmptySet).Select(_ => (Expression)new Set(emptySetSign));
            Variable = Token.EqualTo(TokenType.Variable).Select(x => (Expression)new Variable(x.ToStringValue()));
            ExpressionInParens =
                from lparen in Token.EqualTo(TokenType.LParen)
                from expr in Parse.Ref(() => Union)
                from rparen in Token.EqualTo(TokenType.RParen)
                select (Expression)new Parens(expr);
            Factor = Set.Try()
               .Or(UniverseSet).Try()
               .Or(EmptySet).Try()
               .Or(Variable).Try()
               .Or(ExpressionInParens).Try();
            PrefixNegation =
                from tokens in Token.EqualTo(TokenType.PrefixNegation).AtLeastOnce()
                from expr in Parse.Ref(() => Union)
                select CreateNegationOperation(negationSign, isPrefixNegation, expr, tokens.Length);
            PostfixNegation =
                from factor in Factor
                from tokens in Token.EqualTo(TokenType.PostfixNegation).AtLeastOnce()
                select CreateNegationOperation(negationSign, isPrefixNegation, factor, tokens.Length);
            Term = PostfixNegation.Try().Or(PrefixNegation).Try().Or(Factor);
            Difference = Parse.Chain(
                Token.EqualTo(TokenType.Difference).Select(_ => differenceSign),
                Term,
                (value, child1, child2) => new Difference(value, child1, child2));
            Intersection = Parse.Chain(
                Token.EqualTo(TokenType.Intersection).Select(_ => intersectionSign),
                Difference,
                (value, child1, child2) => new Intersection(value, child1, child2));
            Union = Parse.Chain(
                Token.EqualTo(TokenType.Union).Select(_ => unionSign),
                Intersection,
                (value, child1, child2) => new Union(value, child1, child2));
            SymmetricDifference = Parse.Chain(
                Token.EqualTo(TokenType.SymmetricDifference).Select(_ => symmetricDifferenceSign),
                Union,
                (value, child1, child2) => new SymmetricDifference(value, child1, child2));
        }

        Expression CreateNegationOperation(string value, bool isPrefix, Expression expression, int count)
        {
            if (count < 1) throw new ArgumentOutOfRangeException(nameof(count));

            var negation = new NegationOperation(value, expression, isPrefix);

            if (count == 1)
                return negation;

            return CreateNegationOperation(value, isPrefix, negation, --count);
        }

        public ExpressionParser BuildTree => SymmetricDifference.AtEnd().Select(x => (Expression)new Tree("Tree", x));
    }
}