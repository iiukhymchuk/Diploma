using SetTheory.Structs;
using Superpower.Model;

namespace SetTheory
{
    public static class Tokenizer
    {
        public static Superpower.Model.Result<TokenList<TokenType>> TryTokenize(this string source)
        {
            return Syntax.Main.TryTokenize(source);
        }

        public static Superpower.Model.Result<TokenList<TokenType>> TryTokenizeWithVariables(this string source)
        {
            return Syntax.WithVariables.TryTokenize(source);
        }
    }
}