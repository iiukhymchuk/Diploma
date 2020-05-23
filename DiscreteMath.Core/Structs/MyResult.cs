using System;

namespace DiscreteMath.Core.Structs
{
    public class MyResult<T>
    {
        static readonly MyResult<T> empty = new MyResult<T>();

        public MyResult(string errorMessage, int errorIndex, string token = null)
        {
            ErrorMessage = errorMessage;
            ErrorIndex = errorIndex;
            Token = token;
            HasValue = false;
        }

        MyResult(T value)
        {
            Value = value;
            HasValue = true;
        }

        MyResult()
        {
            HasValue = false;
        }

        public bool HasValue { get; }
        public T Value { get; }
        public string ErrorMessage { get; }
        public string Token { get; }
        public int ErrorIndex { get; }

        public static MyResult<T> Empty() => empty;
        public static MyResult<T> From(T value) => new MyResult<T>(value);

        public static implicit operator MyResult<T>(T value)
        {
            if (value is null)
                return Empty();

            return new MyResult<T>(value);
        }

        public MyResult<T2> With<T2>(Func<T, T2> func)
        {
            if (!HasValue)
                return MyResult<T2>.Empty();

            return MyResult<T2>.From(func(Value));
        }

        public MyResult<T2> With<T2>(Func<T, MyResult<T2>> func)
        {
            if (!HasValue)
                return MyResult<T2>.Empty();

            return func(Value);
        }

        public MyResult<T> EmptyIf(Func<bool> predicate)
        {
            if (!HasValue || predicate())
                return empty;

            return this;
        }

        public MyResult<T> NotEmptyIf(Func<bool> predicate)
        {
            return EmptyIf(() => !predicate());
        }

        public MyResult<T> Do(Action action)
        {
            if (HasValue)
                action();

            return this;
        }

        public MyResult<T> Do(Action<T> action)
        {
            if (HasValue)
                action(Value);

            return this;
        }
    }

    public static class MyResult
    {
        public static MyResult<T> Empty<T>() => MyResult<T>.Empty();
        public static MyResult<T> From<T>(T value) => MyResult<T>.From(value);
    }
}