using DeliveryManagementService.Application.DTOs.Request;
using FluentValidation;
using SharedLibrary.Models.Constants;

namespace DeliveryManagementService.Application.Validators
{
    public class CancelOrderValidator : AbstractValidator<CancelOrderRequestDTO>
    {
        public CancelOrderValidator()
        {
            RuleFor(m => m.OrderNumber)
                .NotEmpty().WithMessage(ValidationMessages.NotNull);
        }
    }
}
