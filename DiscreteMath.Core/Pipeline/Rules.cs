﻿using System.Collections.Generic;

namespace DiscreteMath.Core.Pipeline
{
    public class Rules
    {
        readonly static List<Rule> rules =
            new List<Rule>
            {
                new Rule("_A - O = _A", "Nutral rule"),
                new Rule("_A - _A = O", "Own inverse rule"),
                new Rule("_A △ O = _A", "Nutral rule"),
                new Rule("_A △ _A = O", "Own inverse rule"),
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
                new Rule("_A'' = _A", "Involution rule"),
                new Rule("(_A + _B) * (_A + _C) = (_A + (_B * _C))", "Distributive rule"),
                new Rule("(_A * _B) + (_A * _C) = (_A * (_B + _C))", "Distributive rule"),
                new Rule("_A + (_A * _B) = _A", "Absorption rule"),
                new Rule("_A + _A * _B = _A", "Absorption rule"),
                new Rule("_A * (_A + _B) = _A", "Absorption rule"),
                new Rule("(_A + _B)' = (_A' * _B')", "De Morgan's rule"),
                new Rule("(_A * _B)' = (_A' + _B')", "De Morgan's rule"),
                new Rule("_A - _B = _A * _B'", "Difference definition"),
                new Rule("_A △ _B = ((_A - _B) + (_B - _A))", "Symmetric difference definition"),
                new Rule("_A + (_B * _C) = (_A + _B) * (_A + _C)", "Distributive rule"),
                new Rule("_A * (_B + _C) = (_A * _B) + (_A * _C)", "Distributive rule"),
                new Rule("_A' * _B' = (_A + _B)'", "De Morgan's rule"),
                new Rule("_A' + _B' = (_A * _B)'", "De Morgan's rule"),
            };

        public List<Rule> GetRules()
        {
            return rules;
        }
    }
}