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
            interpeter = new Interpreter(new PatternMatcher(rules.GetRules()), new Normalizer(rules.GetNormalizationRules()));
        }

        [DataTestMethod]
        [DataRow("(A' + C)' + (B + B* C) * (B' + (B + C)')", "A ∩ C'")]
        public void InterpreterReturnsExpectedResults(string input, string expected)
        {
            var tokenizer = syntax.GetTokenizer();
            var tokensResult = tokenizer.TryTokenize(input);
            var parseResult = grammar.BuildTree(tokensResult.Value);
            var resultLines = interpeter.Interpretate((Tree) parseResult.Value);

            var actual = resultLines.Last().SimplifiedExpression;
            Assert.AreEqual(expected, actual);
        }
    }
}