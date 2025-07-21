using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Contracts.Services;
using Application.Dtos.AuctionResults;
using Microsoft.Extensions.Options;
using RestSharp;

namespace Infrastructure.Services.Users;

public class UsersServiceClient : IUserService
{
    private readonly RestClient _client;

    public UsersServiceClient(IOptions<UsersServiceOptions> options)
    {
        _client = new RestClient(new RestClientOptions(options.Value.BaseUrl));
    }
    /// <summary>
    /// Recupera información de usuarios participantes en subastas, utilizando una lista de identificadores únicos.
    /// Envía una solicitud HTTP tipo <c>POST</c> al endpoint <c>batch</c> con los IDs de usuarios en el cuerpo JSON.
    /// Valida la respuesta y la deserializa como una colección de objetos <see cref="AuctionUserDto"/>.
    /// En caso de respuesta inválida o sin datos, retorna una lista vacía.
    /// </summary>
    /// <param name="usersIds">
    /// Lista de identificadores únicos (<see cref="Guid"/>) de los usuarios cuyas subastas se desean consultar.
    /// Puede contener valores nulos (<c>null</c>) que serán ignorados por el servicio remoto si corresponde.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{List{AuctionUserDto}}"/> que representa la operación asincrónica:
    /// devuelve una lista de usuarios con la información recuperada si la respuesta es exitosa y contiene datos válidos;
    /// de lo contrario, retorna una lista vacía.
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
