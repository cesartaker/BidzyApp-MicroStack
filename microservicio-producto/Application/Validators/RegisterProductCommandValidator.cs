using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Commands;
using FluentValidation;

namespace Application.Validators;

public class RegisterProductCommandValidator : AbstractValidator<RegisterProductCommand>
{
    public RegisterProductCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("El nombre del producto es obligatorio");
        RuleFor(x => x.Description).NotEmpty().WithMessage("El producto debe tener una descripción");
        RuleFor(x => x.BasePrice).NotEmpty().WithMessage("El precio base del producto es obligatorio");
        RuleFor(x => x.Category).NotEmpty().WithMessage("El producto debe tener una categoría");
        RuleFor(x => x.Image).NotNull()
            .WithMessage("La imagen es obligatoria")
            .Must(file => file.Length > 0).WithMessage("La imagen no puede estar vacía");
    }
}
