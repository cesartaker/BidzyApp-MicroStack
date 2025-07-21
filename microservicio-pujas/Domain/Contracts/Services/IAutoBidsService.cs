using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts.Services;

public interface IAutoBidsService
{
    Task RegisterAutoBid(Guid auctionId,Guid bidderId,string bidderName,decimal maxAmount,decimal minBidIncrease);
    Task ExecuteAutoBids(Guid auctionId);
}
