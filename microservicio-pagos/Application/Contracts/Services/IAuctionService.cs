using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contracts.Services;

public interface IAuctionService
{
    Task<HttpStatusCode> SetAuctionStatusAsCompleted(Guid? auctionId);
}
