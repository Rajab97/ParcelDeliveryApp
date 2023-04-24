using DeliveryManagementService.Application.DTOs.Request;
using FluentValidation;
using SharedLibrary.Models.Constants;

namespace DeliveryManagementService.Application.Validators
{
    public class GetDeliveryHistoryOfOrderValidator : AbstractValidator<GetDeliveryHistoryOfOrderRequestDTO>
    {
        public GetDeliveryHistoryOfOrderValidator()
        {
            RuleFor(m=>m.OrderNumber)
                .NotEmpty().WithMessage(ValidationMessages.NotNull);
        }
    }
}
