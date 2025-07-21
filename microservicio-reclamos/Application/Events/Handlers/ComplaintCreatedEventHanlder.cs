using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using MediatR;

namespace Application.Events.Handlers;

public class ComplaintCreatedEventHanlder : INotificationHandler<ComplaintCreatedEvent>
{
    private readonly ISendEndpointProvider _sendEndpointProvider;

    public ComplaintCreatedEventHanlder(ISendEndpointProvider sendEndpointProvider)
    {
        _sendEndpointProvider = sendEndpointProvider;
    }
    /// <summary>
    /// Maneja el evento <see cref="ComplaintCreatedEvent"/> publicando el objeto del reclamo
    /// en la cola de mensajería correspondiente para su procesamiento asíncrono.
    /// </summary>
    /// <param name="notification">Evento que contiene la información del reclamo creado.</param>
    /// <param name="cancellationToken">Token que permite cancelar la operación si es necesario.</param>
    public async Task Handle(ComplaintCreatedEvent notification, CancellationToken cancellationToken)
    {
        var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:complaints_queue"));
        await sendEndpoint.Send(notification.Complaint);
    }
}
