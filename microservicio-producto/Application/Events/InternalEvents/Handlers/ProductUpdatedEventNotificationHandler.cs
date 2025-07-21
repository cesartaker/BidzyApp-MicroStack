using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using MediatR;

namespace Application.Events.InternalEvents.Handlers;

internal class ProductUpdatedEventNotificationHandler : INotificationHandler<ProductUpdatedEventNotification>
{
    private readonly ISendEndpointProvider _sendEndpointProvider;

    public ProductUpdatedEventNotificationHandler(ISendEndpointProvider sendEndpointProvider)
    {
        _sendEndpointProvider = sendEndpointProvider;
    }
    /// <summary>
    /// Maneja el evento <see cref="ProductUpdatedEventNotification"/> enviando el producto actualizado
    /// a la cola de mensajería <c>updated_product_status_queue</c>.
    /// Obtiene el endpoint de envío configurado y publica el producto como mensaje en la cola.
    /// </summary>
    /// <param name="notification">
    /// Notificación que contiene el producto cuya información ha sido actualizada.
    /// </param>
    /// <param name="cancellationToken">
    /// Token para cancelar la operación de forma cooperativa si fuera necesario.
    /// </param>
    /// <returns>
    /// Una <see cref="Task"/> que representa la operación asincrónica de envío del producto actualizado.
    /// </returns>
    public async Task Handle(ProductUpdatedEventNotification notification, CancellationToken cancellationToken)
    {
        var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:updated_product_status_queue"));
        await sendEndpoint.Send(notification.Product);
    }
}
