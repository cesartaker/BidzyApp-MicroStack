using System.Net;
namespace Application.Contracts.Services;

public interface INotificationService
{
    Task<HttpStatusCode> SendNotificationAsync(string email, string message, string subject);
}
