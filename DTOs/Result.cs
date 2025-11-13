using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public class Result<T>
    {
        public bool IsSuccess { get; private set; }
        public string? Code { get; private set; }        // Changed to string
        public string? ErrorMessage { get; private set; }
        public T? Data { get; private set; }

        // Success factory method
        public static Result<T> Success(T data, string code = "success")
        {
            return new Result<T>
            {
                IsSuccess = true,
                Data = data,
                Code = code,
                ErrorMessage = null
            };
        }

        // Failure factory method
        public static Result<T> Failure(string code, string errorMessage)
        {
            return new Result<T>
            {
                IsSuccess = false,
                Data = default,
                Code = code,
                ErrorMessage = errorMessage
            };
        }
    }
}
