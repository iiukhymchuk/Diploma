using DiscreteMath.Core.Language;
using DiscreteMath.Core.Structs;
using DiscreteMath.Core.Utils;
using System.Collections.Generic;
using System.Linq;

namespace DiscreteMath.Core.Pipeline
{
    class Normalizer
    {
        internal MyResult<Substitution> Normalize(Expression expr)
        {
            var descriptions = new List<string>();

            Expression result = expr;

            if (result.TryCombineCommutativeOperators(out result))
                descriptions.Add("Combine commutative operators");

            if (descriptions.Any())
                return new MyResult<Substitution>(
                    new Substitution
                    {
                        ResultingExpression = result.Sort(),
                        Description = string.Join<string>($", ", descriptions.Distinct())
                    });

            return MyResult<Substitution>.Empty();
        }
    }

    static class NormalizerMethods
    {
        internal static bool TryCombineCommutativeOperators(this Expression expr, out Expression result)
        {
            var changed = false;

            result = expr
                .AsTree()
                .ChangeTree((x, parent) => x.IsIntersection() && parent.IsIntersection() && SetChanged(out changed), x => x.Children)
                .ChangeTree((x, parent) => x.IsUnion() && parent.IsUnion() && SetChanged(out changed), x => x.Children)
                .ChangeTree((x, parent) => x.IsSymmetricDifference() && parent.IsSymmetricDifference() && SetChanged(out changed), x => x.Children)
                .AsExpression();

            return changed;
        }

        internal static Expression Sort(this Expression expr)
        {
            return expr
                .AsTree()
                .OrderBy(x => x.ToString())
                .AsExpression();
        }

        static bool SetChanged(out bool changed)
        {
            changed = true;
            return true;
        }
    }
}