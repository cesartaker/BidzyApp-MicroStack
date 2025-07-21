using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using MediatR;

namespace Application.Events.InternalEvents;

public class BidAddedEventNotification: INotification
{
    public Bid Bid { get; set; }

    public BidAddedEventNotification(Bid bid)
    {
        Bid = bid;
    }
}
