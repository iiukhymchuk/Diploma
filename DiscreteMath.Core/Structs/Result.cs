using System;
using ErrorEnum = DiscreteMath.Core.Structs.Error;

namespace DiscreteMath.Core.Structs
{
    public static class Result
    {
        public static Result<T> Success<T>(T value) => new Result<T>(value);

        public static Result<T> Error<T>(Error error, string errorMessage = null) => new Result<T>(error, errorMessage);

        public static Result<T> Exception<T>(Exception exception)
            => Error<T>(ErrorEnum.UnknownException, exception.ToString());

        public static ErrorResult Error(Error error, string errorMessage = null) => new ErrorResult(error, errorMessage);

        public static ErrorResult Exception(Exception exception)
            => new ErrorResult(ErrorEnum.UnknownException, exception.ToString());

        public static Result<T> WrapNull<T>(T val, Error errorIfNull, string errorMessage = null)
            where T : class
        {
            if (val != null)
                return new Result<T>(val);
            return new Result<T>(errorIfNull, errorMessage);
        }
    }
}