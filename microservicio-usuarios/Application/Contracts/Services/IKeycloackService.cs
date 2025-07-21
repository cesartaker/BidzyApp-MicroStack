using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;

namespace Application.Contracts.Services;

public interface IKeycloackService
{
   
    Task<string?> GetClientTokenAsync();
    Task<string?> GetTokenAsync();
    Task<string?> GetUserTokenAsync(string username,string password);
    Task<bool> ValidCredentials(string username, string password);

    Task<string?> GetAdminTokenAsync();
    Task<HttpStatusCode> CreateUserAsync(CreateUserDto request);
    Task<Guid> GetUserIdAsync(string email);
    Task<string?> GetCredentialIdAsync(string userId);
    Task<HttpStatusCode> DeleteCredentialById(string userId,string credentialId);
    Task<HttpStatusCode>UpdateUserPasswordAsync(string userId,string password);
    Task<Guid> GetRolIdAsync(string rolename);
    Task<HttpStatusCode> AssingRoleToUserAsync(Guid userId, Guid roleId, string roleName);
    Task<HttpStatusCode> SendVerifyEmailToUserAsync(Guid userId);
    Task<HttpStatusCode> SendEmailForResetPassword(Guid userId);
    Task<HttpStatusCode> DeleteUserAsync(Guid userId);
    Task<HttpStatusCode> AssingUserToGroupAsync(Guid userId, string groupName);
    Task<string?> GetGroupIdAsync(string groupName);


}
