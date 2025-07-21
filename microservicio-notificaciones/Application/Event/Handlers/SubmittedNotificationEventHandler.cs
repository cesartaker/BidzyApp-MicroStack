using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using MediatR;

namespace Application.Event.Handlers;

public class SubmittedNotificationEventHandler : INotificationHandler<SubmittedNotificationEvent>
{
    private readonly ISendEndpointProvider _sendEndpointProvider;

    public SubmittedNotificationEventHandler(ISendEndpointProvider sendEndpointProvider)
    {
        _sendEndpointProvider = sendEndpointProvider ?? throw new ArgumentNullException(nameof(sendEndpointProvider), "SendEndpointProvider cannot be null");
    }
    /// <summary>
    /// Envía un mensaje a rabbit para el registro de una notificación en la base de datos de lectura.
    /// </summary>
    /// <param name="notification">Instancia de una notificación.</param>
    public async Task Handle(SubmittedNotificationEvent notification, CancellationToken cancellationToken)
    {
        var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:new_notification_queue"));
        await sendEndpoint.Send(notification.Notification, cancellationToken);
    }
}
