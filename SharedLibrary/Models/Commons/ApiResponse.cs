using SharedLibrary.Models.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Models.Commons
{
    public class ApiResponse
    {
        public bool Succeeded { get; set; }
        public string? ErrorMessage { get; set; }
        public Dictionary<string, IEnumerable<string>> ValidationErrors { get; set; }
        public ApiResponse()
        {
            ValidationErrors = new Dictionary<string, IEnumerable<string>>();
        }

        public static ApiResponse Success()
        {
            return new ApiResponse() {
                Succeeded = true,
                ErrorMessage = null,
                ValidationErrors = new Dictionary<string, IEnumerable<string>>()
            };
        }
        public static ApiResponse<T> Success<T>(T result) where T : class
        {
            return new ApiResponse<T>()
            {
                Data = result,
                Succeeded = true,
                ErrorMessage = null,
                ValidationErrors = new Dictionary<string, IEnumerable<string>>()
            };
        }
        public static ApiResponse Error(string message)
        {
            return new ApiResponse()
            {
                Succeeded = false,
                ErrorMessage = message,
                ValidationErrors = new Dictionary<string, IEnumerable<string>>()
            };
        }
        public static ApiResponse Error(Dictionary<string, IEnumerable<string>> validationMessages)
        {
            return new ApiResponse()
            {
                Succeeded = false,
                ErrorMessage = ValidationMessages.ValidationFailed,
                ValidationErrors = validationMessages
            };
        }
        public static ApiResponse FatalError()
        {
            return new ApiResponse()
            {
                Succeeded = false,
                ErrorMessage = ExceptionMessages.ExceptionOccured,
                ValidationErrors = new Dictionary<string, IEnumerable<string>>()
            };
        }
    }

    public class ApiResponse<T> : ApiResponse where T : class
    {
        public T? Data { get; set; }

        public static ApiResponse<T> Success(T result)
        {
            return new ApiResponse<T>()
            {
                Data = result,
                Succeeded = true,
                ErrorMessage = null,
                ValidationErrors = new Dictionary<string, IEnumerable<string>>()
            };
        }
        public new static ApiResponse<T> Error(string message)
        {
            return new ApiResponse<T>()
            {
                Data = null,
                Succeeded = false,
                ErrorMessage = message,
                ValidationErrors = new Dictionary<string, IEnumerable<string>>()
            };
        }
        public new static ApiResponse<T> Error(Dictionary<string, IEnumerable<string>> validationMessages)
        {
            return new ApiResponse<T>()
            {
                Data = null,
                Succeeded = false,
                ErrorMessage = ValidationMessages.ValidationFailed,
                ValidationErrors = validationMessages
            };
        }
        public new static ApiResponse<T> FatalError()
        {
            return new ApiResponse<T>()
            {
                Data = null,
                Succeeded = false,
                ErrorMessage = ExceptionMessages.ExceptionOccured,
                ValidationErrors = new Dictionary<string, IEnumerable<string>>()
            };
        }
    }
}
