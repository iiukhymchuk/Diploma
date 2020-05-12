using System;

namespace SetTheory
{
    public abstract class Expression
    {
        public Type Type => GetType();
        public Guid? Id { get; set; }
        public abstract string Value { get; }
        public abstract Expression[] Children { get; set; }
        public abstract Expression Copy(bool copyId = false);
        public abstract override string ToString();
    }
}