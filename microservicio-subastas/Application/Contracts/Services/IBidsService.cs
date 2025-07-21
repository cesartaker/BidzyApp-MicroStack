using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos.AuctionResults;

namespace Application.Contracts.Services;

public interface IBidsService
{
    Task<List<AuctionBidDto>> GetBidsByAuctionIdAsync(Guid auctionId);
    Task<List<AuctionBidDto>> GetBidsByAuctionIdsAsync(List<Guid> auctionIds);
}
