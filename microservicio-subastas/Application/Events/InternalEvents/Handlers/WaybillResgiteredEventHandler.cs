using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using MediatR;

namespace Application.Events.InternalEvents.Handlers;

public class WaybillResgiteredEventHandler : INotificationHandler<WaybillRegisteredEvent>
{
    private readonly ISendEndpointProvider _sendEndpointProvider;

    public WaybillResgiteredEventHandler(ISendEndpointProvider sendEndpointProvider)
    {
        _sendEndpointProvider = sendEndpointProvider;
    }
    public async Task Handle(WaybillRegisteredEvent notification, CancellationToken cancellationToken)
    {
        var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:waybill_added_queue"));
        await sendEndpoint.Send(notification.Waybill, cancellationToken);
    }
}
