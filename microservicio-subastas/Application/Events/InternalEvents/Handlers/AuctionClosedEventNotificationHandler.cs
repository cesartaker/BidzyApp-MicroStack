using Domain.Models;
using MassTransit;
using MediatR;

namespace Application.Events.InternalEvents.Handlers;

public class AuctionClosedEventNotificationHandler : INotificationHandler<AuctionClosedEventNotification>
{
    private readonly ISendEndpointProvider _sendEndpointProvider;

    public AuctionClosedEventNotificationHandler(ISendEndpointProvider sendEndpointProvider)
    {
        _sendEndpointProvider = sendEndpointProvider;
    }
    public async Task Handle(AuctionClosedEventNotification notification, CancellationToken cancellationToken)
    {
        var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:auction_closed_queue"));
        await sendEndpoint.Send(new AuctionIdOnly { AuctionId = notification.AuctionId},cancellationToken);
    }
}
