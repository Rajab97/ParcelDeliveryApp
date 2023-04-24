using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SharedLibrary.Models.Commons;
using UserManagementService.Application.DTOs;

namespace UserManagementService.Application.Filters
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ValidateRequestAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var arguments = context.ActionArguments.Values.Where(m => m.GetType().IsAssignableTo(typeof(IValidatableDTO)));
            Dictionary<string, IEnumerable<string>> errors = new Dictionary<string, IEnumerable<string>>();
            foreach (var argument in arguments)
            {
                var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());
                var validator = context.HttpContext.RequestServices.GetService(validatorType) as IValidator;
                if (validator != null)
                {
                    var validationResult = validator.Validate(new ValidationContext<object>(argument));
                    if (!validationResult.IsValid)
                    {
                        foreach (var err in validationResult.Errors.GroupBy(m=>m.PropertyName))
                        {
                            errors.Add(err.Key,err.Select(m=>m.ErrorMessage));
                        }
                    }
                }
            }
            if (errors.Any())
                context.Result = new BadRequestObjectResult(ApiResponse.Error(errors));
            base.OnActionExecuting(context);
        }
    }
}
