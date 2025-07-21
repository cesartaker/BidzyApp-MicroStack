using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Contracts.Services;

public interface IEmailNotification
{
    Task Send(string recipient, string subject,string body);
    Task<bool> TrySendEmail(string email, string body, string subject, int maxRetries = 3, int delayMs = 2000);
}
