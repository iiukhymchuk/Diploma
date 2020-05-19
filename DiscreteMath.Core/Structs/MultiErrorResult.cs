using System;
using System.Threading.Tasks;

namespace DiscreteMath.Core.Structs
{
    public class MultiErrorResult<T>
    {
        public MultiErrorResult(T value)
        {
            Value = value;
        }

        public MultiErrorResult(Error error, params string[] errorMessages)
        {
            HasError = true;
            Error = error;
            ErrorMessages = errorMessages ?? new string[0];
        }

        public bool HasError { get; private set; }
        public Error Error { get; private set; }
        public string[] ErrorMessages { get; private set; }
        public T Value { get; private set; }

        public MultiErrorResult<T2> With<T2>(Func<T, T2> function)
        {
            if (HasError)
                return new MultiErrorResult<T2>(Error, ErrorMessages);

            return new MultiErrorResult<T2>(function(Value));
        }

        public MultiErrorResult<T2> With<T2>(Func<T, MultiErrorResult<T2>> function)
        {
            if (HasError)
                return new MultiErrorResult<T2>(Error, ErrorMessages);

            return function(Value);
        }

        public MultiErrorResult<T2> With<T2>(Func<T, Result<T2>> function)
        {
            if (HasError)
                return new MultiErrorResult<T2>(Error, ErrorMessages);

            var result = function(Value);
            if (result.HasError)
            {
                return new MultiErrorResult<T2>(result.Error, result.ErrorMessage);
            }
            return new MultiErrorResult<T2>(result.Value);
        }

        public Task<MultiErrorResult<T2>> WithAsync<T2>(Func<T, Task<T2>> function)
        {
            if (HasError)
                return Task.FromResult(new MultiErrorResult<T2>(Error, ErrorMessages));

            return function(Value).ContinueWith(x => new MultiErrorResult<T2>(x.Result));
        }

        public Task<MultiErrorResult<T2>> WithAsync<T2>(Func<T, Task<MultiErrorResult<T2>>> function)
        {
            if (HasError)
                return Task.FromResult(new MultiErrorResult<T2>(Error, ErrorMessages));

            return function(Value);
        }

        public Task<MultiErrorResult<T2>> WithAsync<T2>(Func<T, Task<Result<T2>>> function)
        {
            if (HasError)
                return Task.FromResult(new MultiErrorResult<T2>(Error, ErrorMessages));


            return function(Value).ContinueWith(x => x.Result.HasError
                ? new MultiErrorResult<T2>(x.Result.Error, x.Result.ErrorMessage)
                : new MultiErrorResult<T2>(x.Result.Value)
                );
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
    }
}