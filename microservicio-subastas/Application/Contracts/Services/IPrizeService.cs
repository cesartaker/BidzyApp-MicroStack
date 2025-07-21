using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos;
using Domain.Entities;

namespace Application.Contracts.Services;

public interface IPrizeService
{
    Task<List<PrizeDto>> GetPrizesAsync(Guid userId);
    Task AddWaybillAsync(Waybill waybill);
}
