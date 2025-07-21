using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Contracts.Services;
using Application.Dtos.AuctionResults;
using Infrastructure.Services.Users;
using Microsoft.Extensions.Options;
using RestSharp;

namespace Infrastructure.Services.Products;

public class ProductsServiceClient : IProductService
{
    private readonly RestClient _client;
    public ProductsServiceClient(IOptions<ProductsServiceOptions> options)
    {
        _client = new RestClient(new RestClientOptions(options.Value.BaseUrl));
    }
    /// <summary>
    /// Recupera información de productos asociados a una lista de identificadores mediante una solicitud HTTP a un servicio externo.
    /// Envía los <see cref="Guid"/> contenidos en <paramref name="productIds"/> al endpoint <c>batch</c> usando un cuerpo JSON.
    /// Verifica la validez de la respuesta antes de deserializarla como una colección de <see cref="AuctionProductDto"/>.
    /// </summary>
    /// <param name="productIds">
    /// Lista de identificadores únicos de productos (<see cref="Guid"/>), o <c>null</c> si no hay productos a consultar.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{List{AuctionProductDto}}"/> que representa la operación asincrónica:
    /// devuelve una lista con los objetos deserializados si la respuesta es exitosa y contiene datos válidos;
    /// en caso contrario, devuelve una lista vacía.
    /// </returns>
    public async Task<List<AuctionProductDto>> GetProductsInformationByIds(List<Guid>? productIds)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var request = new RestRequest("batch", Method.Post);
        request.AddJsonBody(new { productIds });
        var response = await _client.ExecuteAsync(request);

        if (!response.IsSuccessful || string.IsNullOrWhiteSpace(response.Content))
            return new List<AuctionProductDto>();

        var products = JsonSerializer.Deserialize<List<AuctionProductDto>>(response.Content, options);

        if (products == null || products.Count == 0)
            return new List<AuctionProductDto>();

        return products;
    }
    /// <summary>
    /// Actualiza el estado de un producto específico mediante una solicitud HTTP tipo <c>PATCH</c> al servicio externo.
    /// Envía el <paramref name="productId"/> y el nuevo <paramref name="status"/> en el cuerpo JSON de la solicitud.
    /// Evalúa el resultado y devuelve el código de estado HTTP correspondiente, registrando el error en consola si la operación falla.
    /// </summary>
    /// <param name="productId">
    /// Identificador único del producto (<see cref="Guid"/>) que se desea actualizar.
    /// </param>
    /// <param name="status">
    /// Nuevo estado (<see cref="string"/>) que se desea asignar al producto.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{HttpStatusCode}"/> que representa la operación asincrónica:
    /// devuelve <c>OK</c> si la actualización se realiza con éxito;
    /// en caso contrario, retorna el código recibido del servicio y registra el error en consola.
    /// </returns>
    public async Task<HttpStatusCode> UpdateProductStatus(Guid productId, string status)
    {
        var request = new RestRequest("update", Method.Patch);
        request.AddJsonBody(new
        {
            productId,
            status
        });
        var response = await _client.ExecuteAsync(request);
        if (response.IsSuccessful)
        {
            return HttpStatusCode.OK;
        }
        else
        {
            Console.WriteLine($"Error actualizando estado del producto: {response.ErrorMessage} - {response.Content}");
            return response.StatusCode;
        }
    }
}
