using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Enums;

namespace Application.Contracts.Repositories;

public interface IMongoAuctionReadRepository
{
    Task<HttpStatusCode> AddAuction(Auction auction);
    Task<HttpStatusCode> UpdateAuction(Auction auction);
    Task<List<Auction>> GetAuctionsByUserId(Guid userId);
    Task<List<Auction>> GetAuctionsByStatus(AuctionStatus status);
    Task<List<Auction>> GetAuctionsByStatuses(List<AuctionStatus> statuses);
    Task<Auction> GetAuctionById(Guid id);
    Task<List<Auction>> GetAuctionsByUserIdAndStatus(Guid userId, AuctionStatus status);

}
