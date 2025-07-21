using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using MediatR;

namespace Application.Events.InternalEvents.Handlers;

public class AuctionUpdatedEventNotificationHandler : INotificationHandler<AuctionUpdatedEventNotification>
{
    private readonly ISendEndpointProvider _sendEndpointProvider;

    public AuctionUpdatedEventNotificationHandler(ISendEndpointProvider sendEndpointProvider)
    {
        _sendEndpointProvider = sendEndpointProvider;
    }
    public async Task Handle(AuctionUpdatedEventNotification notification, CancellationToken cancellationToken)
    {
        var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:auction_updated_queue"));
        await sendEndpoint.Send(notification, cancellationToken);
    }
}
