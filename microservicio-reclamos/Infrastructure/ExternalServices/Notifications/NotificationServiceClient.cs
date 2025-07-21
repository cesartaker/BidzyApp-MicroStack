
using System.Net;
using System.Text.Json;
using Application.Contracts.Services;
using Microsoft.Extensions.Options;
using RestSharp;

namespace Infrastructure.ExternalServices.Notifications;

public class NotificationServiceClient : INotificationService
{
    private readonly RestClient _client;

    public NotificationServiceClient(IOptions<NotificationServiceOptions> options)
    {
        _client = new RestClient(new RestClientOptions(options.Value.BaseUrl));
    }
    /// <summary>
    /// Envía una notificación por correo electrónico haciendo una solicitud HTTP POST 
    /// al endpoint externo encargado de enviar correos.
    /// </summary>
    /// <param name="email">Dirección de correo electrónico del destinatario.</param>
    /// <param name="message">Cuerpo del mensaje que se incluirá en el correo.</param>
    /// <param name="subject">Asunto del correo electrónico.</param>
    /// <returns>
    /// Un valor <see cref="HttpStatusCode"/> que indica el resultado de la operación de envío.
    /// </returns>
    public async Task<HttpStatusCode> SendNotificationAsync(string email,string message, string subject)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var request = new RestRequest("auction/send/email", Method.Post);
        request.AddJsonBody(new
        {
            email,
            message,
            subject
        });

        var response = await _client.ExecuteAsync(request);
        return response.StatusCode;
   
    }
}
