using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos;
using MediatR;

namespace Application.Commands;

public record NewBidCommand : IRequest<BidResponseDto?>
{
    public Guid AuctionId { get; set; }
    public Guid BidderId { get; set; } 
    public decimal Amount { get; init; }
    public string bidderName { get; set; }
}

