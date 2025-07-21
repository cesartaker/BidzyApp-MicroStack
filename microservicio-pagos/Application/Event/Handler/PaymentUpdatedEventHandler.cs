using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using MediatR;

namespace Application.Event.Handler;

public class PaymentUpdatedEventHandler : INotificationHandler<PaymentUpdatedEvent>
{
    private readonly ISendEndpointProvider _sendEndpointProvider;

    public PaymentUpdatedEventHandler(ISendEndpointProvider sendEndpointProvider)
    {
        _sendEndpointProvider = sendEndpointProvider;
    }
    /// <summary>
    /// Maneja el evento <see cref="PaymentUpdatedEvent"/> enviando la información del pago actualizado
    /// a una cola de mensajes externa denominada <c>"update_payments_queue"</c>.
    /// Utiliza <see cref="_sendEndpointProvider"/> para obtener el endpoint de envío y propaga los datos mediante <c>Send</c>.
    /// </summary>
    /// <param name="notification">
    /// Evento <see cref="PaymentUpdatedEvent"/> que contiene el objeto <c>Payment</c> con los datos actualizados.
    /// </param>
    /// <param name="cancellationToken">
    /// Token de cancelación que permite abortar la operación asincrónica si el sistema lo requiere.
    /// </param>
    /// <returns>
    /// Una <see cref="Task"/> que representa la operación de envío del mensaje al sistema de mensajería.
    /// </returns>
    public async Task Handle(PaymentUpdatedEvent notification, CancellationToken cancellationToken)
    {
        var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:update_payments_queue"));
        await sendEndpoint.Send(notification.Payment);
    }
}
