namespace SetTheory
{
    public class Substitution
    {
        public Expression Initial { get; set; }
        public Expression Matched { get; set; }
        public Expression Resulting { get; set; }
        public string Description { get; set; }
    }
}