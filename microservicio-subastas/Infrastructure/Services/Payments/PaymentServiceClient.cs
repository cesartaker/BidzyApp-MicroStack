using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Contracts.Services;
using Microsoft.Extensions.Options;
using RestSharp;

namespace Infrastructure.Services.Payments;

public class PaymentServiceClient : IPaymentService
{
    private readonly RestClient _client;
    public PaymentServiceClient(IOptions<PaymentServiceOptions> options)
    {
        _client = new RestClient(new RestClientOptions(options.Value.BaseUrl));
    }
    /// <summary>
    /// Crea una solicitud de pago para una subasta específica, asociada a un usuario y por un monto determinado.
    /// Envía una petición HTTP tipo <c>POST</c> al endpoint <c>create</c> con los datos requeridos en formato JSON.
    /// Evalúa la respuesta del servicio y retorna el código de estado HTTP que indica el resultado.
    /// </summary>
    /// <param name="auctionId">
    /// Identificador único de la subasta (<see cref="Guid"/>) sobre la cual se desea generar el pago.
    /// </param>
    /// <param name="userId">
    /// Identificador único del usuario (<see cref="Guid"/>) que realizará el pago.
    /// </param>
    /// <param name="amount">
    /// Monto monetario a pagar (<see cref="decimal"/>), correspondiente a la subasta indicada.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{HttpStatusCode}"/> que representa la operación asincrónica:
    /// retorna <c>OK</c> si la solicitud de pago se procesó con éxito,
    /// o el código de estado recibido del servicio externo en caso contrario.
    /// </returns>
    public async Task<HttpStatusCode> CreatePayment(Guid auctionId, Guid userId, decimal amount)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var request = new RestRequest("create", Method.Post);
        request.AddJsonBody(new { auctionId,userId,amount });
        var response = await _client.ExecuteAsync(request);
        if (response.IsSuccessful)
        {
            return HttpStatusCode.OK;
        }
        else
        {
            return response.StatusCode;
        }
    }
}
