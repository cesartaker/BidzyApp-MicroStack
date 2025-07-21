using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;
using Domain.Entities;
using Application.DTOs.MongoDTOs;
using System.Net;
using Application.DTOs;

namespace Application.Contracts.Services;

public interface IUserService
{
    Task<HttpStatusCode> AddUserAsync(User user);
    Task<HttpStatusCode> UpdateUserAsync(User user);
    Task<bool> UserExistAsync(string email);
    Task<MongoRoleDto?> GetRoleAsync(string roleName);
    Task<MongoUserResponseDto> GetUser(string email);
    Task sendEmailToUser(string recipient, string subject, string body);
    Task<List<AuctionResultsUserDto>> GetAuctionResultsUserByIdAsync (List<Guid> ids);
    Task<MongoUserDto> GetUserById(Guid id);
}
