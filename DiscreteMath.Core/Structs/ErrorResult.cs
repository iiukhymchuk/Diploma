namespace DiscreteMath.Core.Structs
{
    public class ErrorResult
    {
        public Error Error { get; }
        public string ErrorMessage { get; private set; }

        public ErrorResult(Error error, string errorMessage = null)
        {
            Error = error;
            ErrorMessage = errorMessage;
        }
    }
}