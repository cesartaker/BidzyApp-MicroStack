using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos;
using Domain.Entities;
using Domain.Enums;

namespace Application.Contracts.Services;

public interface IAuctionService
{
    Task<HttpStatusCode> RegisterAuction(Auction auction);
    Task<List<Auction>> GetMyAuctionsAsync(Guid userId);
    Task<List<Auction>> GetAuctionsAsync(AuctionStatus status);
    Task<Auction> GetAuctionByIdAsync(Guid auctionId);
    Task<Auction> CloseAuctionAsync(Guid auctionId,AuctionStatus status, Guid winnerId);
    Task<HttpStatusCode> UpdateAuctionAsync(Auction auction);
    AuctionStatus DetermineAuctionStatus(decimal reservePrice, decimal highestBid);
    Task<List<Auction>> GetAuctionsByStatusesAsync(List<AuctionStatus> statuses);
    Task<List<Auction>> GetAuctionsByUserIdAndStatus(Guid userId,AuctionStatus status);

}
