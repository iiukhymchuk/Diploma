namespace SetTheory
{
    public class Result<T>
    {
        static readonly Result<T> empty = new Result<T>();

        public Result(T value)
        {
            Value = value;
            HasValue = true;
        }

        public Result(string errorMessage, int errorIndex, string token = null)
        {
            ErrorMessage = errorMessage;
            ErrorIndex = errorIndex;
            Token = token;
            HasValue = false;
        }

        Result()
        {
            HasValue = false;
        }

        public static Result<T> Empty() => empty;

        public bool HasValue { get; }
        public T Value { get; }
        public string ErrorMessage { get; }
        public string Token { get; }
        public int ErrorIndex { get; }
    }
}