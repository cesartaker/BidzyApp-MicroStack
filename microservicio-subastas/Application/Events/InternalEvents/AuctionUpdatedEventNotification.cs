using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using MediatR;

namespace Application.Events.InternalEvents;

public class AuctionUpdatedEventNotification : INotification
{
    public Auction Auction { get; set; }

    public AuctionUpdatedEventNotification(Auction auction)
    {
        Auction = auction;
    }
}
