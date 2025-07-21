using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;

public class Auction
{
    public Guid Id { get; set; }
    public Guid AuctioneerId { get; set; }
    public decimal ReservePrice { get; set; }
    public decimal MinBidStep { get; set; }
    public decimal BasePrice { get; set; }
    public string Status { get; set; }

    public Auction(Guid id,Guid auctioneerId, decimal reservePrice, decimal minBidStep, string status, decimal basePrice)
    {
        Id = id;
        AuctioneerId = auctioneerId;
        ReservePrice = reservePrice;
        MinBidStep = minBidStep;
        Status = status;
        BasePrice = basePrice;
    }
}
