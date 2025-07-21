using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Contracts.Services;

public interface IUserAuditService
{
    Task AddToHistory(Guid userId, string description);
    Task<List<UserActivityHistory>> GetHistory(Guid id);
}
