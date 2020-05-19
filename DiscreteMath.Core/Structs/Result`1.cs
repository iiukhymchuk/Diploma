using System;
using System.Threading.Tasks;

namespace DiscreteMath.Core.Structs
{
    public class Result<T>
    {
        public Result(T value)
        {
            Value = value;
        }

        public Result(Error error, string errorMessage = null)
        {
            HasError = true;
            Error = error;
            ErrorMessage = errorMessage;
        }

        public bool HasError { get; private set; }
        public Error Error { get; private set; }
        public string ErrorMessage { get; private set; }
        public T Value { get; private set; }

        /// <summary>
        /// Converts to another result type, if finished with error.
        /// </summary>
        /// <typeparam name="T2"></typeparam>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">This opration can be performed only on failed results.</exception>
        public Result<T2> ConvetTo<T2>()
        {
            if (!HasError)
                throw new InvalidOperationException("This opration can be performed only on failed results.");

            return new Result<T2>(Error, ErrorMessage);
        }

        public Result<T2> With<T2>(Func<T, T2> function)
        {
            if (HasError)
                return new Result<T2>(Error, ErrorMessage);

            return Result.Success(function(Value));
        }

        public Result<T2> With<T2>(Func<T, Result<T2>> function)
        {
            if (HasError)
                return new Result<T2>(Error, ErrorMessage);

            return function(Value);
        }

        public Task<Result<T2>> WithAsync<T2>(Func<T, Task<T2>> function)
        {
            if (HasError)
                return Task.FromResult(new Result<T2>(Error, ErrorMessage));

            return function(Value).ContinueWith(x => Result.Success(x.Result));
        }

        public Task<Result<T2>> WithAsync<T2>(Func<T, Task<Result<T2>>> function)
        {
            if (HasError)
                return Task.FromResult(new Result<T2>(Error, ErrorMessage));

            return function(Value);
        }

        public void Do(Action<T> action)
        {
            if (!HasError)
                action(Value);
        }

        public Task DoAsync(Func<T, Task> action)
        {
            if (!HasError)
                return action(Value);
            else
                return Task.FromResult(false);
        }

        /// <summary>
        /// Allows to return actual value where <see cref="Result{T}"/> is expected.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator Result<T>(T value)
        {
            return Result.Success(value);
        }

        /// <summary>
        /// Allow returning error-code(<see cref="Common.Enums.Error"/> enum), where <see cref="Result{T}"/> is expected.
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        public static implicit operator Result<T>(Error error)
        {
            return new Result<T>(error);
        }

        /// <summary>
        /// Allow returning error-code(<see cref="Common.Enums.Error"/> enum) with description, where <see cref="Result{T}"/> is expected.
        /// 
        /// <code>
        /// Result&lt;int&gt; DoSomething()
        /// {
        ///     return Result.Error(Error.NotFound,"Element not found");
        /// }
        /// </code>
        /// instead of:
        /// <code>
        /// Result&lt;int&gt; DoSomething()
        /// {
        ///     return Result.Error&lt;int&gt;(Error.NotFound,"Element not found");
        /// }
        /// </code>
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        public static implicit operator Result<T>(ErrorResult error)
        {
            return new Result<T>(error.Error, error.ErrorMessage);
        }
    }
}