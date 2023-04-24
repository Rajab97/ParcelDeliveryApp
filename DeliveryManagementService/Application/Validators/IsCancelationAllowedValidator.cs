using DeliveryManagementService.Application.DTOs.Request;
using FluentValidation;
using SharedLibrary.Models.Constants;

namespace DeliveryManagementService.Application.Validators
{
    public class IsCancelationAllowedValidator : AbstractValidator<IsCancelationAllowedRequestDTO>
    {
        public IsCancelationAllowedValidator()
        {
            RuleFor(m => m.OrderNumber)
                .NotEmpty().WithMessage(ValidationMessages.NotNull);
        }
    }
}
