﻿using System.Collections;
using System.Collections.Generic;

namespace SetTheory
{
    public class Rules : IEnumerable<Rule>
    {
        readonly static IEnumerable<Rule> rules =
            new List<Rule>
            {
                new Rule("_A * _A = _A", "Indempodent rule"),
                new Rule("_A + _A = _A", "Indempodent rule"),
                new Rule("_A + U = U", "Domination rule"),
                new Rule("U + _A = U", "Domination rule"),
                new Rule("_A * O = O", "Domination rule"),
                new Rule("O * _A = O", "Domination rule"),
                new Rule("_A + O = _A", "Identity rule"),
                new Rule("O + _A = _A", "Identity rule"),
                new Rule("_A * U = _A", "Identity rule"),
                new Rule("U * _A = _A", "Identity rule"),
                new Rule("_A + _A' = U", "Complement rule"),
                new Rule("_A * _A' = O", "Complement rule"),
                new Rule("O' = U", "Complement rule"),
                new Rule("U' = O", "Complement rule"),
                new Rule("_A + (_A * _B) = _A", "Absorption rule"),
                new Rule("_A + (_B * _A) = _A", "Absorption rule"),
                new Rule("_A * (_A + _B) = _A", "Absorption rule"),
                new Rule("_A * (_B + _A) = _A", "Absorption rule"),
                new Rule("(_A * _B) + _A = _A", "Absorption rule"),
                new Rule("(_B * _A) + _A = _A", "Absorption rule"),
                new Rule("(_A + _B) * _A = _A", "Absorption rule"),
                new Rule("(_B + _A) * _A = _A", "Absorption rule"),
                new Rule("_A'' = _A", "Involution rule"),
                new Rule("(_A + _B)' = (_A' * _B')", "De Morgan's rule"),
                new Rule("(_A * _B)' = (_A' + _B')", "De Morgan's rule"),
            };

        public IEnumerator<Rule> GetEnumerator()
        {
            return rules.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return rules.GetEnumerator();
        }
    }
}