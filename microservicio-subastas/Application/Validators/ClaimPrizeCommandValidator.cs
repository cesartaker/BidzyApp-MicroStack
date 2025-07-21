using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Commands;
using Domain.Enums;
using FluentValidation;

namespace Application.Validators;

public class ClaimPrizeCommandValidator: AbstractValidator<ClaimPrizeCommand>
{
    public ClaimPrizeCommandValidator()
    {
        RuleFor(x => x.auctionId)
            .NotEmpty().WithMessage("Auction ID is required.")
            .NotNull().WithMessage("Auction ID is required");
        RuleFor(x => x.address)
            .NotEmpty().WithMessage("Winner ID is required.")
            .NotNull().WithMessage("Winner ID is required");
        RuleFor(x => x.receptorName)
            .NotEmpty().WithMessage("El nombre del receptor es obligatorio")
            .NotNull().WithMessage("El nombre del receptor es obligatorio");
        RuleFor(x => x.deliveryMethod)
            .Must(value => Enum.IsDefined(typeof(DeliveryMethod), value))
            .WithMessage("El método de entrega seleccionado no es válido.");
    }
}

