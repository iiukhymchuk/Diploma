using Microsoft.VisualStudio.TestTools.UnitTesting;
using SetTheory;
using System.Linq;

namespace Tests.SetTheory
{
    [TestClass]
    public class InterpreterFixture
    {
        readonly Interpreter interpeter;
        readonly IProvideTokenizer syntax;
        readonly Grammar grammar;
        readonly ISettings settings;

        public InterpreterFixture()
        {
            settings = new DefaultSettings();
            var rules = new Rules();
            syntax = new Syntax(settings);
            grammar = new Grammar(settings);
            interpeter = new Interpreter(new PatternMatcher(rules.GetRules()), new Normalizer(settings));
        }

        [DataTestMethod]
        [DataRow("A * A", "A ")]
        [DataRow("B + B", "B ")]
        [DataRow("C + U", "U ")]
        [DataRow("U + C", "U ")]
        [DataRow("D * O", "∅ ")]
        [DataRow("O + D", "D ")]
        [DataRow("A + O", "A ")]
        [DataRow("O + A", "A ")]
        [DataRow("A * U", "A ")]
        [DataRow("U * A", "A ")]
        [DataRow("A + A'", "U ")]
        [DataRow("A' + A", "U ")]
        [DataRow("A * A'", "∅ ")]
        [DataRow("A' * A", "∅ ")]
        [DataRow("O'", "U ")]
        [DataRow("A + (A * B)", "A ")]
        [DataRow("A + A * B", "A ")]
        [DataRow("A + (B * A)", "A ")]
        [DataRow("A + (B * A)", "A ")]
        [DataRow("A * (A + B)", "A ")]
        [DataRow("A * (B + A)", "A ")]
        [DataRow("(A * B) + A", "A ")]
        [DataRow("(B * A) + A", "A ")]
        [DataRow("(A + B) * A", "A ")]
        [DataRow("(B + A) * A", "A ")]
        [DataRow("A''", "A ")]
        [DataRow("(A + B)'", "A' ∩ B' ")]
        [DataRow("(A * B)'", "A' ∪ B' ")]
        public void InterpreterReturnsExpectedResultsAllSimpleRules(string input, string expected)
        {
            var tokenizer = syntax.GetTokenizer();
            var tokensResult = tokenizer.TryTokenize(input);
            var parseResult = grammar.BuildTree(tokensResult.Value);
            var resultLines = interpeter.Interpretate((Tree)parseResult.Value);

            var actual = resultLines.Last().SimplifiedExpression;
            Assert.AreEqual(expected, actual);
        }

        [DataTestMethod]
        [DataRow("(A' + C)' + (B + B * C) * (B' + (B + C)')", "A ∩ C' ")]
        [DataRow("(A' + C)' + (B + B * C) * (B' + (B + C)') + (A' + C)'", "A ∩ C' ")]
        [DataRow("(A ∪ (B ∩ C) ∩ C')", "A ")]
        [DataRow("(A * B) + (A * B') + (A' * B)", "A ∪ B ")]
        [DataRow("(A * B')' + B", "A' ∪ B ")]
        [DataRow("(A' * (B + C)')'", "A ∪ B ∪ C ")]
        public void InterpreterReturnsExpectedResults(string input, string expected)
        {
            var tokenizer = syntax.GetTokenizer();
            var tokensResult = tokenizer.TryTokenize(input);
            var parseResult = grammar.BuildTree(tokensResult.Value);
            var resultLines = interpeter.Interpretate(parseResult.Value);

            var actual = resultLines.Last().SimplifiedExpression;
            Assert.AreEqual(expected, actual);
        }
    }
}