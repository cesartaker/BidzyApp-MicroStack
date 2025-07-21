using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using MediatR;

namespace Application.Event.Handler;

public class PaymentCreatedEventHandler : INotificationHandler<PaymentCreatedEvent>
{
    private readonly ISendEndpointProvider _sendEndpointProvider;

    public PaymentCreatedEventHandler(ISendEndpointProvider sendEndpointProvider)
    {
        _sendEndpointProvider = sendEndpointProvider;
    }
    /// <summary>
    /// Maneja el evento <see cref="PaymentCreatedEvent"/> publicando la información del pago en una cola de mensajes externa.
    /// Obtiene dinámicamente el endpoint de envío para la cola <c>"new_payment_queue"</c>
    /// y transmite el objeto <c>Payment</c> contenido en la notificación.
    /// </summary>
    /// <param name="notification">
    /// Evento <see cref="PaymentCreatedEvent"/> que encapsula los datos del pago recién creado.
    /// </param>
    /// <param name="cancellationToken">
    /// Token de cancelación para controlar la finalización anticipada de la operación asincrónica.
    /// </param>
    /// <returns>
    /// Una <see cref="Task"/> que representa la operación de envío del mensaje al sistema de mensajería.
    /// </returns>
    public async Task Handle(PaymentCreatedEvent notification, CancellationToken cancellationToken)
    {
        var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:new_payment_queue"));
        await sendEndpoint.Send(notification.Payment);
    }
}
