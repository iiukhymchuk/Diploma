using System.Collections;
using System.Collections.Generic;

namespace SetTheory
{
    public class Rules : IEnumerable<Rule>
    {
        readonly static IEnumerable<Rule> rules =
            new List<Rule>
            {
                Rule.FromString("_A * _A = _A", "Indempodent rule"),
                Rule.FromString("_A + _A = _A", "Indempodent rule"),
                Rule.FromString("_A + U = U", "Domination rule"),
                Rule.FromString("U + _A = U", "Domination rule"),
                Rule.FromString("_A * O = O", "Domination rule"),
                Rule.FromString("O * _A = O", "Domination rule"),
                Rule.FromString("_A + O = _A", "Identity rule"),
                Rule.FromString("O + _A = _A", "Identity rule"),
                Rule.FromString("_A * U = _A", "Identity rule"),
                Rule.FromString("U * _A = _A", "Identity rule"),
                Rule.FromString("_A + _A' = U", "Complement rule"),
                Rule.FromString("_A * _A' = O", "Complement rule"),
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