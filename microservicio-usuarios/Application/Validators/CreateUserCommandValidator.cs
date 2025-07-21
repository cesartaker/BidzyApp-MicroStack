using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Commands;
using FluentValidation;
using Domain.Enums;

namespace Application.Validators;

public class CreateUserCommandValidator: AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator() {

        RuleFor(x => x.FirstName).NotEmpty().WithMessage("El nombre es obligatorio");
        RuleFor(x => x.LastName).NotEmpty().WithMessage("El apellido es obligatorio");
        RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("El correo debe ser válido");
        RuleFor(x => x.PhoneNumber).NotEmpty().Matches(@"^\+58\d{10}$").WithMessage("El teléfono debe seguir el formato venezolano");
        RuleFor(x => x.Address).NotEmpty().WithMessage("La dirección es obligatoria");
        RuleFor(x => x.rolName)
            .NotEmpty()
            .Must(value => Enum.TryParse<RoleNames>(value, out _))
            .WithMessage("El rol debe ser un valor válido dentro de RoleNames");
        RuleFor(x => x.password)
            .NotEmpty().WithMessage("La contraseña no puede estar vacía")
            .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres")
            .Matches(@"[A-Z]").WithMessage("La contraseña debe contener al menos una letra mayúscula")
            .Matches(@"[a-z]").WithMessage("La contraseña debe contener al menos una letra minúscula")
            .Matches(@"[\w]").WithMessage("La contraseña debe contener al menos un caracter especial (@, $, %)");

    }
}

