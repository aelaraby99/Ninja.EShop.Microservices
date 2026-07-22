using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NINJA.EShop.Shared.Results
{
    public class Result
    {
        /// <summary>Initialises the success/failure state and validates the success–error invariant.</summary>
        protected Result(bool isSuccess,string? error = null)
        {
            switch (isSuccess)
            {
                case true when !string.IsNullOrEmpty(error):
                    throw new InvalidOperationException("A successful result cannot carry an error.");
                case false when string.IsNullOrEmpty(error):
                    throw new InvalidOperationException("A failed result must carry a non-empty error.");
                default:
                    IsSuccess = isSuccess;
                    Error = error;
                    break;
            }
        }

        /// <summary><c>true</c> when the operation succeeded.</summary>
        public bool IsSuccess { get; }

        /// <summary><c>true</c> when the operation failed.</summary>
        public bool IsFailure => !IsSuccess;

        /// <summary>The failure cause — <see cref="Error.None"/> on success.</summary>
        public string? Error { get; }

        /// <summary>A successful result with no value.</summary>
        public static Result Success() => new(true,string.Empty);

        /// <summary>A failed result carrying <paramref name="error"/>.</summary>
        public static Result Failure(string error) => new(false,error);

        /// <summary>A successful result carrying <paramref name="value"/>.</summary>
        public static Result<TValue> Success<TValue>(TValue value) => Result<TValue>.FromValue(value);

        /// <summary>A failed result of <typeparamref name="TValue"/> carrying <paramref name="error"/>.</summary>
        public static Result<TValue> Failure<TValue>(string error) => Result<TValue>.FromError(error);
    }

    public class Result<TValue>: Result
    {
        private readonly TValue _value;
        private Result(TValue value) : base(true,string.Empty) => _value = value;
        private Result(string error) : base(false,error) => _value = default!;
        /// <summary>
        /// The success payload. Throws <see cref="InvalidOperationException"/> when accessed on a
        /// failed result — check <see cref="Result.IsSuccess"/> first.
        /// </summary>
        public TValue Value => IsSuccess ? _value : throw new InvalidOperationException("Cannot access the value of a failed result.");
        /// <summary>Wraps a success payload. Named alternate for the implicit conversion.</summary>
        public static Result<TValue> FromValue(TValue value) => new(value);
        /// <summary>Wraps a failure error. Named alternate for the implicit conversion.</summary>
        public static Result<TValue> FromError(string error) => new(error);
        /// <summary>Lifts a value into a successful result.</summary>
        public static implicit operator Result<TValue>(TValue value) => new(value);
        /// <summary>Lifts an error into a failed result.</summary>
        public static implicit operator Result<TValue>(string error) => new(error);
    }
}
