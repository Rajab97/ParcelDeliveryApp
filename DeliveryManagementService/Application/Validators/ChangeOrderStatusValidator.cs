using DeliveryManagementService.Application.DTOs.Request;
using FluentValidation;
using SharedLibrary.Models.Constants;

namespace DeliveryManagementService.Application.Validators
{
    public class ChangeOrderStatusValidator: AbstractValidator<ChangeOrderStatusRequestDTO>
    {
        public ChangeOrderStatusValidator()
        {
            RuleFor(x => x.OrderStatus)
                .NotEmpty().WithMessage(ValidationMessages.NotNull);
            RuleFor(x => x.Id)
                .GreaterThan(0);
        }
    }
}
