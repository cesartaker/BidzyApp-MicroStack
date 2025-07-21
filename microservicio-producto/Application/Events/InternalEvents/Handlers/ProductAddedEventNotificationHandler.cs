using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using MediatR;

namespace Application.Events.InternalEvents.Handlers;

public class ProductAddedEventNotificationHandler: INotificationHandler<ProductAddedEventNotification>
{
    private readonly ISendEndpointProvider _sendEndpointProvider;

    public ProductAddedEventNotificationHandler(ISendEndpointProvider sendEndpointProvider)
    {
        _sendEndpointProvider = sendEndpointProvider;
    }
    /// <summary>
    /// Maneja el evento <see cref="ProductAddedEventNotification"/> enviando el producto agregado a la cola de mensajería configurada.
    /// Obtiene el endpoint de envío de mensajes y publica el producto en la cola especificada.
    /// </summary>
    /// <param name="notification">
    /// Notificación que contiene el producto recientemente agregado.
    /// </param>
    /// <param name="cancellationToken">
    /// Token para cancelar la operación de forma cooperativa si es necesario.
    /// </param>
    /// <returns>
    /// Una <see cref="Task"/> que representa la operación asincrónica de envío del mensaje.
    /// </returns>
    public async Task Handle(ProductAddedEventNotification notification, CancellationToken cancellationToken)
    {
        var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:products_queue"));
        await sendEndpoint.Send(notification.Product);
    }
}
