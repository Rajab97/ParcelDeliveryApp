using FluentValidation;
using SharedLibrary.Models.Constants;
using UserManagementService.Application.DTOs;

namespace UserManagementService.Application.Validations
{
    public class CreateAccountValidator : AbstractValidator<CreateAccountRequestDTO>
    {
        public CreateAccountValidator()
        {
            RuleFor(m => m.UserName)
                .NotEmpty().WithMessage(ValidationMessages.NotNull)
                .MinimumLength(5).WithMessage(ValidationMessages.MinLength)
                .MaximumLength(25).WithMessage(ValidationMessages.MaxLength);

            RuleFor(m => m.FirstName)
             .NotEmpty().WithMessage(ValidationMessages.NotNull)
             .MinimumLength(2).WithMessage(ValidationMessages.MinLength)
             .MaximumLength(25).WithMessage(ValidationMessages.MaxLength);

            RuleFor(m => m.LastName)
             .NotEmpty().WithMessage(ValidationMessages.NotNull)
             .MinimumLength(2).WithMessage(ValidationMessages.MinLength)
             .MaximumLength(25).WithMessage(ValidationMessages.MaxLength);

            RuleFor(m => m.Email)
               .NotEmpty().WithMessage(ValidationMessages.NotNull)
               .MinimumLength(3).WithMessage(ValidationMessages.MinLength)
               .MaximumLength(25).WithMessage(ValidationMessages.MaxLength);

            RuleFor(m => m.Role)
             .NotEmpty().WithMessage(ValidationMessages.NotNull);

            RuleFor(m => m.Password)
                  .NotEmpty().WithMessage(ValidationMessages.NotNull)
                  .MinimumLength(8).WithMessage(ValidationMessages.MinLength)
                    .MaximumLength(25).WithMessage(ValidationMessages.MaxLength);
        }
    }
}
