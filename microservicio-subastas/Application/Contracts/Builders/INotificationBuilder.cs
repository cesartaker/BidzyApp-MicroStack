using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos.NotificationQuery;

namespace Application.Contracts.Builders;

public interface INotificationBuilder
{
    Task<NotificationDto> BuildNotificationAsync(Guid auctionId);
}
