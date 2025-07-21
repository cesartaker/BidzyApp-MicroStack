using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos;

public class AutoBid
{
    public Guid BidderId { get; set; } 
    public string BidderName { get; set; } = string.Empty;
    public decimal MaxAmount { get; set; }
    public decimal MinBidIncrease { get; set; }

    public AutoBid(Guid bidderId,string bidderName, decimal maxAmount, decimal minBidIncrease)
    {
        BidderId = bidderId;
        BidderName = bidderName;
        MaxAmount = maxAmount;
        MinBidIncrease = minBidIncrease;
    }
}
