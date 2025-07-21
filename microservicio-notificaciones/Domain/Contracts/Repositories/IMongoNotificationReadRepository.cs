using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Contracts.Repositories;

public interface IMongoNotificationReadRepository
{
    Task<Notification?> AddNotification(Notification notification);
}
