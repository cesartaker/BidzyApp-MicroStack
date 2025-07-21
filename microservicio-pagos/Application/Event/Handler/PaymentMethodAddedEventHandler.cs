using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using MediatR;

namespace Application.Event.Handler;

public class PaymentMethodAddedEventHandler: INotificationHandler<PaymentMethodAddedEvent>
{
    public readonly ISendEndpointProvider _sendEndpointProvider;

    public PaymentMethodAddedEventHandler(ISendEndpointProvider sendEndpointProvider)
    {
        _sendEndpointProvider = sendEndpointProvider;
    }
    /// <summary>
    /// Maneja el evento <see cref="PaymentMethodAddedEvent"/> publicando la información del nuevo método de pago en una cola de mensajes externa.
    /// Obtiene dinámicamente el endpoint para la cola <c>"payment_method_added_queue"</c> utilizando <see cref="_sendEndpointProvider"/>,
    /// y transmite el objeto <c>UserPaymentMethod</c> incluido en la notificación.
    /// </summary>
    /// <param name="notification">
    /// Evento <see cref="PaymentMethodAddedEvent"/> que contiene los datos del método de pago agregado por el usuario.
    /// </param>
    /// <param name="cancellationToken">
    /// Token de cancelación para controlar la finalización anticipada de la operación asincrónica si es necesario.
    /// </param>
    /// <returns>
    /// Una <see cref="Task"/> que representa la operación de envío del mensaje al sistema de mensajería.
    /// </returns>
    public async Task Handle(PaymentMethodAddedEvent notification, CancellationToken cancellationToken)
    {

        var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:payment_method_added_queue"));
        await sendEndpoint.Send(notification.UserPaymentMethod);
    }
}
