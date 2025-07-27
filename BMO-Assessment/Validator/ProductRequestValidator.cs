using BMO_Assessment.Models;
using FluentValidation;

namespace BMO_Assessment.Validator
{
    public class ProductRequestValidator : AbstractValidator<ProductRequest>
    {
        public ProductRequestValidator()
        {
            RuleFor(x => x.ProductName)
                .NotEmpty().WithMessage("Product name is required.")
                .Matches(@"^[a-zA-Z0-9\s\-]{3,50}$")
                .WithMessage("Product name must be alphanumeric and between 3 to 50 characters.");

            RuleFor(x => x.Category)
                .NotEmpty().WithMessage("Category is required.")
                .Matches(@"^[a-zA-Z\s]{2,30}$")
                .WithMessage("Category must contain only letters and spaces.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0.");

            RuleFor(x => x.Description)
                .MaximumLength(200).WithMessage("Description can’t exceed 200 characters.");
        }
    }
}
