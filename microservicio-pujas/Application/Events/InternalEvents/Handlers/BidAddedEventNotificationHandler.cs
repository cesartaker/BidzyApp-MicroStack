using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using MediatR;

namespace Application.Events.InternalEvents.Handlers;

public class BidAddedEventNotificationHandler : INotificationHandler<BidAddedEventNotification>
{
    private readonly ISendEndpointProvider _sendEndpointProvider;

    public BidAddedEventNotificationHandler(ISendEndpointProvider sendEndpointProvider)
    {
        _sendEndpointProvider = sendEndpointProvider;
    }
    /// <summary>
    /// Maneja el evento <see cref="BidAddedEventNotification"/> publicado cuando se registra una nueva puja.
    /// Obtiene el endpoint de envío hacia la cola <c>bid-added-queue</c> y transmite el evento para continuar el flujo de procesamiento.
    /// </summary>
    /// <param name="notification">
    /// Evento <see cref="BidAddedEventNotification"/> que contiene los datos de la puja recientemente registrada.
    /// </param>
    /// <param name="cancellationToken">
    /// Token que permite cancelar la operación asincrónica si el sistema lo requiere.
    /// </param>
    public async Task Handle(BidAddedEventNotification notification, CancellationToken cancellationToken)
    {
        var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:bid-added-queue"));
        await sendEndpoint.Send(notification, cancellationToken);
    }
}
