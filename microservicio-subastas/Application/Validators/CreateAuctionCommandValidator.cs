using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Commands;
using FluentValidation;

namespace Application.Validators;

public class CreateAuctionCommandValidator: AbstractValidator<CreateAuctionCommand>
{
    public CreateAuctionCommandValidator()
    {
        RuleFor(x => x.userId)
            .NotEmpty().WithMessage("User ID is required.")
            .NotNull().WithMessage("User ID is required");
        RuleFor(x => x.productId)
            .NotEmpty().WithMessage("product ID is required.")
            .NotNull().WithMessage("product ID is required");
        RuleFor(x => x.name)
            .NotEmpty().WithMessage("Name can't be empty.")
            .NotNull().WithMessage("Name is required");
        RuleFor(x => x.description).NotEmpty().WithMessage("Description can't be empty.");
        RuleFor(x => x.basePrice)
            .GreaterThan(0).WithMessage("Base price must be greater than zero.");
        RuleFor(x => x.reservePrice)
            .GreaterThan(0).WithMessage("Reserve price must be greater than zero.")
            .GreaterThanOrEqualTo(x => x.basePrice).WithMessage("Reserve price must be greater than or equal to base price.");
        RuleFor(x => x.endDate).NotEmpty().WithMessage("End date is required.")
            .GreaterThan(DateTime.Now).WithMessage("End date must be in the future.");
        RuleFor(x => x.minBidStep)
            .GreaterThan(0).WithMessage("Minimum bid step must be greater than zero.");
        RuleFor(x => x.imageUrl).NotEmpty().WithMessage("Image URL is required.");
            
    }
}
