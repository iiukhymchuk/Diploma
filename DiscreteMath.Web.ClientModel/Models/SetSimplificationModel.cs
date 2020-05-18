using DiscreteMath.Core.Language;
using System.ComponentModel.DataAnnotations;

namespace DiscreteMath.Web.Models
{
    public class SetSimplificationModel
    {
        [Required(ErrorMessage = "The value is required.")] // localize
        [IsSetTheoryExpression(ErrorMessage = "The expression is not valid.")] // localize
        public string Value { get; set; }
    }

    public sealed class IsSetTheoryExpressionAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var settings = new DefaultSettings();
            var syntax = new Syntax(settings);
            var syntaxResult = syntax.GetTokenizer().TryTokenize((string) value);
            if (!syntaxResult.HasValue)
                return false;
            var grammar = new Grammar(settings);
            var grammarResult = grammar.BuildTree(syntaxResult.Value);
            return grammarResult.HasValue;
        }
    }
}