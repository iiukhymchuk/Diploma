namespace SetTheory
{
    public class Substitution
    {
        public Expression InitialExpression { get; set; }
        public Expression ResultingExpression { get; set; }
        public Expression InitialPart { get; set; }
        public Expression ResultingPart { get; set; }
        public string Description { get; set; }
    }
}