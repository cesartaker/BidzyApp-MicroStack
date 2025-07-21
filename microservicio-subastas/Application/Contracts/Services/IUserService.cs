using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos.AuctionResults;

namespace Application.Contracts.Services;

public interface IUserService
{
    Task<List<AuctionUserDto>> GetAuctionUserInformationByIds(List<Guid?> userIds);
}
