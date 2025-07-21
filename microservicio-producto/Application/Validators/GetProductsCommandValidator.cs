using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Commands;
using Domain.Enums;
using FluentValidation;

namespace Application.Validators;

public class GetProductsCommandValidator: AbstractValidator<GetProductsCommand>
{
    public GetProductsCommandValidator()
    {
        RuleFor(x => x.auctioneerId)
            .NotEmpty()
            .WithMessage("El ID del subastador es obligatorio")
            .Must(value => value != Guid.Empty)
            .WithMessage("El ID del subastador no puede ser un GUID vacío");
        
        RuleFor(x => x.status)
            .NotEmpty()
            .Must(value => Enum.TryParse<ProductStatus>(value, out _))
            .WithMessage("El status debe ser un valor válido dentro de ProductStatus");
    }
}
