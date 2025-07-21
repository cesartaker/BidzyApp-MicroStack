using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Commands;
using FluentValidation;

namespace Application.Validators;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .Matches(@"^\+58\d{10}$")
            .WithMessage("El teléfono debe seguir el formato venezolano")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber)); // Solo valida si hay un número

    }
}
