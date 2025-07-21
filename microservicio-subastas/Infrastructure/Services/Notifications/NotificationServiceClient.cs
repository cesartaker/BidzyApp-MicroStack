using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Contracts.Services;
using Infrastructure.Services.Products;
using Microsoft.Extensions.Options;
using RestSharp;

namespace Infrastructure.Services.Notifications;

public class NotificationServiceClient : INotificationService
{
    private readonly RestClient _client;

    public NotificationServiceClient(IOptions<NotificationServiceOptions> options)
    {
        _client = new RestClient(new RestClientOptions(options.Value.BaseUrl));
    }
    /// <summary>
    /// Envía una notificación por correo electrónico utilizando un servicio HTTP externo.
    /// Construye una solicitud <c>POST</c> al endpoint <c>auction/send/email</c> con los datos proporcionados,
    /// y devuelve el código de estado HTTP que indica el resultado de la operación.
    /// </summary>
    /// <param name="email">
    /// Dirección de correo electrónico del destinatario.
    /// </param>
    /// <param name="message">
    /// Contenido del mensaje que se desea enviar.
    /// </param>
    /// <param name="subject">
    /// Asunto del correo electrónico.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{HttpStatusCode}"/> que representa la operación asincrónica:
    /// devuelve el código HTTP correspondiente a la respuesta del servicio de envío.
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
