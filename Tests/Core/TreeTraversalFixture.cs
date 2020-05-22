using DiscreteMath.Core.Language;
using DiscreteMath.Core.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Core
{
    [TestClass]
    public class TreeTraversalFixture
    {
        readonly IProvideTokenizer syntax;
        readonly Grammar grammar;
        readonly ISettings settings;

        public TreeTraversalFixture()
        {
            settings = new DefaultSettings();
            syntax = new Syntax(settings);
            grammar = new Grammar(settings);
        }

        [DataTestMethod]
        [DataRow("A", "A'")]
        [DataRow("A + B", "(A' + B')'")]
        [DataRow("(A * B) + (C * D)", "((A' * B')' + (C' * D')')'")]
        public void ChangeTreeShoudComplementAllTreeMembers(string input, string output)
        {
            var tokenizer = syntax.GetTokenizer();
            var tokensResult = tokenizer.TryTokenize(input);
            var parseResult = grammar.BuildTree(tokensResult.Value);

            var expr = parseResult.Value;
            var actual = expr.AsTree().ChangeTree(x => true, x => new Negation("'", x)).AsExpression();

            var expectedTokens = tokenizer.TryTokenize(output).Value;
            var expected = grammar.BuildTree(expectedTokens).Value;

            var equal = TreeUtils.StrictEquals(expected, actual);

            Assert.IsTrue(equal, "The tree is not complemented");
        }

        [DataTestMethod]
        [DataRow("A'", "A")]
        [DataRow("(A' + B')'", "A + B")]
        [DataRow("((A' * B')' + (C' * D')')'", "(A * B) + (C * D)")]
        public void ChangeTreeShoudRemoveAllComplements(string input, string output)
        {
            var tokenizer = syntax.GetTokenizer();
            var tokensResult = tokenizer.TryTokenize(input);
            var parseResult = grammar.BuildTree(tokensResult.Value);

            var expr = parseResult.Value;
            var actual = expr.AsTree().ChangeTree(x => x.Type == typeof(Negation), x => x.Children[0]).AsExpression();

            var expectedTokens = tokenizer.TryTokenize(output).Value;
            var expected = grammar.BuildTree(expectedTokens).Value;

            var equal = TreeUtils.StrictEquals(expected, actual);

            Assert.IsTrue(equal, "The tree is not complemented");
        }
    }
}