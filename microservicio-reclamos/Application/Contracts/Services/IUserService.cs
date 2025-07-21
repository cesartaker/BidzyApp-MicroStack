
using Application.Dtos;

namespace Application.Contracts.Services;

public interface IUserService
{
    Task<List<AuctionUserDto>> GetAuctionUserInformationByIds(List<Guid?> userIds);
}
