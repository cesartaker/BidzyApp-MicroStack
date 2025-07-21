using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.DTOs.MongoDTOs;

namespace Application.Contracts.Repositories;

public interface IMongoUserRepository
{
    Task<bool> AddUser(MongoUserDto user);
    Task<MongoRoleDto?> GetRole(string roleName);
    Task<MongoUserResponseDto?> GetUserByEmail(string email);
    Task<bool> UpdateUser(MongoUserDto user);
    Task<List<AuctionResultsUserDto>> GetAuctionResultsUserById(List<Guid> ids);
    Task<MongoUserDto> GetUserById(Guid id);

}
