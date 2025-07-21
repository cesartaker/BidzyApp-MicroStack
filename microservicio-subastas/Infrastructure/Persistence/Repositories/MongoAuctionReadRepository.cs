using System.Net;
using Application.Contracts.Repositories;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence.Context;
using MongoDB.Driver;

namespace Infrastructure.Persistence.Repositories;

public class MongoAuctionReadRepository:IMongoAuctionReadRepository
{
    private readonly MongoDbReadContext _context;

    public MongoAuctionReadRepository(MongoDbReadContext context)
    {
        _context = context;
    }
    /// <summary>
    /// Agrega una nueva subasta al repositorio de lectura basado en MongoDB.
    /// Verifica la validez del objeto <see cref="Auction"/> antes de intentar insertarlo.
    /// En caso de éxito, devuelve <see cref="HttpStatusCode.OK"/>; si el objeto es nulo o ocurre una excepción, devuelve el código correspondiente.
    /// </summary>
    /// <param name="auction">
    /// Objeto <see cref="Auction"/> que contiene los datos de la subasta a insertar.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{HttpStatusCode}"/> que representa la operación asincrónica de inserción:
    /// devuelve <c>OK</c> si la subasta se agrega correctamente, <c>BadRequest</c> si la entrada es nula,
    /// o <c>InternalServerError</c> si ocurre una excepción durante el proceso.
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
    /// Actualiza los datos de una subasta existente en el repositorio de lectura basado en MongoDB.
    /// Aplica un filtro por el identificador único de la subasta y reemplaza el documento completo con la nueva información proporcionada.
    /// </summary>
    /// <param name="auction">
    /// Objeto <see cref="Auction"/> que contiene los datos actualizados de la subasta que se desea persistir.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{HttpStatusCode}"/> que representa la operación asincrónica:
    /// devuelve <c>OK</c> para indicar que la actualización se realizó correctamente.
    /// </returns>
    public async Task<HttpStatusCode> UpdateAuction(Auction auction)
    {
        var filter = Builders<Auction>.Filter.Eq(a => a.Id, auction.Id);
        await _context.Auctions.ReplaceOneAsync(filter, auction);
        return HttpStatusCode.OK;
    }
    /// <summary>
    /// Recupera una subasta específica desde el repositorio MongoDB utilizando su identificador único.
    /// Crea un filtro por <see cref="Auction.Id"/> y retorna el primer resultado que coincida.
    /// </summary>
    /// <param name="id">
    /// Identificador único de la subasta (<see cref="Guid"/>) que se desea consultar.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{Auction}"/> que representa la operación asincrónica:
    /// devuelve el objeto <see cref="Auction"/> correspondiente al identificador si existe; de lo contrario, <c>null</c>.
    /// </returns>
    public async Task<Auction> GetAuctionById(Guid id)
    {
        var filter = Builders<Auction>.Filter.Eq(a => a.Id, id);
        return await _context.Auctions.Find(filter).FirstOrDefaultAsync();
    }
    /// <summary>
    /// Recupera todas las subastas que se encuentran en un estado específico desde el repositorio MongoDB.
    /// Construye un filtro por <see cref="AuctionStatus"/> y devuelve la lista completa de coincidencias.
    /// </summary>
    /// <param name="status">
    /// Estado deseado de las subastas (<see cref="AuctionStatus"/>), como <c>Completed</c>, <c>Ended</c> o <c>Cancelled</c>.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{List{Auction}}"/> que representa la operación asincrónica:
    /// devuelve una lista de objetos <see cref="Auction"/> cuyo estado coincide con el valor especificado.
    /// </returns>
    public async Task<List<Auction>> GetAuctionsByStatus(AuctionStatus status)
    {
        var filter = Builders<Auction>.Filter.Eq(a => a.Status, status);
        return await _context.Auctions.Find(filter).ToListAsync();
    }
    /// <summary>
    /// Recupera todas las subastas creadas por un usuario específico desde el repositorio MongoDB.
    /// Aplica un filtro por <see cref="Auction.UserId"/> y retorna la lista completa de coincidencias.
    /// </summary>
    /// <param name="userId">
    /// Identificador único del usuario (<see cref="Guid"/>) cuyas subastas se desean consultar.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{List{Auction}}"/> que representa la operación asincrónica:
    /// devuelve una lista de objetos <see cref="Auction"/> correspondientes al usuario especificado.
    /// </returns>
    /// </returns>
    public async Task<List<Auction>> GetAuctionsByUserId(Guid userId)
    {
        var filter = Builders<Auction>.Filter.Eq(a => a.UserId, userId);
        return await _context.Auctions.Find(filter).ToListAsync();
    }
    /// <summary>
    /// Recupera todas las subastas cuyo estado coincide con alguno de los valores especificados.
    /// Construye un filtro MongoDB que selecciona las subastas con estados incluidos en la lista <see cref="AuctionStatus"/> proporcionada.
    /// </summary>
    /// <param name="statuses">
    /// Lista de estados válidos (<see cref="AuctionStatus"/>) por los cuales se desea filtrar las subastas.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{List{Auction}}"/> que representa la operación asincrónica:
    /// devuelve una colección de objetos <see cref="Auction"/> que tienen alguno de los estados indicados.
    /// </returns>
    public async Task<List<Auction>> GetAuctionsByStatuses(List<AuctionStatus> statuses)
    {
        var filter = Builders<Auction>.Filter.In(a => a.Status, statuses);
        return await _context.Auctions.Find(filter).ToListAsync();

    }
    /// <summary>
    /// Recupera todas las subastas en las que un usuario específico figura como ganador y cuyo estado coincide con el indicado.
    /// Aplica un filtro compuesto en MongoDB que combina los valores de <see cref="Auction.WinnerId"/> y <see cref="Auction.Status"/>.
    /// </summary>
    /// <param name="userId">
    /// Identificador único del usuario (<see cref="Guid"/>) cuya participación como ganador se desea consultar.
    /// </param>
    /// <param name="status">
    /// Estado deseado de las subastas (<see cref="AuctionStatus"/>), como <c>Completed</c> o <c>Delivered</c>.
    /// </param>
    /// <returns>
    /// Una <see cref="Task{List{Auction}}"/> que representa la operación asincrónica:
    /// devuelve una lista de objetos <see cref="Auction"/> que cumplen con ambos criterios de búsqueda.
    /// </returns>
    public async Task<List<Auction>> GetAuctionsByUserIdAndStatus(Guid userId, AuctionStatus status)
    {
        var filter = Builders<Auction>.Filter.And(
            Builders<Auction>.Filter.Eq(a => a.WinnerId, userId),
            Builders<Auction>.Filter.Eq(a => a.Status, status)
        );
       var auctions = await _context.Auctions.Find(filter).ToListAsync();
        return auctions;
    }
}
