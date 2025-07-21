using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Event;
using Domain.Contracts.Services;
using FluentValidation;
using MediatR;
using Microsoft.IdentityModel.Tokens;

namespace Application.Commands.Handlers;

public class SendEmailNotificationCommandHandler : IRequestHandler<SendEmailNotificationCommand, Unit>
{
    private readonly IValidator<SendEmailNotificationCommand> _validator;
    private readonly IMediator _mediator;
    private readonly INotificationService _notificationService;

    public SendEmailNotificationCommandHandler(IValidator<SendEmailNotificationCommand> validator, IMediator mediator,
        INotificationService notificationService)
    {
        _validator = validator;
        _mediator = mediator;
        _notificationService = notificationService;
    }
    /// <summary>
    /// Handler para el envío de notificaciones de correo electrónico.
    /// </summary>
    /// <param name="request">Descripción del parámetro.</param>
    /// <returns>Descripción del valor de retorno.</returns>
    public async Task<Unit> Handle(SendEmailNotificationCommand request, CancellationToken cancellationToken)
    {
        var validationResult = _validator.Validate(request);
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            throw new ValidationException(string.Join(", ", errors));
        }

        var notification = await _notificationService.SendEmailNotification(request.email,request.message,request.subject);

        if(notification != null)
        {
            await _mediator.Publish(new SubmittedNotificationEvent(notification), cancellationToken);
        }

        return Unit.Value;
    }
}
