using System.Text.Json;
using Domain.Contracts.Services;
using Domain.Entities;
using Microsoft.Extensions.Options;
using RestSharp;


namespace Infrastructure.ExternalServices.AuctionService;

public class AuctionServiceClient: IAuctionsService
{
    private readonly RestClient _client;

    public AuctionServiceClient(IOptions<AuctionServiceOptions> options)
    {
        _client = new RestClient(new RestClientOptions(options.Value.BaseUrl));
    }
    /// <summary>
    /// Obtiene la lista de subastas activas desde un servicio externo mediante una solicitud HTTP tipo <c>GET</c> al endpoint <c>active</c>.
    /// Valida la respuesta, deserializa el contenido en una colección de <see cref="AuctionsDto"/>, y transforma cada elemento en una instancia de <see cref="Auction"/>.
    /// Si la respuesta falla o no contiene datos válidos, se retorna una lista vacía.
    /// </summary>
    /// <returns>
    /// Una <see cref="Task{List{Auction}}"/> que representa la operación asincrónica:
    /// devuelve una lista de objetos <see cref="Auction"/> con los datos obtenidos del servicio externo,
    /// o una lista vacía en caso de error o ausencia de subastas activas.
    /// </returns>
    public async Task<List<Auction>> GetActiveAuctions()
    {
        var request = new RestRequest("active", Method.Get);
        var response = await _client.ExecuteAsync(request);

        if (!response.IsSuccessful || string.IsNullOrWhiteSpace(response.Content))
            return new List<Auction>();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var auctionsDto = JsonSerializer.Deserialize<AuctionsDto>(response.Content, options);

        if (auctionsDto?.Auctions == null || auctionsDto.Auctions.Count == 0)
            return new List<Auction>();

        return auctionsDto.Auctions
            .Select(dto => new Auction(
                dto.Id,
                dto.UserId,
                dto.ReservePrice,
                dto.MinBidStep,
                dto.Status,
                dto.BasePrice))
            .ToList();
    }
    /// <summary>
    /// Obtiene los datos de una subasta específica mediante su identificador único.
    /// Realiza una solicitud <c>GET</c> al endpoint <c>active</c> para recuperar la lista de subastas activas.
    /// Filtra la lista buscando la subasta cuyo <paramref name="auctionId"/> coincida con el ID solicitado.
    /// Si se encuentra, construye y retorna una instancia de <see cref="Auction"/> con sus datos; en caso contrario, retorna <c>null</c>.
    /// </summary>
    /// <param name="auctionId">
    /// Identificador único de la subasta (<see cref="Guid"/>) que se desea recuperar.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{Auction}"/> que representa la operación asincrónica:
    /// retorna la subasta encontrada como una instancia de <see cref="Auction"/> o <c>null</c> si no se encuentra o si la solicitud falla.
    /// </returns>
    public async Task<Auction?> GetAuction(Guid auctionId)
    {
        var request = new RestRequest($"active", Method.Get);
        var response = await _client.ExecuteAsync<AuctionsDto>(request);

        if (!response.IsSuccessful || response.Data == null)
            return null;

        // Buscar la subasta con el ID solicitado
        var auction = response.Data.Auctions.FirstOrDefault(auction => auction.Id == auctionId);
        if (auction == null)
            return null;

        return new Auction(auction.Id,auction.UserId, auction.ReservePrice,auction.MinBidStep,auction.Status,auction.BasePrice);
    }
}
