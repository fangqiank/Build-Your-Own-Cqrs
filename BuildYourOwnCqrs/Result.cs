namespace BuildYourOwnCqrs
{
    public class Result
    {
        public bool IsSuccess { get; init; }
        public string Error { get; init; } = string.Empty;

        protected Result(bool isSuccess, string error)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Success() => new(true, string.Empty);
        public static Result Failure(string error) => new(false, error);
    }

    public class Result<T> : Result
    {
        public T Value { get; }

        private Result(T value, bool isSuccess, string error) : base(isSuccess, error)
        {
            if (isSuccess && value == null)
                throw new ArgumentNullException(nameof(value), "Value cannot be null for success results.");

            Value = value;
        }

        public static Result<T> Success(T value) => new(value, true, string.Empty);
        public static Result<T> Failure(string error) => new(default!, false, error);
    }
}
