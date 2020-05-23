using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using DiscreteMath.Core.Language;
using DiscreteMath.Core.Pipeline;
using DiscreteMath.Core.Utils;

namespace Tests.Core
{
    [TestClass]
    public class SimplifierFixture
    {
        readonly Simplifier simplifier;
        readonly IProvideTokenizer syntax;
        readonly Grammar grammar;
        readonly ISettings settings;

        public SimplifierFixture()
        {
            settings = new DefaultSettings();
            var rules = new Rules();
            syntax = new Syntax(settings);
            grammar = new Grammar(settings);
            simplifier = new Simplifier(new PatternMatcher(rules.GetRules(), new RuleApplier()), new Printer());
        }

        [DataTestMethod]
        [DataRow("A △ O", "A")]
        [DataRow("A △ A", "∅")]
        [DataRow("A △ B", "(A ⋂ B') ⋃ (A' ⋂ B)")]
        [DataRow("A - O", "A")]
        [DataRow("A - A", "∅")]
        [DataRow("A - B", "A ∩ B'")]
        [DataRow("A * A", "A")]
        [DataRow("B + B", "B")]
        [DataRow("C + U", "U")]
        [DataRow("U + C", "U")]
        [DataRow("D * O", "∅")]
        [DataRow("O + D", "D")]
        [DataRow("A + O", "A")]
        [DataRow("O + A", "A")]
        [DataRow("A * U", "A")]
        [DataRow("U * A", "A")]
        [DataRow("A + A'", "U")]
        [DataRow("A' + A", "U")]
        [DataRow("A * A'", "∅")]
        [DataRow("A' * A", "∅")]
        [DataRow("O'", "U")]
        [DataRow("A + (A * B)", "A")]
        [DataRow("A + A * B", "A")]
        [DataRow("A + (B * A)", "A")]
        [DataRow("A + (B * A)", "A")]
        [DataRow("A * (A + B)", "A")]
        [DataRow("A * (B + A)", "A")]
        [DataRow("(A * B) + A", "A")]
        [DataRow("(B * A) + A", "A")]
        [DataRow("(A + B) * A", "A")]
        [DataRow("(B + A) * A", "A")]
        [DataRow("A''", "A")]
        [DataRow("(A + B)'", "A' ∩ B'")]
        [DataRow("(A * B)'", "A' ∪ B'")]
        public void InterpreterReturnsExpectedResultsAllSimpleRules(string input, string expected)
        {
            var tokenizer = syntax.GetTokenizer();
            var tokensResult = tokenizer.TryTokenize(input);
            var parseResult = grammar.BuildTree(tokensResult.Value);
            var resultLines = simplifier.Run(parseResult.Value);

            var actual = resultLines.Last().SimplifiedExpression;

            var actualTokens = tokenizer.TryTokenize(actual).Value;
            var actualTree = grammar.BuildTree(actualTokens).Value;

            var expectedTokens = tokenizer.TryTokenize(expected).Value;
            var expectedTree = grammar.BuildTree(expectedTokens).Value;

            var equal = expectedTree.StrictEquals(actualTree);

            Assert.IsTrue(equal, "Not equal");
        }

        [DataTestMethod]
        [DataRow("(A' + C)' + (B + B * C) * (B' + (B + C)')", "A ∩ C'")]
        [DataRow("(A * B * C * D) + (D * C * B * A)", "A * B * C * D")]
        [DataRow("(A ∪ (B ∩ C) ∩ C')", "A")]
        [DataRow("(A' * (B + C)')'", "A ∪ B ∪ C")]
        [DataRow(@"(A ⋃ B) \ (A \ (A ⋂ B))", "B")]
        [DataRow("(A * B')' + B", "A' ∪ B")]
        [DataRow("(B * C) + (B + B * C)", "B")]
        [DataRow("(A' ∪ C)' ∪ (B ∪ B ∩ C) ∩ (B' ∪ (B ∪ C)') ∪ (A' ∪ C)' ∪ A ∪ C ∩ (B ∪ C)", "A ∪ C")]
        [DataRow("(A' + C)' + (B + B * C) * (B' + (B + C)') + (A' + C)'", "A ∩ C'")]
        [DataRow("(A' + C)' + (A' + C)'", "A ∩ C'")]
        [DataRow("(A * B) + (A * B') + (A' * B)", "A ∪ B")]
        [DataRow("(A + B + C) * (A + B' + C) * (A + C)'", "∅")]
        [DataRow("A * (B - A)", "∅")]
        [DataRow("A + (B - A)", "A + B")]
        [DataRow("B - ((B - A) + B)", "∅")]
        [DataRow("(A - (A - (A * B)))", "A * B")]
        [DataRow("(A + B) - (A - B)", "B")]
        [DataRow("(A * B) - (B - A)", "A * B")]
        [DataRow("(A * B) △ (B - A)", "B")]
        [DataRow("(A + B) △ (A - B)", "B")]
        [DataRow("A △ (B - A)", "A + B")]
        [DataRow("A △ (A - B)", "A * B")]
        [DataRow("B - (A △ B)", "A * B")]
        public void InterpreterReturnsExpectedResults(string input, string expected)
        {
            var tokenizer = syntax.GetTokenizer();
            var tokensResult = tokenizer.TryTokenize(input);
            var parseResult = grammar.BuildTree(tokensResult.Value);
            var resultLines = simplifier.Run(parseResult.Value);

            var actual = resultLines.Last().SimplifiedExpression;

            var actualTokens = tokenizer.TryTokenize(actual).Value;
            var actualTree = grammar.BuildTree(actualTokens).Value.Normalize();

            var expectedTokens = tokenizer.TryTokenize(expected).Value;
            var expectedTree = grammar.BuildTree(expectedTokens).Value.Normalize();

            var equal = expectedTree.StrictEquals(actualTree);

            Assert.IsTrue(equal, "Not equal");
            Assert.IsTrue(resultLines.Count() < 40, "Too long solution");
        }
    }
}