using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Contracts.Services;

public interface INotificationService
{
    Task<Notification?> SendEmailNotification(string email,string message, string subject);
    Task<Notification?> SaveNotificationAsync(string email, string message, string subject);
}
