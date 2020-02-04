using SetTheory.Expressions;
using SetTheory.Structs;
using Superpower.Model;

namespace SetTheory
{
    public static class Parser
    {
        public static TokenListParserResult<TokenType, Expression> TryParse(this TokenList<TokenType> tokens)
        {
            return Grammar.Tree(tokens);
        }

        public static TokenListParserResult<TokenType, Expression> TryParseRule(this TokenList<TokenType> tokens)
        {
            return Grammar.Expr(tokens);
        }
    }
}