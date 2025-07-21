using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Commands;
using FluentValidation;

namespace Application.Validators;

public class SendEmailNotificationCommandValidator: AbstractValidator<SendEmailNotificationCommand>
{
    public SendEmailNotificationCommandValidator()
    {
        RuleFor(x => x.email).NotEmpty().EmailAddress();
        RuleFor(x => x.message).NotEmpty().NotEmpty();
        RuleFor(x => x.subject).NotEmpty().NotEmpty();
    }
}

