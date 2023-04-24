using FluentValidation;
using SharedLibrary.Models.Constants;
using UserManagementService.Application.DTOs;

namespace UserManagementService.Application.Validations
{
    public class SignInRequestValidator : AbstractValidator<SignInRequestDTO>
    {
        public SignInRequestValidator()
        {
            RuleFor(m => m.UserName)
                .NotEmpty().WithMessage(ValidationMessages.NotNull)
                .MinimumLength(5).WithMessage(ValidationMessages.MinLength)
                .MaximumLength(25).WithMessage(ValidationMessages.MaxLength);

            RuleFor(m=>m.Password)
                  .NotEmpty().WithMessage(ValidationMessages.NotNull)
                  .MinimumLength(8).WithMessage(ValidationMessages.MinLength)
                    .MaximumLength(25).WithMessage(ValidationMessages.MaxLength);
        }
    }
}
