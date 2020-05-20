using DiscreteMath.Core.Language;
using DiscreteMath.Core.Structs;
using Superpower;
using System;
using System.Collections.Generic;

namespace DiscreteMath.Core.Pipeline
{
    public class Rules
    {
        readonly static List<Rule> rules =
            new List<Rule>
            {
                new Rule("_A'' = _A", "Involution rule", 0.1d),
                new Rule("_A * _A = _A", "Indempodent rule", 0.1d),
                new Rule("_A + _A = _A", "Indempodent rule", 0.1d),
                new Rule("_A + U = U", "Domination rule", 0.1d),
                new Rule("_A * O = O", "Domination rule", 0.1d),
                new Rule("_A + O = _A", "Identity rule", 0.1d),
                new Rule("_A * U = _A", "Identity rule", 0.1d),
                new Rule("_A + _A' = U", "Complement rule", 0.1d),
                new Rule("_A * _A' = O", "Complement rule", 0.1d),
                new Rule("O' = U", "Complement rule", 0.1d),
                new Rule("U' = O", "Complement rule", 0.1d),
                new Rule("_A - O = _A", "Nutral rule", 0.1d),
                new Rule("_A - _A = O", "Own inverse rule", 0.1d),
                new Rule("_A △ O = _A", "Nutral rule", 0.1d),
                new Rule("_A △ _A = O", "Own inverse rule", 0.1d),

                new Rule("_A + (_A' * _B) = (_A + _A') * (_A + _B)", "Distributive rule", 0.2d),
                new Rule("_A * (_A' + _B) = (_A * _A') + (_A * _B)", "Distributive rule", 0.2d),
                new Rule("_A' + (_A * _B) = (_A' + _A) * (_A' + _B)", "Distributive rule", 0.2d),
                new Rule("_A' * (_A + _B) = (_A' * _A) + (_A' * _B)", "Distributive rule", 0.2d),

                new Rule("_A + (_A * _B) = _A", "Absorption rule", 0.25d),
                new Rule("_A + _A * _B = _A", "Absorption rule", 0.25d),
                new Rule("_A * (_A + _B) = _A", "Absorption rule", 0.25d),

                new Rule("(_A + _B) * (_A + _C) = (_A + (_B * _C))", "Distributive rule", 0.3d),
                new Rule("(_A * _B) + (_A * _C) = (_A * (_B + _C))", "Distributive rule", 0.3d),

                new Rule("(_A + _B)' = (_A' * _B')", "De Morgan's rule", 0.4d),
                new Rule("(_A * _B)' = (_A' + _B')", "De Morgan's rule", 0.4d),

                new Rule("_A' * _B' = (_A + _B)'", "De Morgan's rule", 0.5d),
                new Rule("_A' + _B' = (_A * _B)'", "De Morgan's rule", 0.5d),
                new Rule("_A - _B = _A * _B'", "Difference definition", 0.5d),
                new Rule("_A △ _B = ((A ∩ B') ∪ (A' ∩ B))", "Symmetric difference definition", 0.5d),

                new Rule("_A + (_B * _C) = (_A + _B) * (_A + _C)", "Distributive rule", 0.55d),
                new Rule("_A * (_B + _C) = (_A * _B) + (_A * _C)", "Distributive rule", 0.55d),
            };

        public List<Rule> GetRules()
        {
            return rules;
        }
    }

    public class Rule
    {
        static readonly Tokenizer<TokenType> tokenizer;
        static readonly Grammar grammar;

        static Rule()
        {
            var settings = new DefaultSettings();
            tokenizer = new SyntaxWithVariables(settings).GetTokenizer();
            grammar = new Grammar(settings);
        }

        public Rule(string rule, string description, double precidence)
        {
            var array = rule.Split('=', 2);
            var patternIn = Parse(array[0]);
            var patternOut = Parse(array[1]);

            Id = Guid.NewGuid();
            Description = description;
            PatternIn = patternIn;
            PatternOut = patternOut;
            Precidence = precidence;
        }

        public Guid Id{ get; }
        public string Description { get; }
        public Expression PatternIn { get; }
        public Expression PatternOut { get; }
        public double Precidence { get; }

        static Expression Parse(string pattern)
        {
            var tokens = tokenizer.TryTokenize(pattern);
            var result = grammar.BuildTree(tokens.Value);
            return result.Value.Children[0];
        }
    }
}