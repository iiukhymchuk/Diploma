using Microsoft.VisualStudio.TestTools.UnitTesting;
using Superpower.Model;
using System.Collections.Generic;
using System.Linq;
using DiscreteMath.Core.Language;
using DiscreteMath.Core.Structs;

namespace Tests.SetTheory
{
    [TestClass]
    public class SyntaxWithVariablesFixture
    {
        readonly IProvideTokenizer syntax;

        public SyntaxWithVariablesFixture()
        {
            var settings = new DefaultSettings();
            syntax = new SyntaxWithVariables(settings);
        }

        [DataTestMethod]
        [DataRow(" +∪∨| ")]
        [DataRow(" *∩∧& ")]
        [DataRow(" ABCDE ")]
        [DataRow(" U1 ")]
        [DataRow(" O0∅ ")]
        [DataRow(@" \- ")]
        [DataRow(" ) ")]
        [DataRow(" ( ")]
        [DataRow(" ! ")]
        [DataRow(" ' ")]
        public void TokenizerReturnsResultOnValidInputPerLexema(string input)
        {
            var result = syntax.GetTokenizer().TryTokenize(input);
            Assert.IsTrue(result.HasValue);
        }

        [DataTestMethod]
        [DataRow(@"+*AUO\()!'_A")]
        [DataRow("∪∩B10-()!'_B")]
        [DataRow(@"∨∧CU0\()!'_C")]
        [DataRow("|&D1O-()!'_D")]
        [DataRow(@"+*EUO\()!'_E")]
        [DataRow(@" + * A U O \ ( ) ! ' _A ")]
        [DataRow("  ∪  ∩  B  1  0  -  (  )  !  '  _B  ")]
        [DataRow(@" ∨ ∧ C U 0 \ ( ) ! ' _C ")]
        [DataRow(" | & D 1 O - ( ) ! ' _D ")]
        [DataRow(@"  +  *  E  U  O  \  (  )  !  '  _E  ")]
        public void TokenizerReturnsResultOnValidInput(string input)
        {
            var result = syntax.GetTokenizer().TryTokenize(input);
            Assert.IsTrue(result.HasValue);
        }

        [DataTestMethod]
        [DataRow("_")]
        [DataRow("_1")]
        [DataRow("2")]
        [DataRow("%")]
        public void TokenizerReturnsErrorOnInvalidInput(string input)
        {
            var result = syntax.GetTokenizer().TryTokenize(input);
            Assert.IsFalse(result.HasValue);
            Assert.AreEqual(Position.Zero, result.ErrorPosition);
        }

        [TestMethod]
        public void TokenizerReturnsExpectedTokenList()
        {
            var input = @"A ∪ (!B ∩ A') \ A △ B ∩ U ∪ ∅ \ _A";
            var expected = new List<TokenType>
            {
                TokenType.Set,
                TokenType.Union,
                TokenType.LParen,
                TokenType.PrefixNegation,
                TokenType.Set,
                TokenType.Intersection,
                TokenType.Set,
                TokenType.PostfixNegation,
                TokenType.RParen,
                TokenType.Difference,
                TokenType.Set,
                TokenType.SymmetricDifference,
                TokenType.Set,
                TokenType.Intersection,
                TokenType.UniverseSet,
                TokenType.Union,
                TokenType.EmptySet,
                TokenType.Difference,
                TokenType.Variable
            };
            var result = syntax.GetTokenizer().TryTokenize(input);
            var actual = result.Value.Select(x => x.Kind).ToList();
            CollectionAssert.AreEqual(expected, actual);
        }
    }
}