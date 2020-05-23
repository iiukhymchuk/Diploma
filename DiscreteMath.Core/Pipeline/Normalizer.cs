using DiscreteMath.Core.Language;
using DiscreteMath.Core.Utils;
using System.Linq;

namespace DiscreteMath.Core.Pipeline
{
    static class Normalizer
    {
        internal static Expression Normalize(this Expression expr)
        {
            return expr
                .CombineCommutativeOperators()
                .Sort();
        }

        static Expression CombineCommutativeOperators(this Expression expr)
        {
            return expr
                .AsTree()
                .ChangeTree((x, parent) => x.IsIntersection() && parent.IsIntersection(), x => x.Children)
                .ChangeTree((x, parent) => x.IsUnion() && parent.IsUnion(), x => x.Children)
                .ChangeTree((x, parent) => x.IsSymmetricDifference() && parent.IsSymmetricDifference(), x => x.Children)
                .AsExpression();
        }

        static Expression Sort(this Expression expr)
        {
            return expr
                .AsTree()
                .OrderBy(x => x.IsIntersection() || x.IsUnion() || x.IsSymmetricDifference(), x => x.ToString())
                .AsExpression();
        }
    }
}