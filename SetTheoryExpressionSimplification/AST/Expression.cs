using System;

namespace SetTheory
{
    public abstract class Expression
    {
        public Type Type => GetType();
        public abstract string Value { get; }
        public abstract Expression[] Children { get; set; }
        public abstract Expression Copy();
        public abstract override string ToString();
    }
}