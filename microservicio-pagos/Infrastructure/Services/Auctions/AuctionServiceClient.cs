using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts.Services;
using Microsoft.Extensions.Options;
using RestSharp;

namespace Infrastructure.Services.Auctions;

public class AuctionServiceClient: IAuctionService
{
    private readonly RestClient _client;
    public AuctionServiceClient(IOptions<AuctionServiceOptions> options)
    {
        _client = new RestClient(new RestClientOptions(options.Value.BaseUrl));
    }
    /// <summary>
    /// Establece el estado de una subasta como completado enviando una solicitud HTTP POST al endpoint <c>"set-status/completed"</c>.
    /// Incluye el identificador de la subasta en el cuerpo de la petición JSON.
    /// Evalúa la respuesta del servidor y retorna el <see cref="HttpStatusCode"/> correspondiente: 
    /// el estado recibido si la operación fue exitosa, o <see cref="HttpStatusCode.InternalServerError"/> si ocurrió un fallo.
    /// </summary>
    /// <param name="auctionId">
    /// Identificador único de la subasta (<see cref="Guid"/>) que se desea marcar como completada.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{HttpStatusCode}"/> que representa la operación asincrónica:
    /// retorna el código de estado HTTP que indica el resultado del intento de actualización.
    /// </returns>
    public async Task<HttpStatusCode> SetAuctionStatusAsCompleted(Guid? auctionId)
    {
        var request = new RestRequest("set-status/completed", Method.Post);
        request.AddJsonBody(new { auctionId });
        var response = await _client.ExecuteAsync(request);


        if (response.IsSuccessful)
        {
            return response.StatusCode;
        }
        else
        {
            return HttpStatusCode.InternalServerError;
        }
    }
}
