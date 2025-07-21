
using Domain.Models;
using MassTransit;
using MediatR;

namespace Application.Events.InternalEvents.Handlers;

public class AuctionAddedEventNotificationHandler: INotificationHandler<AuctionAddedEventNotification>
{
    private readonly ISendEndpointProvider _sendEndpointProvider;

    public AuctionAddedEventNotificationHandler(ISendEndpointProvider sendEndpointProvider)
    {
        _sendEndpointProvider = sendEndpointProvider;
    }

    public async Task Handle(AuctionAddedEventNotification notification, CancellationToken cancellationToken)
    {
        

        var sendEndpoint1 = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:auction_added_queue"));
        var sendEndpoint2 = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:auction_added_bids_queue"));
        
        await sendEndpoint1.Send(notification,cancellationToken);
        await sendEndpoint2.Send(new AuctionIdOnly { AuctionId = notification.Auction.Id}, cancellationToken);
    }
}
