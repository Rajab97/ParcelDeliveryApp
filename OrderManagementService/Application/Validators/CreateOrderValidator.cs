using FluentValidation;
using OrderManagementService.Application.DTOs.Request;
using SharedLibrary.Models.Constants;

namespace OrderManagementService.Application.Validators
{
    public class CreateOrderValidator : AbstractValidator<CreateOrderRequestDTO>
    {
        public CreateOrderValidator()
        {
            RuleForEach(m => m.OrderItems).SetValidator(new CreateOrderItemValidator());
            RuleFor(m => m.ShippingAddress)
                .NotNull().WithMessage(ValidationMessages.NotNull)
                .MaximumLength(250).WithMessage(ValidationMessages.MaxLength);
            RuleFor(m => m.BillingAddress)
             .NotNull().WithMessage(ValidationMessages.NotNull)
             .MaximumLength(250).WithMessage(ValidationMessages.MaxLength);
        }
    }

    public class CreateOrderItemValidator : AbstractValidator<CreateOrderItemRequestDTO>
    {
        public CreateOrderItemValidator()
        {
            RuleFor(m => m.Price)
                .GreaterThan(0);
            RuleFor(m => m.Quantity)
               .GreaterThan(0);
            RuleFor(m => m.ProductNumber)
                .NotEmpty().WithMessage(ValidationMessages.NotNull)
                .MaximumLength(50).WithMessage(ValidationMessages.MaxLength);
        }
    }
}
