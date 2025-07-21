using System.Text.Json;
using Application.Contracts.Services;
using Application.Dtos;
using Microsoft.Extensions.Options;
using RestSharp;

namespace Infrastructure.ExternalServices.Users;

public class UsersServiceClient : IUserService
{
    private readonly RestClient _client;

    public UsersServiceClient(IOptions<UsersServiceOptions> options)
    {
        _client = new RestClient(new RestClientOptions(options.Value.BaseUrl));
    }
    /// <summary>
    /// Obtiene la información de múltiples usuarios del sistema de subastas,
    /// realizando una solicitud HTTP POST al endpoint <c>batch</c> con sus identificadores,
    /// y deserializa la respuesta en una lista de objetos <see cref="AuctionUserDto"/>.
    /// </summary>
    /// <param name="usersIds">Lista de identificadores de usuario (pueden incluir valores nulos).</param>
    /// <returns>
    /// Una lista de objetos <see cref="AuctionUserDto"/> que contienen los datos de los usuarios solicitados.
    /// Si la respuesta es vacía o la solicitud falla, se devuelve una lista vacía.
    /// </returns>
    public async Task<List<AuctionUserDto>> GetAuctionUserInformationByIds(List<Guid?> usersIds)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var request = new RestRequest("batch", Method.Post);
        request.AddJsonBody(new {usersIds});
        var response = await _client.ExecuteAsync(request);

        if (!response.IsSuccessful || string.IsNullOrWhiteSpace(response.Content))
            return new List<AuctionUserDto>();

        var users = JsonSerializer.Deserialize<List<AuctionUserDto>>(response.Content, options);

        if (users == null || users.Count == 0)
            return new List<AuctionUserDto>();

        return users;
    }
}
