﻿namespace SetTheory
{
    public enum TokenType
    {
        Set = 0,
        UniverseSet,
        EmptySet,
        Variable,
        PrefixNegation,
        PostfixNegation,
        Union,
        Intersection,
        Difference,
        SymmetricDifference,
        LParen,
        RParen
    }
}