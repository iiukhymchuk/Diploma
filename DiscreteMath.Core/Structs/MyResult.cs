namespace DiscreteMath.Core.Structs
{
    public class MyResult<T>
    {
        static readonly MyResult<T> empty = new MyResult<T>();

        public MyResult(T value)
        {
            Value = value;
            HasValue = true;
        }

        public MyResult(string errorMessage, int errorIndex, string token = null)
        {
            ErrorMessage = errorMessage;
            ErrorIndex = errorIndex;
            Token = token;
            HasValue = false;
        }

        MyResult()
        {
            HasValue = false;
        }

        public static MyResult<T> Empty() => empty;

        public bool HasValue { get; }
        public T Value { get; }
        public string ErrorMessage { get; }
        public string Token { get; }
        public int ErrorIndex { get; }
    }
}