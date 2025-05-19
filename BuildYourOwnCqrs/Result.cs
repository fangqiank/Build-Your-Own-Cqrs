namespace BuildYourOwnCqrs
{
    public class Result
    {
        public bool IsSuccess { get; }
        public string Error { get; }

        protected Result(bool isSuccess, string error)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        // 非泛型 Success（无数据）
        public static Result Success() => new Result(true, string.Empty);

        // 非泛型 Failure
        public static Result Failure(string error)
        {
            if (string.IsNullOrEmpty(error))
                throw new ArgumentException("Error message cannot be empty.", nameof(error));

            return new Result(false, error);
        }
    }

    public sealed class Result<T> : Result
    {
        public T Value { get; }

        // 构造函数改为 protected internal，允许通过工厂方法创建
        protected internal Result(T value, bool isSuccess, string error)
            : base(isSuccess, error)
        {
            if (isSuccess && value is null)
                throw new ArgumentNullException(nameof(value), "Value cannot be null for a successful result.");

            Value = value;
        }

        // 泛型 Success（有数据）
        public static Result<T> Success(T value)
        {
            if (value is null)
                throw new ArgumentNullException(nameof(value));

            return new Result<T>(value, true, string.Empty);
        }

        // 泛型 Failure
        public new static Result<T> Failure(string error)
        {
            if (string.IsNullOrEmpty(error))
                throw new ArgumentException("Error message cannot be empty.", nameof(error));

            return new Result<T>(default!, false, error);
        }
    }
}
