using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using MediatR;

namespace Application.Events.Handlers;

public class ComplaintUpdatedEventHandler:INotificationHandler<ComplaintUpdatedEvent>
{
    private readonly ISendEndpointProvider _sendEndpointProvider;

    public ComplaintUpdatedEventHandler(ISendEndpointProvider sendEndpointProvider)
    {
        _sendEndpointProvider = sendEndpointProvider;
    }
    /// <summary>
    /// Maneja el evento <see cref="ComplaintUpdatedEvent"/> enviando la información 
    /// del reclamo actualizado a la cola <c>complaint_updated_queue</c> para su procesamiento posterior.
    /// </summary>
    /// <param name="notification">Evento que contiene los datos del reclamo actualizado.</param>
    /// <param name="cancellationToken">Token que permite cancelar la operación asincrónica si es necesario.</param>
    public async Task Handle(ComplaintUpdatedEvent notification, CancellationToken cancellationToken)
    {
        var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:complaint_updated_queue"));
        await sendEndpoint.Send(notification.Complaint);
    }
}

