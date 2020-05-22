using DiscreteMath.Core.Structs;
using Superpower;
using System;
using System.Collections.Generic;

namespace DiscreteMath.Core.Language
{
    public class Rules
    {
        readonly static List<Rule> rules =
            new List<Rule>
            {
                new Rule("_A'' = _A", "Involution rule"),
                new Rule("_A * _A = _A", "Indempodent rule"),
                new Rule("_A + _A = _A", "Indempodent rule"),
                new Rule("_A + U = U", "Domination rule"),
                new Rule("_A * O = O", "Domination rule"),
                new Rule("_A + O = _A", "Identity rule"),
                new Rule("_A * U = _A", "Identity rule"),
                new Rule("_A + _A' = U", "Complement rule"),
                new Rule("_A * _A' = O", "Complement rule"),
                new Rule("O' = U", "Complement rule"),
                new Rule("U' = O", "Complement rule"),
                new Rule("_A - O = _A", "Nutral rule"),
                new Rule("_A - _A = O", "Own inverse rule"),
                new Rule("_A △ O = _A", "Nutral rule"),
                new Rule("_A △ _A = O", "Own inverse rule"),

                new Rule("_A + (_A' * _B) = (_A + _A') * (_A + _B)", "Distributive rule"),
                new Rule("_A * (_A' + _B) = (_A * _A') + (_A * _B)", "Distributive rule"),
                new Rule("_A' + (_A * _B) = (_A' + _A) * (_A' + _B)", "Distributive rule"),
                new Rule("_A' * (_A + _B) = (_A' * _A) + (_A' * _B)", "Distributive rule"),

                new Rule("_A + (_A * _B) = _A", "Absorption rule"),
                new Rule("_A * (_A + _B) = _A", "Absorption rule"),

                new Rule("(_A + _B) * (_A + _C) = _A + (_B * _C)", "Distributive rule"),
                new Rule("(_A * _B) + (_A * _C) = _A * (_B + _C)", "Distributive rule"),

                new Rule("(_A + _B)' = _A' * _B'", "De Morgan's rule"),
                new Rule("(_A * _B)' = _A' + _B'", "De Morgan's rule"),

                new Rule("_A' * _B' = (_A + _B)'", "De Morgan's rule"),
                new Rule("_A' + _B' = (_A * _B)'", "De Morgan's rule"),
                new Rule("_A - _B = _A * _B'", "Difference definition"),
                new Rule("_A △ _B = (A ∩ B') ∪ (A' ∩ B)", "Symmetric difference definition"),

                new Rule("_A + (_B * _C) = (_A + _B) * (_A + _C)", "Distributive rule"),
                new Rule("_A * (_B + _C) = (_A * _B) + (_A * _C)", "Distributive rule"),
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

        public Rule(string rule, string description)
        {
            var array = rule.Split('=', 2);
            var patternIn = Parse(array[0]);
            var patternOut = Parse(array[1]);

            Id = Guid.NewGuid();
            Description = description;
            PatternIn = patternIn;
            PatternOut = patternOut;
        }

        public Guid Id { get; }
        public string Description { get; }
        public Expression PatternIn { get; }
        public Expression PatternOut { get; }

        static Expression Parse(string pattern)
        {
            var tokens = tokenizer.TryTokenize(pattern);
            return grammar.BuildTree(tokens.Value).Value;
        }
    }
}