using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Context;
using MongoDB.Driver;

namespace Infrastructure.Persistence.Repositories;

public class MongoAuctionWriteRepository: IMongoAuctionWriteRepository
{
    private readonly MongoDbWriteContext _context;
    public MongoAuctionWriteRepository(MongoDbWriteContext context)
    {
        _context = context;
    }
    /// <summary>
    /// Inserta una nueva subasta en el repositorio MongoDB.
    /// Valida que el objeto <see cref="Auction"/> no sea nulo antes de intentar la persistencia.
    /// Devuelve el código de estado HTTP correspondiente según el resultado de la operación.
    /// </summary>
    /// <param name="auction">
    /// Objeto <see cref="Auction"/> que contiene los datos de la subasta a registrar.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{HttpStatusCode}"/> que representa la operación asincrónica:
    /// devuelve <c>OK</c> si la subasta se inserta correctamente,
    /// <c>BadRequest</c> si el objeto es nulo,
    /// o <c>InternalServerError</c> en caso de excepción durante el proceso.
    /// </returns>
    public async Task<HttpStatusCode> AddAuction(Auction auction)
    {
        try
        {
            if (auction == null)
                return HttpStatusCode.BadRequest;

            await _context.Auctions.InsertOneAsync(auction);
            return HttpStatusCode.OK;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al insertar producto: {ex.Message}");
            return HttpStatusCode.InternalServerError;
        }
    }
    /// <summary>
    /// Actualiza una subasta existente en el repositorio MongoDB.
    /// Utiliza un filtro por <see cref="Auction.Id"/> para localizar el documento original y lo reemplaza con los datos actualizados.
    /// </summary>
    /// <param name="auction">
    /// Objeto <see cref="Auction"/> que contiene la información modificada que se desea persistir.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{HttpStatusCode}"/> que representa la operación asincrónica:
    /// devuelve <c>OK</c> para indicar que la subasta fue actualizada correctamente.
    /// </returns>
    public async Task<HttpStatusCode> UpdateAuction(Auction auction)
    {
        var filter = Builders<Auction>.Filter.Eq(a => a.Id, auction.Id);
        await _context.Auctions.ReplaceOneAsync(filter, auction);
        return HttpStatusCode.OK;
    }
}
