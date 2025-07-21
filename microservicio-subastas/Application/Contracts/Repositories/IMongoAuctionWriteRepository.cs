using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Contracts.Repositories;

public interface IMongoAuctionWriteRepository
{
    Task<HttpStatusCode> AddAuction(Auction auction);
    Task<HttpStatusCode> UpdateAuction(Auction auction);
}
