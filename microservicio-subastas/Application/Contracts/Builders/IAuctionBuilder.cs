using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos.AuctionResults;
using Domain.Enums;

namespace Application.Contracts.Builders;

public interface IAuctionBuilder
{
    Task<List<AuctionResultDto>> GetAuctionsHistory(Guid userId, List<AuctionStatus> statuses);
}
