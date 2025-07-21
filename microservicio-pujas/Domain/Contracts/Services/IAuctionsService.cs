using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;



namespace Domain.Contracts.Services;

public interface IAuctionsService
{
    Task<Auction?> GetAuction(Guid auctionId);
    Task<List<Auction>> GetActiveAuctions();
}
