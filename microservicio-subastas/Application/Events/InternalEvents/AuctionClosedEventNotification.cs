using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;
using MediatR;

namespace Application.Events.InternalEvents;

public class AuctionClosedEventNotification: INotification
{
    public Guid AuctionId { get; set; }

    public AuctionClosedEventNotification(Guid auctionId)
    {
        AuctionId = auctionId;
    }
}
