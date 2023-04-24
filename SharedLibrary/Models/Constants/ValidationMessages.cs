using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Models.Constants
{
    public static class ValidationMessages
    {
        public const string NotNull = "{PropertyName} is required";
        public const string ValidationFailed = "One or more validation errors occured";
        public const string MaxLength = "Length of {PropertyName} must be smaller than {MaxLength} characters";
        public const string MinLength = "Length of {PropertyName} must be greater than {MinLength} characters";
    }
}
