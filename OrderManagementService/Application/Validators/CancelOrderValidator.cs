using FluentValidation;
using OrderManagementService.Application.DTOs.Request;

namespace OrderManagementService.Application.Validators
{
    public class CancelOrderValidator : AbstractValidator<CancelOrderRequestDTO>
    {
        public CancelOrderValidator()
        {
            RuleFor(x => x.OrderId)
                .GreaterThan(0);
        }
    }
}
