using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Contracts.Services;
using Application.Dtos.AuctionResults;
using Domain.Entities;
using Microsoft.Extensions.Options;
using RestSharp;

namespace Infrastructure.Services.Bids;

public class BidsServiceClient : IBidsService
{
    private readonly RestClient _client;
    public BidsServiceClient(IOptions<BidsServiceOptions> options)
    {
        _client = new RestClient(new RestClientOptions(options.Value.BaseUrl));
    }
    /// <summary>
    /// Recupera todas las pujas realizadas en una subasta específica, utilizando el identificador de subasta.
    /// Envía una solicitud HTTP al servicio externo, espera la respuesta y deserializa el contenido JSON en una lista de <see cref="AuctionBidDto"/>.
    /// </summary>
    /// <param name="auctionId">
    /// Identificador único de la subasta (<see cref="Guid"/>) de la cual se desean obtener las pujas.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{List{AuctionBidDto}}"/> que representa la operación asincrónica:
    /// devuelve una lista de objetos <see cref="AuctionBidDto"/> si la respuesta es exitosa y contiene datos válidos;
    /// de lo contrario, retorna una lista vacía.
    /// </returns>
    public async Task<List<AuctionBidDto>> GetBidsByAuctionIdAsync(Guid auctionId)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var request = new RestRequest("", Method.Post);
        request.AddJsonBody(new { auctionId });
        
        var response = await _client.ExecuteAsync(request);
        

        if (!response.IsSuccessful || string.IsNullOrWhiteSpace(response.Content))
            return new List<AuctionBidDto>();
        
        var bids = JsonSerializer.Deserialize<List<AuctionBidDto>>(response.Content, options);
        
        if (bids == null || bids.Count == 0)
            return new List<AuctionBidDto>();
        
        return bids;
    }
    /// <summary>
    /// Recupera todas las pujas realizadas en múltiples subastas, identificadas por sus respectivos identificadores.
    /// Envía una solicitud HTTP tipo POST al endpoint <c>batch</c>, incluyendo el conjunto de IDs en el cuerpo JSON.
    /// Valida la respuesta antes de deserializarla como una lista de <see cref="AuctionBidDto"/>.
    /// </summary>
    /// <param name="auctionIds">
    /// Lista de identificadores únicos (<see cref="Guid"/>) de las subastas que se desean consultar.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{List{AuctionBidDto}}"/> que representa la operación asincrónica:
    /// retorna una colección con las pujas si la respuesta es exitosa y contiene datos válidos.
    /// Si la respuesta falla o no hay datos, se retorna una lista vacía.
    /// </returns>
    public async Task<List<AuctionBidDto>> GetBidsByAuctionIdsAsync(List<Guid> auctionIds)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var request = new RestRequest("batch", Method.Post);
        request.AddJsonBody(new { auctionIds });

        var response = await _client.ExecuteAsync(request);


        if (!response.IsSuccessful || string.IsNullOrWhiteSpace(response.Content))
            return new List<AuctionBidDto>();

        var bids = JsonSerializer.Deserialize<List<AuctionBidDto>>(response.Content, options);

        if (bids == null || bids.Count == 0)
            return new List<AuctionBidDto>();

        return bids;
    }
}
