using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Contracts.Repositories;

public interface IMongoBidsReadRepository
{
    Task<Bid> GetHighestBidByAuctionIdAsync(Guid auctionId);
    Task<HttpStatusCode> AddBidAsync(Bid bid);
    Task<List<Bid>> GetBidsByAuctionId(Guid auctionId);
    Task<List<Bid>> GetBidsByAuctionIdsAsync(List<Guid> auctionIds);


}
