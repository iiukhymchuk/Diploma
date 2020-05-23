using System;

namespace DiscreteMath.Core.Language
{
    public abstract class Expression
    {
        public Type Type => GetType();
        public Guid Id { get; protected set; }

        public abstract string Value { get; }
        public abstract Expression[] Children { get; set; }
        public abstract Expression Clone();
        public abstract Expression Copy();
        public abstract override string ToString();
        public Expression WithId(Guid id)
        {
            Id = id;
            return this;
        }

        public virtual Tree AsTree() => new Tree(this);
    }
}