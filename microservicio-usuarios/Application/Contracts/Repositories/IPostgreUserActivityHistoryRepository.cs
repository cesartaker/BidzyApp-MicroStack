using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Contracts.Repositories;

public interface IPostgreUserActivityHistoryRepository
{
    void Add(UserActivityHistory activity);
    Task<List<UserActivityHistory>> GetByUserIdAsync(Guid userId);
}
