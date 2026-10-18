using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tehilim.Core.Results
{
    public class Result<T> : Result
    {
        public T? Value { get; }

        private Result(bool isSuccess, T? value, ErrorType error, string? message)
            : base(isSuccess, error, message)
        {
            Value = value;
        }

        public static Result<T> Success(T value) => new(true, value, ErrorType.None, null);
        public static new Result<T> NotFound(string message) => new(false, default, ErrorType.NotFound, message);
        public static new Result<T> Forbidden(string message) => new(false, default, ErrorType.Forbidden, message);
        public static new Result<T> Conflict(string message) => new(false, default, ErrorType.Conflict, message);
        public static new Result<T> Validation(string message) => new(false, default, ErrorType.Validation, message);
        public static new Result<T> Unauthorized(string message) => new(false, default, ErrorType.Unauthorized, message);
    }
}
