using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Contracts.Services;

public interface IBidsService
{
    Task<HttpStatusCode> AddBidAsync(Bid bid, Auction auction);
    Task<List<Bid>> GetBidsByAuctionIdAsync(Guid auctionId);
    Task<List<Bid>> GetBidsByAuctionIdsAsync(List<Guid> auctionIds);
    Task<Bid> GetHighestBidByAuctionIdAsync(Guid auctionId);

  
}
