using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tehilim.Core.Results
{
    public class Result
    {
        public bool IsSuccess { get; }
        public ErrorType Error { get; }
        public string? Message { get; }

        protected Result(bool isSuccess, ErrorType error, string? message)
        {
            IsSuccess = isSuccess;
            Error = error;
            Message = message;
        }

        public static Result Success() => new(true, ErrorType.None, null);
        public static Result NotFound(string message) => new(false, ErrorType.NotFound, message);
        public static Result Forbidden(string message) => new(false, ErrorType.Forbidden, message);
        public static Result Conflict(string message) => new(false, ErrorType.Conflict, message);
        public static Result Validation(string message) => new(false, ErrorType.Validation, message);
        public static Result Unauthorized(string message) => new(false, ErrorType.Unauthorized, message);
    }
}
